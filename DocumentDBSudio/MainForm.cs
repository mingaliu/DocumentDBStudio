//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace Microsoft.Azure.DocumentDBStudio
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Reflection;
    using System.Windows.Forms;
    using Microsoft.Azure.DocumentDBStudio.Properties;
    using Microsoft.Azure.Documents.Client;
    using Newtonsoft.Json;
    using System.Text;

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
        Action<string, string> currentOperation;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Height = Screen.GetWorkingArea(this).Height * 3 / 4 ;
            this.Width = Screen.GetWorkingArea(this).Width / 2;
            this.Top = 0;

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
            this.homepage = this.homepage.Replace("&BUILDTIME&", t.ToString("f"));

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
            this.tabControl.TabPages.Remove(this.tabPageRequestOptions);

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
            this.treeView1.ImageList = imageList;

            this.InitTreeView();

            this.btnHome_Click(null, null);

            this.splitContainerIntabPage.Panel1Collapsed = true;

            this.toolStripBtnExecute.Enabled = false;
            this.btnExecuteNext.Enabled = false;
            this.UnpackEmbeddedResources();

            this.tsbViewType.Checked = true;
            this.btnHeaders.Checked = false;
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
            this.tsStatus.Text = this.webBrowserResponse.StatusText;
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
            this.tsButtonZoom.Text = this.fontScale.ToString() + "%";
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
            for (int i = 0; i< Settings.Default.AccountSettingsList.Count; i=i+2)
            {
                if (string.Compare(accountEndpoint, Properties.Settings.Default.AccountSettingsList[i], true) == 0)
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
            for (int i = 0; i< Settings.Default.AccountSettingsList.Count; i=i+2)
            {
                if (string.Compare(accountEndpoint, Properties.Settings.Default.AccountSettingsList[i], true) == 0)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                Settings.Default.AccountSettingsList.Add(accountEndpoint);
                Settings.Default.AccountSettingsList.Add( JsonConvert.SerializeObject(accountSettings) );

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
                if (string.Compare(accountEndpoint, Properties.Settings.Default.AccountSettingsList[i], true) == 0)
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

        public int GetMaxItemCountValue()
        {
            var maxItemCount = 10;
            try 
            {
                maxItemCount = Convert.ToInt32(toolStripTextMaxItemCount.Text);
            }
            catch (Exception)
            {
                // Ignore the exception and use the defualt value
            }

            return maxItemCount;
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
            }
            catch (Exception e)
            {
                Program.GetMain().SetResultInBrowser(null, e.ToString(), true);

                // delete it.
                this.RemoveAccountFromSettings(accountEndpoint);
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
                    (e.Node as FeedNode).ShowContextMenu( e.Node.TreeView, e.Location );
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

        public void SetCrudContext(string name, bool showId, string bodytext, Action<string, string> func, CommandContext commandContext = null)
        {
            if(commandContext == null)
            {
                commandContext = new CommandContext();
            }

            this.currentOperation = func;
            this.tabCrudContext.Text = name;
            this.tbCrudContext.Text = bodytext;

            this.toolStripBtnExecute.Enabled = true;
            this.tbCrudContext.ReadOnly = commandContext.IsDelete;

            this.splitContainerInner.Panel1Collapsed = false;
            this.splitContainerIntabPage.Panel1Collapsed = !showId;

            this.tbResponse.Text = "";

            this.tabControl.SelectedTab = this.tabCrudContext;
            this.ButtomSplitContainer.Panel1Collapsed = !commandContext.IsFeed;
            this.btnExecuteNext.Enabled = commandContext.HasContinuation || !commandContext.QueryStarted;
        }


        private void toolStripBtnExecute_Click(object sender, EventArgs e)
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

        public void SetLoadingState()
        {
            //
            this.webBrowserResponse.Url = new Uri(this.loadingGifPath);
        }

        public void SetResultInBrowser(string json, string text, bool executeButtonEnabled, NameValueCollection responseHeaders = null)
        {
            this.currentText = text;
            this.currentJson = json;
            this.DisplayResponseContent();

            this.toolStripBtnExecute.Enabled = executeButtonEnabled;

            this.SetResponseHeaders(responseHeaders);
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
                    headers += string.Format("{0}: {1}\r\n", key, responseHeaders[key]);
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
    }


}
