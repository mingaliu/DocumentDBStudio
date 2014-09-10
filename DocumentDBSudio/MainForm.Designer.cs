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
            this.ButtomSplitContainer = new System.Windows.Forms.SplitContainer();
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
            ((System.ComponentModel.ISupportInitialize)(this.ButtomSplitContainer)).BeginInit();
            this.ButtomSplitContainer.Panel1.SuspendLayout();
            this.ButtomSplitContainer.Panel2.SuspendLayout();
            this.ButtomSplitContainer.SuspendLayout();
            this.feedToolStrip.SuspendLayout();
            this.tsMenu.SuspendLayout();
            this.tsAddress.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.tsTablesEntities.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsStatus,
            this.tsProgress,
            this.tsButtonZoom});
            this.statusStrip.Location = new System.Drawing.Point(0, 843);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
            this.statusStrip.Size = new System.Drawing.Size(1428, 22);
            this.statusStrip.TabIndex = 1;
            this.statusStrip.Text = "statusStrip1";
            // 
            // tsStatus
            // 
            this.tsStatus.Name = "tsStatus";
            this.tsStatus.Size = new System.Drawing.Size(1239, 17);
            this.tsStatus.Spring = true;
            this.tsStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tsProgress
            // 
            this.tsProgress.Name = "tsProgress";
            this.tsProgress.Size = new System.Drawing.Size(100, 16);
            // 
            // tsButtonZoom
            // 
            this.tsButtonZoom.Image = global::Microsoft.Azure.DocumentDBStudio.Properties.Resources.ZoomHS;
            this.tsButtonZoom.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsButtonZoom.Name = "tsButtonZoom";
            this.tsButtonZoom.Size = new System.Drawing.Size(67, 20);
            this.tsButtonZoom.Text = "100%";
            this.tsButtonZoom.ButtonClick += new System.EventHandler(this.tsButtonZoom_ButtonClick);
            // 
            // splitContainerOuter
            // 
            this.splitContainerOuter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainerOuter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerOuter.Location = new System.Drawing.Point(0, 50);
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
            this.splitContainerOuter.Size = new System.Drawing.Size(1428, 793);
            this.splitContainerOuter.SplitterDistance = 474;
            this.splitContainerOuter.SplitterWidth = 5;
            this.splitContainerOuter.TabIndex = 2;
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Margin = new System.Windows.Forms.Padding(4);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(472, 791);
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
            this.splitContainerInner.Size = new System.Drawing.Size(947, 791);
            this.splitContainerInner.SplitterDistance = 161;
            this.splitContainerInner.SplitterWidth = 5;
            this.splitContainerInner.TabIndex = 0;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabRequest);
            this.tabControl.Controls.Add(this.tabCrudContext);
            this.tabControl.Controls.Add(this.tabResponse);
            this.tabControl.Controls.Add(this.tabPageRequestOptions);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Margin = new System.Windows.Forms.Padding(4);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(947, 161);
            this.tabControl.TabIndex = 1;
            // 
            // tabRequest
            // 
            this.tabRequest.Controls.Add(this.tbRequest);
            this.tabRequest.Location = new System.Drawing.Point(4, 26);
            this.tabRequest.Margin = new System.Windows.Forms.Padding(4);
            this.tabRequest.Name = "tabRequest";
            this.tabRequest.Padding = new System.Windows.Forms.Padding(4);
            this.tabRequest.Size = new System.Drawing.Size(939, 131);
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
            this.tbRequest.Size = new System.Drawing.Size(931, 123);
            this.tbRequest.TabIndex = 0;
            // 
            // tabCrudContext
            // 
            this.tabCrudContext.Controls.Add(this.splitContainerIntabPage);
            this.tabCrudContext.Location = new System.Drawing.Point(4, 22);
            this.tabCrudContext.Margin = new System.Windows.Forms.Padding(4);
            this.tabCrudContext.Name = "tabCrudContext";
            this.tabCrudContext.Padding = new System.Windows.Forms.Padding(4);
            this.tabCrudContext.Size = new System.Drawing.Size(939, 135);
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
            this.splitContainerIntabPage.Size = new System.Drawing.Size(931, 127);
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
            this.labelid.Size = new System.Drawing.Size(19, 17);
            this.labelid.TabIndex = 2;
            this.labelid.Text = "Id";
            // 
            // textBoxforId
            // 
            this.textBoxforId.Location = new System.Drawing.Point(35, 3);
            this.textBoxforId.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxforId.Name = "textBoxforId";
            this.textBoxforId.Size = new System.Drawing.Size(389, 23);
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
            this.tbCrudContext.Size = new System.Drawing.Size(931, 87);
            this.tbCrudContext.TabIndex = 0;
            this.tbCrudContext.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.tbCrudContext_PreviewKeyDown);
            // 
            // tabResponse
            // 
            this.tabResponse.Controls.Add(this.tbResponse);
            this.tabResponse.Location = new System.Drawing.Point(4, 22);
            this.tabResponse.Margin = new System.Windows.Forms.Padding(4);
            this.tabResponse.Name = "tabResponse";
            this.tabResponse.Padding = new System.Windows.Forms.Padding(4);
            this.tabResponse.Size = new System.Drawing.Size(939, 135);
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
            this.tbResponse.Size = new System.Drawing.Size(931, 127);
            this.tbResponse.TabIndex = 0;
            // 
            // tabPageRequestOptions
            // 
            this.tabPageRequestOptions.Location = new System.Drawing.Point(4, 22);
            this.tabPageRequestOptions.Name = "tabPageRequestOptions";
            this.tabPageRequestOptions.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageRequestOptions.Size = new System.Drawing.Size(939, 135);
            this.tabPageRequestOptions.TabIndex = 3;
            this.tabPageRequestOptions.Text = "RequestOptions";
            this.tabPageRequestOptions.UseVisualStyleBackColor = true;
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
            this.ButtomSplitContainer.Panel1.Controls.Add(this.feedToolStrip);
            this.ButtomSplitContainer.Panel1MinSize = 0;
            // 
            // ButtomSplitContainer.Panel2
            // 
            this.ButtomSplitContainer.Panel2.Controls.Add(this.webBrowserResponse);
            this.ButtomSplitContainer.Size = new System.Drawing.Size(947, 625);
            this.ButtomSplitContainer.SplitterDistance = 34;
            this.ButtomSplitContainer.TabIndex = 4;
            // 
            // feedToolStrip
            // 
            this.feedToolStrip.Dock = System.Windows.Forms.DockStyle.Fill;
            this.feedToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.feedToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripTextMaxItemCount,
            this.btnExecuteNext});
            this.feedToolStrip.Location = new System.Drawing.Point(0, 0);
            this.feedToolStrip.Name = "feedToolStrip";
            this.feedToolStrip.Size = new System.Drawing.Size(947, 34);
            this.feedToolStrip.TabIndex = 3;
            this.feedToolStrip.Text = "toolStrip1";
            // 
            // toolStripTextMaxItemCount
            // 
            this.toolStripTextMaxItemCount.Name = "toolStripTextMaxItemCount";
            this.toolStripTextMaxItemCount.Size = new System.Drawing.Size(50, 27);
            this.toolStripTextMaxItemCount.Text = "10";
            this.toolStripTextMaxItemCount.TextBoxTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnExecuteNext
            // 
            this.btnExecuteNext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnExecuteNext.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnExecuteNext.Name = "btnExecuteNext";
            this.btnExecuteNext.Size = new System.Drawing.Size(79, 24);
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
            this.webBrowserResponse.Size = new System.Drawing.Size(947, 587);
            this.webBrowserResponse.TabIndex = 0;
            // 
            // tsMenu
            // 
            this.tsMenu.BackColor = System.Drawing.SystemColors.Control;
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
            this.tsMenu.Location = new System.Drawing.Point(0, 25);
            this.tsMenu.Name = "tsMenu";
            this.tsMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.tsMenu.Size = new System.Drawing.Size(1428, 25);
            this.tsMenu.TabIndex = 3;
            this.tsMenu.Text = "toolStrip2";
            // 
            // btnBack
            // 
            this.btnBack.Image = global::Microsoft.Azure.DocumentDBStudio.Properties.Resources.NavBack;
            this.btnBack.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(52, 22);
            this.btnBack.Text = "Back";
            // 
            // btnForward
            // 
            this.btnForward.Enabled = false;
            this.btnForward.Image = global::Microsoft.Azure.DocumentDBStudio.Properties.Resources.NavForward;
            this.btnForward.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnForward.Name = "btnForward";
            this.btnForward.Size = new System.Drawing.Size(70, 22);
            this.btnForward.Text = "Forward";
            // 
            // btnHome
            // 
            this.btnHome.Image = global::Microsoft.Azure.DocumentDBStudio.Properties.Resources.HomeHS;
            this.btnHome.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnHome.Name = "btnHome";
            this.btnHome.Size = new System.Drawing.Size(60, 22);
            this.btnHome.Text = "Home";
            this.btnHome.Click += new System.EventHandler(this.btnHome_Click);
            // 
            // toolStripBtnExecute
            // 
            this.toolStripBtnExecute.Image = ((System.Drawing.Image)(resources.GetObject("toolStripBtnExecute.Image")));
            this.toolStripBtnExecute.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripBtnExecute.Name = "toolStripBtnExecute";
            this.toolStripBtnExecute.Size = new System.Drawing.Size(67, 22);
            this.toolStripBtnExecute.Text = "Execute";
            this.toolStripBtnExecute.Click += new System.EventHandler(this.toolStripBtnExecute_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Image = global::Microsoft.Azure.DocumentDBStudio.Properties.Resources.RefreshDocViewHS;
            this.btnRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(66, 22);
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.Visible = false;
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnHeaders
            // 
            this.btnHeaders.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnHeaders.Image = ((System.Drawing.Image)(resources.GetObject("btnHeaders.Image")));
            this.btnHeaders.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnHeaders.Name = "btnHeaders";
            this.btnHeaders.Size = new System.Drawing.Size(139, 22);
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
            this.btnEditRequests.Size = new System.Drawing.Size(76, 22);
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
            this.tsbViewType.Size = new System.Drawing.Size(61, 22);
            this.tsbViewType.Text = "Text View";
            this.tsbViewType.Click += new System.EventHandler(this.tsbViewType_Click);
            // 
            // tsAddress
            // 
            this.tsAddress.AutoSize = false;
            this.tsAddress.BackColor = System.Drawing.SystemColors.Control;
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
            this.tsLabelUrl.Size = new System.Drawing.Size(34, 30);
            this.tsLabelUrl.Text = "URL: ";
            // 
            // cbUrl
            // 
            this.cbUrl.AutoSize = false;
            this.cbUrl.MaxDropDownItems = 1;
            this.cbUrl.Name = "cbUrl";
            this.cbUrl.Size = new System.Drawing.Size(300, 23);
            // 
            // btnGo
            // 
            this.btnGo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnGo.Image = ((System.Drawing.Image)(resources.GetObject("btnGo.Image")));
            this.btnGo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(26, 30);
            this.btnGo.Text = "Go";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(8, 3, 0, 3);
            this.menuStrip1.Size = new System.Drawing.Size(1428, 25);
            this.menuStrip1.TabIndex = 5;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 19);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.optionsToolStripMenuItem.Text = "&Add Account";
            this.optionsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 19);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "&About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // tsTablesEntities
            // 
            this.tsTablesEntities.AutoSize = false;
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
            this.btnQueryTable.Size = new System.Drawing.Size(50, 30);
            this.btnQueryTable.Text = "QueryT";
            this.btnQueryTable.ToolTipText = "QueryTable";
            // 
            // btnCreateTable
            // 
            this.btnCreateTable.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnCreateTable.Image = ((System.Drawing.Image)(resources.GetObject("btnCreateTable.Image")));
            this.btnCreateTable.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCreateTable.Name = "btnCreateTable";
            this.btnCreateTable.Size = new System.Drawing.Size(52, 30);
            this.btnCreateTable.Text = "CreateT";
            this.btnCreateTable.ToolTipText = "CreateTable";
            // 
            // btnDeleteTable
            // 
            this.btnDeleteTable.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnDeleteTable.Image = ((System.Drawing.Image)(resources.GetObject("btnDeleteTable.Image")));
            this.btnDeleteTable.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDeleteTable.Name = "btnDeleteTable";
            this.btnDeleteTable.Size = new System.Drawing.Size(51, 30);
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
            this.btnQueryEntities.Size = new System.Drawing.Size(49, 30);
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
            this.btnNextPage.Size = new System.Drawing.Size(61, 30);
            this.btnNextPage.Text = "NextPage";
            // 
            // btnInsertEntity
            // 
            this.btnInsertEntity.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnInsertEntity.Image = ((System.Drawing.Image)(resources.GetObject("btnInsertEntity.Image")));
            this.btnInsertEntity.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnInsertEntity.Name = "btnInsertEntity";
            this.btnInsertEntity.Size = new System.Drawing.Size(46, 30);
            this.btnInsertEntity.Text = "InsertE";
            this.btnInsertEntity.ToolTipText = "InsertEntity";
            // 
            // btnUpdateEntity
            // 
            this.btnUpdateEntity.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnUpdateEntity.Image = ((System.Drawing.Image)(resources.GetObject("btnUpdateEntity.Image")));
            this.btnUpdateEntity.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnUpdateEntity.Name = "btnUpdateEntity";
            this.btnUpdateEntity.Size = new System.Drawing.Size(55, 30);
            this.btnUpdateEntity.Text = "UpdateE";
            this.btnUpdateEntity.ToolTipText = "UpdateEntity";
            // 
            // tsbMergeEntity
            // 
            this.tsbMergeEntity.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbMergeEntity.Image = ((System.Drawing.Image)(resources.GetObject("tsbMergeEntity.Image")));
            this.tsbMergeEntity.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbMergeEntity.Name = "tsbMergeEntity";
            this.tsbMergeEntity.Size = new System.Drawing.Size(51, 30);
            this.tsbMergeEntity.Text = "MergeE";
            // 
            // btnDeleteEntity
            // 
            this.btnDeleteEntity.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnDeleteEntity.Image = ((System.Drawing.Image)(resources.GetObject("btnDeleteEntity.Image")));
            this.btnDeleteEntity.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDeleteEntity.Name = "btnDeleteEntity";
            this.btnDeleteEntity.Size = new System.Drawing.Size(50, 30);
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
            this.tsbEditTemplate.Size = new System.Drawing.Size(84, 30);
            this.tsbEditTemplate.Text = "Edit Template";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
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
            this.ButtomSplitContainer.Panel1.ResumeLayout(false);
            this.ButtomSplitContainer.Panel1.PerformLayout();
            this.ButtomSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ButtomSplitContainer)).EndInit();
            this.ButtomSplitContainer.ResumeLayout(false);
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
    }
}

