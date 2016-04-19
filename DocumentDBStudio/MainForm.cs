//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace Microsoft.Azure.DocumentDBStudio
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Reflection;
    using System.Threading;
    using System.Windows.Forms;
    using Microsoft.Azure.DocumentDBStudio.Properties;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Microsoft.Azure.Documents.Routing;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public partial class MainForm : Form
    {
        private int defaultFontPoint = 9;
        private int fontScale = 100;

        private string loadingGifPath;
        private string prettyJSONTemplate;

        private string currentJson;
        private string currentText;
        private string homepage;

        private string appTempPath;
        Action<object, RequestOptions> currentOperationCallback;

        private RequestOptions requestOptions;
        CheckBox cbEnableScan;
        CheckBox cbEnableCrossPartitionQuery;

        private DocumentCollection newDocumentCollection;

        private OperationType operationType;
        private ResourceType resourceType;
        private OfferType offerType;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(arg => this.CheckCurrentRelease());

            this.Height = Screen.GetWorkingArea(this).Height * 3 / 4;
            this.Width = Screen.GetWorkingArea(this).Width / 2;
            this.Top = 0;
            this.Text = Constants.ApplicationName;

            using (Stream stm = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Microsoft.Azure.DocumentDBStudio.Resources.home.html"))
            {
                using (StreamReader reader = new StreamReader(stm))
                {
                    this.homepage = reader.ReadToEnd();
                }
            }
            this.homepage = this.homepage.Replace("&VERSION&", Constants.ProductVersion);

            DateTime t = System.IO.File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location);
            DateTimeOffset dateOffset = new DateTimeOffset(t, TimeZoneInfo.Local.GetUtcOffset(t));
            this.homepage = this.homepage.Replace("&BUILDTIME&", t.ToString("f", CultureInfo.CurrentCulture));

            this.cbUrl.Items.Add("about:home");
            this.cbUrl.SelectedIndex = 0;
            this.cbUrl.KeyDown += new KeyEventHandler(cbUrl_KeyDown);

            this.btnBack.Enabled = false;

            this.splitContainerOuter.Panel1Collapsed = false;
            this.splitContainerInner.Panel1Collapsed = true;
            this.ButtomSplitContainer.Panel1Collapsed = true;

            this.KeyPreview = true;
            this.PreviewKeyDown += new PreviewKeyDownEventHandler(MainForm_PreviewKeyDown);

            this.webBrowserResponse.PreviewKeyDown += new PreviewKeyDownEventHandler(webBrowserResponse_PreviewKeyDown);
            this.webBrowserResponse.StatusTextChanged += new EventHandler(webBrowserResponse_StatusTextChanged);
            this.webBrowserResponse.ScriptErrorsSuppressed = true;

            this.tabControl.SelectedTab = this.tabCrudContext;
            this.tabControl.TabPages.Remove(this.tabRequest);
            this.tabControl.TabPages.Remove(this.tabDocumentCollection);
            this.tabControl.TabPages.Remove(this.tabOffer);

            ImageList imageList = new ImageList();
            imageList.Images.Add("Default", Resources.DocDBpng);
            imageList.Images.Add("Feed", Resources.Feedpng);
            imageList.Images.Add("Javascript", Resources.Javascriptpng);
            imageList.Images.Add("User", Resources.Userpng);
            imageList.Images.Add("Permission", Resources.Permissionpng);
            imageList.Images.Add("DatabaseAccount", Resources.DatabaseAccountpng);
            imageList.Images.Add("SystemFeed", Resources.SystemFeedpng);
            imageList.Images.Add("Attachment", Resources.Attachmentpng);
            imageList.Images.Add("Conflict", Resources.Conflictpng);
            imageList.Images.Add("Offer", Resources.Offerpng);
            this.treeView1.ImageList = imageList;

            this.InitTreeView();

            this.btnHome_Click(null, null);

            this.splitContainerIntabPage.Panel1Collapsed = true;

            this.toolStripBtnExecute.Enabled = false;
            this.btnExecuteNext.Enabled = false;
            this.UnpackEmbeddedResources();

            this.tsbViewType.Checked = true;
            this.btnHeaders.Checked = false;

            this.cbRequestOptionsApply_CheckedChanged(null, null);
            this.cbIndexingPolicyDefault_CheckedChanged(null, null);

            cbEnableScan = new CheckBox();
            cbEnableScan.Text = "EnableScanInQuery";
            cbEnableScan.CheckState = CheckState.Indeterminate;
            ToolStripControlHost host1 = new ToolStripControlHost(cbEnableScan);
            feedToolStrip.Items.Insert(1, host1);

            cbEnableCrossPartitionQuery = new CheckBox();
            cbEnableCrossPartitionQuery.Text = "EnableCrossPartitionQuery";
            cbEnableCrossPartitionQuery.CheckState = CheckState.Indeterminate;
            ToolStripControlHost host2 = new ToolStripControlHost(cbEnableCrossPartitionQuery);
            feedToolStrip.Items.Insert(2, host2);

            lbIncludedPath.Items.Add(new IncludedPath() { Path = "/" });

            this.offerType = OfferType.StandardSingle;
            this.tbThroughput.Text = "400";

        }


        private void UnpackEmbeddedResources()
        {
            this.appTempPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DocumentDBStudio");

            if (!Directory.Exists(this.appTempPath))
            {
                Directory.CreateDirectory(this.appTempPath);
            }

            this.loadingGifPath = Path.Combine(this.appTempPath, "loading.gif");

            using (Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Microsoft.Azure.DocumentDBStudio.Resources.loading.gif"))
            {
                using (FileStream fileStream = File.Create(this.loadingGifPath))
                {
                    stream.CopyTo(fileStream);
                }
            }

            using (Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Microsoft.Azure.DocumentDBStudio.Resources.prettyJSON.backbone-min.js"))
            {
                using (FileStream fileStream = File.Create(Path.Combine(this.appTempPath, "backbone-min.js")))
                {
                    stream.CopyTo(fileStream);
                }
            }
            using (Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Microsoft.Azure.DocumentDBStudio.Resources.prettyJSON.jquery-1.11.1.min.js"))
            {
                using (FileStream fileStream = File.Create(Path.Combine(this.appTempPath, "jquery-1.11.1.min.js")))
                {
                    stream.CopyTo(fileStream);
                }
            }
            using (Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Microsoft.Azure.DocumentDBStudio.Resources.prettyJSON.pretty-json.css"))
            {
                using (FileStream fileStream = File.Create(Path.Combine(this.appTempPath, "pretty-json.css")))
                {
                    stream.CopyTo(fileStream);
                }
            }
            using (Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Microsoft.Azure.DocumentDBStudio.Resources.prettyJSON.pretty-json-min.js"))
            {
                using (FileStream fileStream = File.Create(Path.Combine(this.appTempPath, "pretty-json-min.js")))
                {
                    stream.CopyTo(fileStream);
                }
            }
            using (Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Microsoft.Azure.DocumentDBStudio.Resources.prettyJSON.underscore-min.js"))
            {
                using (FileStream fileStream = File.Create(Path.Combine(this.appTempPath, "underscore-min.js")))
                {
                    stream.CopyTo(fileStream);
                }
            }

            using (Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Microsoft.Azure.DocumentDBStudio.Resources.prettyJSON.PrettyPrintJSONTemplate.html"))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    this.prettyJSONTemplate = reader.ReadToEnd();
                }
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            // ToolStrips don't appear to have a way to "spring" their items like status bars
            cbUrl.Width = this.tsAddress.Width - 40 - this.tsLabelUrl.Width - this.btnGo.Width;
        }

        void webBrowserResponse_StatusTextChanged(object sender, EventArgs e)
        {
        }

        void cbUrl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.webBrowserResponse.Navigate(this.cbUrl.Text);
                e.Handled = true;
            }
        }

        void webBrowserResponse_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (this.webBrowserResponse.Focused)
            {
                HandlePreviewKeyDown(e.KeyCode, e.Modifiers);
            }
        }

        void MainForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (!this.webBrowserResponse.Focused && !this.cbUrl.Focused)
            {
                HandlePreviewKeyDown(e.KeyCode, e.Modifiers);
            }
        }

        bool HandlePreviewKeyDown(Keys key, Keys modifiers)
        {
            if (key == Keys.Back)
            {
                // Don't steal backspace from the URL combo box
                if (!this.cbUrl.Focused)
                {
                    return true;
                }
            }
            else if (key == Keys.F5)
            {
                return true;
            }
            else if (key == Keys.Enter)
            {
                this.webBrowserResponse.Navigate(this.cbUrl.Text);
                return true;
            }
            else if (key == Keys.W && modifiers == Keys.Control)
            {
                // Exit the app on Ctrl + W like browser tabs
                this.Close();
                return true;
            }
            else if (key == Keys.D && modifiers == Keys.Alt)
            {
                // Focus the URL in the address bar
                this.cbUrl.SelectAll();
                this.cbUrl.Focus();
            }
            return false;
        }

        private void tbCrudContext_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F5:
                    {
                        if (this.toolStripBtnExecute.Enabled)
                        {
                            toolStripBtnExecute_Click(null, null);
                        }
                    }
                    break;
                case Keys.A:
                    if (e.Control)
                    {
                        tbCrudContext.SelectAll();
                    }
                    break;
            }
        }

        //
        private void DisplayResponseContent()
        {
            if (this.tsbViewType.Checked)
            {
                this.PrettyPrintJson(this.currentJson, this.currentText);
            }
            else
            {
                string htmlResponse = "";

                if (!string.IsNullOrEmpty(this.currentJson))
                {
                    htmlResponse = Helper.FormatTextAsHtml(this.currentJson, false);
                }
                if (!string.IsNullOrEmpty(this.currentText))
                {
                    htmlResponse += "\r\n\r\n" + Helper.FormatTextAsHtml(this.currentText, false);
                }
                this.DisplayHtmlContentInScale(htmlResponse);
            }
        }

        void DisplayHtmlContentInScale(string htmlResponse)
        {
            if (this.fontScale != 100)
            {
                // current scaled font
                float fontPt = this.defaultFontPoint * (this.fontScale / 100.0f);

                // todo: make this a well defined class
                string style = "{ font-size: " + fontPt + "pt; }";
                string s = htmlResponse.Replace("{ font-size: 9pt; }", style);
                this.webBrowserResponse.DocumentText = s;
            }
            else
            {
                this.webBrowserResponse.DocumentText = htmlResponse;
            }
        }

        private void tsButtonZoom_ButtonClick(object sender, EventArgs e)
        {
            switch (this.tsButtonZoom.Text)
            {
                case "100%":
                    this.fontScale = 125;
                    break;
                case "125%":
                    this.fontScale = 150;
                    break;
                case "150%":
                    this.fontScale = 175;
                    break;
                case "175%":
                    this.fontScale = 100;
                    break;

            }
            this.tsButtonZoom.Text = this.fontScale.ToString(CultureInfo.CurrentCulture) + "%";
            this.tbRequest.Font = new Font(this.tbRequest.Font.FontFamily.Name, this.defaultFontPoint * (this.fontScale / 100.0f));
            this.tbResponse.Font = new Font(this.tbResponse.Font.FontFamily.Name, this.defaultFontPoint * (this.fontScale / 100.0f));
            this.Font = new Font(this.tbResponse.Font.FontFamily.Name, this.defaultFontPoint * (this.fontScale / 100.0f));

            // we don't support pretty print for font scaling yet.
            if (!this.tsbViewType.Checked)
            {
                DisplayResponseContent();
            }
        }

        private void btnHeaders_Click(object sender, EventArgs e)
        {
            if (this.splitContainerInner.Panel1Collapsed == true)
            {
                this.splitContainerInner.Panel1Collapsed = false;
                this.btnHeaders.Checked = true;
                this.btnHeaders.Text = "Hide Response Headers";

                this.tabControl.SelectedTab = this.tabResponse;
            }
            else
            {
                this.splitContainerInner.Panel1Collapsed = true;
                this.btnHeaders.Checked = false;
                this.btnHeaders.Text = "Show Response Headers";
            }
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            //
            DisplayHtmlContentInScale(this.homepage);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this, Constants.ApplicationName + "\nVersion " + Constants.ProductVersion,
                            "About",
                            MessageBoxButtons.OK);
        }

        private void tsbViewType_Click(object sender, EventArgs e)
        {
            if (this.tsbViewType.Checked)
                this.tsbViewType.Text = "Pretty Json View";
            else
                this.tsbViewType.Text = "Text View";

            if ((this.webBrowserResponse.Url.AbsoluteUri == "about:blank" && this.webBrowserResponse.DocumentTitle != "DataModelBrowserHome")
                || this.webBrowserResponse.Url.Scheme == "file")
            {
                this.DisplayResponseContent();
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Bring up account setings dialog
            SettingsForm dlg = new SettingsForm();
            DialogResult dr = dlg.ShowDialog(this);
            if (dr == DialogResult.OK)
            {
                this.AddAccountToSettings(dlg.AccountEndpoint, dlg.AccountSettings);
            }
        }

        public void ChangeAccountSettings(TreeNode thisNode, string accountEndpoint)
        {
            this.treeView1.SelectedNode = thisNode;

            for (int i = 0; i < Settings.Default.AccountSettingsList.Count; i = i + 2)
            {
                if (string.Compare(accountEndpoint, Properties.Settings.Default.AccountSettingsList[i], StringComparison.OrdinalIgnoreCase) == 0)
                {
                    AccountSettings accountSettings = (AccountSettings)JsonConvert.DeserializeObject(Settings.Default.AccountSettingsList[i + 1], typeof(AccountSettings));

                    // Bring up account setings dialog
                    SettingsForm dlg = new SettingsForm();
                    dlg.AccountEndpoint = accountEndpoint;
                    dlg.AccountSettings = accountSettings;

                    DialogResult dr = dlg.ShowDialog(this);
                    if (dr == DialogResult.OK)
                    {
                        thisNode.Remove();
                        RemoveAccountFromSettings(dlg.AccountEndpoint);
                        AddAccountToSettings(dlg.AccountEndpoint, dlg.AccountSettings);
                    }

                    break;
                }
            }

        }

        private void AddAccountToSettings(string accountEndpoint, AccountSettings accountSettings)
        {
            bool found = false;
            // if the account is not in tree view top level, add it!
            for (int i = 0; i < Settings.Default.AccountSettingsList.Count; i = i + 2)
            {
                if (string.Compare(accountEndpoint, Properties.Settings.Default.AccountSettingsList[i], StringComparison.OrdinalIgnoreCase) == 0)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                Settings.Default.AccountSettingsList.Add(accountEndpoint);
                Settings.Default.AccountSettingsList.Add(JsonConvert.SerializeObject(accountSettings));

                Settings.Default.Save();

                AddConnectionTreeNode(accountEndpoint, accountSettings);
            }
        }

        public void RemoveAccountFromSettings(string accountEndpoint)
        {
            int index = -1;
            // if the account is not in tree view top level, add it!
            for (int i = 0; i < Settings.Default.AccountSettingsList.Count; i = i + 2)
            {
                if (string.Compare(accountEndpoint, Properties.Settings.Default.AccountSettingsList[i], StringComparison.OrdinalIgnoreCase) == 0)
                {
                    index = i;
                    break;
                }
            }

            if (index >= 0)
            {
                Settings.Default.AccountSettingsList.RemoveRange(index, 2);
                Settings.Default.Save();
            }
        }

        public FeedOptions GetFeedOptions()
        {
            FeedOptions feedOptions = new FeedOptions();

            try
            {
                feedOptions.MaxItemCount = Convert.ToInt32(toolStripTextMaxItemCount.Text, CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                // Ignore the exception and use the defualt value
            }

            if (cbEnableScan.CheckState == CheckState.Checked)
            {
                feedOptions.EnableScanInQuery = true;
            }
            else if (cbEnableScan.CheckState == CheckState.Unchecked)
            {
                feedOptions.EnableScanInQuery = false;
            }

            if (cbEnableCrossPartitionQuery.CheckState == CheckState.Checked)
            {
                feedOptions.EnableCrossPartitionQuery = true;
            }
            else if (cbEnableCrossPartitionQuery.CheckState == CheckState.Unchecked)
            {
                feedOptions.EnableCrossPartitionQuery = false;
            }

            return feedOptions;
        }

        public RequestOptions GetRequestOptions()
        {
                if (tbPostTrigger.Modified)
                {
                    string postTrigger = tbPostTrigger.Text;
                    if (!string.IsNullOrEmpty(postTrigger))
                    {
                        // split by ;
                        string[] segments = postTrigger.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                        this.requestOptions.PostTriggerInclude = segments;
                    }
                    tbPostTrigger.Modified = false;
                }

                if (tbPreTrigger.Modified)
                {
                    string preTrigger = tbPreTrigger.Text;
                    if (!string.IsNullOrEmpty(preTrigger))
                    {
                        // split by ;
                        string[] segments = preTrigger.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                        this.requestOptions.PreTriggerInclude = segments;
                    }
                }

                if (tbAccessConditionText.Modified)
                {
                    string condition = tbAccessConditionText.Text;
                    if (!string.IsNullOrEmpty(condition))
                    {
                        this.requestOptions.AccessCondition.Condition = condition;
                    }
                }

            if (tbPartitionKeyForRequestOption.Modified)
            {
                string partitionKey = tbPartitionKeyForRequestOption.Text;
                if (!string.IsNullOrEmpty(partitionKey))
                {
                    this.requestOptions.PartitionKey = new PartitionKey(partitionKey);
            }
            }

            if ((this.resourceType == ResourceType.DocumentCollection || this.resourceType == ResourceType.Offer) &&
                (this.operationType == OperationType.Create || this.operationType == OperationType.Replace))
            {
                if (this.offerType == OfferType.StandardSingle || this.offerType == OfferType.StandardElastic)
                {
                    this.requestOptions.OfferThroughput = int.Parse(this.tbThroughput.Text, CultureInfo.CurrentCulture);
                    this.requestOptions.OfferType = null;
                }
                else
                {
                    this.requestOptions.OfferType = this.offerType.ToString();
                    this.requestOptions.OfferThroughput = null;
                }
            }
            return this.requestOptions;
        }

        private delegate DialogResult MessageBoxDelegate(string msg, string title, MessageBoxButtons buttons, MessageBoxIcon icon);

        private DialogResult ShowMessage(string msg, string title, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            return MessageBox.Show(this, msg, title, buttons, icon);
        }

        public void CheckCurrentRelease()
        {
            Thread.Sleep(3000);

            Uri uri = new Uri("https://api.github.com/repos/mingaliu/documentdbstudio/releases");

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpRequestMessage request = new HttpRequestMessage()
                    {
                        RequestUri = uri,
                        Method = HttpMethod.Get,
                    };
                    request.Headers.UserAgent.Add(new ProductInfoHeaderValue("DocumentDBStudio", Constants.ProductVersion.ToString()));

                    HttpResponseMessage response = client.SendAsync(request).Result;

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        JArray releaseJson = JArray.Parse(response.Content.ReadAsStringAsync().Result);
                        JToken latestRelease = releaseJson.First;
                        JToken latestReleaseTag = latestRelease["tag_name"];
                        string latestReleaseString = latestReleaseTag.ToString();

                        if (string.Compare(Constants.ProductVersion.ToString(), latestReleaseString, StringComparison.OrdinalIgnoreCase) < 0)
                        {
                            this.Invoke(new MessageBoxDelegate(ShowMessage),
                                string.Format(CultureInfo.InvariantCulture, "Please update the DocumentDB studio to the latest version {0} at https://github.com/mingaliu/DocumentDBStudio/releases", latestReleaseString),
                                Constants.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                catch (Exception)
                {
                    // ignore any exception here.
                }
            }
        }

        private void InitTreeView()
        {
            if (Settings.Default.AccountSettingsList == null)
            {
                Settings.Default.AccountSettingsList = new List<string>();
            }
            // load the account settings from the List.
            for (int i = 0; i < Settings.Default.AccountSettingsList.Count; i = i + 2)
            {
                AccountSettings accountSettings = (AccountSettings)JsonConvert.DeserializeObject(Settings.Default.AccountSettingsList[i + 1], typeof(AccountSettings));
                this.AddConnectionTreeNode(Settings.Default.AccountSettingsList[i], accountSettings);
            }
        }

        private void AddConnectionTreeNode(string accountEndpoint, AccountSettings accountSettings)
        {
            try
            {
                string suffix = Constants.ApplicationName + "/" + Constants.ProductVersion;

                DocumentClient client = new DocumentClient(new Uri(accountEndpoint), accountSettings.MasterKey,
                    new ConnectionPolicy
                    {
                        ConnectionMode = accountSettings.ConnectionMode,
                        ConnectionProtocol = accountSettings.Protocol,
                        // enable after we support the automatic failover from client .
                        //DisableAutomaticFailover = !accountSettings.EnableFailOver,
                        UserAgentSuffix = suffix
                    });

                DatabaseAccountNode dbaNode = new DatabaseAccountNode(accountEndpoint, client);
                this.treeView1.Nodes.Add(dbaNode);

                // Update the map.
                DocumentClientExtension.AddOrUpdate(client.ServiceEndpoint.Host, accountSettings.IsNameBased);
                if (accountSettings.IsNameBased)
                {
                    dbaNode.ForeColor = Color.Green;
                }
                else
                {
                    dbaNode.ForeColor = Color.Blue;
                }
            }
            catch (Exception e)
            {
                Program.GetMain().SetResultInBrowser(null, e.ToString(), true);
            }
        }

        private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node is FeedNode)
            {
                (e.Node as FeedNode).Refresh(false);
            }
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (e.Node is FeedNode)
                {
                    (e.Node as FeedNode).ShowContextMenu(e.Node.TreeView, e.Location);
                }

            }
            else if (e.Button == MouseButtons.Left)
            {
                // render the JSON in the right panel.
                this.currentText = null;
                this.currentJson = null;

                if (e.Node is ResourceNode)
                {
                    ResourceNode node = e.Node as ResourceNode;
                    string body = node.GetBody();

                    if (!string.IsNullOrEmpty(body))
                    {
                        this.currentText = body;
                    }
                }

                if (e.Node.Tag is string)
                {
                    this.currentText = e.Node.Tag.ToString();
                }
                else if (e.Node is DatabaseAccountNode)
                {
                    this.currentJson = JsonConvert.SerializeObject(e.Node.Tag, Newtonsoft.Json.Formatting.Indented);
                }
                else if (e.Node.Tag != null)
                {
                    this.currentJson = e.Node.Tag.ToString();
                }

                if (this.currentJson == null && this.currentText == null)
                {
                    this.currentText = e.Node.Text;
                }

                this.DisplayResponseContent();

            }
        }

        internal void SetCrudContext(TreeNode node, OperationType operation, ResourceType resourceType, string bodytext, 
            Action<object, RequestOptions> func, CommandContext commandContext = null)
        {
            if (commandContext == null)
            {
                commandContext = new CommandContext();
            }

            this.treeView1.SelectedNode = node;

            this.operationType = operation;
            this.resourceType = resourceType;
            this.currentOperationCallback = func;
            this.tabCrudContext.Text = operation.ToString() + " " + resourceType.ToString();
            this.tbCrudContext.Text = bodytext;

            this.toolStripBtnExecute.Enabled = true;
            this.tbCrudContext.ReadOnly = commandContext.IsDelete;

            // the split panel at the right side, Panel1: tab Control,  Panel2: ButtomSplitContainer (next page and browser)
            this.splitContainerInner.Panel1Collapsed = false;

            bool showId = false;
            if ( (resourceType == ResourceType.Trigger || resourceType == ResourceType.UserDefinedFunction || resourceType == ResourceType.StoredProcedure)
                 && (operation == OperationType.Create || operation == OperationType.Replace))
            {
                showId = true;
            }

            //the split panel inside Tab. Panel1: Id input box, Panel2: Edit CRUD resource.
            this.splitContainerIntabPage.Panel1Collapsed = !showId;

            this.tbResponse.Text = "";

            //the split panel at right bottom. Panel1: NextPage, Panel2: Browser.
            if (commandContext.IsFeed)
            {
                this.ButtomSplitContainer.Panel1Collapsed = false;
                this.ButtomSplitContainer.Panel1.Controls.Clear();
                this.ButtomSplitContainer.Panel1.Controls.Add(this.feedToolStrip);
            }
            else if (commandContext.IsCreateTrigger)
            {
                this.ButtomSplitContainer.Panel1Collapsed = false;
                this.ButtomSplitContainer.Panel1.Controls.Clear();
                this.ButtomSplitContainer.Panel1.Controls.Add(this.triggerPanel);
            }
            else
            {
                this.ButtomSplitContainer.Panel1Collapsed = true;
            }

            this.SetNextPageVisibility(commandContext);

            // Prepare the tab pages
            this.tabControl.TabPages.Remove(this.tabDocumentCollection);
            this.tabControl.TabPages.Remove(this.tabOffer);
            this.tabControl.TabPages.Remove(this.tabCrudContext);


            if (this.resourceType == ResourceType.DocumentCollection && (this.operationType == OperationType.Create || this.operationType == OperationType.Replace))
            {
                this.tabControl.TabPages.Insert(0, this.tabDocumentCollection);
                if (this.operationType == OperationType.Create)
                {
                    this.tbCollectionId.Enabled = true;
                    this.tbCollectionId.Text = "DocumentCollection Id";

                    this.tabControl.TabPages.Insert(0, this.tabOffer);
                    this.tabControl.SelectedTab = this.tabOffer;

                    this.webBrowserResponse.Navigate(new Uri("https://azure.microsoft.com/en-us/documentation/articles/documentdb-performance-levels/"));
                }
                else
                {
                    this.tbCollectionId.Enabled = false;
                    this.tbCollectionId.Text = (node.Tag as Resource).Id;
                    this.tabControl.SelectedTab = this.tabDocumentCollection;
                }
            }
            else
            {
                this.tabControl.TabPages.Insert(0, this.tabCrudContext);
                this.tabControl.SelectedTab = this.tabCrudContext;
            }
        }

        public void SetNextPageVisibility(CommandContext commandContext)
                {
            this.btnExecuteNext.Enabled = commandContext.HasContinuation || !commandContext.QueryStarted;
                }

        private bool ValidateInput()
        {
            if ((this.resourceType == ResourceType.DocumentCollection || this.resourceType == ResourceType.Offer) &&
                (this.operationType == OperationType.Create || this.operationType == OperationType.Replace))
            {
                // basic validation:
                if (this.offerType == OfferType.StandardElastic || this.offerType == OfferType.StandardSingle)
                {
                    int throughput;
                    if (!int.TryParse(this.tbThroughput.Text, NumberStyles.Integer, CultureInfo.CurrentCulture, out throughput))
                    {
                        MessageBox.Show(this, 
                                        "Offer throughput has to be integer",
                                        Constants.ApplicationName + "\nVersion " + Constants.ProductVersion,
                                        MessageBoxButtons.OK);
                        return false;
            }

                    if (throughput % 100 != 0)
                    {
                        MessageBox.Show(this, 
                                        "Offer throughput has to be multiple of 100",
                                        Constants.ApplicationName + "\nVersion " + Constants.ProductVersion,
                                        MessageBoxButtons.OK);
                        return false;
                    }

                    if (this.offerType == OfferType.StandardElastic)
                    {
                        if (throughput < 10000 || throughput > 250000)
                        {
                            MessageBox.Show(this, 
                                            "Offer throughput has to be between 10K and 250K for partitioned collection",
                                            Constants.ApplicationName + "\nVersion " + Constants.ProductVersion,
                                            MessageBoxButtons.OK);
                            return false;
                        }

                        // now verify partition key
                        string partitionKey = tbPartitionKeyForCollectionCreate.Text;
                        if (string.IsNullOrEmpty(partitionKey) || partitionKey[0] != '/' || partitionKey[partitionKey.Length -1 ] == ' ')
                        {
                            MessageBox.Show(this, 
                                            "PartitionKey is not in valid format",
                                            Constants.ApplicationName + "\nVersion " + Constants.ProductVersion,
                                            MessageBoxButtons.OK);
                            return false;
                        }
                    }
            else
            {
                        if (throughput < 400 || throughput > 10000)
                {
                            MessageBox.Show(this, 
                                            "Offer throughput has to be between 400 and 10000 for single partition collection",
                                            Constants.ApplicationName + "\nVersion " + Constants.ProductVersion,
                                            MessageBoxButtons.OK);
                            return false;
                }
            }
        }
            }

            return true;
        }

        private void toolStripBtnExecute_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
            {
                return;
            }

            this.SetLoadingState();

            RequestOptions requestOptions = this.GetRequestOptions();

            if (this.resourceType == ResourceType.DocumentCollection && (this.operationType == OperationType.Create || this.operationType == OperationType.Replace))
            {
                this.newDocumentCollection.IndexingPolicy.IncludedPaths.Clear();
                foreach (object item in lbIncludedPath.Items)
                {
                    IncludedPath includedPath = item as IncludedPath;
                    this.newDocumentCollection.IndexingPolicy.IncludedPaths.Add(includedPath);
                }

                this.newDocumentCollection.IndexingPolicy.ExcludedPaths.Clear();
                foreach (object item in lbExcludedPath.Items)
                {
                    String excludedPath = item as String;
                    this.newDocumentCollection.IndexingPolicy.ExcludedPaths.Add(new ExcludedPath() { Path = excludedPath });
                }

                this.newDocumentCollection.Id = tbCollectionId.Text;

                if (this.operationType == OperationType.Create)
                {
                    if (this.offerType == OfferType.StandardElastic)
                    {

                        this.newDocumentCollection.PartitionKey.Paths.Add(tbPartitionKeyForCollectionCreate.Text);
                    }
                }

                this.currentOperationCallback(newDocumentCollection, requestOptions);
            }
            else if (this.resourceType == ResourceType.Offer && this.operationType == OperationType.Replace)
            {
                this.currentOperationCallback(newDocumentCollection, requestOptions);
            }
            else if (this.resourceType == ResourceType.Trigger && this.operationType == OperationType.Create)
            {
                Trigger trigger = new Trigger();
                trigger.Body = this.tbCrudContext.Text;
                trigger.Id = this.textBoxforId.Text;
                trigger.TriggerOperation = TriggerOperation.All;
                if (rbPreTrigger.Checked)
                    trigger.TriggerType = TriggerType.Pre;
                else if (rbPostTrigger.Checked)
                    trigger.TriggerType = TriggerType.Post;

                this.currentOperationCallback(trigger, requestOptions);
            }
            else if (this.resourceType == ResourceType.Trigger && this.operationType == OperationType.Replace)
            {
                Trigger trigger = new Trigger();
                trigger.Body = this.tbCrudContext.Text;
                trigger.Id = this.textBoxforId.Text;
                this.currentOperationCallback(trigger, requestOptions);
           }

            else if (this.resourceType == ResourceType.UserDefinedFunction
                 && (this.operationType == OperationType.Create || this.operationType == OperationType.Replace))
            {
                UserDefinedFunction udf = new UserDefinedFunction();
                udf.Body = this.tbCrudContext.Text;
                udf.Id = this.textBoxforId.Text;
                this.currentOperationCallback(udf, requestOptions);
            }
            else if (this.resourceType == ResourceType.StoredProcedure
                 && (this.operationType == OperationType.Create || this.operationType == OperationType.Replace))
            {
                StoredProcedure storedProcedure = new StoredProcedure();
                storedProcedure.Body = this.tbCrudContext.Text;
                storedProcedure.Id = this.textBoxforId.Text;
                this.currentOperationCallback(storedProcedure, requestOptions);
            }
            else
            {
                if (!string.IsNullOrEmpty(this.tbCrudContext.SelectedText))
                {
                    this.currentOperationCallback(this.tbCrudContext.SelectedText, requestOptions);
                }
                else
                {
                    if (this.resourceType == ResourceType.StoredProcedure && this.operationType == OperationType.Execute && !this.tbCrudContext.Modified)
                    {
                        this.currentOperationCallback(null, requestOptions);
                    }
                    else
                    {
                        this.currentOperationCallback(this.tbCrudContext.Text, requestOptions);
                    }
                }
            }
        }

        public void SetLoadingState()
        {
            //
            this.webBrowserResponse.Url = new Uri(this.loadingGifPath);
        }

        public void RenderFile(string fileName)
        {
            //
            this.webBrowserResponse.Url = new Uri(fileName);
        }

        public void SetResultInBrowser(string json, string text, bool executeButtonEnabled, NameValueCollection responseHeaders = null)
        {
            this.currentText = text;
            this.currentJson = json;
            this.DisplayResponseContent();

            this.toolStripBtnExecute.Enabled = executeButtonEnabled;

            this.SetResponseHeaders(responseHeaders);
        }

        public void SetStatus(string status)
        {
            this.tsStatus.Text = status;
        }

        private void PrettyPrintJson(string json, string extraText)
        {
            if (string.IsNullOrEmpty(json))
            {
                json = "\"\"";
            }
            string prettyPrint = prettyJSONTemplate.Replace("&JSONSTRINGREPLACE&", json);

            if (string.IsNullOrEmpty(extraText))
            {
                extraText = "";
            }

            prettyPrint = prettyPrint.Replace("&EXTRASTRINGREPLACE&", Helper.FormatTextAsHtml(extraText, false, false));

            // save prettyePrint to file.
            string prettyPrintHtml = Path.Combine(this.appTempPath, "prettyPrint.Html");

            using (StreamWriter outfile = new StreamWriter(prettyPrintHtml))
            {
                outfile.Write(prettyPrint);
            }

            // now launch it in broswer!
            this.webBrowserResponse.Url = new Uri(prettyPrintHtml);
        }

        public void SetResponseHeaders(NameValueCollection responseHeaders)
        {
            if (responseHeaders != null)
            {
                string headers = "";

                string itemCountValue = null;
                string continuationValue = null;

                foreach (string key in responseHeaders.Keys)
                {
                    headers += string.Format(CultureInfo.InvariantCulture, "{0}: {1}\r\n", key, responseHeaders[key]);

                    if (string.Compare("x-ms-request-charge", key, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        this.tsStatus.Text = this.tsStatus.Text + ", RequestChange: " + responseHeaders[key];
                    }
                    if (string.Compare("x-ms-item-count", key, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        itemCountValue = responseHeaders[key];
                    }
                    if (string.Compare("x-ms-continuation", key, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        continuationValue = responseHeaders[key];
                    }
                }

                if (itemCountValue != null)
                {
                    this.tsStatus.Text = this.tsStatus.Text + ", Count: " + itemCountValue;
                    if (continuationValue != null)
                    {
                        this.tsStatus.Text = this.tsStatus.Text + "+";
                    }
                }

                this.tbResponse.Text = headers;
            }
        }

        private void btnExecuteNext_Click(object sender, EventArgs e)
        {
            this.SetLoadingState();

            if (!string.IsNullOrEmpty(this.tbCrudContext.SelectedText))
            {
                this.currentOperationCallback(this.tbCrudContext.SelectedText, null);
            }
            else
            {
                this.currentOperationCallback(this.tbCrudContext.Text, null);
            }
        }

        private void cbRequestOptionsApply_CheckedChanged(object sender, EventArgs e)
        {
            this.CreateDefaultRequestOptions();
            if (cbRequestOptionsApply.Checked)
            {
                rbIndexingDefault.Enabled = false;
                rbIndexingExclude.Enabled = false;
                rbIndexingInclude.Enabled = false;

                rbAccessConditionIfMatch.Enabled = false;
                rbAccessConditionIfNoneMatch.Enabled = false;
                tbAccessConditionText.Enabled = false;

                rbConsistencyBound.Enabled = false;
                rbConsistencyEventual.Enabled = false;
                rbConsistencySession.Enabled = false;
                rbConsistencyStrong.Enabled = false;

                tbPreTrigger.Enabled = false;
                tbPostTrigger.Enabled = false;

                tbPartitionKeyForRequestOption.Enabled = false;
            }
            else
            {
                rbIndexingDefault.Enabled = true;
                rbIndexingExclude.Enabled = true;
                rbIndexingInclude.Enabled = true;

                rbAccessConditionIfMatch.Enabled = true;
                rbAccessConditionIfNoneMatch.Enabled = true;
                tbAccessConditionText.Enabled = true;

                rbConsistencyEventual.Enabled = true;
                rbConsistencyBound.Enabled = true;
                rbConsistencySession.Enabled = true;
                rbConsistencyStrong.Enabled = true;

                tbPreTrigger.Enabled = true;
                tbPostTrigger.Enabled = true;

                tbPartitionKeyForRequestOption.Enabled = true;
            }
        }

        private void CreateDefaultRequestOptions()
        {
            this.requestOptions = new RequestOptions();

            if (rbIndexingDefault.Checked)
            {
                this.requestOptions.IndexingDirective = IndexingDirective.Default;
            }
            else if (rbIndexingExclude.Checked)
            {
                this.requestOptions.IndexingDirective = IndexingDirective.Exclude;
            }
            else if (rbIndexingInclude.Checked)
            {
                this.requestOptions.IndexingDirective = IndexingDirective.Include;
            }

            this.requestOptions.AccessCondition = new AccessCondition();
            if (rbAccessConditionIfMatch.Checked)
            {
                this.requestOptions.AccessCondition.Type = AccessConditionType.IfMatch;
            }
            else if (rbAccessConditionIfNoneMatch.Checked)
            {
                this.requestOptions.AccessCondition.Type = AccessConditionType.IfNoneMatch;
            }

            string condition = tbAccessConditionText.Text;
            if (!string.IsNullOrEmpty(condition))
            {
                this.requestOptions.AccessCondition.Condition = condition;
            }

            if (rbConsistencyStrong.Checked)
            {
                this.requestOptions.ConsistencyLevel = ConsistencyLevel.Strong;
            }
            else if (rbConsistencyBound.Checked)
            {
                this.requestOptions.ConsistencyLevel = ConsistencyLevel.BoundedStaleness;
            }
            else if (rbConsistencySession.Checked)
            {
                this.requestOptions.ConsistencyLevel = ConsistencyLevel.Session;
            }
            else if (rbConsistencyEventual.Checked)
            {
                this.requestOptions.ConsistencyLevel = ConsistencyLevel.Eventual;
            }

            string preTrigger = tbPreTrigger.Text;
            if (!string.IsNullOrEmpty(preTrigger))
            {
                // split by ;
                string[] segments = preTrigger.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                this.requestOptions.PreTriggerInclude = segments;
            }

            string postTrigger = tbPostTrigger.Text;
            if (!string.IsNullOrEmpty(postTrigger))
            {
                // split by ;
                string[] segments = postTrigger.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                this.requestOptions.PostTriggerInclude = segments;
            }

            string partitionKey = tbPartitionKeyForRequestOption.Text;
            if (!string.IsNullOrEmpty(partitionKey))
            {
                this.requestOptions.PartitionKey = new PartitionKey(partitionKey);
            }
        }

        private void rbIndexingDefault_CheckedChanged(object sender, EventArgs e)
        {
            this.requestOptions.IndexingDirective = IndexingDirective.Default;
        }

        private void rbIndexingInclude_CheckedChanged(object sender, EventArgs e)
        {
            this.requestOptions.IndexingDirective = IndexingDirective.Include;
        }

        private void rbIndexingExclude_CheckedChanged(object sender, EventArgs e)
        {
            this.requestOptions.IndexingDirective = IndexingDirective.Exclude;
        }

        private void rbAccessConditionIfMatch_CheckedChanged(object sender, EventArgs e)
        {
            this.requestOptions.AccessCondition.Type = AccessConditionType.IfMatch;
        }

        private void rbAccessConditionIfNoneMatch_CheckedChanged(object sender, EventArgs e)
        {
            this.requestOptions.AccessCondition.Type = AccessConditionType.IfNoneMatch;
        }

        private void rbConsistencyStrong_CheckedChanged(object sender, EventArgs e)
        {
            this.requestOptions.ConsistencyLevel = ConsistencyLevel.Strong;
        }

        private void rbConsistencyBound_CheckedChanged(object sender, EventArgs e)
        {
            this.requestOptions.ConsistencyLevel = ConsistencyLevel.BoundedStaleness;
        }

        private void rbConsistencySession_CheckedChanged(object sender, EventArgs e)
        {
            this.requestOptions.ConsistencyLevel = ConsistencyLevel.Session;
        }

        private void rbConsistencyEventual_CheckedChanged(object sender, EventArgs e)
        {
            this.requestOptions.ConsistencyLevel = ConsistencyLevel.Eventual;
        }

        private void btnAddIncludePath_Click(object sender, EventArgs e)
        {
            IncludedPathForm dlg = new IncludedPathForm();
            dlg.StartPosition = FormStartPosition.CenterParent;
            DialogResult dr = dlg.ShowDialog(this);
            if (dr == DialogResult.OK)
            {
                this.lbIncludedPath.Items.Add(dlg.IncludedPath);
            }
        }

        private void btnRemovePath_Click(object sender, EventArgs e)
        {
            this.lbIncludedPath.Items.RemoveAt(this.lbIncludedPath.SelectedIndex);
        }

        private void lbIncludedPath_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.lbIncludedPath.SelectedItem != null)
            {
                btnEdit.Enabled = true;
                btnRemovePath.Enabled = true;
            }
            else
            {
                btnEdit.Enabled = false;
                btnRemovePath.Enabled = false;
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            IncludedPath includedPath = this.lbIncludedPath.SelectedItem as IncludedPath;

            IncludedPathForm dlg = new IncludedPathForm();
            dlg.StartPosition = FormStartPosition.CenterParent;

            dlg.SetIncludedPath(includedPath);

            DialogResult dr = dlg.ShowDialog(this);
            if (dr == DialogResult.OK)
            {
                this.lbIncludedPath.Items[this.lbIncludedPath.SelectedIndex] = dlg.IncludedPath;
            }
        }

        private void btnAddExcludedPath_Click(object sender, EventArgs e)
        {
            ExcludedPathForm dlg = new ExcludedPathForm();
            DialogResult dr = dlg.ShowDialog(this);
            if (dr == DialogResult.OK)
            {
                this.lbExcludedPath.Items.Add(dlg.ExcludedPath);
            }
        }

        private void btnRemoveExcludedPath_Click(object sender, EventArgs e)
        {
            this.lbExcludedPath.Items.RemoveAt(this.lbExcludedPath.SelectedIndex);
        }

        private void lbExcludedPath_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.lbExcludedPath.SelectedItem != null)
            {
                btnRemoveExcludedPath.Enabled = true;
            }
            else
            {
                btnRemoveExcludedPath.Enabled = false;
            }
        }

        private void cbIndexingPolicyDefault_CheckedChanged(object sender, EventArgs e)
        {
            if (cbIndexingPolicyDefault.Checked)
            {
                cbAutomatic.Enabled = false;
                rbConsistent.Enabled = false;
                rbLazy.Enabled = false;

                lbIncludedPath.Enabled = false;
                btnAddIncludePath.Enabled = false;
                btnRemovePath.Enabled = false;
                btnEdit.Enabled = false;

                lbExcludedPath.Enabled = false;
                btnAddExcludedPath.Enabled = false;
                btnRemoveExcludedPath.Enabled = false;

                tbPartitionKeyForCollectionCreate.Enabled = false;

                this.newDocumentCollection = new DocumentCollection();
            }
            else
            {
                cbAutomatic.Enabled = true;
                rbConsistent.Enabled = true;
                rbLazy.Enabled = true;

                lbIncludedPath.Enabled = true;
                btnAddIncludePath.Enabled = true;
                btnRemovePath.Enabled = false;
                btnEdit.Enabled = false;

                lbExcludedPath.Enabled = true;
                btnAddExcludedPath.Enabled = true;
                btnRemoveExcludedPath.Enabled = false;

                tbPartitionKeyForCollectionCreate.Enabled = true;

                this.CreateDefaultIndexingPolicy();
            }
        }


        private void CreateDefaultIndexingPolicy()
        {
            this.newDocumentCollection.IndexingPolicy.Automatic = cbAutomatic.Checked;

            if (this.rbConsistent.Checked)
            {
                this.newDocumentCollection.IndexingPolicy.IndexingMode = IndexingMode.Consistent;
            }
            else
            {
                this.newDocumentCollection.IndexingPolicy.IndexingMode = IndexingMode.Lazy;
            }
        }

        private void cbAutomatic_CheckedChanged(object sender, EventArgs e)
        {
            this.newDocumentCollection.IndexingPolicy.Automatic = cbAutomatic.Checked;
        }

        private void rbConsistent_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rbConsistent.Checked)
            {
                this.newDocumentCollection.IndexingPolicy.IndexingMode = IndexingMode.Consistent;
            }
            else
            {
                this.newDocumentCollection.IndexingPolicy.IndexingMode = IndexingMode.Lazy;
            }
        }

        private void rbLazy_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rbConsistent.Checked)
            {
                this.newDocumentCollection.IndexingPolicy.IndexingMode = IndexingMode.Consistent;
            }
            else
            {
                this.newDocumentCollection.IndexingPolicy.IndexingMode = IndexingMode.Lazy;
            }
        }

        private void rbOfferS1_CheckedChanged(object sender, EventArgs e)
        {
            if (rbOfferS1.Checked)
            {
                this.offerType = OfferType.S1;
                this.labelThroughput.Text = "Fixed 250 RU. 10GB Storage";
                this.tbThroughput.Enabled = false;
                this.tbThroughput.Text = "250";
            }
        }

        private void rbOfferS2_CheckedChanged(object sender, EventArgs e)
        {
            if (rbOfferS2.Checked)
            {
                this.offerType = OfferType.S2;
                this.labelThroughput.Text = "Fixed 1000 RU. 10GB Storage ";
                this.tbThroughput.Enabled = false;
                this.tbThroughput.Text = "1000";
             }
        }

        private void rbOfferS3_CheckedChanged(object sender, EventArgs e)
        {
            if (rbOfferS3.Checked)
            {
                this.offerType = OfferType.S3;
                this.labelThroughput.Text = "Fixed 2500 RU. 10GB Storage ";
                this.tbThroughput.Enabled = false;
                this.tbThroughput.Text = "2500";
            }
        }

        private void rbElasticCollection_CheckedChanged(object sender, EventArgs e)
        {
            if (rbElasticCollection.Checked)
            {
                this.offerType = OfferType.StandardElastic;
                this.tbPartitionKeyForCollectionCreate.Enabled = true;
                this.labelThroughput.Text = "Allowed values > 10k and <= 250k. 250GB Storage";
                this.tbThroughput.Enabled = true;
                this.tbThroughput.Text = "20000";
            }
            else
                tbPartitionKeyForCollectionCreate.Enabled = false;

        }

        private void cbShowLegacyOffer_CheckedChanged(object sender, EventArgs e)
        {
            this.rbOfferS1.Visible = this.cbShowLegacyOffer.Checked;
            this.rbOfferS2.Visible = this.cbShowLegacyOffer.Checked;
            this.rbOfferS3.Visible = this.cbShowLegacyOffer.Checked;

            if (!this.cbShowLegacyOffer.Checked)
            {
                this.rbSinglePartition.Checked = true;
            }
    }

        private void rbSinglePartition_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rbSinglePartition.Checked)
            {
                this.offerType = OfferType.StandardSingle;
                this.labelThroughput.Text = "Allowed values between 400 - 10k. 10GB Storage";
                this.tbThroughput.Enabled = true;
                this.tbThroughput.Text = "400";
            }
        }

}
}