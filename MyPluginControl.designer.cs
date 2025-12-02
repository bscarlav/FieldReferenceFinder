namespace FieldReferenceFinder
{
    partial class MyPluginControl
    {
        /// <summary> 
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.toolStripMenu = new System.Windows.Forms.ToolStrip();
            this.tsbLoadTables = new System.Windows.Forms.ToolStripButton();
            this.tsbSearch = new System.Windows.Forms.ToolStripButton();
            this.tssSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbExport = new System.Windows.Forms.ToolStripButton();
            this.panelMain = new System.Windows.Forms.Panel();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.groupBoxSearchOptions = new System.Windows.Forms.GroupBox();
            this.checkBoxViews = new System.Windows.Forms.CheckBox();
            this.checkBoxForms = new System.Windows.Forms.CheckBox();
            this.checkBoxCanvasApps = new System.Windows.Forms.CheckBox();
            this.checkBoxCloudFlows = new System.Windows.Forms.CheckBox();
            this.checkBoxWorkflows = new System.Windows.Forms.CheckBox();
            this.checkBoxWebResources = new System.Windows.Forms.CheckBox();
            this.groupBoxFieldSelection = new System.Windows.Forms.GroupBox();
            this.comboBoxFields = new System.Windows.Forms.ComboBox();
            this.labelField = new System.Windows.Forms.Label();
            this.comboBoxTables = new System.Windows.Forms.ComboBox();
            this.labelTable = new System.Windows.Forms.Label();
            this.groupBoxResults = new System.Windows.Forms.GroupBox();
            this.dataGridViewResults = new System.Windows.Forms.DataGridView();
            this.ColumnType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnLocation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnContext = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.toolStripMenu.SuspendLayout();
            this.panelMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.groupBoxSearchOptions.SuspendLayout();
            this.groupBoxFieldSelection.SuspendLayout();
            this.groupBoxResults.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewResults)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStripMenu
            // 
            this.toolStripMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStripMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbLoadTables,
            this.tsbSearch,
            this.tssSeparator2,
            this.tsbExport});
            this.toolStripMenu.Location = new System.Drawing.Point(0, 0);
            this.toolStripMenu.Name = "toolStripMenu";
            this.toolStripMenu.Size = new System.Drawing.Size(1200, 34);
            this.toolStripMenu.TabIndex = 4;
            this.toolStripMenu.Text = "toolStrip1";
            // 
            // tsbLoadTables
            // 
            this.tsbLoadTables.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbLoadTables.Name = "tsbLoadTables";
            this.tsbLoadTables.Size = new System.Drawing.Size(108, 29);
            this.tsbLoadTables.Text = "Load Tables";
            this.tsbLoadTables.Click += new System.EventHandler(this.tsbLoadTables_Click);
            // 
            // tsbSearch
            // 
            this.tsbSearch.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbSearch.Name = "tsbSearch";
            this.tsbSearch.Size = new System.Drawing.Size(68, 29);
            this.tsbSearch.Text = "Search";
            this.tsbSearch.Click += new System.EventHandler(this.tsbSearch_Click);
            // 
            // tssSeparator2
            // 
            this.tssSeparator2.Name = "tssSeparator2";
            this.tssSeparator2.Size = new System.Drawing.Size(6, 34);
            // 
            // tsbExport
            // 
            this.tsbExport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbExport.Name = "tsbExport";
            this.tsbExport.Size = new System.Drawing.Size(67, 29);
            this.tsbExport.Text = "Export";
            this.tsbExport.Click += new System.EventHandler(this.tsbExport_Click);
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add(this.splitContainer);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 34);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(1200, 597);
            this.panelMain.TabIndex = 5;
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.groupBoxSearchOptions);
            this.splitContainer.Panel1.Controls.Add(this.groupBoxFieldSelection);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.groupBoxResults);
            this.splitContainer.Size = new System.Drawing.Size(1200, 597);
            this.splitContainer.SplitterDistance = 198;
            this.splitContainer.TabIndex = 0;
            // 
            // groupBoxSearchOptions
            // 
            this.groupBoxSearchOptions.Controls.Add(this.checkBoxViews);
            this.groupBoxSearchOptions.Controls.Add(this.checkBoxForms);
            this.groupBoxSearchOptions.Controls.Add(this.checkBoxCanvasApps);
            this.groupBoxSearchOptions.Controls.Add(this.checkBoxCloudFlows);
            this.groupBoxSearchOptions.Controls.Add(this.checkBoxWorkflows);
            this.groupBoxSearchOptions.Controls.Add(this.checkBoxWebResources);
            this.groupBoxSearchOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxSearchOptions.Location = new System.Drawing.Point(603, 0);
            this.groupBoxSearchOptions.Name = "groupBoxSearchOptions";
            this.groupBoxSearchOptions.Size = new System.Drawing.Size(597, 198);
            this.groupBoxSearchOptions.TabIndex = 1;
            this.groupBoxSearchOptions.TabStop = false;
            this.groupBoxSearchOptions.Text = "Search Options";
            // 
            // checkBoxViews
            // 
            this.checkBoxViews.AutoSize = true;
            this.checkBoxViews.Checked = true;
            this.checkBoxViews.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxViews.Location = new System.Drawing.Point(227, 58);
            this.checkBoxViews.Name = "checkBoxViews";
            this.checkBoxViews.Size = new System.Drawing.Size(77, 24);
            this.checkBoxViews.TabIndex = 5;
            this.checkBoxViews.Text = "Views";
            this.checkBoxViews.UseVisualStyleBackColor = true;
            // 
            // checkBoxForms
            // 
            this.checkBoxForms.AutoSize = true;
            this.checkBoxForms.Checked = true;
            this.checkBoxForms.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxForms.Location = new System.Drawing.Point(227, 28);
            this.checkBoxForms.Name = "checkBoxForms";
            this.checkBoxForms.Size = new System.Drawing.Size(80, 24);
            this.checkBoxForms.TabIndex = 4;
            this.checkBoxForms.Text = "Forms";
            this.checkBoxForms.UseVisualStyleBackColor = true;
            // 
            // checkBoxCanvasApps
            // 
            this.checkBoxCanvasApps.AutoSize = true;
            this.checkBoxCanvasApps.Checked = true;
            this.checkBoxCanvasApps.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxCanvasApps.Location = new System.Drawing.Point(20, 120);
            this.checkBoxCanvasApps.Name = "checkBoxCanvasApps";
            this.checkBoxCanvasApps.Size = new System.Drawing.Size(129, 24);
            this.checkBoxCanvasApps.TabIndex = 3;
            this.checkBoxCanvasApps.Text = "Canvas Apps";
            this.checkBoxCanvasApps.UseVisualStyleBackColor = true;
            // 
            // checkBoxCloudFlows
            // 
            this.checkBoxCloudFlows.AutoSize = true;
            this.checkBoxCloudFlows.Checked = true;
            this.checkBoxCloudFlows.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxCloudFlows.Location = new System.Drawing.Point(20, 90);
            this.checkBoxCloudFlows.Name = "checkBoxCloudFlows";
            this.checkBoxCloudFlows.Size = new System.Drawing.Size(121, 24);
            this.checkBoxCloudFlows.TabIndex = 2;
            this.checkBoxCloudFlows.Text = "Cloud Flows";
            this.checkBoxCloudFlows.UseVisualStyleBackColor = true;
            // 
            // checkBoxWorkflows
            // 
            this.checkBoxWorkflows.AutoSize = true;
            this.checkBoxWorkflows.Checked = true;
            this.checkBoxWorkflows.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxWorkflows.Location = new System.Drawing.Point(20, 60);
            this.checkBoxWorkflows.Name = "checkBoxWorkflows";
            this.checkBoxWorkflows.Size = new System.Drawing.Size(109, 24);
            this.checkBoxWorkflows.TabIndex = 1;
            this.checkBoxWorkflows.Text = "Processes";
            this.checkBoxWorkflows.UseVisualStyleBackColor = true;
            // 
            // checkBoxWebResources
            // 
            this.checkBoxWebResources.AutoSize = true;
            this.checkBoxWebResources.Checked = true;
            this.checkBoxWebResources.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxWebResources.Location = new System.Drawing.Point(20, 30);
            this.checkBoxWebResources.Name = "checkBoxWebResources";
            this.checkBoxWebResources.Size = new System.Drawing.Size(149, 24);
            this.checkBoxWebResources.TabIndex = 0;
            this.checkBoxWebResources.Text = "Web Resources";
            this.checkBoxWebResources.UseVisualStyleBackColor = true;
            // 
            // groupBoxFieldSelection
            // 
            this.groupBoxFieldSelection.Controls.Add(this.comboBoxFields);
            this.groupBoxFieldSelection.Controls.Add(this.labelField);
            this.groupBoxFieldSelection.Controls.Add(this.comboBoxTables);
            this.groupBoxFieldSelection.Controls.Add(this.labelTable);
            this.groupBoxFieldSelection.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupBoxFieldSelection.Location = new System.Drawing.Point(0, 0);
            this.groupBoxFieldSelection.Name = "groupBoxFieldSelection";
            this.groupBoxFieldSelection.Size = new System.Drawing.Size(603, 198);
            this.groupBoxFieldSelection.TabIndex = 0;
            this.groupBoxFieldSelection.TabStop = false;
            this.groupBoxFieldSelection.Text = "Field Selection";
            // 
            // comboBoxFields
            // 
            this.comboBoxFields.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.comboBoxFields.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboBoxFields.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxFields.FormattingEnabled = true;
            this.comboBoxFields.Location = new System.Drawing.Point(80, 60);
            this.comboBoxFields.Name = "comboBoxFields";
            this.comboBoxFields.Size = new System.Drawing.Size(500, 28);
            this.comboBoxFields.TabIndex = 3;
            // 
            // labelField
            // 
            this.labelField.AutoSize = true;
            this.labelField.Location = new System.Drawing.Point(20, 63);
            this.labelField.Name = "labelField";
            this.labelField.Size = new System.Drawing.Size(47, 20);
            this.labelField.TabIndex = 2;
            this.labelField.Text = "Field:";
            // 
            // comboBoxTables
            // 
            this.comboBoxTables.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.comboBoxTables.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboBoxTables.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTables.FormattingEnabled = true;
            this.comboBoxTables.Location = new System.Drawing.Point(80, 25);
            this.comboBoxTables.Name = "comboBoxTables";
            this.comboBoxTables.Size = new System.Drawing.Size(500, 28);
            this.comboBoxTables.TabIndex = 1;
            this.comboBoxTables.SelectionChangeCommitted += new System.EventHandler(this.comboBoxTables_SelectionChangeCommitted);
            // 
            // labelTable
            // 
            this.labelTable.AutoSize = true;
            this.labelTable.Location = new System.Drawing.Point(20, 28);
            this.labelTable.Name = "labelTable";
            this.labelTable.Size = new System.Drawing.Size(52, 20);
            this.labelTable.TabIndex = 0;
            this.labelTable.Text = "Table:";
            // 
            // groupBoxResults
            // 
            this.groupBoxResults.Controls.Add(this.dataGridViewResults);
            this.groupBoxResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxResults.Location = new System.Drawing.Point(0, 0);
            this.groupBoxResults.Name = "groupBoxResults";
            this.groupBoxResults.Size = new System.Drawing.Size(1200, 395);
            this.groupBoxResults.TabIndex = 0;
            this.groupBoxResults.TabStop = false;
            this.groupBoxResults.Text = "Search Results";
            // 
            // dataGridViewResults
            // 
            this.dataGridViewResults.AllowUserToAddRows = false;
            this.dataGridViewResults.AllowUserToDeleteRows = false;
            this.dataGridViewResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewResults.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnType,
            this.ColumnName,
            this.ColumnLocation,
            this.ColumnContext});
            this.dataGridViewResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewResults.Location = new System.Drawing.Point(3, 22);
            this.dataGridViewResults.Name = "dataGridViewResults";
            this.dataGridViewResults.ReadOnly = true;
            this.dataGridViewResults.RowHeadersWidth = 62;
            this.dataGridViewResults.Size = new System.Drawing.Size(1194, 370);
            this.dataGridViewResults.TabIndex = 0;
            // 
            // ColumnType
            // 
            this.ColumnType.HeaderText = "Type";
            this.ColumnType.MinimumWidth = 8;
            this.ColumnType.Name = "ColumnType";
            this.ColumnType.ReadOnly = true;
            this.ColumnType.Width = 120;
            // 
            // ColumnName
            // 
            this.ColumnName.HeaderText = "Name";
            this.ColumnName.MinimumWidth = 8;
            this.ColumnName.Name = "ColumnName";
            this.ColumnName.ReadOnly = true;
            this.ColumnName.Width = 250;
            // 
            // ColumnLocation
            // 
            this.ColumnLocation.HeaderText = "Location";
            this.ColumnLocation.MinimumWidth = 8;
            this.ColumnLocation.Name = "ColumnLocation";
            this.ColumnLocation.ReadOnly = true;
            this.ColumnLocation.Width = 300;
            // 
            // ColumnContext
            // 
            this.ColumnContext.HeaderText = "Context";
            this.ColumnContext.MinimumWidth = 8;
            this.ColumnContext.Name = "ColumnContext";
            this.ColumnContext.ReadOnly = true;
            this.ColumnContext.Width = 500;
            // 
            // MyPluginControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.toolStripMenu);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "MyPluginControl";
            this.Size = new System.Drawing.Size(1200, 631);
            this.Load += new System.EventHandler(this.MyPluginControl_Load);
            this.toolStripMenu.ResumeLayout(false);
            this.toolStripMenu.PerformLayout();
            this.panelMain.ResumeLayout(false);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.groupBoxSearchOptions.ResumeLayout(false);
            this.groupBoxSearchOptions.PerformLayout();
            this.groupBoxFieldSelection.ResumeLayout(false);
            this.groupBoxFieldSelection.PerformLayout();
            this.groupBoxResults.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewResults)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStrip toolStripMenu;
        private System.Windows.Forms.ToolStripButton tsbSearch;
        private System.Windows.Forms.ToolStripButton tsbExport;
        private System.Windows.Forms.ToolStripSeparator tssSeparator2;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.GroupBox groupBoxFieldSelection;
        private System.Windows.Forms.ComboBox comboBoxFields;
        private System.Windows.Forms.Label labelField;
        private System.Windows.Forms.ComboBox comboBoxTables;
        private System.Windows.Forms.Label labelTable;
        private System.Windows.Forms.GroupBox groupBoxSearchOptions;
        private System.Windows.Forms.CheckBox checkBoxCanvasApps;
        private System.Windows.Forms.CheckBox checkBoxCloudFlows;
        private System.Windows.Forms.CheckBox checkBoxWorkflows;
        private System.Windows.Forms.CheckBox checkBoxWebResources;
        private System.Windows.Forms.GroupBox groupBoxResults;
        private System.Windows.Forms.DataGridView dataGridViewResults;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnType;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnLocation;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnContext;
        private System.Windows.Forms.ToolStripButton tsbLoadTables;
        private System.Windows.Forms.CheckBox checkBoxViews;
        private System.Windows.Forms.CheckBox checkBoxForms;
    }
}
