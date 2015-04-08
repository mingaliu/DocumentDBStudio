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
        Action<string, object> currentOperation;

        private RequestOptions requestOptions;
        CheckBox cbEnableScan;

        private DocumentCollection collectionToCreate;

        private String currentCrudName;
        private String offerType;

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
            this.tabControl.TabPages.Remove(this.tabDocumentCollectionPolicy);

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
            ToolStripControlHost host = new ToolStripControlHost(cbEnableScan);
            feedToolStrip.Items.Insert(1, host);

            lbIncludedPath.Items.Add(new IndexingPath() { Path = "/", IndexType = IndexType.Hash });
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

            return feedOptions;
        }

        public RequestOptions GetRequestOptions(bool isCollection = false)
        {
            if (this.requestOptions != null)
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
            }

            RequestOptions requestOptions = this.requestOptions;

            if (isCollection)
            {
                if (requestOptions != null)
                {
                    requestOptions.OfferType = this.offerType;
                }
                else
                {
                    requestOptions = new RequestOptions() { OfferType = this.offerType };
                }
            }

            return requestOptions;
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
                DocumentClient client = new DocumentClient(new Uri(accountEndpoint), accountSettings.MasterKey,
                    new ConnectionPolicy { ConnectionMode = accountSettings.ConnectionMode, ConnectionProtocol = accountSettings.Protocol });

                DatabaseAccountNode dbaNode = new DatabaseAccountNode(accountEndpoint, client);
                this.treeView1.Nodes.Add(dbaNode);

                dbaNode.Tag = client.GetDatabaseAccountAsync().Result;

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

                if (e.Node is DocumentNode)
                {
                    DocumentNode node = e.Node as DocumentNode;
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

        public void SetCrudContext(TreeNode node, string name, bool showId, string bodytext, Action<string, object> func, CommandContext commandContext = null)
        {
            if (commandContext == null)
            {
                commandContext = new CommandContext();
            }

            this.treeView1.SelectedNode = node;

            this.currentCrudName = name;

            this.currentOperation = func;
            this.tabCrudContext.Text = name;
            this.tbCrudContext.Text = bodytext;

            this.toolStripBtnExecute.Enabled = true;
            this.tbCrudContext.ReadOnly = commandContext.IsDelete;

            // the whole left split panel.
            this.splitContainerInner.Panel1Collapsed = false;
            //the split panel inside Tab. Panel1: Id, Panel2: Edit CRUD.
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

            if (string.Compare(name, "Create documentCollection", StringComparison.OrdinalIgnoreCase) == 0 ||
                string.Compare(name, "Replace DocumentCollection", StringComparison.OrdinalIgnoreCase) == 0)
            {
                if (string.Compare(name, "Create documentCollection", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    cbIndexingPolicyDefault.Enabled = true;
                }
                else
                {
                    cbIndexingPolicyDefault.Enabled = false;
                }

                if (this.tabControl.TabPages.Contains(this.tabCrudContext))
                {
                    this.tabControl.TabPages.Insert(0, this.tabDocumentCollectionPolicy);
                    this.tabControl.TabPages.Remove(this.tabCrudContext);
                }
                this.tabControl.SelectedTab = this.tabDocumentCollectionPolicy;
            }
            else
            {
                if (this.tabControl.TabPages.Contains(this.tabDocumentCollectionPolicy))
                {
                    this.tabControl.TabPages.Remove(this.tabDocumentCollectionPolicy);
                    this.tabControl.TabPages.Insert(0, this.tabCrudContext);
                }
                this.tabControl.SelectedTab = this.tabCrudContext;
            }
        }

        public void SetNextPageVisibility(CommandContext commandContext)
        {
            this.btnExecuteNext.Enabled = commandContext.HasContinuation || !commandContext.QueryStarted;
        }

        private void toolStripBtnExecute_Click(object sender, EventArgs e)
        {
            this.SetLoadingState();

            if (string.Compare(this.currentCrudName, "Create documentCollection", StringComparison.OrdinalIgnoreCase) == 0 ||
                string.Compare(this.currentCrudName, "Replace documentCollection", StringComparison.OrdinalIgnoreCase) == 0)
            {
                this.collectionToCreate.IndexingPolicy.IncludedPaths.Clear();
                foreach (object item in lbIncludedPath.Items)
                {
                    IndexingPath path = item as IndexingPath;
                    this.collectionToCreate.IndexingPolicy.IncludedPaths.Add(path);
                }

                this.collectionToCreate.IndexingPolicy.ExcludedPaths.Clear();
                foreach (object item in lbExcludedPath.Items)
                {
                    String path = item as String;
                    this.collectionToCreate.IndexingPolicy.ExcludedPaths.Add(path);
                }

                this.collectionToCreate.Id = tbCollectionId.Text;
                this.currentOperation(null, collectionToCreate);
            }
            else if (this.currentCrudName.StartsWith("Create trigger", StringComparison.OrdinalIgnoreCase))
            {
                Trigger trigger = new Documents.Trigger();
                trigger.Body = this.tbCrudContext.Text;
                trigger.Id = this.textBoxforId.Text;
                trigger.TriggerOperation = Documents.TriggerOperation.All;
                if (rbPreTrigger.Checked)
                    trigger.TriggerType = Documents.TriggerType.Pre;
                else if (rbPostTrigger.Checked)
                    trigger.TriggerType = Documents.TriggerType.Post;

                this.currentOperation(null, trigger);
            }
            else
            {
                if (!string.IsNullOrEmpty(this.tbCrudContext.SelectedText))
                {
                    this.currentOperation(this.tbCrudContext.SelectedText, this.textBoxforId.Text);
                }
                else
                {
                    if (this.currentCrudName.StartsWith("Execute StoredProcedure", StringComparison.Ordinal) && !this.tbCrudContext.Modified)
                    {
                        this.currentOperation(null, this.textBoxforId.Text);
                    }
                    else
                    {
                        this.currentOperation(this.tbCrudContext.Text, this.textBoxforId.Text);
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
                foreach (string key in responseHeaders.Keys)
                {
                    headers += string.Format(CultureInfo.InvariantCulture, "{0}: {1}\r\n", key, responseHeaders[key]);

                    if (string.Compare("x-ms-request-charge", key, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        this.tsStatus.Text = this.tsStatus.Text + ", RequestChange: " + responseHeaders[key];
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
                this.currentOperation(this.tbCrudContext.SelectedText, this.textBoxforId.Text);
            }
            else
            {
                this.currentOperation(this.tbCrudContext.Text, this.textBoxforId.Text);
            }
        }

        private void cbRequestOptionsApply_CheckedChanged(object sender, EventArgs e)
        {
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

                this.requestOptions = null;
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

                this.CreateDefaultRequestOptions();
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
            IndexingPathForm dlg = new IndexingPathForm();
            DialogResult dr = dlg.ShowDialog(this);
            if (dr == DialogResult.OK)
            {
                this.lbIncludedPath.Items.Add(dlg.IndexingPath);
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
            IndexingPath path = this.lbIncludedPath.SelectedItem as IndexingPath;

            IndexingPathForm dlg = new IndexingPathForm();

            dlg.SetIndexingPath(path);

            DialogResult dr = dlg.ShowDialog(this);
            if (dr == DialogResult.OK)
            {
                this.lbIncludedPath.Items[this.lbIncludedPath.SelectedIndex] = path;
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

                this.collectionToCreate = new DocumentCollection();
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

                this.CreateDefaultIndexingPolicy();
            }
        }


        private void CreateDefaultIndexingPolicy()
        {
            this.collectionToCreate.IndexingPolicy.Automatic = cbAutomatic.Checked;

            if (this.rbConsistent.Checked)
            {
                this.collectionToCreate.IndexingPolicy.IndexingMode = IndexingMode.Consistent;
            }
            else
            {
                this.collectionToCreate.IndexingPolicy.IndexingMode = IndexingMode.Lazy;
            }
        }

        private void cbAutomatic_CheckedChanged(object sender, EventArgs e)
        {
            this.collectionToCreate.IndexingPolicy.Automatic = cbAutomatic.Checked;
        }

        private void rbConsistent_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rbConsistent.Checked)
            {
                this.collectionToCreate.IndexingPolicy.IndexingMode = IndexingMode.Consistent;
            }
            else
            {
                this.collectionToCreate.IndexingPolicy.IndexingMode = IndexingMode.Lazy;
            }
        }

        private void rbLazy_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rbConsistent.Checked)
            {
                this.collectionToCreate.IndexingPolicy.IndexingMode = IndexingMode.Consistent;
            }
            else
            {
                this.collectionToCreate.IndexingPolicy.IndexingMode = IndexingMode.Lazy;
            }
        }

        private void rbOfferS1_CheckedChanged(object sender, EventArgs e)
        {
            this.offerType = "S1";
        }

        private void rbOfferS2_CheckedChanged(object sender, EventArgs e)
        {
            this.offerType = "S2";
        }

        private void rbOfferS3_CheckedChanged(object sender, EventArgs e)
        {
            this.offerType = "S3";
        }


    }


}