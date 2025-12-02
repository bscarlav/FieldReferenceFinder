using McTools.Xrm.Connection;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow.Activities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Activities.Statements;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.PeerToPeer.Collaboration;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XrmToolBox.Extensibility;
using static ScintillaNET.Style;

namespace FieldReferenceFinder
{
    public partial class MyPluginControl : PluginControlBase
    {
        private Settings mySettings;
        private List<FieldReferenceResult> searchResults;

        public MyPluginControl()
        {
            InitializeComponent();
            searchResults = new List<FieldReferenceResult>();
        }

        private void MyPluginControl_Load(object sender, EventArgs e)
        {
            ShowInfoNotification("Field Reference Finder - Select a table and field to search for references", new Uri("https://github.com/bscarlav/FieldReferenceFinder"));

            // Loads or creates the settings for the plugin
            if (!SettingsManager.Instance.TryLoad(GetType(), out mySettings))
            {
                mySettings = new Settings();
                LogWarning("Settings not found => a new settings file has been created!");
            }
            else
            {
                LogInfo("Settings found and loaded");
            }

            // Load tables when connected
            if (Service != null)
            {
                LoadTables();
            }
        }

        private void tsbLoadTables_Click(object sender, EventArgs e)
        {
            ExecuteMethod(LoadTables);
        }


        private void tsbSearch_Click(object sender, EventArgs e)
        {
            if (comboBoxTables.SelectedItem == null || comboBoxFields.SelectedItem == null)
            {
                MessageBox.Show("Please select both a table and a field before searching.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ExecuteMethod(SearchFieldReferences);
        }

        private void tsbExport_Click(object sender, EventArgs e)
        {
            if (searchResults == null || searchResults.Count == 0)
            {
                MessageBox.Show("No results to export. Please run a search first.", "No Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SaveFileDialog saveDialog = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                Title = "Export Search Results",
                FileName = $"FieldReferences_{comboBoxTables.SelectedItem}_{comboBoxFields.SelectedItem}_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
            };

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    ExportToCsv(saveDialog.FileName);
                    MessageBox.Show($"Results exported successfully to {saveDialog.FileName}", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error exporting results: {ex.Message}", "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void LoadTables()
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Loading tables...",
                Work = (worker, args) =>
                {
                    try
                    {
                        // Use RetrieveAllEntitiesRequest to get all entity metadata
                        var request = new Microsoft.Xrm.Sdk.Messages.RetrieveAllEntitiesRequest
                        {
                            EntityFilters = Microsoft.Xrm.Sdk.Metadata.EntityFilters.Entity,
                            RetrieveAsIfPublished = true
                        };

                        var response = (Microsoft.Xrm.Sdk.Messages.RetrieveAllEntitiesResponse)Service.Execute(request);
                        
                        // Filter for custom entities and standard entities that are valid for external integration
                        var entities = response.EntityMetadata
                            .OrderBy(e => e.LogicalName)
                            .Select(e => e.LogicalName)
                            .ToList();

                        args.Result = entities;
                    }
                    catch (Exception ex)
                    {
                        //args.Error = ex;
                    }
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        MessageBox.Show(args.Error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    var entities = args.Result as List<string>;
                    if (entities != null)
                    {
                        comboBoxTables.Items.Clear();
                        foreach (var entityName in entities)
                        {
                            comboBoxTables.Items.Add(entityName);
                        }
                        LogInfo($"Loaded {entities.Count} tables");
                    }
                }
            });
        }

        private void LoadFields(string tableName)
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = $"Loading fields for {tableName}...",
                Work = (worker, args) =>
                {
                    try
                    {
                        // Use RetrieveEntityRequest to get entity metadata including attributes
                        var request = new Microsoft.Xrm.Sdk.Messages.RetrieveEntityRequest
                        {
                            LogicalName = tableName,
                            EntityFilters = Microsoft.Xrm.Sdk.Metadata.EntityFilters.Attributes,
                            RetrieveAsIfPublished = true
                        };

                        var response = (Microsoft.Xrm.Sdk.Messages.RetrieveEntityResponse)Service.Execute(request);
                        
                        // Filter for custom attributes that are not managed
                        var attributes = response.EntityMetadata.Attributes
                            .Where(a => a.AttributeType != Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.Virtual)
                            .OrderBy(a => a.LogicalName)
                            .Select(a => a.LogicalName)
                            .ToList();

                        args.Result = attributes;
                    }
                    catch (Exception ex)
                    {
                        //args.Error = ex;
                    }
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        MessageBox.Show(args.Error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    var attributes = args.Result as List<string>;
                    if (attributes != null)
                    {
                        comboBoxFields.Items.Clear();
                        foreach (var attributeName in attributes)
                        {
                            comboBoxFields.Items.Add(attributeName);
                        }
                        LogInfo($"Loaded {attributes.Count} fields for {tableName}");
                    }
                }
            });
        }

        private void SearchFieldReferences()
        {
            var selectedTable = comboBoxTables.SelectedItem.ToString();
            var selectedField = comboBoxFields.SelectedItem.ToString();
            var fieldName = $"{selectedTable}.{selectedField}";

            searchResults.Clear();
            dataGridViewResults.Rows.Clear();

            WorkAsync(new WorkAsyncInfo
            {
                Message = $"Searching for references to {fieldName}...",
                Work = (worker, args) =>
                {
                    var results = new List<FieldReferenceResult>();

                    // Search Web Resources
                    if (checkBoxWebResources.Checked)
                    {
                        results.AddRange(SearchWebResources(selectedTable, selectedField, worker));
                    }

                    // Search Workflows
                    if (checkBoxWorkflows.Checked)
                    {
                        results.AddRange(SearchWorkflows(selectedTable, selectedField, worker));
                    }

                    // Search Canvas Apps
                    if (checkBoxCanvasApps.Checked)
                    {
                        results.AddRange(SearchCanvasApps(selectedTable, selectedField, worker));
                    }

                    // Search Forms
                    if (checkBoxForms.Checked)
                    {
                        results.AddRange(SearchForms(selectedTable, selectedField, worker));
                    }
                    // Search Views
                    if (checkBoxViews.Checked)
                    {
                        results.AddRange(SearchViews(selectedTable, selectedField, worker));
                    }

                    args.Result = results;
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        MessageBox.Show(args.Error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    searchResults = args.Result as List<FieldReferenceResult>;
                    if (searchResults != null)
                    {
                        DisplayResults();
                        LogInfo($"Search completed. Found {searchResults.Count} references.");
                    }
                }
            });
        }

        private List<FieldReferenceResult> SearchWebResources(string tableName, string fieldName, BackgroundWorker worker)
        {
            var results = new List<FieldReferenceResult>();

            try
            {
                var publisherFetch = @"<fetch mapping=""logical"">
                    <entity name=""publisher"">
                      <attribute name=""customizationprefix"" />
                      <filter>
                         <condition attribute=""isreadonly"" operator=""eq"" value=""false"" />
                      </filter>
                    </entity>
                </fetch>";

                var prefixes = Service.RetrieveMultiple(new FetchExpression(publisherFetch)).Entities
                    .Where(e => !String.IsNullOrEmpty(e.GetAttributeValue<string>("customizationprefix")))
                    .Select(e => e.GetAttributeValue<string>("customizationprefix"));

                var prefixConditions = string.Join("", prefixes.Select(p => $"<condition attribute='name' operator='begins-with' value='{p}_' />"));

                var webresourceFetch = $@"<fetch mapping='logical'>
                        <entity name='webresource'>
                            <attribute name='name' />
                            <attribute name='webresourcetype' />
                            <attribute name='content' />
                            <filter type='and'>
                                <condition attribute='webresourcetype' operator='in'>
                                    <value>1</value> <!-- HTML -->
                                    <value>2</value> <!-- CSS -->
                                    <value>3</value> <!-- JS -->
                                    <value>4</value> <!-- XML -->
                                </condition>
                                <condition attribute='ishidden' operator='eq' value='false' />
                                <condition attribute='iscustomizable' operator='eq' value='true' />
                                <filter type='or'>
                                    {prefixConditions}
                                </filter>
                            </filter>
                        </entity>
                    </fetch>";

                var webResources = Service.RetrieveMultiple(new FetchExpression(webresourceFetch)).Entities;

                foreach (var resource in webResources)
                {
                    var content = resource.GetAttributeValue<string>("content");
                    if (!string.IsNullOrEmpty(content))
                    {
                        var decodedContent = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(content));
                        if (decodedContent.ToLower().Contains(fieldName.ToLower()))
                        {
                            results.Add(new FieldReferenceResult
                            {
                                Type = GetWebResourceType(resource.GetAttributeValue<OptionSetValue>("webresourcetype").Value),
                                Name = resource.GetAttributeValue<string>("name"),
                                Location = $"Web Resource: {resource.GetAttributeValue<string>("name")}",
                                Context = ExtractContext(decodedContent, fieldName)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError($"Error searching web resources: {ex.Message}");
            }

            return results;
        }

        private List<FieldReferenceResult> SearchWorkflows(string tableName, string fieldName, BackgroundWorker worker)
        {
            var results = new List<FieldReferenceResult>();

            try
            {
                var publisherFetch = @"<fetch mapping=""logical"">
                    <entity name=""publisher"">
                      <attribute name=""customizationprefix"" />
                      <filter>
                         <condition attribute=""isreadonly"" operator=""eq"" value=""false"" />
                      </filter>
                    </entity>
                </fetch>";

                var prefixes = Service.RetrieveMultiple(new FetchExpression(publisherFetch)).Entities
                    .Where(e => !String.IsNullOrEmpty(e.GetAttributeValue<string>("customizationprefix")))
                    .Select(e => e.GetAttributeValue<string>("customizationprefix"));

                var prefixConditions = string.Join("", prefixes.Select(p => $"<condition attribute='name' operator='begins-with' value='{p}_' />"));

                var workflowFetch = $@"<fetch mapping='logical'>
                        <entity name='workflow'>
                            <attribute name='name' />
                            <attribute name='xaml' />
                            <attribute name='clientdata' />
                            <attribute name='category' />                            
                            <attribute name='workflowid' />
                            <filter type='and'>
                                <condition attribute='iscustomizable' operator='eq' value='true' />
                                <filter type='or'>                                    
                                    <condition attribute='xaml' operator='like' value='%{fieldName}%' />
                                    <condition attribute='clientdata' operator='like' value='%{fieldName}%' />
                                </filter>
                            </filter>
                        </entity>
                    </fetch>";

                var workflows = Service.RetrieveMultiple(new FetchExpression(workflowFetch)).Entities;
                //worker.ReportProgress(0, $"Searching {workflows.Count} workflows...");

                foreach (var workflow in workflows)
                {
                    var xaml = workflow.GetAttributeValue<string>("xaml");
                    if (!string.IsNullOrEmpty(xaml) && results.Where(frr => frr.Id == workflow.Id).Count() == 0)
                    {
                        if (xaml.ToLower().Contains(fieldName.ToLower()))
                        {
                            results.Add(new FieldReferenceResult
                            {
                                // todo, get category string from int
                                Id = workflow.Id,
                                Type = GetCategoryName(workflow.GetAttributeValue<OptionSetValue>("category").Value),
                                Name = workflow.GetAttributeValue<string>("name"),
                                Location = $"Workflow: {workflow.GetAttributeValue<string>("name")}",
                                Context = ExtractContext(xaml, fieldName)
                            });
                            continue;
                        }
                    }
                    var clientdata = workflow.GetAttributeValue<string>("clientdata");
                    if (!string.IsNullOrEmpty(clientdata) && results.Where(frr => frr.Id == workflow.Id).Count() == 0)
                    {
                        if (clientdata.ToLower().Contains(fieldName.ToLower()))
                        {
                            results.Add(new FieldReferenceResult
                            {
                                Id = workflow.Id,
                                Type = GetCategoryName(workflow.GetAttributeValue<OptionSetValue>("category").Value),
                                Name = workflow.GetAttributeValue<string>("name"),
                                Location = $"Workflow: {workflow.GetAttributeValue<string>("name")}",
                                Context = ExtractContext(clientdata, fieldName)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError($"Error searching workflows: {ex.Message}");
            }

            return results;
        }

        private List<FieldReferenceResult> SearchCanvasApps(string tableName, string fieldName, BackgroundWorker worker)
        {
            var results = new List<FieldReferenceResult>();
            var searchTerms = new[] { fieldName, $"{tableName}.{fieldName}" };

            try
            {
                var workflowFetch = $@"<fetch mapping='logical'>
                        <entity name='canvasapp'>
                            <attribute name='name' />
                            <attribute name='displayname' />
                            <attribute name='databasereferences' />
                            <filter type='and'>
                                <condition attribute='iscustomizable' operator='eq' value='true' />
                                <filter type='or'>                                    
                                    <condition attribute='databasereferences' operator='like' value='%{tableName}%' />
                                </filter>
                            </filter>
                        </entity>
                    </fetch>";

                var apps = Service.RetrieveMultiple(new FetchExpression(workflowFetch)).Entities;

                foreach (var app in apps)
                {
                    var dbReferences = app.GetAttributeValue<string>("databasereferences");
                    if (!string.IsNullOrEmpty(dbReferences))
                    {
                        if (dbReferences.ToLower().Contains(tableName.ToLower()))
                        {
                            results.Add(new FieldReferenceResult
                            {
                                Type = "Canvas App",
                                Name = app.GetAttributeValue<string>("name"),
                                Location = $"Canvas App: {app.GetAttributeValue<string>("displayname")}",
                                Context = ExtractContext(dbReferences, tableName)
                            });
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError($"Error searching canvas apps: {ex.Message}");
            }

            return results;
        }

        private List<FieldReferenceResult> SearchForms(string tableName, string fieldName, BackgroundWorker worker)
        {
            var results = new List<FieldReferenceResult>();
            var searchTerms = new[] { fieldName, $"{tableName}.{fieldName}" };

            try
            {
                var workflowFetch = $@"<fetch mapping='logical'>
                        <entity name='systemform'>
                            <attribute name='name' />
                            <attribute name='formxml' />
                            <attribute name='type' />
                            <attribute name='objecttypecode' />
                            <filter type='and'>
                                <filter type='or'>                                    
                                    <condition attribute='formxml' operator='like' value='%{fieldName}%' />
                                </filter>
                            </filter>
                        </entity>
                    </fetch>";

                var forms = Service.RetrieveMultiple(new FetchExpression(workflowFetch)).Entities;

                foreach (var form in forms)
                {
                    var formXml = form.GetAttributeValue<string>("formxml");
                    if (!string.IsNullOrEmpty(formXml) && results.Where(frr => frr.Id == form.Id).Count() == 0)
                    {
                        if (formXml.ToLower().Contains(fieldName.ToLower()))
                        {
                            results.Add(new FieldReferenceResult
                            {
                                // todo, get category string from int
                                Id = form.Id,
                                Type = GetTypeName(form.GetAttributeValue<OptionSetValue>("type").Value),
                                Name = form.GetAttributeValue<string>("name"),
                                Location = $"{form.GetAttributeValue<string>("objecttypecode")}: {form.GetAttributeValue<string>("name")}",
                                Context = ExtractContext(formXml, fieldName)
                            });
                            continue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError($"Error searching canvas apps: {ex.Message}");
            }

            return results;
        }

        private List<FieldReferenceResult> SearchViews(string tableName, string fieldName, BackgroundWorker worker)
        {
            var results = new List<FieldReferenceResult>();
            var searchTerms = new[] { fieldName, $"{tableName}.{fieldName}" };

            try
            {
                var workflowFetch = $@"<fetch mapping='logical'>
                        <entity name='savedquery'>
                            <attribute name='name' />
                            <attribute name='layoutxml' />
                            <attribute name='fetchxml' />
                            <filter type='and'>
                                <filter type='or'>                                    
                                    <condition attribute='layoutxml' operator='like' value='%{fieldName}%' />
                                    <condition attribute='fetchxml' operator='like' value='%{fieldName}%' />
                                </filter>
                            </filter>
                        </entity>
                    </fetch>";

                var views = Service.RetrieveMultiple(new FetchExpression(workflowFetch)).Entities;

                foreach (var view in views)
                {
                    var layoutxml = view.GetAttributeValue<string>("layoutxml");
                    if (!string.IsNullOrEmpty(layoutxml) && results.Where(frr => frr.Id == view.Id).Count() == 0)
                    {
                        if (layoutxml.ToLower().Contains(fieldName.ToLower()))
                        {
                            results.Add(new FieldReferenceResult
                            {
                                // todo, get category string from int
                                Id = view.Id,
                                Type = "View",
                                Name = view.GetAttributeValue<string>("name"),
                                Location = $"View: {view.GetAttributeValue<string>("name")}",
                                Context = ExtractContext(layoutxml, fieldName)
                            });
                            continue;
                        }
                    }
                    var fetchXml = view.GetAttributeValue<string>("fetchxml");
                    if (!string.IsNullOrEmpty(fetchXml) && results.Where(frr => frr.Id == view.Id).Count() == 0)
                    {
                        if (fetchXml.ToLower().Contains(fieldName.ToLower()))
                        {
                            results.Add(new FieldReferenceResult
                            {
                                Id = view.Id,
                                Type = "View",
                                Name = view.GetAttributeValue<string>("name"),
                                Location = $"View: {view.GetAttributeValue<string>("name")}",
                                Context = ExtractContext(fetchXml, fieldName)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError($"Error searching canvas apps: {ex.Message}");
            }

            return results;
        }

        private string ExtractContext(string content, string searchTerm)
        {
            var index = content.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase);
            if (index == -1) return "Context not found";

            var start = Math.Max(0, index - 50);
            var end = Math.Min(content.Length, index + searchTerm.Length + 50);
            var context = content.Substring(start, end - start);

            // Clean up the context
            context = context.Replace("\r", " ").Replace("\n", " ").Replace("\t", " ");
            while (context.Contains("  "))
            {
                context = context.Replace("  ", " ");
            }

            return context.Trim();
        }

        private void DisplayResults()
        {
            dataGridViewResults.Rows.Clear();
            foreach (var result in searchResults)
            {
                dataGridViewResults.Rows.Add(result.Type, result.Name, result.Location, result.Context);
            }
        }

        private void ExportToCsv(string fileName)
        {
            using (var writer = new StreamWriter(fileName))
            {
                writer.WriteLine("Type,Name,Location,Context");
                foreach (var result in searchResults)
                {
                    writer.WriteLine($"\"{result.Type}\",\"{result.Name}\",\"{result.Location}\",\"{result.Context.Replace("\"", "\"\"")}\"");
                }
            }
        }

        private string GetWebResourceType(int categoryValue)
        {
            switch (categoryValue)
            {
                case 1:
                    return "HTML";
                case 2:
                    return "CSS";
                case 3:
                    return "JS";
                case 4:
                    return "XML";
                default:
                    return "Unknown";
            }
        }
        private string GetCategoryName(int categoryValue)
        {
            switch (categoryValue)
            {
                case 0:
                    return "Workflow";
                case 1:
                    return "Dialog";
                case 2:
                    return "Business Rule";
                case 3:
                    return "Action";
                case 4:
                    return "Business Process Flow";
                case 5:
                    return "Modern Flow";
                case 6:
                    return "Desktop Flow";
                case 7:
                    return "AI Flow";
                default:
                    return "Unknown";
            }
        }

        private string GetTypeName(int type)
        {
            switch (type)
            {
                case 0:
                    return "Dashboard";
                case 1:
                    return "AppointmentBook";
                case 2:
                    return "Main";
                case 3:
                    return "MiniCampaignBO";
                case 4:
                    return "Preview";
                case 5:
                    return "Mobile - Express";
                case 6:
                    return "Quick View Form";
                case 7:
                    return "Quick Create";
                case 8:
                    return "Dialog";
                case 9:
                    return "Task Flow Form";
                case 10:
                    return "InteractionCentricDashboard";
                case 11:
                    return "Card";
                case 12:
                    return "Main - Interactive experience";
                case 13:
                    return "Contextual Dashboard";
                case 100:
                    return "Other";
                case 101:
                    return "MainBackup";
                case 102:
                    return "AppointmentBookBackup";
                case 103:
                    return "Power BI Dashboard";
                default:
                    return "Unknown";
            }
        }

        /// <summary>
        /// This event occurs when the plugin is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyPluginControl_OnCloseTool(object sender, EventArgs e)
        {
            // Before leaving, save the settings
            SettingsManager.Instance.Save(GetType(), mySettings);
        }

        /// <summary>
        /// This event occurs when the connection has been updated in XrmToolBox
        /// </summary>
        public override void UpdateConnection(IOrganizationService newService, ConnectionDetail detail, string actionName, object parameter)
        {
            base.UpdateConnection(newService, detail, actionName, parameter);

            if (mySettings != null && detail != null)
            {
                mySettings.LastUsedOrganizationWebappUrl = detail.WebApplicationUrl;
                LogInfo("Connection has changed to: {0}", detail.WebApplicationUrl);
            }

            // Load tables when connection is established
            if (Service != null && comboBoxTables.Items.Count == 0)
            {
                LoadTables();
            }
        }

        private void comboBoxTables_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (comboBoxTables.SelectedItem != null)
            {
                LoadFields(comboBoxTables.SelectedItem.ToString());
            }
        }
    }

    public class FieldReferenceResult
    {   public Guid Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Context { get; set; }
    }
}