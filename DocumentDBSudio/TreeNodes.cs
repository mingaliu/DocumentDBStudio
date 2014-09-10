//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace Microsoft.Azure.DocumentDBStudio
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Microsoft.Azure.DocumentDBStudio.Properties;
    using Microsoft.Azure.Documents.Client;
    using Microsoft.Azure.Documents.Linq;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using System.Text;

    internal class AccountSettings
    {
        public string MasterKey;
        public ConnectionMode ConnectionMode;
        public Protocol Protocol;
    }

    enum ResourceType
    {
        Document,
        User,
        StoredProcedure,
        UserDefinedFunction,
        Trigger,
        Permission,
        Attachment,
        Conflict
    }

    abstract class FeedNode : TreeNode
    {
        protected bool isFirstTime = true;
        abstract public void ShowContextMenu(TreeView treeview, Point p);

        abstract public void Refresh(bool forceRefresh);
    }

    class TreeNodeConstants 
    {
        static public string LoadingNode = "Loading";
    }

    class DatabaseAccountNode : FeedNode
    {
        private DocumentClient client;
        private string accountEndpoint;
        private TreeNode loadindNode;
        private ContextMenu contextMenu = new ContextMenu();

        public DatabaseAccountNode(string endpointName, DocumentClient client)
        {
            this.accountEndpoint = endpointName;

            this.Text = endpointName;
            if (string.Compare(endpointName, Constants.LocalEmulatorEndpoint, true) == 0)
            {
                this.Text = "devEmulator";
            }
            this.ImageKey = "DatabaseAccount";
            this.SelectedImageKey = "DatabaseAccount";

            this.client = client;
            this.Tag = "This represents the DatabaseAccount. Right click to add Database";
            this.loadindNode = new TreeNode(TreeNodeConstants.LoadingNode);

            this.Nodes.Add(this.loadindNode);

            MenuItem myMenuItem = new MenuItem("Add Database");
            myMenuItem.Click += new EventHandler(myMenuItemAddDatabase_Click);
            this.contextMenu.MenuItems.Add(myMenuItem);

            MenuItem myMenuItem1 = new MenuItem("Refresh Databases feed");
            myMenuItem1.Click += new EventHandler((sender, e) => Refresh(true));
            this.contextMenu.MenuItems.Add(myMenuItem1);

            this.contextMenu.MenuItems.Add("-");

            MenuItem myMenuItem2 = new MenuItem("Remove setting");
            myMenuItem2.Click += new EventHandler(myMenuItemRemoveDatabaseAccount_Click);
            this.contextMenu.MenuItems.Add(myMenuItem2);
            MenuItem myMenuItem3 = new MenuItem("Change setting");
            myMenuItem3.Click += new EventHandler(myMenuItemChangeSetting_Click);
            this.contextMenu.MenuItems.Add(myMenuItem3);
        }
        public DocumentClient DocumentClient
        {
            get { return this.client; }
        }

        void myMenuItemChangeSetting_Click(object sender, EventArgs e)
        {
            Program.GetMain().ChangeAccountSettings(this, this.accountEndpoint);
        }

        void myMenuItemAddDatabase_Click(object sender, EventArgs e)
        {
            // 
            dynamic d = new System.Dynamic.ExpandoObject();
            d.id = "Here is your Database Id";
            string x = JsonConvert.SerializeObject(d, Newtonsoft.Json.Formatting.Indented);
            Program.GetMain().SetCrudContext("Add database", false, x, this.AddDatabase);
        }

        void myMenuItemRemoveDatabaseAccount_Click(object sender, EventArgs e)
        {
            // 
            this.Remove();
            Program.GetMain().RemoveAccountFromSettings(this.accountEndpoint);
        }

        private async void FillWithChildren()
        {
            try
            {
                this.Tag = await client.GetDatabaseAccountAsync();

                FeedResponse<Documents.Database> databases = await this.client.ReadDatabaseFeedAsync();

                foreach (Documents.Database db in databases)
                {
                    DatabaseNode node = new DatabaseNode(client, db);
                    this.Nodes.Add(node);
                }

                Program.GetMain().SetResponseHeaders(databases.ResponseHeaders);
            }
            catch (AggregateException e)
            {
                Program.GetMain().SetResultInBrowser(null, e.InnerException.ToString(), true);
            }
            catch (Exception e)
            {
                Program.GetMain().SetResultInBrowser(null, e.ToString(), true);
            }
            finally
            {
                this.loadindNode.Remove();
            }

        }

        override public void ShowContextMenu(TreeView treeview, Point p)
        {
            this.contextMenu.Show(treeview, p);
        }

        override public void Refresh(bool forceRefresh)
        {
            if (forceRefresh || this.isFirstTime)
            {
                this.isFirstTime = false;
                this.Nodes.Clear();
                this.Nodes.Add(this.loadindNode);

                FillWithChildren();
            }
        }

        async void AddDatabase(string text, string optional)
        {
            try
            {
                Documents.Database db = (Documents.Database)JsonConvert.DeserializeObject(text, typeof(Documents.Database));

                ResourceResponse<Documents.Database> newdb = await this.client.CreateDatabaseAsync(db);

                this.Nodes.Add(new DatabaseNode(this.client, newdb.Resource));

                // set the result window
                string json = JsonConvert.SerializeObject(newdb.Resource, Newtonsoft.Json.Formatting.Indented);

                Program.GetMain().SetResultInBrowser(json, null, false, newdb.ResponseHeaders);
            }
            catch (AggregateException e)
            {
                Program.GetMain().SetResultInBrowser(null, e.InnerException.ToString(), true);
            }
            catch (Exception e)
            {
                Program.GetMain().SetResultInBrowser(null, e.ToString(), true);
            }
        }
    }

    class DatabaseNode : FeedNode
    {
        private DocumentClient client;
        private ContextMenu contextMenu = new ContextMenu();

        public DatabaseNode(DocumentClient localclient, Documents.Database db)
        {
            this.Text = db.Id;
            this.Tag = db;
            this.client = localclient;
            this.ImageKey = "SystemFeed";
            this.SelectedImageKey = "SystemFeed";

            this.Nodes.Add(new UsersNode(this.client));

            MenuItem myMenuItem1 = new MenuItem("Update Database");
            myMenuItem1.Click += new EventHandler(myMenuItemUpdateDatabase_Click);
            this.contextMenu.MenuItems.Add(myMenuItem1);
            MenuItem myMenuItem = new MenuItem("Delete Database");
            myMenuItem.Click += new EventHandler(myMenuItemDeleteDatabase_Click);
            this.contextMenu.MenuItems.Add(myMenuItem);

            this.contextMenu.MenuItems.Add("-");
            
            MenuItem myMenuItem2 = new MenuItem("Add DocumentCollection");
            myMenuItem2.Click += new EventHandler(myMenuItemAddDocumentCollection_Click);
            this.contextMenu.MenuItems.Add(myMenuItem2);
            MenuItem myMenuItem4 = new MenuItem("Refresh DocumentCollections");
            myMenuItem4.Click += new EventHandler((sender, e) => Refresh(true));
            this.contextMenu.MenuItems.Add(myMenuItem4);

        }
        public DocumentClient DocumentClient
        {
            get { return this.client; }
        }

        void myMenuItemDeleteDatabase_Click(object sender, EventArgs e)
        {
            string x = this.Tag.ToString();
            Program.GetMain().SetCrudContext("Delete database", false, x, this.DeleteDatabase, true);
        }

        void myMenuItemUpdateDatabase_Click(object sender, EventArgs e)
        {
            string x = this.Tag.ToString();
            Program.GetMain().SetCrudContext("Update database", false, x, this.UpdateDatabase);
        }

        void myMenuItemAddDocumentCollection_Click(object sender, EventArgs e)
        {
            dynamic d = new System.Dynamic.ExpandoObject();
            d.id = "Here is your DocumentCollection Id";

            string x = JsonConvert.SerializeObject(d, Newtonsoft.Json.Formatting.Indented);
            Program.GetMain().SetCrudContext("Add documentCollection", false, x, this.AddDocumentCollection);
        }

        override public void ShowContextMenu(TreeView treeview, Point p)
        {
            this.contextMenu.Show(treeview, p);
        }

        override public void Refresh(bool forceRefresh)
        {
            if (forceRefresh || this.isFirstTime)
            {
                this.isFirstTime = false;
                this.Nodes.Clear();

                this.Nodes.Add(new UsersNode(this.client));

                FillWithChildren();
            }
        }

        async void AddDocumentCollection(string text, string optional)
        {
            try
            {
                Documents.DocumentCollection coll = (Documents.DocumentCollection)JsonConvert.DeserializeObject(text, typeof(Documents.DocumentCollection));
                Documents.Database db = (Documents.Database)this.Tag;

                ResourceResponse<Documents.DocumentCollection> newcoll = await this.client.CreateDocumentCollectionAsync(db.SelfLink, coll);

                // set the result window
                string json = JsonConvert.SerializeObject(newcoll.Resource, Newtonsoft.Json.Formatting.Indented);

                Program.GetMain().SetResultInBrowser(json, null, false, newcoll.ResponseHeaders);

                this.Nodes.Add(new DocumentCollectionNode(this.client, newcoll.Resource));
            }
            catch (AggregateException e)
            {
                Program.GetMain().SetResultInBrowser(null, e.InnerException.ToString(), true);
            }
            catch (Exception e)
            {
                Program.GetMain().SetResultInBrowser(null, e.ToString(), true);
            }
        }

        async void UpdateDatabase(string text, string optional)
        {
            try
            {
                Documents.Database db = (Documents.Database)JsonConvert.DeserializeObject(text, typeof(Documents.Database));
                ResourceResponse<Documents.Database> newdb = await this.client.ReplaceDatabaseAsync(db);

                // set the result window
                string json = JsonConvert.SerializeObject(newdb.Resource, Newtonsoft.Json.Formatting.Indented);

                Program.GetMain().SetResultInBrowser(json, null, false, newdb.ResponseHeaders);

            }
            catch (AggregateException e)
            {
                Program.GetMain().SetResultInBrowser(null, e.InnerException.ToString(), true);
            }
            catch (Exception e)
            {
                Program.GetMain().SetResultInBrowser(null, e.ToString(), true);
            }
        }

        async void DeleteDatabase(string text, string optional)
        {
            try
            {
                Documents.Database db = (Documents.Database)this.Tag;
                ResourceResponse<Documents.Database> newdb = await this.client.DeleteDatabaseAsync(db.SelfLink);

                Program.GetMain().SetResultInBrowser(null, "Delete database succeed!", false, newdb.ResponseHeaders);

                this.Remove();
            }
            catch (AggregateException e)
            {
                Program.GetMain().SetResultInBrowser(null, e.InnerException.ToString(), true);
            }
            catch (Exception e)
            {
                Program.GetMain().SetResultInBrowser(null, e.ToString(), true);
            }
        }

        async public void FillWithChildren()
        {
            try
            {
                FeedResponse<Documents.DocumentCollection> colls = await this.client.ReadDocumentCollectionFeedAsync(((Documents.Database)this.Tag).SelfLink);

                foreach (Documents.DocumentCollection coll in colls)
                {
                    DocumentCollectionNode node = new DocumentCollectionNode(client, coll);
                    this.Nodes.Add(node);
                }

                Program.GetMain().SetResponseHeaders(colls.ResponseHeaders);
            }
            catch (AggregateException e)
            {
                Program.GetMain().SetResultInBrowser(null, e.InnerException.ToString(), true);
            }
            catch (Exception e)
            {
                Program.GetMain().SetResultInBrowser(null, e.ToString(), true);
            }
        }
    }

    class DocumentCollectionNode : FeedNode
    {
        private DocumentClient client;
        private ContextMenu contextMenu = new ContextMenu();

        public DocumentCollectionNode(DocumentClient client, Documents.DocumentCollection coll)
        {
            this.Text = coll.Id;
            this.Tag = coll;
            this.client = client;
            this.ImageKey = "SystemFeed";
            this.SelectedImageKey = "SystemFeed";

            this.Nodes.Add(new StoredProceduresNode(this.client));
            this.Nodes.Add(new UDFsNode(this.client));
            this.Nodes.Add(new TriggersNode(this.client));
            this.Nodes.Add(new ConflictsNode(this.client));

            MenuItem myMenuItem6 = new MenuItem("Delete DocumentCollection");
            myMenuItem6.Click += new EventHandler(myMenuItemDeleteDocumentCollection_Click);
            this.contextMenu.MenuItems.Add(myMenuItem6);

            this.contextMenu.MenuItems.Add("-");

            MenuItem myMenuItem = new MenuItem("Add Document");
            myMenuItem.Click += new EventHandler(myMenuItemAddDocument_Click);
            this.contextMenu.MenuItems.Add(myMenuItem);
            MenuItem myMenuItem9 = new MenuItem("Add Document From File");
            myMenuItem9.Click += new EventHandler(myMenuItemAddDocumentFromFile_Click);
            this.contextMenu.MenuItems.Add(myMenuItem9);
            MenuItem myMenuItem1 = new MenuItem("Refresh Documents feed");
            myMenuItem1.Click += new EventHandler((sender, e) => Refresh(true));
            this.contextMenu.MenuItems.Add(myMenuItem1);
            MenuItem myMenuItem2 = new MenuItem("Query Documents");
            myMenuItem2.Click += new EventHandler(myMenuItemQueryDocument_Click);
            this.contextMenu.MenuItems.Add(myMenuItem2);
        }

        void myMenuItemDeleteDocumentCollection_Click(object sender, EventArgs e)
        {
            string x = this.Tag.ToString();
            Program.GetMain().SetCrudContext("Delete DocumentCollection", false, x, this.DeleteDocumentCollection, true);
        }

        async void DeleteDocumentCollection(string text, string optional)
        {
            try
            {
                Documents.DocumentCollection coll = (Documents.DocumentCollection)JsonConvert.DeserializeObject(text, typeof(Documents.DocumentCollection));
                ResourceResponse<Documents.DocumentCollection> newcoll = await this.client.DeleteDocumentCollectionAsync(coll.SelfLink);

                Program.GetMain().SetResultInBrowser(null, "Delete DocumentCollection succeed!", false, newcoll.ResponseHeaders);

                this.Remove();
            }
            catch (AggregateException e)
            {
                Program.GetMain().SetResultInBrowser(null, e.InnerException.ToString(), true);
            }
            catch (Exception e)
            {
                Program.GetMain().SetResultInBrowser(null, e.ToString(), true);
            }
        }

        void myMenuItemAddDocumentFromFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            DialogResult dr = ofd.ShowDialog();

            if (dr == DialogResult.OK)
            {
                string filename = ofd.FileName;
                // 
                string text = System.IO.File.ReadAllText(filename);

                Program.GetMain().SetCrudContext("Add document", false, text, this.AddDocument);
            }
        }

        void myMenuItemAddDocument_Click(object sender, EventArgs e)
        {
            // 
            dynamic d = new System.Dynamic.ExpandoObject();
            d.id = "Here is your Document Id";
            string x = JsonConvert.SerializeObject(d, Newtonsoft.Json.Formatting.Indented);
            Program.GetMain().SetCrudContext("Add document", false, x, this.AddDocument);
        }


        void myMenuItemQueryDocument_Click(object sender, EventArgs e)
        {
            // 
            Program.GetMain().SetCrudContext(string.Format("Query Documents from Collection {0}", (this.Tag as Documents.DocumentCollection).Id),
                false, "select * from c", this.QueryDocuments);
        }


        async void AddDocument(string text, string optional)
        {
            try
            {
                object document = JsonConvert.DeserializeObject(text);

                ResourceResponse<Documents.Document> newdocument = await this.client.CreateDocumentAsync((this.Tag as Documents.DocumentCollection).SelfLink, document);

                this.Nodes.Add(new DocumentNode(this.client, newdocument.Resource, ResourceType.Document));

                // set the result window
                string json = newdocument.Resource.ToString();

                Program.GetMain().SetResultInBrowser(json, null, false, newdocument.ResponseHeaders);

            }
            catch (AggregateException e)
            {
                Program.GetMain().SetResultInBrowser(null, e.InnerException.ToString(), true);
            }
            catch (Exception e)
            {
                Program.GetMain().SetResultInBrowser(null, e.ToString(), true);
            }
        }

        async void QueryDocuments(string queryText, string optional)
        {
            try
            {
                // text is the querytext.
                IDocumentQuery<dynamic> q = this.client.CreateDocumentQuery((this.Tag as Documents.DocumentCollection).SelfLink,
                    queryText, null).AsDocumentQuery();
                FeedResponse<dynamic> r = await q.ExecuteNextAsync();

                // set the result window
                string text = null;
                if (r.Count > 1)
                {
                    text = string.Format("Returned {0} documents", r.Count);
                }
                else
                {
                    text = string.Format("Returned {0} document", r.Count);
                }

                string jsonarray = "[";
                int index = 0;
                foreach (dynamic d in r)
                {
                    index++;
                    jsonarray += d.ToString();
                    if (index == r.Count)
                    {
                        jsonarray += "]";
                    }
                    else
                    {
                        jsonarray += ",";
                    }
                }

                Program.GetMain().SetResultInBrowser(jsonarray, text, true, r.ResponseHeaders);

            }
            catch (AggregateException e)
            {
                Program.GetMain().SetResultInBrowser(null, e.InnerException.ToString(), true);
            }
            catch (Exception e)
            {
                Program.GetMain().SetResultInBrowser(null, e.ToString(), true);
            }
        }

        override public void ShowContextMenu(TreeView treeview, Point p)
        {
            this.contextMenu.Show(treeview, p);
        }

        override public void Refresh(bool forceRefresh)
        {
            if (forceRefresh || this.isFirstTime)
            {
                isFirstTime = false;
                this.Nodes.Clear();
                this.Nodes.Add(new StoredProceduresNode(this.client));
                this.Nodes.Add(new UDFsNode(this.client));
                this.Nodes.Add(new TriggersNode(this.client));
                this.Nodes.Add(new ConflictsNode(this.client));

                FillWithChildren();
            }
        }

        public void FillWithChildren()
        {
            try
            {
                FeedResponse<dynamic> docs = this.client.ReadDocumentFeedAsync(((Documents.DocumentCollection)this.Tag).SelfLink).Result;

                foreach (var doc in docs)
                {
                    DocumentNode node = new DocumentNode(client, doc, ResourceType.Document);
                    this.Nodes.Add(node);
                }
                Program.GetMain().SetResponseHeaders(docs.ResponseHeaders);

            }
            catch (AggregateException e)
            {
                Program.GetMain().SetResultInBrowser(null, e.InnerException.ToString(), true);
            }
            catch (Exception e)
            {
                Program.GetMain().SetResultInBrowser(null, e.ToString(), true);
            }
        }
    }


    class DocumentNode : FeedNode
    {
        private DocumentClient client;
        private ContextMenu contextMenu = new ContextMenu();
        private ResourceType resourceType = 0;

        public DocumentNode(DocumentClient client, dynamic document, ResourceType resoureType)
        {
            this.resourceType = resoureType;
            this.Text = (document as Documents.Resource).Id;
            this.Tag = document;
            this.client = client;

            if (this.resourceType != ResourceType.Conflict)
            {
                MenuItem myMenuItem1 = new MenuItem("Update " + this.resourceType.ToString());
                myMenuItem1.Click += new EventHandler(myMenuItemUpdate_Click);
                this.contextMenu.MenuItems.Add(myMenuItem1);
            } 

            MenuItem myMenuItem = new MenuItem("Delete " + this.resourceType.ToString());
            myMenuItem.Click += new EventHandler(myMenuItemDelete_Click);
            this.contextMenu.MenuItems.Add(myMenuItem);

            if (this.resourceType == ResourceType.Permission) 
            {
                this.ImageKey = "Permission";
                this.SelectedImageKey = "Permission";
            }
            else if (this.resourceType == ResourceType.Attachment)
            {
                this.ImageKey = "Attachment";
                this.SelectedImageKey = "Attachment";
            }
            else if (this.resourceType == ResourceType.StoredProcedure || this.resourceType == ResourceType.Trigger || this.resourceType == ResourceType.UserDefinedFunction) 
            {
                this.ImageKey = "Javascript";
                this.SelectedImageKey = "Javascript";
                if (this.resourceType == ResourceType.StoredProcedure) 
                {
                    MenuItem myMenuItem2 = new MenuItem("Execute " + this.resourceType.ToString());
                    myMenuItem2.Click += new EventHandler(myMenuItemExecuteSP_Click);
                    this.contextMenu.MenuItems.Add(myMenuItem2);
                }
            }
            else if (this.resourceType == ResourceType.User)
            {
                this.ImageKey = "User";
                this.SelectedImageKey = "User";

                this.Nodes.Add(new PermissionsNode(this.client));
            }
            else if (this.resourceType == ResourceType.Document)
            {
                this.Nodes.Add(new TreeNode("Fake"));

                this.contextMenu.MenuItems.Add("-");

                MenuItem myMenuItem3 = new MenuItem("Add attachment");
                myMenuItem3.Click += new EventHandler(myMenuItemAttachment_Click);
                this.contextMenu.MenuItems.Add(myMenuItem3);

                MenuItem myMenuItem4 = new MenuItem("Add attachment from file");
                myMenuItem4.Click += new EventHandler(myMenuItemAttachmentFromFile_Click);
                this.contextMenu.MenuItems.Add(myMenuItem4);
            }
            else if (this.resourceType == ResourceType.Conflict)
            {
                this.ImageKey = "Conflict";
                this.SelectedImageKey = "Conflict";
            }

        }

        void myMenuItemUpdate_Click(object sender, EventArgs e)
        {
            if (this.resourceType == ResourceType.StoredProcedure)
            {
                Program.GetMain().SetCrudContext("Update " + this.resourceType.ToString(), true, (this.Tag as Documents.StoredProcedure).Body, this.UpdateNode);
            }
            else if (this.resourceType == ResourceType.Trigger)
            {
                Program.GetMain().SetCrudContext("Update " + this.resourceType.ToString(), true, (this.Tag as Documents.Trigger).Body, this.UpdateNode);
            }
            else if (this.resourceType == ResourceType.UserDefinedFunction)
            {
                Program.GetMain().SetCrudContext("Update " + this.resourceType.ToString(), true, (this.Tag as Documents.UserDefinedFunction).Body, this.UpdateNode);
            }
            else
            {
                string x = this.Tag.ToString();
                Program.GetMain().SetCrudContext("Update " + this.resourceType.ToString(), false, x, this.UpdateNode);
            }
        }

        void myMenuItemAttachment_Click(object sender, EventArgs e)
        {
            Documents.Attachment attachment = new Documents.Attachment();
            attachment.Id = "Here is your attachment Id";
            attachment.ContentType = "application-content-type";
            attachment.MediaLink = "internal link or Azure blob or Amazon S3 link";

            string x = attachment.ToString();
            Program.GetMain().SetCrudContext("Add attachment for this document " + this.resourceType.ToString(), false, x, this.AddAttachment);
        }

        async void myMenuItemAttachmentFromFile_Click(object sender, EventArgs eventArg)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            DialogResult dr = ofd.ShowDialog();

            if (dr == DialogResult.OK)
            {
                string filename = ofd.FileName;
                // 
                // todo: present the dialog for Slut name and Content type
                // 
                Program.GetMain().SetLoadingState();

                try
                {

                    using (FileStream stream = new FileStream(filename,
                        FileMode.Open, FileAccess.Read))
                    {
                        MediaOptions options = new MediaOptions()
                        {
                            ContentType = "application/octet-stream",
                            Slug = ofd.FileName
                        };

                        ResourceResponse<Documents.Attachment> rr = await this.client.CreateAttachmentAsync((this.Tag as Documents.Document).AttachmentsLink,
                            stream);
                        string json = rr.Resource.ToString();

                        Program.GetMain().SetResultInBrowser(json, null, false, rr.ResponseHeaders);

                        this.Nodes.Add(new DocumentNode(this.client, rr.Resource, ResourceType.Attachment));
                    }
                }
                catch (Exception e)
                {
                    Program.GetMain().SetResultInBrowser(null, e.ToString(), true);
                }
            }
        }

        void myMenuItemExecuteSP_Click(object sender, EventArgs e)
        {
            Program.GetMain().SetCrudContext("Execute " + this.resourceType.ToString() + " " + 
                (this.Tag as Documents.Resource).Id, false, 
                "Here is the input parameters to the storedProcedure. Input each parameter as one line without quotation mark.", this.ExecuteStoredProcedure);
        }

        void myMenuItemDelete_Click(object sender, EventArgs e)
        {
            string x = this.Tag.ToString();
            Program.GetMain().SetCrudContext("Delete " + this.resourceType.ToString(), false, x, this.DeleteNode, true);
        }

        async void AddAttachment(string text, string optional)
        {
            try
            {
                Documents.Attachment attachment = (Documents.Attachment)JsonConvert.DeserializeObject(text, typeof(Documents.Attachment));

                ResourceResponse<Documents.Attachment> rr = await this.client.CreateAttachmentAsync((this.Tag as Documents.Resource).SelfLink,
                    attachment);

                string json = rr.Resource.ToString();

                Program.GetMain().SetResultInBrowser(json, null, false, rr.ResponseHeaders);

                this.Nodes.Add(new DocumentNode(this.client, rr.Resource, ResourceType.Attachment));
            }
            catch (AggregateException e)
            {
                Program.GetMain().SetResultInBrowser(null, e.InnerException.ToString(), true);
            }
            catch (Exception e)
            {
                Program.GetMain().SetResultInBrowser(null, e.ToString(), true);
            }
        }

        async void ExecuteStoredProcedure(string text, string optional)
        {
            try
            {
                List<string> inputParamters = new List<string>();
                if (!string.IsNullOrEmpty(text))
                {
                    using (StringReader sr = new StringReader(text))
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (!string.IsNullOrEmpty(line))
                            {
                                inputParamters.Add(line);
                            }
                        }//while
                    }//usi
                }
                StoredProcedureResponse<string> rr = await this.client.ExecuteStoredProcedureAsync<string>((this.Tag as Documents.Resource).SelfLink,
                    inputParamters.ToArray());
                string json = rr.Response;

                Program.GetMain().SetResultInBrowser(json, null, true, rr.ResponseHeaders);

            }
            catch (AggregateException e)
            {
                Program.GetMain().SetResultInBrowser(null, e.InnerException.ToString(), true);
            }
            catch (Exception e)
            {
                Program.GetMain().SetResultInBrowser(null, e.ToString(), true);
            }
        }

        async void UpdateNode(string text, string optional)
        {
            try
            {
                string json = null;
                if (this.resourceType == ResourceType.Document)
                {
                    Documents.Document doc = (Documents.Document)JsonConvert.DeserializeObject(text, typeof(Documents.Document));
                    ResourceResponse<Documents.Document> rr = await this.client.ReplaceDocumentAsync (doc.SelfLink, doc);
                    json = rr.Resource.ToString();

                    this.Tag = rr.Resource;
                    this.Text = rr.Resource.Id;
                    // set the result window
                    Program.GetMain().SetResultInBrowser(json, null, false, rr.ResponseHeaders);
                }
                else if (this.resourceType == ResourceType.StoredProcedure)
                {
                    Documents.StoredProcedure sp = this.Tag as Documents.StoredProcedure;
                    sp.Body = text;
                    if (!string.IsNullOrEmpty(optional)) { sp.Id = optional; }
                    ResourceResponse<Documents.StoredProcedure> rr = await this.client.ReplaceStoredProcedureAsync(sp);
                    json = rr.Resource.ToString();
                    this.Tag = rr.Resource;
                    this.Text = rr.Resource.Id;
                    // set the result window
                    Program.GetMain().SetResultInBrowser(json, null, false, rr.ResponseHeaders);
                }
                else if (this.resourceType == ResourceType.User)
                {
                    Documents.User sp = (Documents.User)JsonConvert.DeserializeObject(text, typeof(Documents.User));
                    ResourceResponse<Documents.User> rr = await this.client.ReplaceUserAsync(sp);
                    json = rr.Resource.ToString();
                    this.Tag = rr.Resource;
                    this.Text = rr.Resource.Id;
                    // set the result window
                    Program.GetMain().SetResultInBrowser(json, null, false, rr.ResponseHeaders);
                }
                else if (this.resourceType == ResourceType.Trigger)
                {
                    Documents.Trigger sp = this.Tag as Documents.Trigger;
                    sp.Body = text;
                    if (!string.IsNullOrEmpty(optional)) { sp.Id = optional; }
                    ResourceResponse<Documents.Trigger> rr = await this.client.ReplaceTriggerAsync(sp);
                    json = rr.Resource.ToString();
                    this.Tag = rr.Resource;
                    this.Text = rr.Resource.Id;
                    // set the result window
                    Program.GetMain().SetResultInBrowser(json, null, false, rr.ResponseHeaders);
                }
                else if (this.resourceType == ResourceType.UserDefinedFunction)
                {
                    Documents.UserDefinedFunction sp = this.Tag as Documents.UserDefinedFunction;
                    sp.Body = text;
                    if (!string.IsNullOrEmpty(optional)) { sp.Id = optional; }
                    ResourceResponse<Documents.UserDefinedFunction> rr = await this.client.ReplaceUserDefinedFunctionAsync(sp);
                    json = rr.Resource.ToString();
                    this.Tag = rr.Resource;
                    this.Text = rr.Resource.Id;
                    // set the result window
                    Program.GetMain().SetResultInBrowser(json, null, false, rr.ResponseHeaders);
                }
                else if (this.resourceType == ResourceType.Permission)
                {
                    Documents.Permission sp = Documents.Resource.LoadFrom<Documents.Permission>(new MemoryStream(Encoding.UTF8.GetBytes(text)));
                    ResourceResponse<Documents.Permission> rr = await this.client.ReplacePermissionAsync(sp);
                    json = rr.Resource.ToString();
                    this.Tag = rr.Resource;
                    this.Text = rr.Resource.Id;
                    // set the result window
                    Program.GetMain().SetResultInBrowser(json, null, false, rr.ResponseHeaders);
                }
                else if (this.resourceType == ResourceType.Attachment)
                {
                    Documents.Attachment sp = (Documents.Attachment)JsonConvert.DeserializeObject(text, typeof(Documents.Attachment));
                    ResourceResponse<Documents.Attachment> rr = await this.client.ReplaceAttachmentAsync(sp);
                    json = rr.Resource.ToString();
                    this.Tag = rr.Resource;
                    this.Text = rr.Resource.Id;
                    // set the result window
                    Program.GetMain().SetResultInBrowser(json, null, false, rr.ResponseHeaders);
                }
            }
            catch (AggregateException e)
            {
                Program.GetMain().SetResultInBrowser(null, e.InnerException.ToString(), true);
            }
            catch (Exception e)
            {
                Program.GetMain().SetResultInBrowser(null, e.ToString(), true);
            }
        }

        async void DeleteNode(string text, string optional)
        {
            try
            {
                if (this.resourceType == ResourceType.Document)
                {
                    Documents.Document doc = (Documents.Document)JsonConvert.DeserializeObject(text, typeof(Documents.Document));
                    ResourceResponse<Documents.Document> rr = await this.client.DeleteDocumentAsync(doc.SelfLink);
                    Program.GetMain().SetResultInBrowser(null, "Delete Document succeed!", false, rr.ResponseHeaders);

                }
                else if (this.resourceType == ResourceType.StoredProcedure)
                {
                    Documents.StoredProcedure sp = (Documents.StoredProcedure)JsonConvert.DeserializeObject(text, typeof(Documents.StoredProcedure));
                    ResourceResponse<Documents.StoredProcedure> rr = await this.client.DeleteStoredProcedureAsync(sp.SelfLink);
                    Program.GetMain().SetResultInBrowser(null, "Delete StoredProcedure succeed!", false, rr.ResponseHeaders);
                }
                else if (this.resourceType == ResourceType.User)
                {
                    Documents.User sp = (Documents.User)JsonConvert.DeserializeObject(text, typeof(Documents.User));
                    ResourceResponse<Documents.User> rr = await this.client.DeleteUserAsync(sp.SelfLink);
                    Program.GetMain().SetResultInBrowser(null, "Delete User succeed!", false, rr.ResponseHeaders);
                }
                else if (this.resourceType == ResourceType.Trigger)
                {
                    Documents.Trigger sp = (Documents.Trigger)JsonConvert.DeserializeObject(text, typeof(Documents.Trigger));
                    ResourceResponse<Documents.Trigger> rr = await this.client.DeleteTriggerAsync(sp.SelfLink);
                    Program.GetMain().SetResultInBrowser(null, "Delete Trigger succeed!", false, rr.ResponseHeaders);
                }
                else if (this.resourceType == ResourceType.UserDefinedFunction)
                {
                    Documents.UserDefinedFunction sp = (Documents.UserDefinedFunction)JsonConvert.DeserializeObject(text, typeof(Documents.UserDefinedFunction));
                    ResourceResponse<Documents.UserDefinedFunction> rr = await this.client.DeleteUserDefinedFunctionAsync(sp.SelfLink);
                    Program.GetMain().SetResultInBrowser(null, "Delete UserDefinedFunction succeed!", false, rr.ResponseHeaders);
                }
                else if (this.resourceType == ResourceType.Permission)
                {
                    Documents.Permission sp = (Documents.Permission)JsonConvert.DeserializeObject(text, typeof(Documents.Permission));
                    ResourceResponse<Documents.Permission> rr = await this.client.DeletePermissionAsync(sp.SelfLink);
                    Program.GetMain().SetResultInBrowser(null, "Delete Permission succeed!", false, rr.ResponseHeaders);
                }
                else if (this.resourceType == ResourceType.Attachment)
                {
                    Documents.Attachment sp = (Documents.Attachment)JsonConvert.DeserializeObject(text, typeof(Documents.Attachment));
                    ResourceResponse<Documents.Attachment> rr = await this.client.DeleteAttachmentAsync(sp.SelfLink);
                    Program.GetMain().SetResultInBrowser(null, "Delete Attachment succeed!", false, rr.ResponseHeaders);
                }
                else if (this.resourceType == ResourceType.Conflict)
                {
                    Documents.Conflict sp = (Documents.Conflict)JsonConvert.DeserializeObject(text, typeof(Documents.Conflict));
                    ResourceResponse<Documents.Conflict> rr = await this.client.DeleteConflictAsync(sp.SelfLink);
                    Program.GetMain().SetResultInBrowser(null, "Delete Conflict succeed!", false, rr.ResponseHeaders);
                }
                // Remove the node.
                this.Remove();

            }
            catch (AggregateException e)
            {
                Program.GetMain().SetResultInBrowser(null, e.InnerException.ToString(), true);
            }
            catch (Exception e)
            {
                Program.GetMain().SetResultInBrowser(null, e.ToString(), true);
            }
        }

        override public void ShowContextMenu(TreeView treeview, Point p)
        {
            this.contextMenu.Show(treeview, p);
        }

        override public void Refresh(bool forceRefresh)
        {
            if (forceRefresh || this.isFirstTime)
            {
                this.isFirstTime = false;
                this.Nodes.Clear();

                if (this.resourceType == ResourceType.User)
                {
                    this.Nodes.Add(new PermissionsNode(this.client));
                }
                else if (this.resourceType == ResourceType.Document)
                {
                    FillWithChildren();
                }

            }
        }

        public string GetBody()
        {
            string body = null;
            if (this.resourceType == ResourceType.StoredProcedure)
            {
                body = "\nThe storedprocedure Javascript function: \n\n" + (this.Tag as Documents.StoredProcedure).Body;
            }
            else if (this.resourceType == ResourceType.Trigger)
            {
                body = "\nThe trigger Javascript function: \n\n" + (this.Tag as Documents.Trigger).Body;
            }
            else if (this.resourceType == ResourceType.UserDefinedFunction)
            {
                body = "\nThe stored Javascript function: \n\n" + (this.Tag as Documents.UserDefinedFunction).Body;
            }
            return body;
        }

        public void FillWithChildren()
        {
            try
            {
                FeedResponse<Documents.Attachment> attachments = this.client.ReadAttachmentFeedAsync ((this.Tag as Documents.Document).AttachmentsLink).Result;

                foreach (var attachment in attachments)
                {
                    DocumentNode node = new DocumentNode(client, attachment, ResourceType.Attachment);
                    this.Nodes.Add(node);
                }
                Program.GetMain().SetResponseHeaders(attachments.ResponseHeaders);

            }
            catch (AggregateException e)
            {
                Program.GetMain().SetResultInBrowser(null, e.InnerException.ToString(), true);
            }
            catch (Exception e)
            {
                Program.GetMain().SetResultInBrowser(null, e.ToString(), true);
            }
        }
    }

    class StoredProceduresNode : FeedNode
    {
        private DocumentClient client;
        private ContextMenu contextMenu = new ContextMenu();

        public StoredProceduresNode(DocumentClient client)
        {
            this.Text = "StoredProcedures";
            this.client = client;
            this.Nodes.Add(new TreeNode("fake"));
            this.Tag = "This represents the StoredProcedure feed. Right click to add StoredProcedure";

            this.ImageKey = "Feed";
            this.SelectedImageKey = "Feed";

            MenuItem myMenuItem = new MenuItem("Add StoredProcedure");
            myMenuItem.Click += new EventHandler(myMenuItemAddStoredProcedure_Click);
            this.contextMenu.MenuItems.Add(myMenuItem);
            MenuItem myMenuItem2 = new MenuItem("Add StoredProcedure From File");
            myMenuItem2.Click += new EventHandler(myMenuItemAddStoredProcedureFromFile_Click);
            this.contextMenu.MenuItems.Add(myMenuItem2);

            MenuItem myMenuItem1 = new MenuItem("Refresh StoredProcedures feed");
            myMenuItem1.Click += new EventHandler((sender, e) => Refresh(true));
            this.contextMenu.MenuItems.Add(myMenuItem1);

        }

        void myMenuItemAddStoredProcedure_Click(object sender, EventArgs e)
        {
            // 
            Program.GetMain().SetCrudContext(string.Format("Add StoredProcedure in collection {0}", (this.Parent.Tag as Documents.DocumentCollection).Id),
                true, "function() { \r\n \r\n}", this.AddStoredProcedure);
        }

        void myMenuItemAddStoredProcedureFromFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            DialogResult dr = ofd.ShowDialog();

            if (dr == DialogResult.OK)
            {
                string filename = ofd.FileName;
                // 
                string text = System.IO.File.ReadAllText(filename);

                Program.GetMain().SetCrudContext("Add StoredProcedure", false, text, this.AddStoredProcedure);
            }
        }

        async void AddStoredProcedure(string body, string id)
        {
            try
            {
                Documents.StoredProcedure sp = new Documents.StoredProcedure();
                sp.Body = body;
                sp.Id = id;

                ResourceResponse<Documents.StoredProcedure> newsp =  await this.client.CreateStoredProcedureAsync((this.Parent.Tag as Documents.DocumentCollection).SelfLink, sp);

                this.Nodes.Add(new DocumentNode(this.client, newsp.Resource, ResourceType.StoredProcedure));

                // set the result window
                string json = newsp.Resource.ToString();

                Program.GetMain().SetResultInBrowser(json, null, false, newsp.ResponseHeaders);

            }
            catch (AggregateException e)
            {
                Program.GetMain().SetResultInBrowser(null, e.InnerException.ToString(), true);
            }
            catch (Exception e)
            {
                Program.GetMain().SetResultInBrowser(null, e.ToString(), true);
            }
        }

        override public void ShowContextMenu(TreeView treeview, Point p)
        {
            this.contextMenu.Show(treeview, p);
        }

        override public void Refresh(bool forceRefresh)
        {
            if (forceRefresh || this.isFirstTime)
            {
                this.isFirstTime = false;
                this.Nodes.Clear();
                FillWithChildren();
            }
        }

        public void FillWithChildren()
        {
            try
            {
                DocumentCollectionNode collnode = (DocumentCollectionNode)this.Parent;
                FeedResponse<Documents.StoredProcedure> sps = this.client.ReadStoredProcedureFeedAsync((collnode.Tag as Documents.DocumentCollection).SelfLink).Result;

                foreach (var sp in sps)
                {
                    DocumentNode node = new DocumentNode(client, sp, ResourceType.StoredProcedure);
                    this.Nodes.Add(node);
                }
                Program.GetMain().SetResponseHeaders(sps.ResponseHeaders);

            }
            catch (AggregateException e)
            {
                Program.GetMain().SetResultInBrowser(null, e.InnerException.ToString(), true);
            }
            catch (Exception e)
            {
                Program.GetMain().SetResultInBrowser(null, e.ToString(), true);
            }
        }
    }

    class UDFsNode : FeedNode
    {
        private DocumentClient client;
        private ContextMenu contextMenu = new ContextMenu();

        public UDFsNode(DocumentClient client)
        {
            this.Text = "UDFs";
            this.client = client;
            this.Nodes.Add(new TreeNode("fake"));
            this.Tag = "This represents the UserDefinedFunction feed. Right click to add UserDefinedFunction";
            this.ImageKey = "Feed";
            this.SelectedImageKey = "Feed";

            MenuItem myMenuItem = new MenuItem("Add UserDefinedFunction");
            myMenuItem.Click += new EventHandler(myMenuItemAddUDF_Click);
            this.contextMenu.MenuItems.Add(myMenuItem);
            MenuItem myMenuItem2 = new MenuItem("Add UserDefinedFunction from File");
            myMenuItem2.Click += new EventHandler(myMenuItemAddUDF_Click);
            this.contextMenu.MenuItems.Add(myMenuItem2);
            MenuItem myMenuItem1 = new MenuItem("Refresh UserDefinedFunction feed");
            myMenuItem1.Click += new EventHandler((sender, e) => Refresh(true));
            this.contextMenu.MenuItems.Add(myMenuItem1);
        }

        void myMenuItemAddUDF_Click(object sender, EventArgs e)
        {
            // 
            Program.GetMain().SetCrudContext(string.Format("Add UserDefinedFunction in collection {0}", (this.Parent.Tag as Documents.DocumentCollection).Id),
                true, "function() { \r\n \r\n}", this.AddUDF);
        }

        void myMenuItemAddUDFFromFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            DialogResult dr = ofd.ShowDialog();

            if (dr == DialogResult.OK)
            {
                string filename = ofd.FileName;
                // 
                string text = System.IO.File.ReadAllText(filename);

                Program.GetMain().SetCrudContext("Add UDF", false, text, this.AddUDF);
            }
        }
        async void AddUDF(string body, string id)
        {
            try
            {
                Documents.UserDefinedFunction udf = new Documents.UserDefinedFunction();
                udf.Body = body;
                udf.Id = id;

                ResourceResponse<Documents.UserDefinedFunction> newudf = await this.client.CreateUserDefinedFunctionAsync ((this.Parent.Tag as Documents.DocumentCollection).SelfLink, udf);

                this.Nodes.Add(new DocumentNode(this.client, newudf.Resource, ResourceType.UserDefinedFunction));

                // set the result window
                string json = newudf.Resource.ToString();

                Program.GetMain().SetResultInBrowser(json, null, false, newudf.ResponseHeaders);

            }
            catch (AggregateException e)
            {
                Program.GetMain().SetResultInBrowser(null, e.InnerException.ToString(), true);
            }
            catch (Exception e)
            {
                Program.GetMain().SetResultInBrowser(null, e.ToString(), true);
            }
        }

        override public void ShowContextMenu(TreeView treeview, Point p)
        {
            this.contextMenu.Show(treeview, p);
        }

        override public void Refresh(bool forceRefresh)
        {
            if (forceRefresh || this.isFirstTime)
            {
                this.isFirstTime = false;
                this.Nodes.Clear();
                FillWithChildren();
            }
        }

        public void FillWithChildren()
        {
            try
            {
                DocumentCollectionNode collnode = (DocumentCollectionNode)this.Parent;
                FeedResponse<Documents.UserDefinedFunction> sps = this.client.ReadUserDefinedFunctionFeedAsync((collnode.Tag as Documents.DocumentCollection).SelfLink).Result;

                foreach (var sp in sps)
                {
                    DocumentNode node = new DocumentNode(client, sp, ResourceType.UserDefinedFunction);
                    this.Nodes.Add(node);
                }
                Program.GetMain().SetResponseHeaders(sps.ResponseHeaders);

            }
            catch (AggregateException e)
            {
                Program.GetMain().SetResultInBrowser(null, e.InnerException.ToString(), true);
            }
            catch (Exception e)
            {
                Program.GetMain().SetResultInBrowser(null, e.ToString(), true);
            }
        }
    }

    class TriggersNode : FeedNode
    {
        private DocumentClient client;
        private ContextMenu contextMenu = new ContextMenu();

        public TriggersNode(DocumentClient client)
        {
            this.Text = "Triggers";
            this.client = client;
            this.Nodes.Add(new TreeNode("fake"));
            this.Tag = "This represents the Triggers feed. Right click to add Trigger";
            this.ImageKey = "Feed";
            this.SelectedImageKey = "Feed";

            MenuItem myMenuItem = new MenuItem("Add Trigger");
            myMenuItem.Click += new EventHandler(myMenuItemAddTrigger_Click);
            this.contextMenu.MenuItems.Add(myMenuItem);
            MenuItem myMenuItem2 = new MenuItem("Add Trigger from file");
            myMenuItem2.Click += new EventHandler(myMenuItemAddTriggerFromFile_Click);
            this.contextMenu.MenuItems.Add(myMenuItem2);
            MenuItem myMenuItem1 = new MenuItem("Refresh Triggers feed");
            myMenuItem1.Click += new EventHandler((sender, e) => Refresh(true));
            this.contextMenu.MenuItems.Add(myMenuItem1);
        }

        void myMenuItemAddTriggerFromFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            DialogResult dr = ofd.ShowDialog();

            if (dr == DialogResult.OK)
            {
                string filename = ofd.FileName;
                // 
                string text = System.IO.File.ReadAllText(filename);

                Program.GetMain().SetCrudContext("Add trigger", false, text, this.AddTrigger);
            }
        }

        void myMenuItemAddTrigger_Click(object sender, EventArgs e)
        {
            // 
            Program.GetMain().SetCrudContext(string.Format("Add trigger in collection {0}", (this.Parent.Tag as Documents.DocumentCollection).Id),
                true, "function() { \r\n \r\n}", this.AddTrigger);
        }

        async void AddTrigger(string body, string id)
        {
            try
            {
                Documents.Trigger trigger = new Documents.Trigger();
                trigger.Body = body;
                trigger.Id = id;
                trigger.TriggerOperation = Documents.TriggerOperation.All;
                trigger.TriggerType = Documents.TriggerType.Pre;

                ResourceResponse<Documents.Trigger> newtrigger = await this.client.CreateTriggerAsync((this.Parent.Tag as Documents.DocumentCollection).SelfLink, trigger);

                this.Nodes.Add(new DocumentNode(this.client, newtrigger.Resource, ResourceType.Trigger));

                // set the result window
                string json = newtrigger.Resource.ToString();

                Program.GetMain().SetResultInBrowser(json, null, false, newtrigger.ResponseHeaders);

            }
            catch (AggregateException e)
            {
                Program.GetMain().SetResultInBrowser(null, e.InnerException.ToString(), true);
            }
            catch (Exception e)
            {
                Program.GetMain().SetResultInBrowser(null, e.ToString(), true);
            }
        }

        override public void ShowContextMenu(TreeView treeview, Point p)
        {
            this.contextMenu.Show(treeview, p);
        }

        override public void Refresh(bool forceRefresh)
        {
            if (forceRefresh || this.isFirstTime)
            {
                this.isFirstTime = false;
                this.Nodes.Clear();
                FillWithChildren();
            }
        }

        public void FillWithChildren()
        {
            try
            {
                DocumentCollectionNode collnode = (DocumentCollectionNode)this.Parent;
                FeedResponse<Documents.Trigger> sps = this.client.ReadTriggerFeedAsync((collnode.Tag as Documents.DocumentCollection).SelfLink).Result;

                foreach (var sp in sps)
                {
                    DocumentNode node = new DocumentNode(client, sp, ResourceType.Trigger);
                    this.Nodes.Add(node);
                }
                Program.GetMain().SetResponseHeaders(sps.ResponseHeaders);

            }
            catch (AggregateException e)
            {
                Program.GetMain().SetResultInBrowser(null, e.InnerException.ToString(), true);
            }
            catch (Exception e)
            {
                Program.GetMain().SetResultInBrowser(null, e.ToString(), true);
            }

        }
    }

    class UsersNode : FeedNode
    {
        private DocumentClient client;
        private ContextMenu contextMenu = new ContextMenu();

        public UsersNode(DocumentClient client)
        {
            this.Text = "Users";
            this.client = client;
            this.Nodes.Add(new TreeNode("fake"));
            this.Tag = "This represents the Users feed. Right click to add user";
            this.ImageKey = "User";
            this.SelectedImageKey = "User";

            MenuItem myMenuItem = new MenuItem("Add User");
            myMenuItem.Click += new EventHandler(myMenuItemAddUser_Click);
            this.contextMenu.MenuItems.Add(myMenuItem);
            MenuItem myMenuItem1 = new MenuItem("Refresh Users feed");
            myMenuItem1.Click += new EventHandler((sender, e) => Refresh(true));
            this.contextMenu.MenuItems.Add(myMenuItem1);
        }

        void myMenuItemAddUser_Click(object sender, EventArgs e)
        {
            dynamic d = new System.Dynamic.ExpandoObject();
            d.id = "Here is your user Id";
            string x = JsonConvert.SerializeObject(d, Newtonsoft.Json.Formatting.Indented);
            Program.GetMain().SetCrudContext(string.Format("Add user in database {0}", (this.Parent.Tag as Documents.Database).Id),
                false, x, this.AddUser);
        }

        async void AddUser(string body, string id)
        {
            try
            {
                Documents.User user = (Documents.User)JsonConvert.DeserializeObject(body, typeof(Documents.User));

                ResourceResponse<Documents.User> newUser = await this.client.CreateUserAsync((this.Parent.Tag as Documents.Database).SelfLink, user);

                this.Nodes.Add(new DocumentNode(this.client, newUser.Resource, ResourceType.User));

                // set the result window
                string json = newUser.Resource.ToString();

                Program.GetMain().SetResultInBrowser(json, null, false, newUser.ResponseHeaders);

            }
            catch (AggregateException e)
            {
                Program.GetMain().SetResultInBrowser(null, e.InnerException.ToString(), true);
            }
            catch (Exception e)
            {
                Program.GetMain().SetResultInBrowser(null, e.ToString(), true);
            }
        }

        override public void ShowContextMenu(TreeView treeview, Point p)
        {
            this.contextMenu.Show(treeview, p);
        }

        override public void Refresh(bool forceRefresh)
        {
            if (forceRefresh || this.isFirstTime)
            {
                this.isFirstTime = false;
                this.Nodes.Clear();
                FillWithChildren();
            }
        }

        public void FillWithChildren()
        {
            try
            {
                FeedResponse<Documents.User> sps = this.client.ReadUserFeedAsync((this.Parent.Tag as Documents.Database).SelfLink).Result;

                foreach (var sp in sps)
                {
                    DocumentNode node = new DocumentNode(client, sp, ResourceType.User);
                    this.Nodes.Add(node);
                }
                Program.GetMain().SetResponseHeaders(sps.ResponseHeaders);

            }
            catch (AggregateException e)
            {
                Program.GetMain().SetResultInBrowser(null, e.InnerException.ToString(), true);
            }
            catch (Exception e)
            {
                Program.GetMain().SetResultInBrowser(null, e.ToString(), true);
            }

        }
    }

    class PermissionsNode : FeedNode
    {
        private DocumentClient client;
        private ContextMenu contextMenu = new ContextMenu();

        public PermissionsNode(DocumentClient client)
        {
            this.Text = "Permissions";
            this.client = client;
            this.Nodes.Add(new TreeNode("fake"));
            this.Tag = "This represents the Permissions feed. Right click to add permission";
            this.ImageKey = "Permission";
            this.SelectedImageKey = "Permission";

            MenuItem myMenuItem = new MenuItem("Add Permission");
            myMenuItem.Click += new EventHandler(myMenuItemAddPermission_Click);
            this.contextMenu.MenuItems.Add(myMenuItem);
            MenuItem myMenuItem1 = new MenuItem("Refresh Permissions feed");
            myMenuItem1.Click += new EventHandler((sender, e) => Refresh(true));
            this.contextMenu.MenuItems.Add(myMenuItem1);
        }

        void myMenuItemAddPermission_Click(object sender, EventArgs e)
        {
            Documents.Permission permission = new Documents.Permission();
            permission.Id = "Here is your permission Id";
            permission.PermissionMode = Documents.PermissionMode.Read;
            permission.ResourceLink = "your resource link";

            string x = permission.ToString();

            Program.GetMain().SetCrudContext(string.Format("Add permission for user {0}", (this.Parent.Tag as Documents.Resource).Id),
                false, x, this.AddPermission);
        }

        async void AddPermission(string body, string id)
        {
            try
            {
                Documents.Permission permission = Documents.Resource.LoadFrom<Documents.Permission>(new MemoryStream(Encoding.UTF8.GetBytes(body)));

                ResourceResponse<Documents.Permission> newtpermission = await this.client.CreatePermissionAsync((this.Parent.Tag as Documents.Resource).SelfLink, permission);

                this.Nodes.Add(new DocumentNode(this.client, newtpermission.Resource, ResourceType.Permission));

                // set the result window
                string json = newtpermission.Resource.ToString();

                Program.GetMain().SetResultInBrowser(json, null, false, newtpermission.ResponseHeaders);

            }
            catch (AggregateException e)
            {
                Program.GetMain().SetResultInBrowser(null, e.InnerException.ToString(), true);
            }
            catch (Exception e)
            {
                Program.GetMain().SetResultInBrowser(null, e.ToString(), true);
            }
        }

        override public void ShowContextMenu(TreeView treeview, Point p)
        {
            this.contextMenu.Show(treeview, p);
        }

        override public void Refresh(bool forceRefresh)
        {
            if (forceRefresh || this.isFirstTime)
            {
                this.isFirstTime = false;
                this.Nodes.Clear();
                FillWithChildren();
            }
        }

        public void FillWithChildren()
        {
            try
            {
                FeedResponse<Documents.Permission> sps = this.client.ReadPermissionFeedAsync ((this.Parent.Tag as Documents.User).SelfLink).Result;

                foreach (var sp in sps)
                {
                    DocumentNode node = new DocumentNode(client, sp, ResourceType.Permission);
                    this.Nodes.Add(node);
                }
                Program.GetMain().SetResponseHeaders(sps.ResponseHeaders);

            }
            catch (AggregateException e)
            {
                Program.GetMain().SetResultInBrowser(null, e.InnerException.ToString(), true);
            }
            catch (Exception e)
            {
                Program.GetMain().SetResultInBrowser(null, e.ToString(), true);
            }

        }
    }

    class ConflictsNode : FeedNode
    {
        private DocumentClient client;
        private ContextMenu contextMenu = new ContextMenu();

        public ConflictsNode(DocumentClient client)
        {
            this.Text = "Conflicts";
            this.client = client;
            this.Nodes.Add(new TreeNode("fake"));
            this.Tag = "This represents the Conflicts feed.";
            this.ImageKey = "Conflict";
            this.SelectedImageKey = "Conflict";

            MenuItem myMenuItem1 = new MenuItem("Refresh Conflict feed");
            myMenuItem1.Click += new EventHandler((sender, e) => Refresh(true));
            this.contextMenu.MenuItems.Add(myMenuItem1);
        }

        override public void ShowContextMenu(TreeView treeview, Point p)
        {
            this.contextMenu.Show(treeview, p);
        }

        override public void Refresh(bool forceRefresh)
        {
            if (forceRefresh || this.isFirstTime)
            {
                this.isFirstTime = false;
                this.Nodes.Clear();
                FillWithChildren();
            }
        }

        public void FillWithChildren()
        {
            try
            {
                FeedResponse<Documents.Conflict> feedConflicts = this.client.ReadConflictFeedAsync((this.Parent.Tag as Documents.DocumentCollection).ConflictsLink).Result;

                foreach (var sp in feedConflicts)
                {
                    DocumentNode node = new DocumentNode(client, sp, ResourceType.Conflict);
                    this.Nodes.Add(node);
                }
                Program.GetMain().SetResponseHeaders(feedConflicts.ResponseHeaders);

            }
            catch (AggregateException e)
            {
                Program.GetMain().SetResultInBrowser(null, e.InnerException.ToString(), true);
            }
            catch (Exception e)
            {
                Program.GetMain().SetResultInBrowser(null, e.ToString(), true);
            }

        }
    }

}
