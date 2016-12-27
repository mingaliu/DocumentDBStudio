//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using Microsoft.Azure.DocumentDBStudio.Helpers;

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
    using Properties;
    using Documents;
    using Documents.Client;
    using Documents.Routing;
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
            ThreadPool.QueueUserWorkItem(arg => CheckCurrentRelease());

            Height = Screen.GetWorkingArea(this).Height * 3 / 4;
            Width = Screen.GetWorkingArea(this).Width / 2;
            Top = 0;
            Text = Constants.ApplicationName;

            using (Stream stm = Assembly.GetExecutingAssembly().GetManifestResourceStream("Microsoft.Azure.DocumentDBStudio.Resources.home.html"))
            {
                using (StreamReader reader = new StreamReader(stm))
                {
                    homepage = reader.ReadToEnd();
                }
            }
            homepage = homepage.Replace("&VERSION&", Constants.ProductVersion);

            DateTime t = File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location);
            DateTimeOffset dateOffset = new DateTimeOffset(t, TimeZoneInfo.Local.GetUtcOffset(t));
            homepage = homepage.Replace("&BUILDTIME&", t.ToString("f", CultureInfo.CurrentCulture));

            cbUrl.Items.Add("about:home");
            cbUrl.SelectedIndex = 0;
            cbUrl.KeyDown += cbUrl_KeyDown;

            btnBack.Enabled = false;

            splitContainerOuter.Panel1Collapsed = false;
            splitContainerInner.Panel1Collapsed = true;
            ButtomSplitContainer.Panel1Collapsed = true;

            KeyPreview = true;
            PreviewKeyDown += MainForm_PreviewKeyDown;

            webBrowserResponse.PreviewKeyDown += webBrowserResponse_PreviewKeyDown;
            webBrowserResponse.StatusTextChanged += webBrowserResponse_StatusTextChanged;
            webBrowserResponse.ScriptErrorsSuppressed = true;

            tabControl.SelectedTab = tabCrudContext;
            tabControl.TabPages.Remove(tabRequest);
            tabControl.TabPages.Remove(tabDocumentCollection);
            tabControl.TabPages.Remove(tabOffer);

            var imageList = BuildImageList();
            treeView1.ImageList = imageList;

            InitTreeView();

            btnHome_Click(null, null);

            splitContainerIntabPage.Panel1Collapsed = true;

            toolStripBtnExecute.Enabled = false;
            btnExecuteNext.Enabled = false;
            UnpackEmbeddedResources();

            btnHeaders.Checked = false;

            cbRequestOptionsApply_CheckedChanged(null, null);
            cbIndexingPolicyDefault_CheckedChanged(null, null);

            cbEnableScan = new CheckBox
            {
                Text = "EnableScanInQuery",
                CheckState = CheckState.Indeterminate
            };
            ToolStripControlHost host1 = new ToolStripControlHost(cbEnableScan);
            feedToolStrip.Items.Insert(3, host1);

            cbEnableCrossPartitionQuery = new CheckBox
            {
                Text = "EnableCrossPartitionQuery",
                CheckState = CheckState.Indeterminate
            };
            ToolStripControlHost host2 = new ToolStripControlHost(cbEnableCrossPartitionQuery);
            feedToolStrip.Items.Insert(4, host2);

            lbIncludedPath.Items.Add(new IncludedPath() { Path = "/" });

            offerType = OfferType.StandardSingle;
            tbThroughput.Text = "400";

            tsbHideDocumentSystemProperties.Checked = Settings.Default.HideDocumentSystemProperties;
            SetDocumentSystemPropertiesTabText();

            tsbViewType.Checked = Settings.Default.TextView;
            SetViewTypeTabText();
        }

        private static ImageList BuildImageList()
        {
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
            return imageList;
        }


        private void UnpackEmbeddedResources()
        {
            appTempPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DocumentDBStudio");

            if (!Directory.Exists(appTempPath))
            {
                Directory.CreateDirectory(appTempPath);
            }

            loadingGifPath = Path.Combine(appTempPath, "loading.gif");

            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Microsoft.Azure.DocumentDBStudio.Resources.loading.gif"))
            {
                using (FileStream fileStream = File.Create(loadingGifPath))
                {
                    stream.CopyTo(fileStream);
                }
            }

            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Microsoft.Azure.DocumentDBStudio.Resources.prettyJSON.backbone-min.js"))
            {
                using (FileStream fileStream = File.Create(Path.Combine(appTempPath, "backbone-min.js")))
                {
                    stream.CopyTo(fileStream);
                }
            }
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Microsoft.Azure.DocumentDBStudio.Resources.prettyJSON.jquery-1.11.1.min.js"))
            {
                using (FileStream fileStream = File.Create(Path.Combine(appTempPath, "jquery-1.11.1.min.js")))
                {
                    stream.CopyTo(fileStream);
                }
            }
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Microsoft.Azure.DocumentDBStudio.Resources.prettyJSON.pretty-json.css"))
            {
                using (FileStream fileStream = File.Create(Path.Combine(appTempPath, "pretty-json.css")))
                {
                    stream.CopyTo(fileStream);
                }
            }
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Microsoft.Azure.DocumentDBStudio.Resources.prettyJSON.pretty-json-min.js"))
            {
                using (FileStream fileStream = File.Create(Path.Combine(appTempPath, "pretty-json-min.js")))
                {
                    stream.CopyTo(fileStream);
                }
            }
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Microsoft.Azure.DocumentDBStudio.Resources.prettyJSON.underscore-min.js"))
            {
                using (FileStream fileStream = File.Create(Path.Combine(appTempPath, "underscore-min.js")))
                {
                    stream.CopyTo(fileStream);
                }
            }

            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Microsoft.Azure.DocumentDBStudio.Resources.prettyJSON.PrettyPrintJSONTemplate.html"))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    prettyJSONTemplate = reader.ReadToEnd();
                }
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            // ToolStrips don't appear to have a way to "spring" their items like status bars
            cbUrl.Width = tsAddress.Width - 40 - tsLabelUrl.Width - btnGo.Width;
        }

        void webBrowserResponse_StatusTextChanged(object sender, EventArgs e)
        {
        }

        void cbUrl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                webBrowserResponse.Navigate(cbUrl.Text);
                e.Handled = true;
            }
        }

        void webBrowserResponse_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (webBrowserResponse.Focused)
            {
                HandlePreviewKeyDown(e.KeyCode, e.Modifiers);
            }
        }

        void MainForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (!webBrowserResponse.Focused && !cbUrl.Focused)
            {
                HandlePreviewKeyDown(e.KeyCode, e.Modifiers);
            }
        }

        bool HandlePreviewKeyDown(Keys key, Keys modifiers)
        {
            if (key == Keys.Back)
            {
                // Don't steal backspace from the URL combo box
                if (!cbUrl.Focused)
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
                webBrowserResponse.Navigate(cbUrl.Text);
                return true;
            }
            else if (key == Keys.W && modifiers == Keys.Control)
            {
                // Exit the app on Ctrl + W like browser tabs
                Close();
                return true;
            }
            else if (key == Keys.D && modifiers == Keys.Alt)
            {
                // Focus the URL in the address bar
                cbUrl.SelectAll();
                cbUrl.Focus();
            }
            return false;
        }

        private void tbCrudContext_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F5:
                    {
                        if (toolStripBtnExecute.Enabled)
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
            if (tsbViewType.Checked)
            {
                PrettyPrintJson(currentJson, currentText);
            }
            else
            {
                string htmlResponse = "";

                if (!string.IsNullOrEmpty(currentJson))
                {
                    htmlResponse = Helper.FormatTextAsHtml(currentJson, false);
                }
                if (!string.IsNullOrEmpty(currentText))
                {
                    htmlResponse += "\r\n\r\n" + Helper.FormatTextAsHtml(currentText, false);
                }
                DisplayHtmlContentInScale(htmlResponse);
            }
        }

        void DisplayHtmlContentInScale(string htmlResponse)
        {
            if (fontScale != 100)
            {
                // current scaled font
                float fontPt = defaultFontPoint * (fontScale / 100.0f);

                // todo: make this a well defined class
                string style = "{ font-size: " + fontPt + "pt; }";
                string s = htmlResponse.Replace("{ font-size: 9pt; }", style);
                webBrowserResponse.DocumentText = s;
            }
            else
            {
                webBrowserResponse.DocumentText = htmlResponse;
            }
        }

        private void tsButtonZoom_ButtonClick(object sender, EventArgs e)
        {
            switch (tsButtonZoom.Text)
            {
                case "100%":
                    fontScale = 125;
                    break;
                case "125%":
                    fontScale = 150;
                    break;
                case "150%":
                    fontScale = 175;
                    break;
                case "175%":
                    fontScale = 100;
                    break;

            }
            tsButtonZoom.Text = fontScale.ToString(CultureInfo.CurrentCulture) + "%";
            tbRequest.Font = new Font(tbRequest.Font.FontFamily.Name, defaultFontPoint * (fontScale / 100.0f));
            tbResponse.Font = new Font(tbResponse.Font.FontFamily.Name, defaultFontPoint * (fontScale / 100.0f));
            Font = new Font(tbResponse.Font.FontFamily.Name, defaultFontPoint * (fontScale / 100.0f));

            // we don't support pretty print for font scaling yet.
            if (!tsbViewType.Checked)
            {
                DisplayResponseContent();
            }
        }

        private void btnHeaders_Click(object sender, EventArgs e)
        {
            if (splitContainerInner.Panel1Collapsed == true)
            {
                splitContainerInner.Panel1Collapsed = false;
                btnHeaders.Checked = true;
                btnHeaders.Text = "Hide Response Headers";

                tabControl.SelectedTab = tabResponse;
            }
            else
            {
                splitContainerInner.Panel1Collapsed = true;
                btnHeaders.Checked = false;
                btnHeaders.Text = "Show Response Headers";
            }
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            //
            DisplayHtmlContentInScale(homepage);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this, Constants.ApplicationName + "\nVersion " + Constants.ProductVersion,
                            "About",
                            MessageBoxButtons.OK);
        }

        private void tsbViewType_Click(object sender, EventArgs e)
        {
            Settings.Default.TextView = tsbViewType.Checked;
            Settings.Default.Save();

            SetViewTypeTabText();

            if ((webBrowserResponse.Url.AbsoluteUri == "about:blank" && webBrowserResponse.DocumentTitle != "DataModelBrowserHome")
                || webBrowserResponse.Url.Scheme == "file")
            {
                DisplayResponseContent();
            }
        }

        private void SetViewTypeTabText()
        {
            if (Settings.Default.TextView)
                tsbViewType.Text = "Pretty Json View";
            else
                tsbViewType.Text = "Text View";
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Bring up account setings dialog
            SettingsForm dlg = new SettingsForm();
            DialogResult dr = dlg.ShowDialog(this);
            if (dr == DialogResult.OK)
            {
                AddAccountToSettings(dlg.AccountEndpoint, dlg.AccountSettings);
            }
        }

        public void ChangeAccountSettings(TreeNode thisNode, string accountEndpoint)
        {
            treeView1.SelectedNode = thisNode;

            for (int i = 0; i < Settings.Default.AccountSettingsList.Count; i = i + 2)
            {
                if (string.Compare(accountEndpoint, Settings.Default.AccountSettingsList[i], StringComparison.OrdinalIgnoreCase) == 0)
                {
                    AccountSettings accountSettings = (AccountSettings)JsonConvert.DeserializeObject(Settings.Default.AccountSettingsList[i + 1], typeof(AccountSettings));

                    // Bring up account setings dialog
                    SettingsForm dlg = new SettingsForm
                    {
                        AccountEndpoint = accountEndpoint,
                        AccountSettings = accountSettings
                    };

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
                if (string.Compare(accountEndpoint, Settings.Default.AccountSettingsList[i], StringComparison.OrdinalIgnoreCase) == 0)
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
                if (string.Compare(accountEndpoint, Settings.Default.AccountSettingsList[i], StringComparison.OrdinalIgnoreCase) == 0)
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

            try
            {
                feedOptions.MaxDegreeOfParallelism = Convert.ToInt32(toolStripTextMaxDop.Text, CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                // Ignore the exception and use the defualt value
            }

            try
            {
                feedOptions.MaxBufferedItemCount = Convert.ToInt32(toolStripTextMaxBuffItem.Text, CultureInfo.InvariantCulture);
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
                    requestOptions.PostTriggerInclude = segments;
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
                    requestOptions.PreTriggerInclude = segments;
                }
            }

            if (tbAccessConditionText.Modified)
            {
                string condition = tbAccessConditionText.Text;
                if (!string.IsNullOrEmpty(condition))
                {
                    requestOptions.AccessCondition.Condition = condition;
                }
            }

            if (tbPartitionKeyForRequestOption.Modified)
            {
                string partitionKey = tbPartitionKeyForRequestOption.Text;
                if (!string.IsNullOrEmpty(partitionKey))
                {
                    requestOptions.PartitionKey = new PartitionKey(partitionKey);
                }
            }

            if ((resourceType == ResourceType.DocumentCollection || resourceType == ResourceType.Offer) &&
                (operationType == OperationType.Create || operationType == OperationType.Replace))
            {
                if (offerType == OfferType.StandardSingle || offerType == OfferType.StandardElastic)
                {
                    requestOptions.OfferThroughput = int.Parse(tbThroughput.Text, CultureInfo.CurrentCulture);
                    requestOptions.OfferType = null;
                }
                else
                {
                    requestOptions.OfferType = offerType.ToString();
                    requestOptions.OfferThroughput = null;
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
                            Invoke(new MessageBoxDelegate(ShowMessage),
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
                AddConnectionTreeNode(Settings.Default.AccountSettingsList[i], accountSettings);
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
                        EnableEndpointDiscovery = accountSettings.EnableEndpointDiscovery,
                        UserAgentSuffix = suffix
                    });

                DatabaseAccountNode dbaNode = new DatabaseAccountNode(accountEndpoint, client);
                treeView1.Nodes.Add(dbaNode);

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
            switch (e.Button)
            {
                case MouseButtons.Right:
                    if (e.Node is FeedNode)
                    {
                        (e.Node as FeedNode).ShowContextMenu(e.Node.TreeView, e.Location);
                    }
                    break;
                case MouseButtons.Left:
                    // render the JSON in the right panel.
                    currentText = null;
                    currentJson = null;

                    if (e.Node is ResourceNode)
                    {
                        var node = e.Node as ResourceNode;
                        var body = node.GetBody();

                        if (!string.IsNullOrEmpty(body))
                        {
                            currentText = body;
                        }
                    }

                    if (e.Node.Tag is string)
                    {
                        currentText = e.Node.Tag.ToString();
                    }
                    else if (e.Node is DatabaseAccountNode)
                    {
                        currentJson = JsonConvert.SerializeObject(e.Node.Tag, Formatting.Indented);
                    }
                    else if (e.Node.Tag != null)
                    {
                        var tag = e.Node.Tag;
                        currentJson = tag.ToString();
                        currentJson = DocumentHelper.RemoveInternalDocumentValues(currentJson);
                    }

                    if (currentJson == null && currentText == null)
                    {
                        currentText = e.Node.Text;
                    }

                    DisplayResponseContent();
                    break;
            }
        }

        internal void SetCrudContext(
            TreeNode node, OperationType operation, ResourceType resourceType, string bodytext, 
            Action<object, RequestOptions> func, CommandContext commandContext = null
        )
        {
            if (commandContext == null)
            {
                commandContext = new CommandContext();
            }

            treeView1.SelectedNode = node;

            operationType = operation;
            this.resourceType = resourceType;
            currentOperationCallback = func;
            tabCrudContext.Text = string.Format("{0} {1}", operation, resourceType);
            tbCrudContext.Text = bodytext;

            toolStripBtnExecute.Enabled = true;
            tbCrudContext.ReadOnly = commandContext.IsDelete;

            // the split panel at the right side, Panel1: tab Control,  Panel2: ButtomSplitContainer (next page and browser)
            splitContainerInner.Panel1Collapsed = false;

            var showId = (resourceType == ResourceType.Trigger || resourceType == ResourceType.UserDefinedFunction || resourceType == ResourceType.StoredProcedure)
                 && (operation == OperationType.Create || operation == OperationType.Replace);

            //the split panel inside Tab. Panel1: Id input box, Panel2: Edit CRUD resource.
            splitContainerIntabPage.Panel1Collapsed = !showId;

            tbResponse.Text = "";

            //the split panel at right bottom. Panel1: NextPage, Panel2: Browser.
            if (commandContext.IsFeed)
            {
                ButtomSplitContainer.Panel1Collapsed = false;
                ButtomSplitContainer.Panel1.Controls.Clear();
                ButtomSplitContainer.Panel1.Controls.Add(feedToolStrip);
            }
            else if (commandContext.IsCreateTrigger)
            {
                ButtomSplitContainer.Panel1Collapsed = false;
                ButtomSplitContainer.Panel1.Controls.Clear();
                ButtomSplitContainer.Panel1.Controls.Add(triggerPanel);
            }
            else
            {
                ButtomSplitContainer.Panel1Collapsed = true;
            }

            SetNextPageVisibility(commandContext);

            // Prepare the tab pages
            tabControl.TabPages.Remove(tabDocumentCollection);
            tabControl.TabPages.Remove(tabOffer);
            tabControl.TabPages.Remove(tabCrudContext);


            if (this.resourceType == ResourceType.DocumentCollection && (operationType == OperationType.Create || operationType == OperationType.Replace))
            {
                tabControl.TabPages.Insert(0, tabDocumentCollection);
                if (operationType == OperationType.Create)
                {
                    tbCollectionId.Enabled = true;
                    tbCollectionId.Text = "DocumentCollection Id";

                    tabControl.TabPages.Insert(0, tabOffer);
                    tabControl.SelectedTab = tabOffer;

                    webBrowserResponse.Navigate(new Uri("https://azure.microsoft.com/en-us/documentation/articles/documentdb-performance-levels/"));
                }
                else
                {
                    tbCollectionId.Enabled = false;
                    tbCollectionId.Text = (node.Tag as Resource).Id;
                    tabControl.SelectedTab = tabDocumentCollection;
                }
            }
            else
            {
                tabControl.TabPages.Insert(0, tabCrudContext);
                tabControl.SelectedTab = tabCrudContext;
            }
        }

        public void SetNextPageVisibility(CommandContext commandContext)
        {
            btnExecuteNext.Enabled = commandContext.HasContinuation || !commandContext.QueryStarted;
        }

        private bool ValidateInput()
        {
            if ((resourceType == ResourceType.DocumentCollection || resourceType == ResourceType.Offer) &&
                (operationType == OperationType.Create || operationType == OperationType.Replace))
            {
                // basic validation:
                if (offerType == OfferType.StandardElastic || offerType == OfferType.StandardSingle)
                {
                    int throughput;
                    if (!int.TryParse(tbThroughput.Text, NumberStyles.Integer, CultureInfo.CurrentCulture, out throughput))
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

                    if (offerType == OfferType.StandardElastic)
                    {
                        // remove 250K from the internal version
                        if (throughput < 10000)
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

            SetLoadingState();

            var requestOptions = GetRequestOptions();

            if (resourceType == ResourceType.DocumentCollection && (operationType == OperationType.Create || operationType == OperationType.Replace))
            {
                newDocumentCollection.IndexingPolicy.IncludedPaths.Clear();
                foreach (var item in lbIncludedPath.Items)
                {
                    var includedPath = item as IncludedPath;
                    newDocumentCollection.IndexingPolicy.IncludedPaths.Add(includedPath);
                }

                newDocumentCollection.IndexingPolicy.ExcludedPaths.Clear();
                foreach (var item in lbExcludedPath.Items)
                {
                    var excludedPath = item as string;
                    newDocumentCollection.IndexingPolicy.ExcludedPaths.Add(new ExcludedPath()
                    {
                        Path = excludedPath
                    });
                }

                newDocumentCollection.Id = tbCollectionId.Text;

                if (operationType == OperationType.Create)
                {
                    if (offerType == OfferType.StandardElastic)
                    {

                        newDocumentCollection.PartitionKey.Paths.Add(tbPartitionKeyForCollectionCreate.Text);
                    }
                }

                currentOperationCallback(newDocumentCollection, requestOptions);
            }
            else if (resourceType == ResourceType.Offer && operationType == OperationType.Replace)
            {
                currentOperationCallback(newDocumentCollection, requestOptions);
            }
            else if (resourceType == ResourceType.Trigger && operationType == OperationType.Create)
            {
                var trigger = new Trigger
                {
                    Body = tbCrudContext.Text,
                    Id = textBoxforId.Text,
                    TriggerOperation = TriggerOperation.All
                };
                if (rbPreTrigger.Checked)
                    trigger.TriggerType = TriggerType.Pre;
                else if (rbPostTrigger.Checked)
                    trigger.TriggerType = TriggerType.Post;

                currentOperationCallback(trigger, requestOptions);
            }
            else if (resourceType == ResourceType.Trigger && operationType == OperationType.Replace)
            {
                var trigger = new Trigger
                {
                    Body = tbCrudContext.Text,
                    Id = textBoxforId.Text
                };
                currentOperationCallback(trigger, requestOptions);
           }

            else if (resourceType == ResourceType.UserDefinedFunction
                 && (operationType == OperationType.Create || operationType == OperationType.Replace))
            {
                var udf = new UserDefinedFunction
                {
                    Body = tbCrudContext.Text,
                    Id = textBoxforId.Text
                };
                currentOperationCallback(udf, requestOptions);
            }
            else if (resourceType == ResourceType.StoredProcedure
                 && (operationType == OperationType.Create || operationType == OperationType.Replace))
            {
                var storedProcedure = new StoredProcedure
                {
                    Body = tbCrudContext.Text,
                    Id = textBoxforId.Text
                };
                currentOperationCallback(storedProcedure, requestOptions);
            }
            else
            {
                if (!string.IsNullOrEmpty(tbCrudContext.SelectedText))
                {
                    currentOperationCallback(tbCrudContext.SelectedText, requestOptions);
                }
                else
                {
                    if (resourceType == ResourceType.StoredProcedure && operationType == OperationType.Execute && !tbCrudContext.Modified)
                    {
                        currentOperationCallback(null, requestOptions);
                    }
                    else
                    {
                        currentOperationCallback(tbCrudContext.Text, requestOptions);
                    }
                }
            }
        }

        public void SetLoadingState()
        {
            //
            webBrowserResponse.Url = new Uri(loadingGifPath);
        }

        public void RenderFile(string fileName)
        {
            //
            webBrowserResponse.Url = new Uri(fileName);
        }

        public void SetResultInBrowser(string json, string text, bool executeButtonEnabled, NameValueCollection responseHeaders = null)
        {
            currentText = text;
            currentJson = json;
            DisplayResponseContent();

            toolStripBtnExecute.Enabled = executeButtonEnabled;

            SetResponseHeaders(responseHeaders);
        }

        public void SetStatus(string status)
        {
            tsStatus.Text = status;
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
            string prettyPrintHtml = Path.Combine(appTempPath, "prettyPrint.Html");

            using (StreamWriter outfile = new StreamWriter(prettyPrintHtml))
            {
                outfile.Write(prettyPrint);
            }

            // now launch it in broswer!
            webBrowserResponse.Url = new Uri(prettyPrintHtml);
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
                        tsStatus.Text = tsStatus.Text + ", Request Charge: " + responseHeaders[key];
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
                    tsStatus.Text = tsStatus.Text + ", Count: " + itemCountValue;
                    if (continuationValue != null)
                    {
                        tsStatus.Text = tsStatus.Text + "+";
                    }
                }

                tbResponse.Text = headers;
            }
        }

        private void btnExecuteNext_Click(object sender, EventArgs e)
        {
            SetLoadingState();

            if (!string.IsNullOrEmpty(tbCrudContext.SelectedText))
            {
                currentOperationCallback(tbCrudContext.SelectedText, null);
            }
            else
            {
                currentOperationCallback(tbCrudContext.Text, null);
            }
        }

        private void cbRequestOptionsApply_CheckedChanged(object sender, EventArgs e)
        {
            CreateDefaultRequestOptions();
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
            requestOptions = new RequestOptions();

            if (rbIndexingDefault.Checked)
            {
                requestOptions.IndexingDirective = IndexingDirective.Default;
            }
            else if (rbIndexingExclude.Checked)
            {
                requestOptions.IndexingDirective = IndexingDirective.Exclude;
            }
            else if (rbIndexingInclude.Checked)
            {
                requestOptions.IndexingDirective = IndexingDirective.Include;
            }

            requestOptions.AccessCondition = new AccessCondition();
            if (rbAccessConditionIfMatch.Checked)
            {
                requestOptions.AccessCondition.Type = AccessConditionType.IfMatch;
            }
            else if (rbAccessConditionIfNoneMatch.Checked)
            {
                requestOptions.AccessCondition.Type = AccessConditionType.IfNoneMatch;
            }

            string condition = tbAccessConditionText.Text;
            if (!string.IsNullOrEmpty(condition))
            {
                requestOptions.AccessCondition.Condition = condition;
            }

            if (rbConsistencyStrong.Checked)
            {
                requestOptions.ConsistencyLevel = ConsistencyLevel.Strong;
            }
            else if (rbConsistencyBound.Checked)
            {
                requestOptions.ConsistencyLevel = ConsistencyLevel.BoundedStaleness;
            }
            else if (rbConsistencySession.Checked)
            {
                requestOptions.ConsistencyLevel = ConsistencyLevel.Session;
            }
            else if (rbConsistencyEventual.Checked)
            {
                requestOptions.ConsistencyLevel = ConsistencyLevel.Eventual;
            }

            string preTrigger = tbPreTrigger.Text;
            if (!string.IsNullOrEmpty(preTrigger))
            {
                // split by ;
                string[] segments = preTrigger.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                requestOptions.PreTriggerInclude = segments;
            }

            string postTrigger = tbPostTrigger.Text;
            if (!string.IsNullOrEmpty(postTrigger))
            {
                // split by ;
                string[] segments = postTrigger.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                requestOptions.PostTriggerInclude = segments;
            }

            string partitionKey = tbPartitionKeyForRequestOption.Text;
            if (!string.IsNullOrEmpty(partitionKey))
            {
                requestOptions.PartitionKey = new PartitionKey(partitionKey);
            }
        }

        private void rbIndexingDefault_CheckedChanged(object sender, EventArgs e)
        {
            requestOptions.IndexingDirective = IndexingDirective.Default;
        }

        private void rbIndexingInclude_CheckedChanged(object sender, EventArgs e)
        {
            requestOptions.IndexingDirective = IndexingDirective.Include;
        }

        private void rbIndexingExclude_CheckedChanged(object sender, EventArgs e)
        {
            requestOptions.IndexingDirective = IndexingDirective.Exclude;
        }

        private void rbAccessConditionIfMatch_CheckedChanged(object sender, EventArgs e)
        {
            requestOptions.AccessCondition.Type = AccessConditionType.IfMatch;
        }

        private void rbAccessConditionIfNoneMatch_CheckedChanged(object sender, EventArgs e)
        {
            requestOptions.AccessCondition.Type = AccessConditionType.IfNoneMatch;
        }

        private void rbConsistencyStrong_CheckedChanged(object sender, EventArgs e)
        {
            requestOptions.ConsistencyLevel = ConsistencyLevel.Strong;
        }

        private void rbConsistencyBound_CheckedChanged(object sender, EventArgs e)
        {
            requestOptions.ConsistencyLevel = ConsistencyLevel.BoundedStaleness;
        }

        private void rbConsistencySession_CheckedChanged(object sender, EventArgs e)
        {
            requestOptions.ConsistencyLevel = ConsistencyLevel.Session;
        }

        private void rbConsistencyEventual_CheckedChanged(object sender, EventArgs e)
        {
            requestOptions.ConsistencyLevel = ConsistencyLevel.Eventual;
        }

        private void btnAddIncludePath_Click(object sender, EventArgs e)
        {
            IncludedPathForm dlg = new IncludedPathForm();
            dlg.StartPosition = FormStartPosition.CenterParent;
            DialogResult dr = dlg.ShowDialog(this);
            if (dr == DialogResult.OK)
            {
                lbIncludedPath.Items.Add(dlg.IncludedPath);
            }
        }

        private void btnRemovePath_Click(object sender, EventArgs e)
        {
            lbIncludedPath.Items.RemoveAt(lbIncludedPath.SelectedIndex);
        }

        private void lbIncludedPath_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbIncludedPath.SelectedItem != null)
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
            IncludedPath includedPath = lbIncludedPath.SelectedItem as IncludedPath;

            IncludedPathForm dlg = new IncludedPathForm();
            dlg.StartPosition = FormStartPosition.CenterParent;

            dlg.SetIncludedPath(includedPath);

            DialogResult dr = dlg.ShowDialog(this);
            if (dr == DialogResult.OK)
            {
                lbIncludedPath.Items[lbIncludedPath.SelectedIndex] = dlg.IncludedPath;
            }
        }

        private void btnAddExcludedPath_Click(object sender, EventArgs e)
        {
            ExcludedPathForm dlg = new ExcludedPathForm();
            DialogResult dr = dlg.ShowDialog(this);
            if (dr == DialogResult.OK)
            {
                lbExcludedPath.Items.Add(dlg.ExcludedPath);
            }
        }

        private void btnRemoveExcludedPath_Click(object sender, EventArgs e)
        {
            lbExcludedPath.Items.RemoveAt(lbExcludedPath.SelectedIndex);
        }

        private void lbExcludedPath_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbExcludedPath.SelectedItem != null)
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

                newDocumentCollection = new DocumentCollection();
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

                CreateDefaultIndexingPolicy();
            }
        }


        private void CreateDefaultIndexingPolicy()
        {
            newDocumentCollection.IndexingPolicy.Automatic = cbAutomatic.Checked;

            if (rbConsistent.Checked)
            {
                newDocumentCollection.IndexingPolicy.IndexingMode = IndexingMode.Consistent;
            }
            else
            {
                newDocumentCollection.IndexingPolicy.IndexingMode = IndexingMode.Lazy;
            }
        }

        private void cbAutomatic_CheckedChanged(object sender, EventArgs e)
        {
            newDocumentCollection.IndexingPolicy.Automatic = cbAutomatic.Checked;
        }

        private void rbConsistent_CheckedChanged(object sender, EventArgs e)
        {
            if (rbConsistent.Checked)
            {
                newDocumentCollection.IndexingPolicy.IndexingMode = IndexingMode.Consistent;
            }
            else
            {
                newDocumentCollection.IndexingPolicy.IndexingMode = IndexingMode.Lazy;
            }
        }

        private void rbLazy_CheckedChanged(object sender, EventArgs e)
        {
            if (rbConsistent.Checked)
            {
                newDocumentCollection.IndexingPolicy.IndexingMode = IndexingMode.Consistent;
            }
            else
            {
                newDocumentCollection.IndexingPolicy.IndexingMode = IndexingMode.Lazy;
            }
        }

        private void rbOfferS1_CheckedChanged(object sender, EventArgs e)
        {
            if (rbOfferS1.Checked)
            {
                offerType = OfferType.S1;
                labelThroughput.Text = "Fixed 250 RU. 10GB Storage";
                tbThroughput.Enabled = false;
                tbThroughput.Text = "250";
            }
        }

        private void rbOfferS2_CheckedChanged(object sender, EventArgs e)
        {
            if (rbOfferS2.Checked)
            {
                offerType = OfferType.S2;
                labelThroughput.Text = "Fixed 1000 RU. 10GB Storage ";
                tbThroughput.Enabled = false;
                tbThroughput.Text = "1000";
             }
        }

        private void rbOfferS3_CheckedChanged(object sender, EventArgs e)
        {
            if (rbOfferS3.Checked)
            {
                offerType = OfferType.S3;
                labelThroughput.Text = "Fixed 2500 RU. 10GB Storage ";
                tbThroughput.Enabled = false;
                tbThroughput.Text = "2500";
            }
        }

        private void rbElasticCollection_CheckedChanged(object sender, EventArgs e)
        {
            if (rbElasticCollection.Checked)
            {
                offerType = OfferType.StandardElastic;
                tbPartitionKeyForCollectionCreate.Enabled = true;
                labelThroughput.Text = "Allowed values > 10k and <= 250k. 250GB Storage";
                tbThroughput.Enabled = true;
                tbThroughput.Text = "20000";
            }
            else
                tbPartitionKeyForCollectionCreate.Enabled = false;

        }

        private void cbShowLegacyOffer_CheckedChanged(object sender, EventArgs e)
        {
            rbOfferS1.Visible = cbShowLegacyOffer.Checked;
            rbOfferS2.Visible = cbShowLegacyOffer.Checked;
            rbOfferS3.Visible = cbShowLegacyOffer.Checked;

            if (!cbShowLegacyOffer.Checked)
            {
                rbSinglePartition.Checked = true;
            }
        }

        private void rbSinglePartition_CheckedChanged(object sender, EventArgs e)
        {
            if (rbSinglePartition.Checked)
            {
                offerType = OfferType.StandardSingle;
                labelThroughput.Text = "Allowed values between 400 - 10k. 10GB Storage";
                tbThroughput.Enabled = true;
                tbThroughput.Text = "400";
            }
        }

        private void feedToolStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void tsbHideDocumentSystemProperties_Click(object sender, EventArgs e)
        {
            Settings.Default.HideDocumentSystemProperties = tsbHideDocumentSystemProperties.Checked;
            Settings.Default.Save();

            SetDocumentSystemPropertiesTabText();

            if ((webBrowserResponse.Url.AbsoluteUri == "about:blank" && webBrowserResponse.DocumentTitle != "DataModelBrowserHome")
                || webBrowserResponse.Url.Scheme == "file")
            {
                DisplayResponseContent();
            }
        }

        private void SetDocumentSystemPropertiesTabText()
        {
            tsbHideDocumentSystemProperties.Text = Settings.Default.HideDocumentSystemProperties
                ? "Hide SysProperties"
                : "Show SysProperties";
        }
    }
}