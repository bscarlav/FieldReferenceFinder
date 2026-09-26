using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using FieldReferenceFinder.PluginCodeScanning;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

internal static class Program
{
    private static void Assert(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
    }

    public static int Main()
    {
        // Test fixture input only. The production scanner accepts SDK bytes, never file paths.
        var bytes = File.ReadAllBytes(Assembly.GetExecutingAssembly().Location);
        var report = new ScanReport();
        new InMemoryCodeAnalyzer().Analyze(bytes, Guid.NewGuid(), "Fixture", "ownerid", report, CancellationToken.None);
        var fixtureMatches = report.Matches.Where(m => m.Member.StartsWith("Fixture.")).ToList();
        Assert(fixtureMatches.Any(m => m.Member.Contains(".Direct")), "Missing direct attribute reference");
        Assert(fixtureMatches.Any(m => m.Member.Contains(".Fetch")), "Missing embedded FetchXML reference");
        Assert(!fixtureMatches.Any(m => m.Member.Contains(".Similar")), "Matched a longer field name");
        Assert(!fixtureMatches.Any(m => m.Member.Contains(".get_")), "Reported an unused generated property");
        Assert(!fixtureMatches.Any(m => m.Member.Contains(".Declaration")), "Reported an attribute declaration");
        Assert(report.Issues.Count == 0, "Fixture decompilation had failures");
        try
        {
            new InMemoryCodeAnalyzer().Analyze(new byte[] { 1, 2, 3 }, Guid.NewGuid(),
                "Invalid", "ownerid", new ScanReport(), CancellationToken.None);
            throw new Exception("Invalid assembly accepted");
        }
        catch (BadImageFormatException) { }
        try
        {
            new InMemoryCodeAnalyzer().Analyze(bytes, Guid.NewGuid(), "Cancelled", "ownerid",
                new ScanReport(), new CancellationToken(true));
            throw new Exception("Cancellation ignored");
        }
        catch (OperationCanceledException) { }

        var service = new FixtureService(bytes);
        var scan = new PluginCodeScanner(service).Scan("ownerid");
        Assert(service.Pages == 2, "Assembly paging was not followed");
        Assert(service.ContentRetrievals == 3, "Unexpected content retrievals");
        Assert(scan.AssembliesExamined == 4 && scan.AssembliesCompleted == 1, "Incorrect coverage counts");
        Assert(scan.Issues.Count == 3, "Missing package/empty/corrupt diagnostics");
        Assert(scan.Matches.Any(m => m.Member.Contains("Fixture.Direct")), "Failed assembly prevented valid results");
        Console.WriteLine("PASS: direct/XML matches, token boundaries, unused property/declaration exclusion, invalid DLL, cancellation, paging, retrieval failures and partial coverage.");
        return 0;
    }
}

internal sealed class Fixture
{
    public object Direct(Dictionary<string, object> entity) { return entity["ownerid"]; }
    public string Fetch() { return "<attribute name='ownerid' />"; }
    public string Similar() { return "previousownerid owneridname"; }
    public string OwnerId { get { return "ownerid"; } }
    [System.ComponentModel.Description("ownerid")]
    public string Declaration() { return "unrelated"; }
}

internal sealed class FixtureService : IOrganizationService
{
    private readonly byte[] bytes;
    private readonly Guid valid = Guid.NewGuid();
    private readonly Guid empty = Guid.NewGuid();
    private readonly Guid corrupt = Guid.NewGuid();
    public int Pages;
    public int ContentRetrievals;
    public FixtureService(byte[] bytes) { this.bytes = bytes; }
    public EntityCollection RetrieveMultiple(QueryBase query)
    {
        Pages++;
        var expression = (QueryExpression)query;
        AssertCookie(expression);
        var page = new EntityCollection { MoreRecords = Pages == 1, PagingCookie = "next" };
        if (Pages == 1)
        {
            page.Entities.Add(Row(Guid.NewGuid(), 4, 0));
            page.Entities.Add(Row(corrupt, 0, 1));
        }
        else
        {
            page.Entities.Add(Row(empty, 0, 1));
            page.Entities.Add(Row(valid, 0, 1));
        }
        return page;
    }
    private void AssertCookie(QueryExpression query)
    {
        if (Pages == 2 && (query.PageInfo.PageNumber != 2 || query.PageInfo.PagingCookie != "next"))
            throw new Exception("Missing paging cookie");
    }
    private Entity Row(Guid id, int source, int customizationLevel) =>
        new Entity("pluginassembly", id) { ["name"] = "Fixture", ["sourcetype"] = new OptionSetValue(source), ["customizationlevel"] = (byte)customizationLevel };
    public Entity Retrieve(string name, Guid id, ColumnSet columns)
    {
        ContentRetrievals++;
        return new Entity(name, id) { ["content"] = id == valid ? Convert.ToBase64String(bytes) :
            id == corrupt ? "invalid base64" : null };
    }
    public Guid Create(Entity entity) => throw new NotSupportedException();
    public void Update(Entity entity) => throw new NotSupportedException();
    public void Delete(string name, Guid id) => throw new NotSupportedException();
    public OrganizationResponse Execute(OrganizationRequest request) => throw new NotSupportedException();
    public void Associate(string name, Guid id, Relationship relationship, EntityReferenceCollection entities) => throw new NotSupportedException();
    public void Disassociate(string name, Guid id, Relationship relationship, EntityReferenceCollection entities) => throw new NotSupportedException();
}
