namespace Microsoft.Azure.DocumentDBStudio
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.tsStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.tsButtonZoom = new System.Windows.Forms.ToolStripSplitButton();
            this.splitContainerOuter = new System.Windows.Forms.SplitContainer();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.splitContainerInner = new System.Windows.Forms.SplitContainer();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabRequest = new System.Windows.Forms.TabPage();
            this.tbRequest = new System.Windows.Forms.TextBox();
            this.tabCrudContext = new System.Windows.Forms.TabPage();
            this.splitContainerIntabPage = new System.Windows.Forms.SplitContainer();
            this.labelid = new System.Windows.Forms.Label();
            this.textBoxforId = new System.Windows.Forms.TextBox();
            this.tbCrudContext = new System.Windows.Forms.TextBox();
            this.tabResponse = new System.Windows.Forms.TabPage();
            this.tbResponse = new System.Windows.Forms.TextBox();
            this.tabPageRequestOptions = new System.Windows.Forms.TabPage();
            this.cbRequestOptionsApply = new System.Windows.Forms.CheckBox();
            this.labelPostTrigger = new System.Windows.Forms.Label();
            this.tbPostTrigger = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbPreTrigger = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.rbConsistencyEventual = new System.Windows.Forms.RadioButton();
            this.rbConsistencySession = new System.Windows.Forms.RadioButton();
            this.rbConsistencyBound = new System.Windows.Forms.RadioButton();
            this.rbConsistencyStrong = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbAccessConditionIfNoneMatch = new System.Windows.Forms.RadioButton();
            this.rbAccessConditionIfMatch = new System.Windows.Forms.RadioButton();
            this.tbAccessConditionText = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbIndexingExclude = new System.Windows.Forms.RadioButton();
            this.rbIndexingInclude = new System.Windows.Forms.RadioButton();
            this.rbIndexingDefault = new System.Windows.Forms.RadioButton();
            this.tabDocumentCollectionPolicy = new System.Windows.Forms.TabPage();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.rbOfferS3 = new System.Windows.Forms.RadioButton();
            this.rbOfferS2 = new System.Windows.Forms.RadioButton();
            this.rbOfferS1 = new System.Windows.Forms.RadioButton();
            this.btnRemoveExcludedPath = new System.Windows.Forms.Button();
            this.btnAddExcludedPath = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.rbLazy = new System.Windows.Forms.RadioButton();
            this.rbConsistent = new System.Windows.Forms.RadioButton();
            this.lbExcludedPath = new System.Windows.Forms.ListBox();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnRemovePath = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.btnAddIncludePath = new System.Windows.Forms.Button();
            this.lbIncludedPath = new System.Windows.Forms.ListBox();
            this.labelCollectionId = new System.Windows.Forms.Label();
            this.tbCollectionId = new System.Windows.Forms.TextBox();
            this.cbIndexingPolicyDefault = new System.Windows.Forms.CheckBox();
            this.cbAutomatic = new System.Windows.Forms.CheckBox();
            this.ButtomSplitContainer = new System.Windows.Forms.SplitContainer();
            this.triggerPanel = new System.Windows.Forms.Panel();
            this.rbPostTrigger = new System.Windows.Forms.RadioButton();
            this.rbPreTrigger = new System.Windows.Forms.RadioButton();
            this.feedToolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripTextMaxItemCount = new System.Windows.Forms.ToolStripTextBox();
            this.btnExecuteNext = new System.Windows.Forms.ToolStripButton();
            this.webBrowserResponse = new System.Windows.Forms.WebBrowser();
            this.tsMenu = new System.Windows.Forms.ToolStrip();
            this.btnBack = new System.Windows.Forms.ToolStripButton();
            this.btnForward = new System.Windows.Forms.ToolStripButton();
            this.btnHome = new System.Windows.Forms.ToolStripButton();
            this.toolStripBtnExecute = new System.Windows.Forms.ToolStripButton();
            this.btnRefresh = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnHeaders = new System.Windows.Forms.ToolStripButton();
            this.btnEditRequests = new System.Windows.Forms.ToolStripButton();
            this.tsbViewType = new System.Windows.Forms.ToolStripButton();
            this.tsAddress = new System.Windows.Forms.ToolStrip();
            this.tsLabelUrl = new System.Windows.Forms.ToolStripLabel();
            this.cbUrl = new System.Windows.Forms.ToolStripComboBox();
            this.btnGo = new System.Windows.Forms.ToolStripButton();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsTablesEntities = new System.Windows.Forms.ToolStrip();
            this.btnQueryTable = new System.Windows.Forms.ToolStripButton();
            this.btnCreateTable = new System.Windows.Forms.ToolStripButton();
            this.btnDeleteTable = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnQueryEntities = new System.Windows.Forms.ToolStripButton();
            this.btnNextPage = new System.Windows.Forms.ToolStripButton();
            this.btnInsertEntity = new System.Windows.Forms.ToolStripButton();
            this.btnUpdateEntity = new System.Windows.Forms.ToolStripButton();
            this.tsbMergeEntity = new System.Windows.Forms.ToolStripButton();
            this.btnDeleteEntity = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbEditTemplate = new System.Windows.Forms.ToolStripButton();
            this.statusStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerOuter)).BeginInit();
            this.splitContainerOuter.Panel1.SuspendLayout();
            this.splitContainerOuter.Panel2.SuspendLayout();
            this.splitContainerOuter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerInner)).BeginInit();
            this.splitContainerInner.Panel1.SuspendLayout();
            this.splitContainerInner.Panel2.SuspendLayout();
            this.splitContainerInner.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabRequest.SuspendLayout();
            this.tabCrudContext.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerIntabPage)).BeginInit();
            this.splitContainerIntabPage.Panel1.SuspendLayout();
            this.splitContainerIntabPage.Panel2.SuspendLayout();
            this.splitContainerIntabPage.SuspendLayout();
            this.tabResponse.SuspendLayout();
            this.tabPageRequestOptions.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabDocumentCollectionPolicy.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ButtomSplitContainer)).BeginInit();
            this.ButtomSplitContainer.Panel1.SuspendLayout();
            this.ButtomSplitContainer.Panel2.SuspendLayout();
            this.ButtomSplitContainer.SuspendLayout();
            this.triggerPanel.SuspendLayout();
            this.feedToolStrip.SuspendLayout();
            this.tsMenu.SuspendLayout();
            this.tsAddress.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.tsTablesEntities.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsStatus,
            this.tsProgress,
            this.tsButtonZoom});
            this.statusStrip.Location = new System.Drawing.Point(0, 839);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
            this.statusStrip.Size = new System.Drawing.Size(1428, 26);
            this.statusStrip.TabIndex = 1;
            this.statusStrip.Text = "statusStrip1";
            // 
            // tsStatus
            // 
            this.tsStatus.Name = "tsStatus";
            this.tsStatus.Size = new System.Drawing.Size(1222, 21);
            this.tsStatus.Spring = true;
            this.tsStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tsProgress
            // 
            this.tsProgress.Name = "tsProgress";
            this.tsProgress.Size = new System.Drawing.Size(100, 20);
            // 
            // tsButtonZoom
            // 
            this.tsButtonZoom.Image = global::Microsoft.Azure.DocumentDBStudio.Properties.Resources.ZoomHS;
            this.tsButtonZoom.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsButtonZoom.Name = "tsButtonZoom";
            this.tsButtonZoom.Size = new System.Drawing.Size(84, 24);
            this.tsButtonZoom.Text = "100%";
            this.tsButtonZoom.ButtonClick += new System.EventHandler(this.tsButtonZoom_ButtonClick);
            // 
            // splitContainerOuter
            // 
            this.splitContainerOuter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainerOuter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerOuter.Location = new System.Drawing.Point(0, 57);
            this.splitContainerOuter.Margin = new System.Windows.Forms.Padding(4);
            this.splitContainerOuter.Name = "splitContainerOuter";
            // 
            // splitContainerOuter.Panel1
            // 
            this.splitContainerOuter.Panel1.Controls.Add(this.treeView1);
            // 
            // splitContainerOuter.Panel2
            // 
            this.splitContainerOuter.Panel2.Controls.Add(this.splitContainerInner);
            this.splitContainerOuter.Size = new System.Drawing.Size(1428, 782);
            this.splitContainerOuter.SplitterDistance = 476;
            this.splitContainerOuter.SplitterWidth = 5;
            this.splitContainerOuter.TabIndex = 2;
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Margin = new System.Windows.Forms.Padding(4);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(474, 780);
            this.treeView1.TabIndex = 0;
            this.treeView1.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView1_BeforeExpand);
            this.treeView1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
            // 
            // splitContainerInner
            // 
            this.splitContainerInner.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerInner.Location = new System.Drawing.Point(0, 0);
            this.splitContainerInner.Margin = new System.Windows.Forms.Padding(4);
            this.splitContainerInner.Name = "splitContainerInner";
            this.splitContainerInner.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerInner.Panel1
            // 
            this.splitContainerInner.Panel1.Controls.Add(this.tabControl);
            // 
            // splitContainerInner.Panel2
            // 
            this.splitContainerInner.Panel2.Controls.Add(this.ButtomSplitContainer);
            this.splitContainerInner.Size = new System.Drawing.Size(945, 780);
            this.splitContainerInner.SplitterDistance = 158;
            this.splitContainerInner.SplitterWidth = 5;
            this.splitContainerInner.TabIndex = 0;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabRequest);
            this.tabControl.Controls.Add(this.tabCrudContext);
            this.tabControl.Controls.Add(this.tabResponse);
            this.tabControl.Controls.Add(this.tabPageRequestOptions);
            this.tabControl.Controls.Add(this.tabDocumentCollectionPolicy);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Margin = new System.Windows.Forms.Padding(4);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(945, 158);
            this.tabControl.TabIndex = 1;
            // 
            // tabRequest
            // 
            this.tabRequest.Controls.Add(this.tbRequest);
            this.tabRequest.Location = new System.Drawing.Point(4, 29);
            this.tabRequest.Margin = new System.Windows.Forms.Padding(4);
            this.tabRequest.Name = "tabRequest";
            this.tabRequest.Padding = new System.Windows.Forms.Padding(4);
            this.tabRequest.Size = new System.Drawing.Size(937, 125);
            this.tabRequest.TabIndex = 0;
            this.tabRequest.Text = "Request Headers";
            this.tabRequest.UseVisualStyleBackColor = true;
            // 
            // tbRequest
            // 
            this.tbRequest.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbRequest.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbRequest.Location = new System.Drawing.Point(4, 4);
            this.tbRequest.Margin = new System.Windows.Forms.Padding(4);
            this.tbRequest.Multiline = true;
            this.tbRequest.Name = "tbRequest";
            this.tbRequest.ReadOnly = true;
            this.tbRequest.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbRequest.Size = new System.Drawing.Size(929, 117);
            this.tbRequest.TabIndex = 0;
            // 
            // tabCrudContext
            // 
            this.tabCrudContext.Controls.Add(this.splitContainerIntabPage);
            this.tabCrudContext.Location = new System.Drawing.Point(4, 25);
            this.tabCrudContext.Margin = new System.Windows.Forms.Padding(4);
            this.tabCrudContext.Name = "tabCrudContext";
            this.tabCrudContext.Padding = new System.Windows.Forms.Padding(4);
            this.tabCrudContext.Size = new System.Drawing.Size(937, 132);
            this.tabCrudContext.TabIndex = 2;
            this.tabCrudContext.Text = "Operation Editor";
            this.tabCrudContext.UseVisualStyleBackColor = true;
            // 
            // splitContainerIntabPage
            // 
            this.splitContainerIntabPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerIntabPage.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainerIntabPage.Location = new System.Drawing.Point(4, 4);
            this.splitContainerIntabPage.Margin = new System.Windows.Forms.Padding(4);
            this.splitContainerIntabPage.Name = "splitContainerIntabPage";
            this.splitContainerIntabPage.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerIntabPage.Panel1
            // 
            this.splitContainerIntabPage.Panel1.Controls.Add(this.labelid);
            this.splitContainerIntabPage.Panel1.Controls.Add(this.textBoxforId);
            this.splitContainerIntabPage.Panel1MinSize = 35;
            // 
            // splitContainerIntabPage.Panel2
            // 
            this.splitContainerIntabPage.Panel2.Controls.Add(this.tbCrudContext);
            this.splitContainerIntabPage.Size = new System.Drawing.Size(929, 124);
            this.splitContainerIntabPage.SplitterDistance = 35;
            this.splitContainerIntabPage.SplitterWidth = 5;
            this.splitContainerIntabPage.TabIndex = 0;
            // 
            // labelid
            // 
            this.labelid.AutoSize = true;
            this.labelid.Location = new System.Drawing.Point(5, 5);
            this.labelid.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelid.Name = "labelid";
            this.labelid.Size = new System.Drawing.Size(24, 22);
            this.labelid.TabIndex = 2;
            this.labelid.Text = "Id";
            // 
            // textBoxforId
            // 
            this.textBoxforId.Location = new System.Drawing.Point(35, 3);
            this.textBoxforId.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxforId.Name = "textBoxforId";
            this.textBoxforId.Size = new System.Drawing.Size(389, 27);
            this.textBoxforId.TabIndex = 1;
            // 
            // tbCrudContext
            // 
            this.tbCrudContext.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbCrudContext.Location = new System.Drawing.Point(0, 0);
            this.tbCrudContext.Margin = new System.Windows.Forms.Padding(4);
            this.tbCrudContext.Multiline = true;
            this.tbCrudContext.Name = "tbCrudContext";
            this.tbCrudContext.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbCrudContext.Size = new System.Drawing.Size(929, 84);
            this.tbCrudContext.TabIndex = 0;
            this.tbCrudContext.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.tbCrudContext_PreviewKeyDown);
            // 
            // tabResponse
            // 
            this.tabResponse.Controls.Add(this.tbResponse);
            this.tabResponse.Location = new System.Drawing.Point(4, 25);
            this.tabResponse.Margin = new System.Windows.Forms.Padding(4);
            this.tabResponse.Name = "tabResponse";
            this.tabResponse.Padding = new System.Windows.Forms.Padding(4);
            this.tabResponse.Size = new System.Drawing.Size(937, 132);
            this.tabResponse.TabIndex = 1;
            this.tabResponse.Text = "Response Headers";
            this.tabResponse.UseVisualStyleBackColor = true;
            // 
            // tbResponse
            // 
            this.tbResponse.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbResponse.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbResponse.Location = new System.Drawing.Point(4, 4);
            this.tbResponse.Margin = new System.Windows.Forms.Padding(4);
            this.tbResponse.Multiline = true;
            this.tbResponse.Name = "tbResponse";
            this.tbResponse.ReadOnly = true;
            this.tbResponse.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbResponse.Size = new System.Drawing.Size(929, 124);
            this.tbResponse.TabIndex = 0;
            // 
            // tabPageRequestOptions
            // 
            this.tabPageRequestOptions.Controls.Add(this.cbRequestOptionsApply);
            this.tabPageRequestOptions.Controls.Add(this.labelPostTrigger);
            this.tabPageRequestOptions.Controls.Add(this.tbPostTrigger);
            this.tabPageRequestOptions.Controls.Add(this.label1);
            this.tabPageRequestOptions.Controls.Add(this.tbPreTrigger);
            this.tabPageRequestOptions.Controls.Add(this.groupBox3);
            this.tabPageRequestOptions.Controls.Add(this.groupBox2);
            this.tabPageRequestOptions.Controls.Add(this.groupBox1);
            this.tabPageRequestOptions.Location = new System.Drawing.Point(4, 25);
            this.tabPageRequestOptions.Name = "tabPageRequestOptions";
            this.tabPageRequestOptions.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageRequestOptions.Size = new System.Drawing.Size(937, 132);
            this.tabPageRequestOptions.TabIndex = 3;
            this.tabPageRequestOptions.Text = "RequestOptions";
            this.tabPageRequestOptions.UseVisualStyleBackColor = true;
            // 
            // cbRequestOptionsApply
            // 
            this.cbRequestOptionsApply.AutoSize = true;
            this.cbRequestOptionsApply.Checked = true;
            this.cbRequestOptionsApply.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbRequestOptionsApply.Location = new System.Drawing.Point(10, 10);
            this.cbRequestOptionsApply.Name = "cbRequestOptionsApply";
            this.cbRequestOptionsApply.Size = new System.Drawing.Size(123, 26);
            this.cbRequestOptionsApply.TabIndex = 7;
            this.cbRequestOptionsApply.Text = "Use default";
            this.cbRequestOptionsApply.UseVisualStyleBackColor = true;
            this.cbRequestOptionsApply.CheckedChanged += new System.EventHandler(this.cbRequestOptionsApply_CheckedChanged);
            // 
            // labelPostTrigger
            // 
            this.labelPostTrigger.AutoSize = true;
            this.labelPostTrigger.Location = new System.Drawing.Point(585, 63);
            this.labelPostTrigger.Name = "labelPostTrigger";
            this.labelPostTrigger.Size = new System.Drawing.Size(104, 22);
            this.labelPostTrigger.TabIndex = 6;
            this.labelPostTrigger.Text = "PostTrigger";
            // 
            // tbPostTrigger
            // 
            this.tbPostTrigger.Location = new System.Drawing.Point(585, 83);
            this.tbPostTrigger.Name = "tbPostTrigger";
            this.tbPostTrigger.Size = new System.Drawing.Size(348, 27);
            this.tbPostTrigger.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(584, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 22);
            this.label1.TabIndex = 4;
            this.label1.Text = "PreTrigger";
            // 
            // tbPreTrigger
            // 
            this.tbPreTrigger.Location = new System.Drawing.Point(584, 35);
            this.tbPreTrigger.Name = "tbPreTrigger";
            this.tbPreTrigger.Size = new System.Drawing.Size(348, 27);
            this.tbPreTrigger.TabIndex = 3;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.rbConsistencyEventual);
            this.groupBox3.Controls.Add(this.rbConsistencySession);
            this.groupBox3.Controls.Add(this.rbConsistencyBound);
            this.groupBox3.Controls.Add(this.rbConsistencyStrong);
            this.groupBox3.Location = new System.Drawing.Point(378, 6);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(200, 131);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "ConsistencyLevel";
            // 
            // rbConsistencyEventual
            // 
            this.rbConsistencyEventual.AutoSize = true;
            this.rbConsistencyEventual.Location = new System.Drawing.Point(6, 102);
            this.rbConsistencyEventual.Name = "rbConsistencyEventual";
            this.rbConsistencyEventual.Size = new System.Drawing.Size(101, 26);
            this.rbConsistencyEventual.TabIndex = 7;
            this.rbConsistencyEventual.Text = "Eventual";
            this.rbConsistencyEventual.UseVisualStyleBackColor = true;
            this.rbConsistencyEventual.CheckedChanged += new System.EventHandler(this.rbConsistencyEventual_CheckedChanged);
            // 
            // rbConsistencySession
            // 
            this.rbConsistencySession.AutoSize = true;
            this.rbConsistencySession.Checked = true;
            this.rbConsistencySession.Location = new System.Drawing.Point(6, 76);
            this.rbConsistencySession.Name = "rbConsistencySession";
            this.rbConsistencySession.Size = new System.Drawing.Size(95, 26);
            this.rbConsistencySession.TabIndex = 6;
            this.rbConsistencySession.TabStop = true;
            this.rbConsistencySession.Text = "Session";
            this.rbConsistencySession.UseVisualStyleBackColor = true;
            this.rbConsistencySession.CheckedChanged += new System.EventHandler(this.rbConsistencySession_CheckedChanged);
            // 
            // rbConsistencyBound
            // 
            this.rbConsistencyBound.AutoSize = true;
            this.rbConsistencyBound.Location = new System.Drawing.Point(6, 49);
            this.rbConsistencyBound.Name = "rbConsistencyBound";
            this.rbConsistencyBound.Size = new System.Drawing.Size(182, 26);
            this.rbConsistencyBound.TabIndex = 5;
            this.rbConsistencyBound.Text = "BoundedStaleness";
            this.rbConsistencyBound.UseVisualStyleBackColor = true;
            this.rbConsistencyBound.CheckedChanged += new System.EventHandler(this.rbConsistencyBound_CheckedChanged);
            // 
            // rbConsistencyStrong
            // 
            this.rbConsistencyStrong.AutoSize = true;
            this.rbConsistencyStrong.Location = new System.Drawing.Point(6, 22);
            this.rbConsistencyStrong.Name = "rbConsistencyStrong";
            this.rbConsistencyStrong.Size = new System.Drawing.Size(84, 26);
            this.rbConsistencyStrong.TabIndex = 4;
            this.rbConsistencyStrong.Text = "Strong";
            this.rbConsistencyStrong.UseVisualStyleBackColor = true;
            this.rbConsistencyStrong.CheckedChanged += new System.EventHandler(this.rbConsistencyStrong_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rbAccessConditionIfNoneMatch);
            this.groupBox2.Controls.Add(this.rbAccessConditionIfMatch);
            this.groupBox2.Controls.Add(this.tbAccessConditionText);
            this.groupBox2.Location = new System.Drawing.Point(161, 37);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 100);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "AccessCondition";
            // 
            // rbAccessConditionIfNoneMatch
            // 
            this.rbAccessConditionIfNoneMatch.AutoSize = true;
            this.rbAccessConditionIfNoneMatch.Location = new System.Drawing.Point(17, 46);
            this.rbAccessConditionIfNoneMatch.Name = "rbAccessConditionIfNoneMatch";
            this.rbAccessConditionIfNoneMatch.Size = new System.Drawing.Size(131, 26);
            this.rbAccessConditionIfNoneMatch.TabIndex = 4;
            this.rbAccessConditionIfNoneMatch.Text = "IfNoneMatch";
            this.rbAccessConditionIfNoneMatch.UseVisualStyleBackColor = true;
            this.rbAccessConditionIfNoneMatch.CheckedChanged += new System.EventHandler(this.rbAccessConditionIfNoneMatch_CheckedChanged);
            // 
            // rbAccessConditionIfMatch
            // 
            this.rbAccessConditionIfMatch.AutoSize = true;
            this.rbAccessConditionIfMatch.Checked = true;
            this.rbAccessConditionIfMatch.Location = new System.Drawing.Point(17, 22);
            this.rbAccessConditionIfMatch.Name = "rbAccessConditionIfMatch";
            this.rbAccessConditionIfMatch.Size = new System.Drawing.Size(88, 26);
            this.rbAccessConditionIfMatch.TabIndex = 3;
            this.rbAccessConditionIfMatch.TabStop = true;
            this.rbAccessConditionIfMatch.Text = "IfMatch";
            this.rbAccessConditionIfMatch.UseVisualStyleBackColor = true;
            this.rbAccessConditionIfMatch.CheckedChanged += new System.EventHandler(this.rbAccessConditionIfMatch_CheckedChanged);
            // 
            // tbAccessConditionText
            // 
            this.tbAccessConditionText.Location = new System.Drawing.Point(17, 67);
            this.tbAccessConditionText.Name = "tbAccessConditionText";
            this.tbAccessConditionText.Size = new System.Drawing.Size(177, 27);
            this.tbAccessConditionText.TabIndex = 2;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbIndexingExclude);
            this.groupBox1.Controls.Add(this.rbIndexingInclude);
            this.groupBox1.Controls.Add(this.rbIndexingDefault);
            this.groupBox1.Location = new System.Drawing.Point(3, 37);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(149, 94);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "IndexingDirective";
            // 
            // rbIndexingExclude
            // 
            this.rbIndexingExclude.AutoSize = true;
            this.rbIndexingExclude.Location = new System.Drawing.Point(7, 71);
            this.rbIndexingExclude.Name = "rbIndexingExclude";
            this.rbIndexingExclude.Size = new System.Drawing.Size(95, 26);
            this.rbIndexingExclude.TabIndex = 2;
            this.rbIndexingExclude.Text = "Exclude";
            this.rbIndexingExclude.UseVisualStyleBackColor = true;
            this.rbIndexingExclude.CheckedChanged += new System.EventHandler(this.rbIndexingExclude_CheckedChanged);
            // 
            // rbIndexingInclude
            // 
            this.rbIndexingInclude.AutoSize = true;
            this.rbIndexingInclude.Location = new System.Drawing.Point(7, 46);
            this.rbIndexingInclude.Name = "rbIndexingInclude";
            this.rbIndexingInclude.Size = new System.Drawing.Size(88, 26);
            this.rbIndexingInclude.TabIndex = 1;
            this.rbIndexingInclude.Text = "Include";
            this.rbIndexingInclude.UseVisualStyleBackColor = true;
            this.rbIndexingInclude.CheckedChanged += new System.EventHandler(this.rbIndexingInclude_CheckedChanged);
            // 
            // rbIndexingDefault
            // 
            this.rbIndexingDefault.AutoSize = true;
            this.rbIndexingDefault.Checked = true;
            this.rbIndexingDefault.Location = new System.Drawing.Point(7, 23);
            this.rbIndexingDefault.Name = "rbIndexingDefault";
            this.rbIndexingDefault.Size = new System.Drawing.Size(88, 26);
            this.rbIndexingDefault.TabIndex = 0;
            this.rbIndexingDefault.TabStop = true;
            this.rbIndexingDefault.Text = "Default";
            this.rbIndexingDefault.UseVisualStyleBackColor = true;
            this.rbIndexingDefault.CheckedChanged += new System.EventHandler(this.rbIndexingDefault_CheckedChanged);
            // 
            // tabDocumentCollectionPolicy
            // 
            this.tabDocumentCollectionPolicy.Controls.Add(this.groupBox5);
            this.tabDocumentCollectionPolicy.Controls.Add(this.btnRemoveExcludedPath);
            this.tabDocumentCollectionPolicy.Controls.Add(this.btnAddExcludedPath);
            this.tabDocumentCollectionPolicy.Controls.Add(this.label3);
            this.tabDocumentCollectionPolicy.Controls.Add(this.groupBox4);
            this.tabDocumentCollectionPolicy.Controls.Add(this.lbExcludedPath);
            this.tabDocumentCollectionPolicy.Controls.Add(this.btnEdit);
            this.tabDocumentCollectionPolicy.Controls.Add(this.btnRemovePath);
            this.tabDocumentCollectionPolicy.Controls.Add(this.label2);
            this.tabDocumentCollectionPolicy.Controls.Add(this.btnAddIncludePath);
            this.tabDocumentCollectionPolicy.Controls.Add(this.lbIncludedPath);
            this.tabDocumentCollectionPolicy.Controls.Add(this.labelCollectionId);
            this.tabDocumentCollectionPolicy.Controls.Add(this.tbCollectionId);
            this.tabDocumentCollectionPolicy.Controls.Add(this.cbIndexingPolicyDefault);
            this.tabDocumentCollectionPolicy.Controls.Add(this.cbAutomatic);
            this.tabDocumentCollectionPolicy.Location = new System.Drawing.Point(4, 25);
            this.tabDocumentCollectionPolicy.Name = "tabDocumentCollectionPolicy";
            this.tabDocumentCollectionPolicy.Padding = new System.Windows.Forms.Padding(3);
            this.tabDocumentCollectionPolicy.Size = new System.Drawing.Size(937, 132);
            this.tabDocumentCollectionPolicy.TabIndex = 4;
            this.tabDocumentCollectionPolicy.Text = "DocumentCollection";
            this.tabDocumentCollectionPolicy.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.rbOfferS3);
            this.groupBox5.Controls.Add(this.rbOfferS2);
            this.groupBox5.Controls.Add(this.rbOfferS1);
            this.groupBox5.Location = new System.Drawing.Point(10, 36);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(186, 51);
            this.groupBox5.TabIndex = 15;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "OfferType";
            // 
            // rbOfferS3
            // 
            this.rbOfferS3.AutoSize = true;
            this.rbOfferS3.Location = new System.Drawing.Point(133, 22);
            this.rbOfferS3.Name = "rbOfferS3";
            this.rbOfferS3.Size = new System.Drawing.Size(53, 26);
            this.rbOfferS3.TabIndex = 2;
            this.rbOfferS3.Text = "S3";
            this.rbOfferS3.UseVisualStyleBackColor = true;
            this.rbOfferS3.CheckedChanged += new System.EventHandler(this.rbOfferS3_CheckedChanged);
            // 
            // rbOfferS2
            // 
            this.rbOfferS2.AutoSize = true;
            this.rbOfferS2.Location = new System.Drawing.Point(74, 22);
            this.rbOfferS2.Name = "rbOfferS2";
            this.rbOfferS2.Size = new System.Drawing.Size(53, 26);
            this.rbOfferS2.TabIndex = 1;
            this.rbOfferS2.Text = "S2";
            this.rbOfferS2.UseVisualStyleBackColor = true;
            this.rbOfferS2.CheckedChanged += new System.EventHandler(this.rbOfferS2_CheckedChanged);
            // 
            // rbOfferS1
            // 
            this.rbOfferS1.AutoSize = true;
            this.rbOfferS1.Checked = true;
            this.rbOfferS1.Location = new System.Drawing.Point(16, 22);
            this.rbOfferS1.Name = "rbOfferS1";
            this.rbOfferS1.Size = new System.Drawing.Size(53, 26);
            this.rbOfferS1.TabIndex = 0;
            this.rbOfferS1.TabStop = true;
            this.rbOfferS1.Text = "S1";
            this.rbOfferS1.UseVisualStyleBackColor = true;
            this.rbOfferS1.CheckedChanged += new System.EventHandler(this.rbOfferS1_CheckedChanged);
            // 
            // btnRemoveExcludedPath
            // 
            this.btnRemoveExcludedPath.Enabled = false;
            this.btnRemoveExcludedPath.Location = new System.Drawing.Point(856, 102);
            this.btnRemoveExcludedPath.Name = "btnRemoveExcludedPath";
            this.btnRemoveExcludedPath.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveExcludedPath.TabIndex = 13;
            this.btnRemoveExcludedPath.Text = "Remove";
            this.btnRemoveExcludedPath.UseVisualStyleBackColor = true;
            this.btnRemoveExcludedPath.Click += new System.EventHandler(this.btnRemoveExcludedPath_Click);
            // 
            // btnAddExcludedPath
            // 
            this.btnAddExcludedPath.Location = new System.Drawing.Point(765, 102);
            this.btnAddExcludedPath.Name = "btnAddExcludedPath";
            this.btnAddExcludedPath.Size = new System.Drawing.Size(76, 23);
            this.btnAddExcludedPath.TabIndex = 12;
            this.btnAddExcludedPath.Text = "Add";
            this.btnAddExcludedPath.UseVisualStyleBackColor = true;
            this.btnAddExcludedPath.Click += new System.EventHandler(this.btnAddExcludedPath_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(680, 3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(126, 22);
            this.label3.TabIndex = 11;
            this.label3.Text = "Excluded Path";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.rbLazy);
            this.groupBox4.Controls.Add(this.rbConsistent);
            this.groupBox4.Location = new System.Drawing.Point(221, 17);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(128, 101);
            this.groupBox4.TabIndex = 1;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "IndexingMode";
            // 
            // rbLazy
            // 
            this.rbLazy.AutoSize = true;
            this.rbLazy.Location = new System.Drawing.Point(23, 61);
            this.rbLazy.Name = "rbLazy";
            this.rbLazy.Size = new System.Drawing.Size(68, 26);
            this.rbLazy.TabIndex = 1;
            this.rbLazy.Text = "Lazy";
            this.rbLazy.UseVisualStyleBackColor = true;
            this.rbLazy.CheckedChanged += new System.EventHandler(this.rbLazy_CheckedChanged);
            // 
            // rbConsistent
            // 
            this.rbConsistent.AutoSize = true;
            this.rbConsistent.Checked = true;
            this.rbConsistent.Location = new System.Drawing.Point(23, 34);
            this.rbConsistent.Name = "rbConsistent";
            this.rbConsistent.Size = new System.Drawing.Size(116, 26);
            this.rbConsistent.TabIndex = 0;
            this.rbConsistent.TabStop = true;
            this.rbConsistent.Text = "Consistent";
            this.rbConsistent.UseVisualStyleBackColor = true;
            this.rbConsistent.CheckedChanged += new System.EventHandler(this.rbConsistent_CheckedChanged);
            // 
            // lbExcludedPath
            // 
            this.lbExcludedPath.FormattingEnabled = true;
            this.lbExcludedPath.ItemHeight = 20;
            this.lbExcludedPath.Location = new System.Drawing.Point(674, 24);
            this.lbExcludedPath.Name = "lbExcludedPath";
            this.lbExcludedPath.Size = new System.Drawing.Size(260, 64);
            this.lbExcludedPath.TabIndex = 10;
            this.lbExcludedPath.SelectedIndexChanged += new System.EventHandler(this.lbExcludedPath_SelectedIndexChanged);
            // 
            // btnEdit
            // 
            this.btnEdit.Enabled = false;
            this.btnEdit.Location = new System.Drawing.Point(575, 102);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(81, 23);
            this.btnEdit.TabIndex = 9;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnRemovePath
            // 
            this.btnRemovePath.Enabled = false;
            this.btnRemovePath.Location = new System.Drawing.Point(469, 102);
            this.btnRemovePath.Name = "btnRemovePath";
            this.btnRemovePath.Size = new System.Drawing.Size(75, 23);
            this.btnRemovePath.TabIndex = 8;
            this.btnRemovePath.Text = "Remove";
            this.btnRemovePath.UseVisualStyleBackColor = true;
            this.btnRemovePath.Click += new System.EventHandler(this.btnRemovePath_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(355, 4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(119, 22);
            this.label2.TabIndex = 7;
            this.label2.Text = "Included Path";
            // 
            // btnAddIncludePath
            // 
            this.btnAddIncludePath.Location = new System.Drawing.Point(358, 102);
            this.btnAddIncludePath.Name = "btnAddIncludePath";
            this.btnAddIncludePath.Size = new System.Drawing.Size(76, 23);
            this.btnAddIncludePath.TabIndex = 6;
            this.btnAddIncludePath.Text = "Add";
            this.btnAddIncludePath.UseVisualStyleBackColor = true;
            this.btnAddIncludePath.Click += new System.EventHandler(this.btnAddIncludePath_Click);
            // 
            // lbIncludedPath
            // 
            this.lbIncludedPath.FormattingEnabled = true;
            this.lbIncludedPath.ItemHeight = 20;
            this.lbIncludedPath.Location = new System.Drawing.Point(358, 24);
            this.lbIncludedPath.Name = "lbIncludedPath";
            this.lbIncludedPath.Size = new System.Drawing.Size(298, 64);
            this.lbIncludedPath.TabIndex = 5;
            this.lbIncludedPath.SelectedIndexChanged += new System.EventHandler(this.lbIncludedPath_SelectedIndexChanged);
            // 
            // labelCollectionId
            // 
            this.labelCollectionId.AutoSize = true;
            this.labelCollectionId.Location = new System.Drawing.Point(7, 10);
            this.labelCollectionId.Name = "labelCollectionId";
            this.labelCollectionId.Size = new System.Drawing.Size(29, 22);
            this.labelCollectionId.TabIndex = 4;
            this.labelCollectionId.Text = "Id:";
            // 
            // tbCollectionId
            // 
            this.tbCollectionId.Location = new System.Drawing.Point(36, 7);
            this.tbCollectionId.Name = "tbCollectionId";
            this.tbCollectionId.Size = new System.Drawing.Size(160, 27);
            this.tbCollectionId.TabIndex = 3;
            this.tbCollectionId.Text = "DocumentCollection Id";
            // 
            // cbIndexingPolicyDefault
            // 
            this.cbIndexingPolicyDefault.AutoSize = true;
            this.cbIndexingPolicyDefault.Checked = true;
            this.cbIndexingPolicyDefault.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbIndexingPolicyDefault.Location = new System.Drawing.Point(10, 97);
            this.cbIndexingPolicyDefault.Name = "cbIndexingPolicyDefault";
            this.cbIndexingPolicyDefault.Size = new System.Drawing.Size(89, 26);
            this.cbIndexingPolicyDefault.TabIndex = 2;
            this.cbIndexingPolicyDefault.Text = "Default";
            this.cbIndexingPolicyDefault.UseVisualStyleBackColor = true;
            this.cbIndexingPolicyDefault.CheckedChanged += new System.EventHandler(this.cbIndexingPolicyDefault_CheckedChanged);
            // 
            // cbAutomatic
            // 
            this.cbAutomatic.AutoSize = true;
            this.cbAutomatic.Checked = true;
            this.cbAutomatic.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAutomatic.Location = new System.Drawing.Point(88, 97);
            this.cbAutomatic.Name = "cbAutomatic";
            this.cbAutomatic.Size = new System.Drawing.Size(111, 26);
            this.cbAutomatic.TabIndex = 0;
            this.cbAutomatic.Text = "Automatic";
            this.cbAutomatic.UseVisualStyleBackColor = true;
            this.cbAutomatic.CheckedChanged += new System.EventHandler(this.cbAutomatic_CheckedChanged);
            // 
            // ButtomSplitContainer
            // 
            this.ButtomSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ButtomSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.ButtomSplitContainer.Name = "ButtomSplitContainer";
            this.ButtomSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // ButtomSplitContainer.Panel1
            // 
            this.ButtomSplitContainer.Panel1.Controls.Add(this.triggerPanel);
            this.ButtomSplitContainer.Panel1.Controls.Add(this.feedToolStrip);
            this.ButtomSplitContainer.Panel1MinSize = 0;
            // 
            // ButtomSplitContainer.Panel2
            // 
            this.ButtomSplitContainer.Panel2.Controls.Add(this.webBrowserResponse);
            this.ButtomSplitContainer.Size = new System.Drawing.Size(945, 617);
            this.ButtomSplitContainer.SplitterDistance = 33;
            this.ButtomSplitContainer.TabIndex = 4;
            // 
            // triggerPanel
            // 
            this.triggerPanel.Controls.Add(this.rbPostTrigger);
            this.triggerPanel.Controls.Add(this.rbPreTrigger);
            this.triggerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.triggerPanel.Location = new System.Drawing.Point(0, 0);
            this.triggerPanel.Name = "triggerPanel";
            this.triggerPanel.Size = new System.Drawing.Size(945, 33);
            this.triggerPanel.TabIndex = 4;
            // 
            // rbPostTrigger
            // 
            this.rbPostTrigger.AutoSize = true;
            this.rbPostTrigger.Location = new System.Drawing.Point(119, 7);
            this.rbPostTrigger.Name = "rbPostTrigger";
            this.rbPostTrigger.Size = new System.Drawing.Size(125, 26);
            this.rbPostTrigger.TabIndex = 1;
            this.rbPostTrigger.TabStop = true;
            this.rbPostTrigger.Text = "PostTrigger";
            this.rbPostTrigger.UseVisualStyleBackColor = true;
            // 
            // rbPreTrigger
            // 
            this.rbPreTrigger.AutoSize = true;
            this.rbPreTrigger.Checked = true;
            this.rbPreTrigger.Location = new System.Drawing.Point(19, 7);
            this.rbPreTrigger.Name = "rbPreTrigger";
            this.rbPreTrigger.Size = new System.Drawing.Size(117, 26);
            this.rbPreTrigger.TabIndex = 0;
            this.rbPreTrigger.TabStop = true;
            this.rbPreTrigger.Text = "PreTrigger";
            this.rbPreTrigger.UseVisualStyleBackColor = true;
            // 
            // feedToolStrip
            // 
            this.feedToolStrip.Dock = System.Windows.Forms.DockStyle.Fill;
            this.feedToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.feedToolStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.feedToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripTextMaxItemCount,
            this.btnExecuteNext});
            this.feedToolStrip.Location = new System.Drawing.Point(0, 0);
            this.feedToolStrip.Name = "feedToolStrip";
            this.feedToolStrip.Size = new System.Drawing.Size(945, 33);
            this.feedToolStrip.TabIndex = 3;
            this.feedToolStrip.Text = "toolStrip1";
            // 
            // toolStripTextMaxItemCount
            // 
            this.toolStripTextMaxItemCount.Name = "toolStripTextMaxItemCount";
            this.toolStripTextMaxItemCount.Size = new System.Drawing.Size(50, 33);
            this.toolStripTextMaxItemCount.Text = "10";
            this.toolStripTextMaxItemCount.TextBoxTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnExecuteNext
            // 
            this.btnExecuteNext.Image = global::Microsoft.Azure.DocumentDBStudio.Properties.Resources.NextPagepng;
            this.btnExecuteNext.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnExecuteNext.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.btnExecuteNext.Name = "btnExecuteNext";
            this.btnExecuteNext.Size = new System.Drawing.Size(100, 30);
            this.btnExecuteNext.Text = "Next Page";
            this.btnExecuteNext.Click += new System.EventHandler(this.btnExecuteNext_Click);
            // 
            // webBrowserResponse
            // 
            this.webBrowserResponse.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowserResponse.Location = new System.Drawing.Point(0, 0);
            this.webBrowserResponse.Margin = new System.Windows.Forms.Padding(4);
            this.webBrowserResponse.MinimumSize = new System.Drawing.Size(27, 26);
            this.webBrowserResponse.Name = "webBrowserResponse";
            this.webBrowserResponse.Size = new System.Drawing.Size(945, 580);
            this.webBrowserResponse.TabIndex = 0;
            // 
            // tsMenu
            // 
            this.tsMenu.BackColor = System.Drawing.SystemColors.Control;
            this.tsMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.tsMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnBack,
            this.btnForward,
            this.btnHome,
            this.toolStripBtnExecute,
            this.btnRefresh,
            this.toolStripSeparator1,
            this.btnHeaders,
            this.btnEditRequests,
            this.tsbViewType});
            this.tsMenu.Location = new System.Drawing.Point(0, 30);
            this.tsMenu.Name = "tsMenu";
            this.tsMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.tsMenu.Size = new System.Drawing.Size(1428, 27);
            this.tsMenu.TabIndex = 3;
            this.tsMenu.Text = "toolStrip2";
            // 
            // btnBack
            // 
            this.btnBack.Image = global::Microsoft.Azure.DocumentDBStudio.Properties.Resources.NavBack;
            this.btnBack.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(64, 24);
            this.btnBack.Text = "Back";
            // 
            // btnForward
            // 
            this.btnForward.Enabled = false;
            this.btnForward.Image = global::Microsoft.Azure.DocumentDBStudio.Properties.Resources.NavForward;
            this.btnForward.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnForward.Name = "btnForward";
            this.btnForward.Size = new System.Drawing.Size(87, 24);
            this.btnForward.Text = "Forward";
            // 
            // btnHome
            // 
            this.btnHome.Image = global::Microsoft.Azure.DocumentDBStudio.Properties.Resources.HomeHS;
            this.btnHome.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnHome.Name = "btnHome";
            this.btnHome.Size = new System.Drawing.Size(74, 24);
            this.btnHome.Text = "Home";
            this.btnHome.Click += new System.EventHandler(this.btnHome_Click);
            // 
            // toolStripBtnExecute
            // 
            this.toolStripBtnExecute.Image = ((System.Drawing.Image)(resources.GetObject("toolStripBtnExecute.Image")));
            this.toolStripBtnExecute.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripBtnExecute.Name = "toolStripBtnExecute";
            this.toolStripBtnExecute.Size = new System.Drawing.Size(84, 24);
            this.toolStripBtnExecute.Text = "Execute";
            this.toolStripBtnExecute.Click += new System.EventHandler(this.toolStripBtnExecute_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Image = global::Microsoft.Azure.DocumentDBStudio.Properties.Resources.RefreshDocViewHS;
            this.btnRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(82, 24);
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.Visible = false;
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // btnHeaders
            // 
            this.btnHeaders.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnHeaders.Image = ((System.Drawing.Image)(resources.GetObject("btnHeaders.Image")));
            this.btnHeaders.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnHeaders.Name = "btnHeaders";
            this.btnHeaders.Size = new System.Drawing.Size(175, 24);
            this.btnHeaders.Text = "Show Response Headers";
            this.btnHeaders.ToolTipText = "Show response headers";
            this.btnHeaders.Click += new System.EventHandler(this.btnHeaders_Click);
            // 
            // btnEditRequests
            // 
            this.btnEditRequests.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnEditRequests.Image = ((System.Drawing.Image)(resources.GetObject("btnEditRequests.Image")));
            this.btnEditRequests.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnEditRequests.Name = "btnEditRequests";
            this.btnEditRequests.Size = new System.Drawing.Size(96, 24);
            this.btnEditRequests.Text = "Edit Request";
            this.btnEditRequests.ToolTipText = "Edit next request  (Ctrl+Click)";
            this.btnEditRequests.Visible = false;
            // 
            // tsbViewType
            // 
            this.tsbViewType.CheckOnClick = true;
            this.tsbViewType.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbViewType.Image = ((System.Drawing.Image)(resources.GetObject("tsbViewType.Image")));
            this.tsbViewType.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbViewType.Name = "tsbViewType";
            this.tsbViewType.Size = new System.Drawing.Size(76, 24);
            this.tsbViewType.Text = "Text View";
            this.tsbViewType.Click += new System.EventHandler(this.tsbViewType_Click);
            // 
            // tsAddress
            // 
            this.tsAddress.AutoSize = false;
            this.tsAddress.BackColor = System.Drawing.SystemColors.Control;
            this.tsAddress.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.tsAddress.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsLabelUrl,
            this.cbUrl,
            this.btnGo});
            this.tsAddress.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.tsAddress.Location = new System.Drawing.Point(0, 50);
            this.tsAddress.Name = "tsAddress";
            this.tsAddress.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.tsAddress.Size = new System.Drawing.Size(1428, 33);
            this.tsAddress.TabIndex = 6;
            this.tsAddress.Text = "toolStrip3";
            this.tsAddress.Visible = false;
            // 
            // tsLabelUrl
            // 
            this.tsLabelUrl.Name = "tsLabelUrl";
            this.tsLabelUrl.Size = new System.Drawing.Size(42, 30);
            this.tsLabelUrl.Text = "URL: ";
            // 
            // cbUrl
            // 
            this.cbUrl.AutoSize = false;
            this.cbUrl.MaxDropDownItems = 1;
            this.cbUrl.Name = "cbUrl";
            this.cbUrl.Size = new System.Drawing.Size(300, 28);
            // 
            // btnGo
            // 
            this.btnGo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnGo.Image = ((System.Drawing.Image)(resources.GetObject("btnGo.Image")));
            this.btnGo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(32, 30);
            this.btnGo.Text = "Go";
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(8, 3, 0, 3);
            this.menuStrip1.Size = new System.Drawing.Size(1428, 30);
            this.menuStrip1.TabIndex = 5;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(44, 24);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(170, 26);
            this.optionsToolStripMenuItem.Text = "&Add Account";
            this.optionsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(170, 26);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(53, 24);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(125, 26);
            this.aboutToolStripMenuItem.Text = "&About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // tsTablesEntities
            // 
            this.tsTablesEntities.AutoSize = false;
            this.tsTablesEntities.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.tsTablesEntities.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnQueryTable,
            this.btnCreateTable,
            this.btnDeleteTable,
            this.toolStripSeparator2,
            this.btnQueryEntities,
            this.btnNextPage,
            this.btnInsertEntity,
            this.btnUpdateEntity,
            this.tsbMergeEntity,
            this.btnDeleteEntity,
            this.toolStripSeparator3,
            this.tsbEditTemplate});
            this.tsTablesEntities.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.tsTablesEntities.Location = new System.Drawing.Point(0, 97);
            this.tsTablesEntities.Name = "tsTablesEntities";
            this.tsTablesEntities.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.tsTablesEntities.Size = new System.Drawing.Size(1428, 33);
            this.tsTablesEntities.TabIndex = 4;
            this.tsTablesEntities.Text = "toolStrip1";
            this.tsTablesEntities.Visible = false;
            // 
            // btnQueryTable
            // 
            this.btnQueryTable.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnQueryTable.Image = ((System.Drawing.Image)(resources.GetObject("btnQueryTable.Image")));
            this.btnQueryTable.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnQueryTable.Name = "btnQueryTable";
            this.btnQueryTable.Size = new System.Drawing.Size(60, 30);
            this.btnQueryTable.Text = "QueryT";
            this.btnQueryTable.ToolTipText = "QueryTable";
            // 
            // btnCreateTable
            // 
            this.btnCreateTable.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnCreateTable.Image = ((System.Drawing.Image)(resources.GetObject("btnCreateTable.Image")));
            this.btnCreateTable.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCreateTable.Name = "btnCreateTable";
            this.btnCreateTable.Size = new System.Drawing.Size(64, 30);
            this.btnCreateTable.Text = "CreateT";
            this.btnCreateTable.ToolTipText = "CreateTable";
            // 
            // btnDeleteTable
            // 
            this.btnDeleteTable.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnDeleteTable.Image = ((System.Drawing.Image)(resources.GetObject("btnDeleteTable.Image")));
            this.btnDeleteTable.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDeleteTable.Name = "btnDeleteTable";
            this.btnDeleteTable.Size = new System.Drawing.Size(65, 30);
            this.btnDeleteTable.Text = "DeleteT";
            this.btnDeleteTable.ToolTipText = "DeleteTable";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 33);
            // 
            // btnQueryEntities
            // 
            this.btnQueryEntities.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnQueryEntities.Image = ((System.Drawing.Image)(resources.GetObject("btnQueryEntities.Image")));
            this.btnQueryEntities.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnQueryEntities.Name = "btnQueryEntities";
            this.btnQueryEntities.Size = new System.Drawing.Size(60, 30);
            this.btnQueryEntities.Text = "QueryE";
            this.btnQueryEntities.ToolTipText = "QueryEntities";
            // 
            // btnNextPage
            // 
            this.btnNextPage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnNextPage.Enabled = false;
            this.btnNextPage.Image = ((System.Drawing.Image)(resources.GetObject("btnNextPage.Image")));
            this.btnNextPage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnNextPage.Name = "btnNextPage";
            this.btnNextPage.Size = new System.Drawing.Size(76, 30);
            this.btnNextPage.Text = "NextPage";
            // 
            // btnInsertEntity
            // 
            this.btnInsertEntity.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnInsertEntity.Image = ((System.Drawing.Image)(resources.GetObject("btnInsertEntity.Image")));
            this.btnInsertEntity.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnInsertEntity.Name = "btnInsertEntity";
            this.btnInsertEntity.Size = new System.Drawing.Size(57, 30);
            this.btnInsertEntity.Text = "InsertE";
            this.btnInsertEntity.ToolTipText = "InsertEntity";
            // 
            // btnUpdateEntity
            // 
            this.btnUpdateEntity.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnUpdateEntity.Image = ((System.Drawing.Image)(resources.GetObject("btnUpdateEntity.Image")));
            this.btnUpdateEntity.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnUpdateEntity.Name = "btnUpdateEntity";
            this.btnUpdateEntity.Size = new System.Drawing.Size(70, 30);
            this.btnUpdateEntity.Text = "UpdateE";
            this.btnUpdateEntity.ToolTipText = "UpdateEntity";
            // 
            // tsbMergeEntity
            // 
            this.tsbMergeEntity.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbMergeEntity.Image = ((System.Drawing.Image)(resources.GetObject("tsbMergeEntity.Image")));
            this.tsbMergeEntity.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbMergeEntity.Name = "tsbMergeEntity";
            this.tsbMergeEntity.Size = new System.Drawing.Size(64, 30);
            this.tsbMergeEntity.Text = "MergeE";
            // 
            // btnDeleteEntity
            // 
            this.btnDeleteEntity.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnDeleteEntity.Image = ((System.Drawing.Image)(resources.GetObject("btnDeleteEntity.Image")));
            this.btnDeleteEntity.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDeleteEntity.Name = "btnDeleteEntity";
            this.btnDeleteEntity.Size = new System.Drawing.Size(65, 30);
            this.btnDeleteEntity.Text = "DeleteE";
            this.btnDeleteEntity.ToolTipText = "DeleteEntity";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 33);
            // 
            // tsbEditTemplate
            // 
            this.tsbEditTemplate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbEditTemplate.Image = ((System.Drawing.Image)(resources.GetObject("tsbEditTemplate.Image")));
            this.tsbEditTemplate.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbEditTemplate.Name = "tsbEditTemplate";
            this.tsbEditTemplate.Size = new System.Drawing.Size(105, 30);
            this.tsbEditTemplate.Text = "Edit Template";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1428, 865);
            this.Controls.Add(this.splitContainerOuter);
            this.Controls.Add(this.tsAddress);
            this.Controls.Add(this.tsMenu);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.tsTablesEntities);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MainForm";
            this.Text = "Azure DocumentDB Studio";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.splitContainerOuter.Panel1.ResumeLayout(false);
            this.splitContainerOuter.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerOuter)).EndInit();
            this.splitContainerOuter.ResumeLayout(false);
            this.splitContainerInner.Panel1.ResumeLayout(false);
            this.splitContainerInner.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerInner)).EndInit();
            this.splitContainerInner.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.tabRequest.ResumeLayout(false);
            this.tabRequest.PerformLayout();
            this.tabCrudContext.ResumeLayout(false);
            this.splitContainerIntabPage.Panel1.ResumeLayout(false);
            this.splitContainerIntabPage.Panel1.PerformLayout();
            this.splitContainerIntabPage.Panel2.ResumeLayout(false);
            this.splitContainerIntabPage.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerIntabPage)).EndInit();
            this.splitContainerIntabPage.ResumeLayout(false);
            this.tabResponse.ResumeLayout(false);
            this.tabResponse.PerformLayout();
            this.tabPageRequestOptions.ResumeLayout(false);
            this.tabPageRequestOptions.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabDocumentCollectionPolicy.ResumeLayout(false);
            this.tabDocumentCollectionPolicy.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ButtomSplitContainer.Panel1.ResumeLayout(false);
            this.ButtomSplitContainer.Panel1.PerformLayout();
            this.ButtomSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ButtomSplitContainer)).EndInit();
            this.ButtomSplitContainer.ResumeLayout(false);
            this.triggerPanel.ResumeLayout(false);
            this.triggerPanel.PerformLayout();
            this.feedToolStrip.ResumeLayout(false);
            this.feedToolStrip.PerformLayout();
            this.tsMenu.ResumeLayout(false);
            this.tsMenu.PerformLayout();
            this.tsAddress.ResumeLayout(false);
            this.tsAddress.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tsTablesEntities.ResumeLayout(false);
            this.tsTablesEntities.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.SplitContainer splitContainerOuter;
        private System.Windows.Forms.SplitContainer splitContainerInner;
        //private System.Windows.Forms.WebBrowser webBrowserResponse;
        private System.Windows.Forms.WebBrowser webBrowserResponse;
        private System.Windows.Forms.ToolStripStatusLabel tsStatus;
        private System.Windows.Forms.ToolStrip tsMenu;
        private System.Windows.Forms.ToolStrip tsAddress;
        private System.Windows.Forms.ToolStripLabel tsLabelUrl;
        private System.Windows.Forms.ToolStripButton btnGo;
        private System.Windows.Forms.ToolStripSplitButton tsButtonZoom;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnBack;
        private System.Windows.Forms.ToolStripComboBox cbUrl;
        private System.Windows.Forms.ToolStripProgressBar tsProgress;
        private System.Windows.Forms.ToolStripButton btnHeaders;
        private System.Windows.Forms.ToolStripButton btnHome;
        private System.Windows.Forms.ToolStripButton btnEditRequests;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabRequest;
        private System.Windows.Forms.TabPage tabResponse;
        private System.Windows.Forms.TextBox tbRequest;
        private System.Windows.Forms.TextBox tbResponse;
        private System.Windows.Forms.ToolStripButton btnForward;
        private System.Windows.Forms.ToolStripButton btnRefresh;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStrip tsTablesEntities;
        private System.Windows.Forms.ToolStripButton btnQueryTable;
        private System.Windows.Forms.ToolStripButton btnCreateTable;
        private System.Windows.Forms.ToolStripButton btnDeleteTable;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton btnQueryEntities;
        private System.Windows.Forms.ToolStripButton btnInsertEntity;
        private System.Windows.Forms.ToolStripButton btnUpdateEntity;
        private System.Windows.Forms.ToolStripButton btnDeleteEntity;
        private System.Windows.Forms.ToolStripButton btnNextPage;
        private System.Windows.Forms.ToolStripButton tsbViewType;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton tsbMergeEntity;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton tsbEditTemplate;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.TabPage tabCrudContext;
        private System.Windows.Forms.TextBox tbCrudContext;
        private System.Windows.Forms.ToolStripButton toolStripBtnExecute;
        private System.Windows.Forms.TextBox textBoxforId;
        private System.Windows.Forms.SplitContainer splitContainerIntabPage;
        private System.Windows.Forms.Label labelid;
        private System.Windows.Forms.TabPage tabPageRequestOptions;
        private System.Windows.Forms.ToolStrip feedToolStrip;
        private System.Windows.Forms.ToolStripTextBox toolStripTextMaxItemCount;
        private System.Windows.Forms.ToolStripButton btnExecuteNext;
        private System.Windows.Forms.SplitContainer ButtomSplitContainer;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox tbAccessConditionText;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox cbRequestOptionsApply;
        private System.Windows.Forms.Label labelPostTrigger;
        private System.Windows.Forms.TextBox tbPostTrigger;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbPreTrigger;
        private System.Windows.Forms.RadioButton rbAccessConditionIfNoneMatch;
        private System.Windows.Forms.RadioButton rbAccessConditionIfMatch;
        private System.Windows.Forms.RadioButton rbConsistencyEventual;
        private System.Windows.Forms.RadioButton rbConsistencySession;
        private System.Windows.Forms.RadioButton rbConsistencyBound;
        private System.Windows.Forms.RadioButton rbConsistencyStrong;
        private System.Windows.Forms.RadioButton rbIndexingExclude;
        private System.Windows.Forms.RadioButton rbIndexingInclude;
        private System.Windows.Forms.RadioButton rbIndexingDefault;
        private System.Windows.Forms.TabPage tabDocumentCollectionPolicy;
        private System.Windows.Forms.CheckBox cbAutomatic;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton rbLazy;
        private System.Windows.Forms.RadioButton rbConsistent;
        private System.Windows.Forms.Label labelCollectionId;
        private System.Windows.Forms.TextBox tbCollectionId;
        private System.Windows.Forms.CheckBox cbIndexingPolicyDefault;
        private System.Windows.Forms.ListBox lbIncludedPath;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnRemovePath;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnAddIncludePath;
        private System.Windows.Forms.ListBox lbExcludedPath;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnRemoveExcludedPath;
        private System.Windows.Forms.Button btnAddExcludedPath;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.RadioButton rbOfferS3;
        private System.Windows.Forms.RadioButton rbOfferS2;
        private System.Windows.Forms.RadioButton rbOfferS1;
        private System.Windows.Forms.Panel triggerPanel;
        private System.Windows.Forms.RadioButton rbPostTrigger;
        private System.Windows.Forms.RadioButton rbPreTrigger;
    }
}

