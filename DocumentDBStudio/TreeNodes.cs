//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using MimeSharp;

namespace Microsoft.Azure.DocumentDBStudio
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.DocumentDBStudio.Properties;
    using Microsoft.Azure.Documents.Client;
    using Microsoft.Azure.Documents.Linq;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json.Serialization;
    using System.Globalization;
    using System.Collections.Specialized;

    internal class AccountSettings
    {
        public string MasterKey;
        public ConnectionMode ConnectionMode;
        public Protocol Protocol;
        public bool IsNameBased;
        public bool EnableEndpointDiscovery;
    }

    public class CommandContext
    {
        public bool IsDelete;
        public bool IsFeed;
        public bool IsCreateTrigger;
        public bool HasContinuation;
        public bool QueryStarted;
        public CommandContext()
        {
            this.IsDelete = false;
            this.IsFeed = false;
            this.HasContinuation = false;
            this.QueryStarted = false;
            this.IsCreateTrigger = false;
        }
    }

    enum ResourceType
    {
        DatabaseAccount,
        Database,
        DocumentCollection,
        Document,
        User,
        StoredProcedure,
        UserDefinedFunction,
        Trigger,
        Permission,
        Attachment,
        Conflict,
        Offer
    }

    enum OperationType
    {
        Create,
        Replace,
        Read,
        Delete,
        Query,
        Execute,
    }

    enum OfferType
    {
        S1,
        S2,
        S3,
        StandardSingle,
        StandardElastic
    }

    abstract class FeedNode : TreeNode
    {
        protected bool isFirstTime = true;
        abstract public void ShowContextMenu(TreeView treeview, Point p);

        abstract public void Refresh(bool forceRefresh);
    }

    class DatabaseAccountNode : FeedNode
    {
        private DocumentClient client;
        private string accountEndpoint;
        private ContextMenu contextMenu = new ContextMenu();

        public DatabaseAccountNode(string endpointName, DocumentClient client)
        {
            this.accountEndpoint = endpointName;

            this.Text = endpointName;

            this.ImageKey = "DatabaseAccount";
            this.SelectedImageKey = "DatabaseAccount";

            this.client = client;
            this.Tag = "This represents the DatabaseAccount. Right click to add Database";

            this.Nodes.Add(new OfferNode(this.client));
            {
                MenuItem menuItem = new MenuItem("Read DatabaseAccount");
                menuItem.Click += new EventHandler(myMenuItemReadDatabaseAccount_Click);
                this.contextMenu.MenuItems.Add(menuItem);
            }

            {
                MenuItem menuItem = new MenuItem("Create Database");
                menuItem.Click += new EventHandler(myMenuItemCreateDatabase_Click);
                this.contextMenu.MenuItems.Add(menuItem);
            }

            {
                MenuItem menuItem = new MenuItem("Refresh Databases feed");
                menuItem.Click += new EventHandler((sender, e) => Refresh(true));
                this.contextMenu.MenuItems.Add(menuItem);
            }

            {
                MenuItem menuItem = new MenuItem("Query Database");
                menuItem.Click += new EventHandler(myMenuItemQueryDatabase_Click);
                this.contextMenu.MenuItems.Add(menuItem);
            }

            this.contextMenu.MenuItems.Add("-");

            {
                MenuItem menuItem = new MenuItem("Remove setting");
                menuItem.Click += new EventHandler(myMenuItemRemoveDatabaseAccount_Click);
                this.contextMenu.MenuItems.Add(menuItem);
            }

            {
                MenuItem menuItem = new MenuItem("Change setting");
                menuItem.Click += new EventHandler(myMenuItemChangeSetting_Click);
                this.contextMenu.MenuItems.Add(menuItem);
            }
        }

        void myMenuItemChangeSetting_Click(object sender, EventArgs e)
        {
            Program.GetMain().ChangeAccountSettings(this, this.accountEndpoint);
        }

        async void myMenuItemReadDatabaseAccount_Click(object sender, EventArgs eArgs)
        {
            try
            {
                DatabaseAccount databaseAccount;
                using (PerfStatus.Start("ReadDatabase"))
                {
                    databaseAccount = await this.client.GetDatabaseAccountAsync();
                }
                // set the result window
                string json = JsonConvert.SerializeObject(databaseAccount, Newtonsoft.Json.Formatting.Indented);

                this.Tag = databaseAccount;
                Program.GetMain().SetResultInBrowser(json, null, false);

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

        void myMenuItemCreateDatabase_Click(object sender, EventArgs e)
        {
            // 
            dynamic d = new System.Dynamic.ExpandoObject();
            d.id = "Here is your Database Id";
            string x = JsonConvert.SerializeObject(d, Newtonsoft.Json.Formatting.Indented);
            Program.GetMain().SetCrudContext(this, OperationType.Create,  ResourceType.Database, x, this.CreateDatabaseAsync);
        }

        void myMenuItemQueryDatabase_Click(object sender, EventArgs e)
        {
            Program.GetMain().SetCrudContext(this, OperationType.Query, ResourceType.Database, "select * from c", this.QueryDatabasesAsync);
        }

        void myMenuItemRemoveDatabaseAccount_Click(object sender, EventArgs e)
        {
            // 
            this.Remove();
            Program.GetMain().RemoveAccountFromSettings(this.accountEndpoint);
        }

        async void QueryDatabasesAsync(object resource, RequestOptions requestOptions)
        {
            try
            {
                string queryText = resource as string;

                FeedResponse<Database> r;
                using (PerfStatus.Start("QueryDatabase"))
                {
                    // text is the querytext.
                    IDocumentQuery<dynamic> q = this.client.CreateDatabaseQuery(queryText).AsDocumentQuery();
                    r = await q.ExecuteNextAsync<Database>();
                }

                // set the result window
                string text = null;
                if (r.Count > 1)
                {
                    text = string.Format(CultureInfo.InvariantCulture, "Returned {0} dataqbases", r.Count);
                }
                else
                {
                    text = string.Format(CultureInfo.InvariantCulture, "Returned {0} dataqbases", r.Count);
                }

                string jsonarray = "[";
                int index = 0;
                foreach (dynamic d in r)
                {
                    index++;
                    // currently Query.ToString() has Formatting.Indented, but the public release doesn't have yet.
                    jsonarray += d.ToString();

                    if (index == r.Count)
                    {
                        jsonarray += "]";
                    }
                    else
                    {
                        jsonarray += ",\r\n";
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

        private async void FillWithChildren()
        {
            try
            {
                FeedResponse<Database> databases;
                using (PerfStatus.Start("ReadDatabaseFeed"))
                {
                    databases = await this.client.ReadDatabaseFeedAsync();
                }

                foreach (Database db in databases)
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

                this.Nodes.Add(new OfferNode(this.client));

                FillWithChildren();
            }
        }

        async void CreateDatabaseAsync(object resource, RequestOptions requestOptions)
        {
            try
            {
                string text = resource as string;
                Database db = (Database)JsonConvert.DeserializeObject(text, typeof(Database));

                ResourceResponse<Database> newdb;
                using (PerfStatus.Start("CreateDatabase"))
                {
                    newdb = await this.client.CreateDatabaseAsync(db, requestOptions);
                }
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
        private string currentContinuation = null;
        private CommandContext currentQueryCommandContext = null;

        public DatabaseNode(DocumentClient localclient, Database db)
        {
            this.Text = db.Id;
            this.Tag = db;
            this.client = localclient;
            this.ImageKey = "SystemFeed";
            this.SelectedImageKey = "SystemFeed";

            this.Nodes.Add(new UserNode(this.client));

            {
                MenuItem menuItem = new MenuItem("Read Database");
                menuItem.Click += new EventHandler(myMenuItemReadDatabase_Click);
                this.contextMenu.MenuItems.Add(menuItem);
            }

            {
                MenuItem menuItem = new MenuItem("Delete Database");
                menuItem.Click += new EventHandler(myMenuItemDeleteDatabase_Click);
                this.contextMenu.MenuItems.Add(menuItem);
            }

            this.contextMenu.MenuItems.Add("-");

            {
                MenuItem menuItem = new MenuItem("Create DocumentCollection");
                menuItem.Click += new EventHandler(myMenuItemCreateDocumentCollection_Click);
                this.contextMenu.MenuItems.Add(menuItem);
            }

            {
                MenuItem menuItem = new MenuItem("Refresh DocumentCollections Feed");
                menuItem.Click += new EventHandler((sender, e) => Refresh(true));
                this.contextMenu.MenuItems.Add(menuItem);
            }

            {
                MenuItem menuItem = new MenuItem("Query DocumentCollections");
                menuItem.Click += new EventHandler(myMenuItemQueryDocumentCollection_Click);
                this.contextMenu.MenuItems.Add(menuItem);
            }

        }

        async void myMenuItemReadDatabase_Click(object sender, EventArgs eArgs)
        {
            try
            {
                ResourceResponse<Database> database;
                using (PerfStatus.Start("ReadDatabase"))
                {
                    database = await this.client.ReadDatabaseAsync(((Database)this.Tag).GetLink(this.client), Program.GetMain().GetRequestOptions());
                }
                // set the result window
                string json = JsonConvert.SerializeObject(database.Resource, Newtonsoft.Json.Formatting.Indented);

                Program.GetMain().SetResultInBrowser(json, null, false, database.ResponseHeaders);
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


        void myMenuItemQueryDocumentCollection_Click(object sender, EventArgs e)
        {
            this.currentQueryCommandContext = new CommandContext();
            this.currentQueryCommandContext.IsFeed = true;

            // reset continuation token
            this.currentContinuation = null;

            Program.GetMain().SetCrudContext(this, OperationType.Query, ResourceType.Document, "select * from c", this.QueryDocumentCollectionsAsync, this.currentQueryCommandContext);
        }

        async void QueryDocumentCollectionsAsync(object resource, RequestOptions requestOptions)
        {
            try
            {
                string queryText = resource as string;
                // text is the querytext.
                IDocumentQuery<dynamic> q = null;

                FeedOptions feedOptions = Program.GetMain().GetFeedOptions();

                if (requestOptions == null)
                {
                    // requestOptions = null means it is from the next page. We only attempt to continue using the RequestContinuation for next page button
                    if (!string.IsNullOrEmpty(this.currentContinuation) && string.IsNullOrEmpty(feedOptions.RequestContinuation))
                    {
                        feedOptions.RequestContinuation = this.currentContinuation;
                    }
                }

                q = this.client.CreateDocumentCollectionQuery((this.Tag as Database).GetLink(this.client), queryText, feedOptions).AsDocumentQuery();

                Stopwatch sw = Stopwatch.StartNew();

                FeedResponse<dynamic> r;
                using (PerfStatus.Start("QueryDocument"))
                {
                    r = await q.ExecuteNextAsync();
                }
                sw.Stop();
                this.currentContinuation = r.ResponseContinuation;
                this.currentQueryCommandContext.HasContinuation = !string.IsNullOrEmpty(this.currentContinuation);
                this.currentQueryCommandContext.QueryStarted = true;

                // set the result window
                string text = null;
                if (r.Count > 1)
                {
                    text = string.Format(CultureInfo.InvariantCulture, "Returned {0} collections in {1} ms.", r.Count, sw.ElapsedMilliseconds);
                }
                else
                {
                    text = string.Format(CultureInfo.InvariantCulture, "Returned {0} collections in {1} ms.", r.Count, sw.ElapsedMilliseconds);
                }

                if (r.ResponseContinuation != null)
                {
                    text += " (more results might be available)";
                }

                string jsonarray = "[";
                int index = 0;
                foreach (dynamic d in r)
                {
                    index++;
                    // currently Query.ToString() has Formatting.Indented, but the public release doesn't have yet.
                    jsonarray += d.ToString();

                    if (index == r.Count)
                    {
                        jsonarray += "]";
                    }
                    else
                    {
                        jsonarray += ",\r\n";
                    }
                }

                Program.GetMain().SetResultInBrowser(jsonarray, text, true, r.ResponseHeaders);
                Program.GetMain().SetNextPageVisibility(this.currentQueryCommandContext);
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


        void myMenuItemDeleteDatabase_Click(object sender, EventArgs e)
        {
            string x = this.Tag.ToString();
            CommandContext context = new CommandContext();
            context.IsDelete = true;
            Program.GetMain().SetCrudContext(this, OperationType.Delete,  ResourceType.Database, x, this.DeleteDatabaseAsync, context);
        }

        void myMenuItemCreateDocumentCollection_Click(object sender, EventArgs e)
        {
            dynamic d = new System.Dynamic.ExpandoObject();
            d.id = "Here is your DocumentCollection Id";

            string x = JsonConvert.SerializeObject(d, Newtonsoft.Json.Formatting.Indented);
            Program.GetMain().SetCrudContext(this, OperationType.Create,  ResourceType.DocumentCollection, x, this.CreateDocumentCollectionAsync);
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

                this.Nodes.Add(new UserNode(this.client));

                FillWithChildren();
            }
        }

        async void CreateDocumentCollectionAsync(object resource, RequestOptions requestOptions)
        {
            try
            {
                DocumentCollection coll = resource as DocumentCollection;
                Database db = (Database)this.Tag;
                ResourceResponse<DocumentCollection> newcoll;
                using (PerfStatus.Start("CreateDocumentCollection"))
                {
                    newcoll = await this.client.CreateDocumentCollectionAsync(db.GetLink(this.client), coll, requestOptions);
                }

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

        async void DeleteDatabaseAsync(object resource, RequestOptions requestOptions)
        {
            try
            {
                Database db = (Database)this.Tag;
                ResourceResponse<Database> newdb;
                using (PerfStatus.Start("DeleteDatabase"))
                {
                    newdb = await this.client.DeleteDatabaseAsync(db.GetLink(this.client), requestOptions);
                }

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
                FeedResponse<DocumentCollection> colls;
                using (PerfStatus.Start("ReadDocumentCollectionFeed"))
                {
                    colls = await this.client.ReadDocumentCollectionFeedAsync(((Database)this.Tag).GetLink(this.client));
                }

                foreach (DocumentCollection coll in colls)
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
        private string currentContinuation = null;
        private CommandContext currentQueryCommandContext = null;
 
        public DocumentCollectionNode(DocumentClient client, DocumentCollection coll)
        {
            this.Text = coll.Id;
            this.Tag = coll;
            this.client = client;
            this.ImageKey = "SystemFeed";
            this.SelectedImageKey = "SystemFeed";

            this.Nodes.Add(new StoredProcedureNode(this.client));
            this.Nodes.Add(new UserDefinedFunctionNode(this.client));
            this.Nodes.Add(new TriggerNode(this.client));
            this.Nodes.Add(new ConflictNode(this.client));

            {
                MenuItem menuItem = new MenuItem("Read DocumentCollection");
                menuItem.Click += new EventHandler(myMenuItemReadDocumentCollection_Click);
                this.contextMenu.MenuItems.Add(menuItem);
            }
            {
                MenuItem menuItem = new MenuItem("Replace DocumentCollection");
                menuItem.Click += new EventHandler(myMenuItemReplaceDocumentCollection_Click);
                this.contextMenu.MenuItems.Add(menuItem);
            }
            {
                MenuItem menuItem = new MenuItem("Delete DocumentCollection");
                menuItem.Click += new EventHandler(myMenuItemDeleteDocumentCollection_Click);
                this.contextMenu.MenuItems.Add(menuItem);
            }
            this.contextMenu.MenuItems.Add("-");
            {
                MenuItem menuItem = new MenuItem("Create Document");
                menuItem.Click += new EventHandler(myMenuItemCreateDocument_Click);
                this.contextMenu.MenuItems.Add(menuItem);
            }
            {
                MenuItem menuItem = new MenuItem("Create Document From File");
                menuItem.Click += new EventHandler(myMenuItemCreateDocumentFromFile_Click);
                this.contextMenu.MenuItems.Add(menuItem);
            }
            {
                MenuItem menuItem = new MenuItem("Create Multiple Documents From Folder");
                menuItem.Click += new EventHandler(myMenuItemCreateDocumentsFromFolder_Click);
                this.contextMenu.MenuItems.Add(menuItem);
            }
            {
                MenuItem menuItem = new MenuItem("Refresh Documents feed");
                menuItem.Click += new EventHandler((sender, e) => Refresh(true));
                this.contextMenu.MenuItems.Add(menuItem);
            }
            {
                MenuItem menuItem = new MenuItem("Query Documents");
                menuItem.Click += new EventHandler(myMenuItemQueryDocument_Click);
                this.contextMenu.MenuItems.Add(menuItem);
            }
        }

        async void myMenuItemReadDocumentCollection_Click(object sender, EventArgs eArgs)
        {
            try
            {
                ResourceResponse<DocumentCollection> rr;
                using (PerfStatus.Start("ReadDocumentCollection"))
                {
                    rr = await this.client.ReadDocumentCollectionAsync(((Resource)this.Tag).GetLink(this.client), Program.GetMain().GetRequestOptions());
                }
                // set the result window
                string json = JsonConvert.SerializeObject(rr.Resource, Newtonsoft.Json.Formatting.Indented);

                Program.GetMain().SetResultInBrowser(json, null, false, rr.ResponseHeaders);

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

        void myMenuItemDeleteDocumentCollection_Click(object sender, EventArgs e)
        {
            string x = this.Tag.ToString();
            CommandContext context = new CommandContext();
            context.IsDelete = true;
            Program.GetMain().SetCrudContext(this, OperationType.Delete,  ResourceType.DocumentCollection, x, this.DeleteDocumentCollectionAsync, context);
        }

        void myMenuItemReplaceDocumentCollection_Click(object sender, EventArgs e)
        {
            string x = this.Tag.ToString();
            CommandContext context = new CommandContext();
            Program.GetMain().SetCrudContext(this, OperationType.Replace, ResourceType.DocumentCollection, x, this.ReplaceDocumentCollectionAsync, context);
        }

        async void ReplaceDocumentCollectionAsync(object resource, RequestOptions requestOptions)
        {
            try
            {
                DocumentCollection coll = (DocumentCollection)this.Tag;

                //Update collection if necessary
                DocumentCollection collToChange = resource as DocumentCollection;
                collToChange.IndexingPolicy = (IndexingPolicy)coll.IndexingPolicy.Clone();

                ResourceResponse<DocumentCollection> response;
                using (PerfStatus.Start("ReplaceDocumentCollection"))
                {
                    response = await client.ReplaceDocumentCollectionExAsync(coll, requestOptions);
                }

                Program.GetMain().SetResultInBrowser(null, "Replace DocumentCollection succeed!", false, response.ResponseHeaders);
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

        async void DeleteDocumentCollectionAsync(object resource, RequestOptions requestOptions)
        {
            try
            {
                DocumentCollection coll = (DocumentCollection)this.Tag;
                ResourceResponse<DocumentCollection> newcoll;
                using (PerfStatus.Start("DeleteDocumentCollection"))
                {
                    newcoll = await this.client.DeleteDocumentCollectionAsync(coll.GetLink(this.client), Program.GetMain().GetRequestOptions());
                }
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

        void myMenuItemCreateDocumentFromFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            DialogResult dr = ofd.ShowDialog();

            if (dr == DialogResult.OK)
            {
                string filename = ofd.FileName;
                // 
                string text = System.IO.File.ReadAllText(filename);

                Program.GetMain().SetCrudContext(this, OperationType.Create, ResourceType.Document, text, this.CreateDocumentAsync);
            }
        }

        async void myMenuItemCreateDocumentsFromFolder_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;

            DialogResult dr = ofd.ShowDialog();

            if (dr == DialogResult.OK)
            {
                string status = string.Format(CultureInfo.InvariantCulture, "Create {0} documents in collection\r\n", ofd.FileNames.Length);
                // Read the files 
                foreach (String filename in ofd.FileNames)
                {

                    // right now assume every file is JSON content
                    string jsonText = System.IO.File.ReadAllText(filename);
                    string fileRootName = Path.GetFileName(filename);

                    object document = JsonConvert.DeserializeObject(jsonText);

                    try
                    {
                        using (PerfStatus.Start("CreateDocument"))
                        {
                            ResourceResponse<Document> newdocument = await this.client.CreateDocumentAsync((this.Tag as DocumentCollection).GetLink(this.client), document, Program.GetMain().GetRequestOptions());
                            status += string.Format(CultureInfo.InvariantCulture, "Succeed adding {0} \r\n", fileRootName);
                        }
                    }
                    catch (DocumentClientException ex)
                    {
                        status += string.Format(CultureInfo.InvariantCulture, "Failed adding {0}, statusCode={1} \r\n", fileRootName, ex.StatusCode);
                    }
                    catch (Exception ex)
                    {
                        status += string.Format(CultureInfo.InvariantCulture, "Failed adding {0}, unknown exception \r\n", fileRootName, ex.Message);
                    }

                    Program.GetMain().SetResultInBrowser(null, status, false);

                }
            }
        }

        void myMenuItemCreateDocument_Click(object sender, EventArgs e)
        {
            // 
            dynamic d = new System.Dynamic.ExpandoObject();
            d.id = "Here is your Document Id";
            string x = JsonConvert.SerializeObject(d, Newtonsoft.Json.Formatting.Indented);
            Program.GetMain().SetCrudContext(this, OperationType.Create,  ResourceType.Document, x, this.CreateDocumentAsync);
        }


        void myMenuItemQueryDocument_Click(object sender, EventArgs e)
        {
            this.currentQueryCommandContext = new CommandContext();
            this.currentQueryCommandContext.IsFeed = true;

            // reset continuation token
            this.currentContinuation = null;

            Program.GetMain().SetCrudContext(this, OperationType.Query, ResourceType.Document, "select * from c", this.QueryDocumentsAsync, this.currentQueryCommandContext);

        }

        async void CreateDocumentAsync(object resource, RequestOptions requestOptions)
        {
            try
            {
                object document = JsonConvert.DeserializeObject(resource as string);

                ResourceResponse<Document> newdocument;
                using (PerfStatus.Start("CreateDocument"))
                {
                    newdocument = await this.client.CreateDocumentAsync((this.Tag as DocumentCollection).GetLink(this.client), document, requestOptions);
                }

                this.Nodes.Add(new ResourceNode(this.client, newdocument.Resource, ResourceType.Document, ((DocumentCollection)this.Tag).PartitionKey));

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

        async void QueryDocumentsAsync(object resource, RequestOptions requestOptions)
        {
            try
            {
                string queryText = resource as string;
                // text is the querytext.
                IDocumentQuery<dynamic> q = null;

                FeedOptions feedOptions = Program.GetMain().GetFeedOptions();

                if (requestOptions == null)
                {
                    // requestOptions = null means it is from the next page. We only attempt to continue using the RequestContinuation for next page button
                    if (!string.IsNullOrEmpty(this.currentContinuation) && string.IsNullOrEmpty(feedOptions.RequestContinuation))
                    {
                        feedOptions.RequestContinuation = this.currentContinuation;
                    }
                }

                q = this.client.CreateDocumentQuery((this.Tag as DocumentCollection).GetLink(this.client), queryText, feedOptions).AsDocumentQuery();

                Stopwatch sw = Stopwatch.StartNew();

                FeedResponse<dynamic> r;
                using (PerfStatus.Start("QueryDocument"))
                {
                    r = await q.ExecuteNextAsync();
                }
                sw.Stop();
                this.currentContinuation = r.ResponseContinuation;
                this.currentQueryCommandContext.HasContinuation = !string.IsNullOrEmpty(this.currentContinuation);
                this.currentQueryCommandContext.QueryStarted = true;

                // set the result window
                string text = null;
                if (r.Count > 1)
                {
                    text = string.Format(CultureInfo.InvariantCulture, "Returned {0} documents in {1} ms.", r.Count, sw.ElapsedMilliseconds);
                }
                else
                {
                    text = string.Format(CultureInfo.InvariantCulture, "Returned {0} document in {1} ms.", r.Count, sw.ElapsedMilliseconds);
                }

                if (r.ResponseContinuation != null)
                {
                    text += " (more results might be available)";
                }

                string jsonarray = "[";
                int index = 0;
                foreach (dynamic d in r)
                {
                    index++;
                    // currently Query.ToString() has Formatting.Indented, but the public release doesn't have yet.
                    jsonarray += d.ToString();

                    if (index == r.Count)
                    {
                        jsonarray += "]";
                    }
                    else
                    {
                        jsonarray += ",\r\n";
                    }
                }

                Program.GetMain().SetResultInBrowser(jsonarray, text, true, r.ResponseHeaders);
                Program.GetMain().SetNextPageVisibility(this.currentQueryCommandContext);
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
                this.Nodes.Add(new StoredProcedureNode(this.client));
                this.Nodes.Add(new UserDefinedFunctionNode(this.client));
                this.Nodes.Add(new TriggerNode(this.client));
                this.Nodes.Add(new ConflictNode(this.client));

                FillWithChildren();
            }
        }

        public void FillWithChildren()
        {
            try
            {
                List<dynamic> docs = new List<dynamic>();
                NameValueCollection responseHeaders = null;

                using (PerfStatus.Start("ReadDocumentFeed"))
                {
                    ResourceFeedReader<Document> feedReader = this.client.CreateDocumentFeedReader(((DocumentCollection)this.Tag).GetLink(this.client), new FeedOptions { EnableCrossPartitionQuery = true });
                    while (feedReader.HasMoreResults && docs.Count() < 100)
                    {
                        FeedResponse<Document> response = feedReader.ExecuteNextAsync().Result;
                        docs.AddRange(response);

                        responseHeaders = response.ResponseHeaders;
                    }
                }
                docs.Sort((first, second) => string.Compare(((Document)first).Id, ((Document)second).Id, StringComparison.Ordinal));

                foreach (var doc in docs)
                {
                    ResourceNode node = new ResourceNode(client, doc, ResourceType.Document, ((DocumentCollection)this.Tag).PartitionKey);
                    this.Nodes.Add(node);
                }

                Program.GetMain().SetResponseHeaders(responseHeaders);
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

    class ResourceNode : FeedNode
    {
        private DocumentClient client;
        private ContextMenu contextMenu = new ContextMenu();
        private ResourceType resourceType = 0;

        public ResourceNode(DocumentClient client, dynamic document, ResourceType resoureType, PartitionKeyDefinition partitionKey = null)
        {
            this.resourceType = resoureType;
            if (this.resourceType == ResourceType.Document)
            {
                string prefix = string.Empty;
                if (partitionKey !=null)
                {
                    if (partitionKey.Paths.Count > 0)
                    {
                        string path = partitionKey.Paths[0];
                        prefix = document.GetPropertyValue<String>(path.Substring(1));
                        prefix = prefix + "_";
                    }
                }
                this.Text = prefix  + (document as Resource).Id;
            }
            else if (this.resourceType == ResourceType.Offer)
            {
                string version = document.GetPropertyValue<String>("offerVersion");
                if (string.IsNullOrEmpty(version))
                {
                    Offer offer = document as Offer;
                    this.Text = offer.OfferType + "_" + offer.GetPropertyValue<String>("offerResourceId");
                }
                else
                {
                    OfferV2 offer = document as OfferV2;
                    this.Text = offer.Content.OfferThroughput + "_" + offer.GetPropertyValue<String>("offerResourceId");
                }
            }
            else
            {
                this.Text = (document as Resource).Id;
            }

            this.Tag = document;
            this.client = client;

            {
                MenuItem menuItem = new MenuItem("Read " + this.resourceType.ToString());
                menuItem.Click += new EventHandler(myMenuItemRead_Click);
                this.contextMenu.MenuItems.Add(menuItem);
            }

            if (this.resourceType != ResourceType.Conflict && this.resourceType != ResourceType.Offer)
            {
                MenuItem menuItem = new MenuItem("Replace " + this.resourceType.ToString());
                menuItem.Click += new EventHandler(myMenuItemUpdate_Click);
                this.contextMenu.MenuItems.Add(menuItem);
            }
            if (this.resourceType != ResourceType.Offer)
            {
                MenuItem menuItem = new MenuItem("Delete " + this.resourceType.ToString());
                menuItem.Click += new EventHandler(myMenuItemDelete_Click);
                this.contextMenu.MenuItems.Add(menuItem);
            }

            if (this.resourceType == ResourceType.Permission)
            {
                this.ImageKey = "Permission";
                this.SelectedImageKey = "Permission";
            }
            else if (this.resourceType == ResourceType.Attachment)
            {
                this.ImageKey = "Attachment";
                this.SelectedImageKey = "Attachment";

                {
                    MenuItem menuItem = new MenuItem("Download media");
                    menuItem.Click += new EventHandler(myMenuItemDownloadMedia_Click);
                    this.contextMenu.MenuItems.Add(menuItem);
                }
                {
                    MenuItem menuItem = new MenuItem("Render media");
                    menuItem.Click += new EventHandler(myMenuItemRenderMedia_Click);
                    this.contextMenu.MenuItems.Add(menuItem);
                }
            }
            else if (this.resourceType == ResourceType.StoredProcedure || this.resourceType == ResourceType.Trigger || this.resourceType == ResourceType.UserDefinedFunction)
            {
                this.ImageKey = "Javascript";
                this.SelectedImageKey = "Javascript";
                if (this.resourceType == ResourceType.StoredProcedure)
                {
                    MenuItem menuItem = new MenuItem("Execute " + this.resourceType.ToString());
                    menuItem.Click += new EventHandler(myMenuItemExecuteStoredProcedure_Click);
                    this.contextMenu.MenuItems.Add(menuItem);
                }
            }
            else if (this.resourceType == ResourceType.User)
            {
                this.ImageKey = "User";
                this.SelectedImageKey = "User";

                this.Nodes.Add(new PermissionNode(this.client));
            }
            else if (this.resourceType == ResourceType.Document)
            {
                this.Nodes.Add(new TreeNode("Fake"));

                this.contextMenu.MenuItems.Add("-");

                {
                    MenuItem menuItem = new MenuItem("Create attachment");
                    menuItem.Click += new EventHandler(myMenuItemCreateAttachment_Click);
                    this.contextMenu.MenuItems.Add(menuItem);
                }
                {
                    MenuItem menuItem = new MenuItem("Create attachment from file");
                    menuItem.Click += new EventHandler(myMenuItemAttachmentFromFile_Click);
                    this.contextMenu.MenuItems.Add(menuItem);
                }
            }
            else if (this.resourceType == ResourceType.Conflict)
            {
                this.ImageKey = "Conflict";
                this.SelectedImageKey = "Conflict";
            }
            else if (this.resourceType == ResourceType.Offer)
            {
                this.ImageKey = "Offer";
                this.SelectedImageKey = "Offer";
            }
        }

        void myMenuItemUpdate_Click(object sender, EventArgs e)
        {
            if (this.resourceType == ResourceType.StoredProcedure)
            {
                Program.GetMain().SetCrudContext(this, OperationType.Replace, this.resourceType, (this.Tag as StoredProcedure).Body, this.ReplaceResourceAsync);
            }
            else if (this.resourceType == ResourceType.Trigger)
            {
                Program.GetMain().SetCrudContext(this, OperationType.Replace, this.resourceType, (this.Tag as Trigger).Body, this.ReplaceResourceAsync);
            }
            else if (this.resourceType == ResourceType.UserDefinedFunction)
            {
                Program.GetMain().SetCrudContext(this, OperationType.Replace, this.resourceType, (this.Tag as UserDefinedFunction).Body, this.ReplaceResourceAsync);
            }
            else
            {
                string x = this.Tag.ToString();
                Program.GetMain().SetCrudContext(this, OperationType.Replace, this.resourceType, x, this.ReplaceResourceAsync);
            }
        }

        async void myMenuItemRead_Click(object sender, EventArgs eventArg)
        {
            try
            {
                if (this.resourceType == ResourceType.Offer)
                {
                    ResourceResponse<Offer> rr;
                    using (PerfStatus.Start("ReadOffer"))
                    {
                        rr = await this.client.ReadOfferAsync(((Resource)this.Tag).SelfLink);
                    }
                    // set the result window
                    string json = JsonConvert.SerializeObject(rr.Resource, Newtonsoft.Json.Formatting.Indented);

                    Program.GetMain().SetResultInBrowser(json, null, false, rr.ResponseHeaders);
                }
                else if (this.resourceType == ResourceType.Document)
                {
                    Document document = ((Document)this.Tag);
                    DocumentCollection collection = ((DocumentCollection)this.Parent.Tag);

                    RequestOptions requestOptions = Program.GetMain().GetRequestOptions();
                    if (collection.PartitionKey != null && collection.PartitionKey.Paths.Count > 0)
                    {
                        requestOptions.PartitionKey = new PartitionKey(DocumentAnalyzer.ExtractPartitionKeyValue(document, collection.PartitionKey));
                    }

                    ResourceResponse<Document> rr;
                    using (PerfStatus.Start("ReadDocument"))
                    {
                        rr = await this.client.ReadDocumentAsync(document.GetLink(this.client), requestOptions);
                    }
                    // set the result window
                    string json = JsonConvert.SerializeObject(rr.Resource, Newtonsoft.Json.Formatting.Indented);

                    Program.GetMain().SetResultInBrowser(json, null, false, rr.ResponseHeaders);
                }
                else if (this.resourceType == ResourceType.Conflict)
                {
                    ResourceResponse<Conflict> rr;
                    using (PerfStatus.Start("ReadConflict"))
                    {
                        rr = await this.client.ReadConflictAsync(((Resource)this.Tag).GetLink(this.client), Program.GetMain().GetRequestOptions());
                    }
                    // set the result window
                    string json = JsonConvert.SerializeObject(rr.Resource, Newtonsoft.Json.Formatting.Indented);

                    Program.GetMain().SetResultInBrowser(json, null, false, rr.ResponseHeaders);
                }
                else if (this.resourceType == ResourceType.Attachment)
                {
                    ResourceResponse<Attachment> rr;
                    Document document = ((Document)this.Tag);
                    DocumentCollection collection = ((DocumentCollection)this.Parent.Tag);

                    RequestOptions requestOptions = Program.GetMain().GetRequestOptions();
                    if (collection.PartitionKey != null && collection.PartitionKey.Paths.Count > 0)
                    {
                        requestOptions.PartitionKey = new PartitionKey(DocumentAnalyzer.ExtractPartitionKeyValue(document, collection.PartitionKey));
                    }

                    using (PerfStatus.Start("ReadAttachment"))
                    {
                        rr = await this.client.ReadAttachmentAsync(document.GetLink(this.client), requestOptions);
                    }

                    // set the result window
                    string json = JsonConvert.SerializeObject(rr.Resource, Newtonsoft.Json.Formatting.Indented);

                    Program.GetMain().SetResultInBrowser(json, null, false, rr.ResponseHeaders);
                }
                else if (this.resourceType == ResourceType.User)
                {
                    ResourceResponse<User> rr;
                    using (PerfStatus.Start("ReadUser"))
                    {
                        rr = await this.client.ReadUserAsync(((Resource)this.Tag).GetLink(this.client), Program.GetMain().GetRequestOptions());
                    }
                    // set the result window
                    string json = JsonConvert.SerializeObject(rr.Resource, Newtonsoft.Json.Formatting.Indented);

                    Program.GetMain().SetResultInBrowser(json, null, false, rr.ResponseHeaders);
                }
                else if (this.resourceType == ResourceType.Permission)
                {
                    ResourceResponse<Permission> rr;
                    using (PerfStatus.Start("ReadPermission"))
                    {
                        rr = await this.client.ReadPermissionAsync(((Resource)this.Tag).GetLink(this.client), Program.GetMain().GetRequestOptions());
                    }
                    // set the result window
                    string json = JsonConvert.SerializeObject(rr.Resource, Newtonsoft.Json.Formatting.Indented);

                    Program.GetMain().SetResultInBrowser(json, null, false, rr.ResponseHeaders);
                }
                else if (this.resourceType == ResourceType.StoredProcedure)
                {
                    ResourceResponse<StoredProcedure> rr;
                    using (PerfStatus.Start("ReadStoredProcedure"))
                    {
                        rr = await this.client.ReadStoredProcedureAsync(((Resource)this.Tag).GetLink(this.client), Program.GetMain().GetRequestOptions());
                    }
                    // set the result window
                    string json = JsonConvert.SerializeObject(rr.Resource, Newtonsoft.Json.Formatting.Indented);

                    Program.GetMain().SetResultInBrowser(json, null, false, rr.ResponseHeaders);
                }
                else if (this.resourceType == ResourceType.Trigger)
                {
                    ResourceResponse<Trigger> rr;
                    using (PerfStatus.Start("ReadTrigger"))
                    {
                        rr = await this.client.ReadTriggerAsync(((Resource)this.Tag).GetLink(this.client), Program.GetMain().GetRequestOptions());
                    }
                    // set the result window
                    string json = JsonConvert.SerializeObject(rr.Resource, Newtonsoft.Json.Formatting.Indented);

                    Program.GetMain().SetResultInBrowser(json, null, false, rr.ResponseHeaders);
                }
                else if (this.resourceType == ResourceType.UserDefinedFunction)
                {
                    ResourceResponse<UserDefinedFunction> rr;
                    using (PerfStatus.Start("ReadUDF"))
                    {
                        rr = await this.client.ReadUserDefinedFunctionAsync(((Resource)this.Tag).GetLink(this.client), Program.GetMain().GetRequestOptions());
                    }
                    // set the result window
                    string json = JsonConvert.SerializeObject(rr.Resource, Newtonsoft.Json.Formatting.Indented);

                    Program.GetMain().SetResultInBrowser(json, null, false, rr.ResponseHeaders);
                }
                else
                {
                    throw new ArgumentException("Unsupported resource type " + this.resourceType);
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

        void myMenuItemCreateAttachment_Click(object sender, EventArgs e)
        {
            Attachment attachment = new Attachment();
            attachment.Id = "Here is your attachment Id";
            attachment.ContentType = "application-content-type";
            attachment.MediaLink = "internal link or Azure blob or Amazon S3 link";

            string x = attachment.ToString();
            Program.GetMain().SetCrudContext(this, OperationType.Create, this.resourceType, x, this.CreateAttachmentAsync);
        }

        async void myMenuItemRenderMedia_Click(object sender, EventArgs eventArg)
        {
            string appTempPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DocumentDBStudio");
            string guidFileName = Guid.NewGuid().ToString();
            string fileName;

            // let's guess the contentype.
            Attachment attachment = this.Tag as Attachment;
            if (string.Compare(attachment.ContentType, "application/octet-stream", StringComparison.OrdinalIgnoreCase) == 0)
            {
                // get the extension from attachment.Id
                int index = attachment.Id.LastIndexOf('.');
                fileName = guidFileName + attachment.Id.Substring(index);
            }
            else if (attachment.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            {
                // treat as image.
                fileName = guidFileName + ".gif";
            }
            else
            {
                fileName = guidFileName + ".txt";
            }

            fileName = Path.Combine(appTempPath, fileName);
            try
            {
                MediaResponse rr;
                using (PerfStatus.Start("DownloadMedia"))
                {
                    rr = await this.client.ReadMediaAsync(attachment.MediaLink);
                }
                using (FileStream fileStream = File.Create(fileName))
                {
                    rr.Media.CopyTo(fileStream);
                }

                Program.GetMain().SetResultInBrowser(null, "It is saved to " + fileName, true);
                Program.GetMain().RenderFile(fileName);
            }
            catch (Exception e)
            {
                Program.GetMain().SetResultInBrowser(null, e.ToString(), true);
            }
        }

        async void myMenuItemDownloadMedia_Click(object sender, EventArgs eventArg)
        {
            Attachment attachment = this.Tag as Attachment;

            // Get the filenanme from attachment.Id
            int index = attachment.Id.LastIndexOf('\\');
            string fileName = attachment.Id;
            if (index > 0)
                fileName = fileName.Substring(index + 1);

            SaveFileDialog ofd = new SaveFileDialog();
            ofd.FileName = fileName;
            DialogResult dr = ofd.ShowDialog();

            if (dr == DialogResult.OK)
            {
                string saveFile = ofd.FileName;
                Program.GetMain().SetLoadingState();

                try
                {
                    MediaResponse rr;
                    using (PerfStatus.Start("DownloadMedia"))
                    {
                        rr = await this.client.ReadMediaAsync(attachment.MediaLink);
                    }
                    using (FileStream fileStream = File.Create(saveFile))
                    {
                        rr.Media.CopyTo(fileStream);
                    }
                    Program.GetMain().SetResultInBrowser(null, "It is saved to " + saveFile, true);
                }
                catch (Exception e)
                {
                    Program.GetMain().SetResultInBrowser(null, e.ToString(), true);
                }
            }
        }

        async void myMenuItemAttachmentFromFile_Click(object sender, EventArgs eventArg)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            DialogResult dr = ofd.ShowDialog();

            if (dr == DialogResult.OK)
            {
                var mime = new Mime();
                var contentType = mime.Lookup(ofd.FileName);
                string filename = ofd.FileName;

                Program.GetMain().SetLoadingState();

                try
                {

                    using (FileStream stream = new FileStream(filename,
                        FileMode.Open, FileAccess.Read))
                    {
                        MediaOptions mediaOptions = new MediaOptions()
                        {
                            ContentType = contentType,
                            Slug = Path.GetFileName(ofd.FileName)
                        };

                        ResourceResponse<Attachment> rr;
                        
                        Document document = ((Document)this.Tag);
                        DocumentCollection collection = ((DocumentCollection)this.Parent.Tag);

                        RequestOptions requestOptions = Program.GetMain().GetRequestOptions();
                        if (collection.PartitionKey != null && collection.PartitionKey.Paths.Count > 0)
                        {
                            requestOptions.PartitionKey = new PartitionKey(DocumentAnalyzer.ExtractPartitionKeyValue(document, collection.PartitionKey));
                        }

                        using (PerfStatus.Start("CreateAttachment"))
                        {
                            rr = await this.client.CreateAttachmentAsync((this.Tag as Document).SelfLink + "/attachments",
                                      stream, mediaOptions, requestOptions);
                        }

                        string json = rr.Resource.ToString();

                        Program.GetMain().SetResultInBrowser(json, null, false, rr.ResponseHeaders);

                        this.Nodes.Add(new ResourceNode(this.client, rr.Resource, ResourceType.Attachment));
                    }
                }
                catch (Exception e)
                {
                    Program.GetMain().SetResultInBrowser(null, e.ToString(), true);
                }
            }
        }

        void myMenuItemExecuteStoredProcedure_Click(object sender, EventArgs e)
        {
            Program.GetMain().SetCrudContext(this, OperationType.Execute, this.resourceType, 
                "Here is the input parameters to the storedProcedure. Input each parameter as one line without quotation mark.", this.ExecuteStoredProcedureAsync);
        }

        void myMenuItemDelete_Click(object sender, EventArgs e)
        {
            string x = this.Tag.ToString();
            CommandContext context = new CommandContext();
            context.IsDelete = true;
            Program.GetMain().SetCrudContext(this, OperationType.Delete, this.resourceType, x, this.DeleteResourceAsync, context);
        }

        async void CreateAttachmentAsync(object resource, RequestOptions requestOptions)
        {
            try
            {
                string text = resource as string;
                Attachment attachment = (Attachment)JsonConvert.DeserializeObject(text, typeof(Attachment));

                ResourceResponse<Attachment> rr;
                using (PerfStatus.Start("CreateAttachment"))
                {
                    rr = await this.client.CreateAttachmentAsync((this.Tag as Resource).GetLink(this.client),
                                        attachment, requestOptions);
                }
                string json = rr.Resource.ToString();

                Program.GetMain().SetResultInBrowser(json, null, false, rr.ResponseHeaders);

                this.Nodes.Add(new ResourceNode(this.client, rr.Resource, ResourceType.Attachment));
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

        async void ExecuteStoredProcedureAsync(object resource, RequestOptions requestOptions)
        {
            try
            {
                string text = resource as string;
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
                var dynamicInputParams = new dynamic[inputParamters.Count];
                for (var i = 0; i < inputParamters.Count; i++)
                {
                    var inputParamter = inputParamters[i];
                    var jTokenParam = JToken.Parse(inputParamter);
                    var dynamicParam = Helper.ConvertJTokenToDynamic(jTokenParam);
                    dynamicInputParams[i] = dynamicParam;
                }

                StoredProcedureResponse<dynamic> rr;
                using (PerfStatus.Start("ExecuateStoredProcedure"))
                {
                    rr = await this.client.ExecuteStoredProcedureAsync<dynamic>((this.Tag as Resource).GetLink(this.client),
                                      dynamicInputParams);
                }
                string executeResult = rr.Response.ToString();

                Program.GetMain().SetResultInBrowser(null, executeResult, true, rr.ResponseHeaders);

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

        async void ReplaceResourceAsync(object resource, RequestOptions requestOptions)
        {
            try
            {
                string json = null;
                if (this.resourceType == ResourceType.Document)
                {
                    string text = resource as string;
                    Document doc = (Document)JsonConvert.DeserializeObject(text, typeof(Document));
                    doc.SetReflectedPropertyValue("AltLink", (this.Tag as Document).GetAltLink());
                    ResourceResponse<Document> rr;
                    using (PerfStatus.Start("ReplaceDocument"))
                    {
                        rr = await this.client.ReplaceDocumentAsync(doc.GetLink(this.client), doc, requestOptions);
                    }
                    json = rr.Resource.ToString();

                    this.Tag = rr.Resource;
                    this.Text = rr.Resource.Id;
                    // set the result window
                    Program.GetMain().SetResultInBrowser(json, null, false, rr.ResponseHeaders);
                }
                else if (this.resourceType == ResourceType.StoredProcedure)
                {
                    StoredProcedure input = resource as StoredProcedure;
                    StoredProcedure sp = this.Tag as StoredProcedure;
                    sp.Body = input.Body;
                    if (!string.IsNullOrEmpty(input.Id)) { sp.Id = input.Id; }
                    ResourceResponse<StoredProcedure> rr;
                    using (PerfStatus.Start("ReplaceStoredProcedure"))
                    {
                        rr = await this.client.ReplaceStoredProcedureExAsync(sp, requestOptions);
                    }
                    json = rr.Resource.ToString();
                    this.Tag = rr.Resource;
                    this.Text = rr.Resource.Id;
                    // set the result window
                    Program.GetMain().SetResultInBrowser(json, null, false, rr.ResponseHeaders);
                }
                else if (this.resourceType == ResourceType.User)
                {
                    string text = resource as string;
                    User sp = (User)JsonConvert.DeserializeObject(text, typeof(User));
                    sp.SetReflectedPropertyValue("AltLink", (this.Tag as User).GetAltLink());
                    ResourceResponse<User> rr;
                    using (PerfStatus.Start("ReplaceUser"))
                    {
                        rr = await this.client.ReplaceUserExAsync(sp, requestOptions);
                    }
                    json = rr.Resource.ToString();
                    this.Tag = rr.Resource;
                    this.Text = rr.Resource.Id;
                    // set the result window
                    Program.GetMain().SetResultInBrowser(json, null, false, rr.ResponseHeaders);
                }
                else if (this.resourceType == ResourceType.Trigger)
                {
                    Trigger input = resource as Trigger;
                    Trigger sp = this.Tag as Trigger;
                    sp.Body = input.Body;
                    if (!string.IsNullOrEmpty(input.Id)) { sp.Id = input.Id; }
                    ResourceResponse<Trigger> rr;
                    using (PerfStatus.Start("ReplaceTrigger"))
                    {
                        rr = await this.client.ReplaceTriggerExAsync(sp, requestOptions);
                    }
                    json = rr.Resource.ToString();
                    this.Tag = rr.Resource;
                    this.Text = rr.Resource.Id;
                    // set the result window
                    Program.GetMain().SetResultInBrowser(json, null, false, rr.ResponseHeaders);
                }
                else if (this.resourceType == ResourceType.UserDefinedFunction)
                {
                    UserDefinedFunction input = resource as UserDefinedFunction;
                    UserDefinedFunction sp = this.Tag as UserDefinedFunction;
                    sp.Body = input.Body;
                    if (!string.IsNullOrEmpty(input.Id)) { sp.Id = input.Id; }
                    ResourceResponse<UserDefinedFunction> rr;
                    using (PerfStatus.Start("ReplaceUDF"))
                    {
                        rr = await this.client.ReplaceUserDefinedFunctionExAsync(sp, requestOptions);
                    }
                    json = rr.Resource.ToString();
                    this.Tag = rr.Resource;
                    this.Text = rr.Resource.Id;
                    // set the result window
                    Program.GetMain().SetResultInBrowser(json, null, false, rr.ResponseHeaders);
                }
                else if (this.resourceType == ResourceType.Permission)
                {
                    string text = resource as string;
                    Permission sp = Resource.LoadFrom<Permission>(new MemoryStream(Encoding.UTF8.GetBytes(text)));
                    sp.SetReflectedPropertyValue("AltLink", (this.Tag as Permission).GetAltLink());
                    ResourceResponse<Permission> rr;
                    using (PerfStatus.Start("ReplacePermission"))
                    {
                        rr = await this.client.ReplacePermissionExAsync(sp, requestOptions);
                    }
                    json = rr.Resource.ToString();
                    this.Tag = rr.Resource;
                    this.Text = rr.Resource.Id;
                    // set the result window
                    Program.GetMain().SetResultInBrowser(json, null, false, rr.ResponseHeaders);
                }
                else if (this.resourceType == ResourceType.Attachment)
                {
                    string text = resource as string;
                    Attachment sp = (Attachment)JsonConvert.DeserializeObject(text, typeof(Attachment));
                    sp.SetReflectedPropertyValue("AltLink", (this.Tag as Attachment).GetAltLink());
                    ResourceResponse<Attachment> rr;

                    Document document = ((Document)this.Parent.Tag);
                    DocumentCollection collection = ((DocumentCollection)this.Parent.Parent.Tag);

                    if (collection.PartitionKey != null && collection.PartitionKey.Paths.Count > 0)
                    {
                        requestOptions.PartitionKey = new PartitionKey(DocumentAnalyzer.ExtractPartitionKeyValue(document, collection.PartitionKey));
                    }

                    using (PerfStatus.Start("ReplaceAttachment"))
                    {
                        rr = await this.client.ReplaceAttachmentExAsync(sp, requestOptions);
                    }

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

        async void DeleteResourceAsync(object resource, RequestOptions requestOptions)
        {
            try
            {
                if (this.resourceType == ResourceType.Document)
                {
                    Document doc = (Document)this.Tag;
                    ResourceResponse<Document> rr;
                    DocumentCollection collection = ((DocumentCollection)this.Parent.Tag);

                    if (collection.PartitionKey != null && collection.PartitionKey.Paths.Count > 0)
                    {
                        requestOptions.PartitionKey = new PartitionKey(DocumentAnalyzer.ExtractPartitionKeyValue(doc, collection.PartitionKey));
                    }

                    using (PerfStatus.Start("DeleteDocument"))
                    {
                        rr = await this.client.DeleteDocumentAsync(doc.GetLink(this.client), requestOptions);
                    }

                    Program.GetMain().SetResultInBrowser(null, "Delete Document succeed!", false, rr.ResponseHeaders);
                }
                else if (this.resourceType == ResourceType.StoredProcedure)
                {
                    StoredProcedure sp = (StoredProcedure)this.Tag;
                    ResourceResponse<StoredProcedure> rr;
                    using (PerfStatus.Start("DeleteStoredProcedure"))
                    {
                        rr = await this.client.DeleteStoredProcedureAsync(sp.GetLink(this.client), requestOptions);
                    }
                    Program.GetMain().SetResultInBrowser(null, "Delete StoredProcedure succeed!", false, rr.ResponseHeaders);
                }
                else if (this.resourceType == ResourceType.User)
                {
                    User sp = (User)this.Tag;
                    ResourceResponse<User> rr;
                    using (PerfStatus.Start("DeleteUser"))
                    {
                        rr = await this.client.DeleteUserAsync(sp.GetLink(this.client), requestOptions);
                    }
                    Program.GetMain().SetResultInBrowser(null, "Delete User succeed!", false, rr.ResponseHeaders);
                }
                else if (this.resourceType == ResourceType.Trigger)
                {
                    Trigger sp = (Trigger)this.Tag;
                    ResourceResponse<Trigger> rr;
                    using (PerfStatus.Start("DeleteTrigger"))
                    {
                        rr = await this.client.DeleteTriggerAsync(sp.GetLink(this.client), requestOptions);
                    }
                    Program.GetMain().SetResultInBrowser(null, "Delete Trigger succeed!", false, rr.ResponseHeaders);
                }
                else if (this.resourceType == ResourceType.UserDefinedFunction)
                {
                    UserDefinedFunction sp = (UserDefinedFunction)this.Tag;
                    ResourceResponse<UserDefinedFunction> rr;
                    using (PerfStatus.Start("DeleteUDF"))
                    {
                        rr = await this.client.DeleteUserDefinedFunctionAsync(sp.GetLink(this.client), requestOptions);
                    }
                    Program.GetMain().SetResultInBrowser(null, "Delete UserDefinedFunction succeed!", false, rr.ResponseHeaders);
                }
                else if (this.resourceType == ResourceType.Permission)
                {
                    Permission sp = (Permission)this.Tag;
                    ResourceResponse<Permission> rr;
                    using (PerfStatus.Start("DeletePermission"))
                    {
                        rr = await this.client.DeletePermissionAsync(sp.GetLink(this.client), requestOptions);
                    }
                    Program.GetMain().SetResultInBrowser(null, "Delete Permission succeed!", false, rr.ResponseHeaders);
                }
                else if (this.resourceType == ResourceType.Attachment)
                {
                    Attachment sp = (Attachment)this.Tag;
                    ResourceResponse<Attachment> rr;

                    Document document = ((Document)this.Parent.Tag);
                    DocumentCollection collection = ((DocumentCollection)this.Parent.Parent.Tag);

                    if (collection.PartitionKey != null && collection.PartitionKey.Paths.Count > 0)
                    {
                        requestOptions.PartitionKey = new PartitionKey(DocumentAnalyzer.ExtractPartitionKeyValue(document, collection.PartitionKey));
                    }

                    using (PerfStatus.Start("DeleteAttachment"))
                    {
                        rr = await this.client.DeleteAttachmentAsync(sp.GetLink(this.client), requestOptions);
                    }

                    Program.GetMain().SetResultInBrowser(null, "Delete Attachment succeed!", false, rr.ResponseHeaders);
                }
                else if (this.resourceType == ResourceType.Conflict)
                {
                    Conflict sp = (Conflict)this.Tag;
                    ResourceResponse<Conflict> rr;
                    using (PerfStatus.Start("DeleteConlict"))
                    {
                        rr = await this.client.DeleteConflictAsync(sp.GetLink(this.client), requestOptions);
                    }
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
                    this.Nodes.Add(new PermissionNode(this.client));
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
                body = "\nThe storedprocedure Javascript function: \n\n" + (this.Tag as StoredProcedure).Body;
            }
            else if (this.resourceType == ResourceType.Trigger)
            {
                body = "\nThe trigger Javascript function: \n\n" + (this.Tag as Trigger).Body;
            }
            else if (this.resourceType == ResourceType.UserDefinedFunction)
            {
                body = "\nThe stored Javascript function: \n\n" + (this.Tag as UserDefinedFunction).Body;
            }
            return body;
        }

        public void FillWithChildren()
        {
            try
            {
                Document document = ((Document)this.Tag);
                DocumentCollection collection = ((DocumentCollection)this.Parent.Tag);

                FeedOptions options = new FeedOptions();
                if (collection.PartitionKey != null && collection.PartitionKey.Paths.Count > 0)
                {
                    options.PartitionKey = new PartitionKey(DocumentAnalyzer.ExtractPartitionKeyValue(document, collection.PartitionKey));
                }

                FeedResponse<Attachment> attachments;
                using (PerfStatus.Start("ReadAttachmentFeed"))
                {
                    attachments = this.client.ReadAttachmentFeedAsync(document.GetLink(this.client), options).Result;
                }

                foreach (var attachment in attachments)
                {
                    ResourceNode node = new ResourceNode(client, attachment, ResourceType.Attachment);
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

    class StoredProcedureNode : FeedNode
    {
        private DocumentClient client;
        private ContextMenu contextMenu = new ContextMenu();

        public StoredProcedureNode(DocumentClient client)
        {
            this.Text = "StoredProcedures";
            this.client = client;
            this.Nodes.Add(new TreeNode("fake"));
            this.Tag = "This represents the StoredProcedure feed. Right click to add StoredProcedure";

            this.ImageKey = "Feed";
            this.SelectedImageKey = "Feed";

            {
                MenuItem menuItem = new MenuItem("Create StoredProcedure");
                menuItem.Click += new EventHandler(myMenuItemAddStoredProcedure_Click);
                this.contextMenu.MenuItems.Add(menuItem);
            }
            {
                MenuItem menuItem = new MenuItem("Create StoredProcedure From File");
                menuItem.Click += new EventHandler(myMenuItemCreateStoredProcedureFromFile_Click);
                this.contextMenu.MenuItems.Add(menuItem);
            }
            {
                MenuItem menuItem = new MenuItem("Refresh StoredProcedures feed");
                menuItem.Click += new EventHandler((sender, e) => Refresh(true));
                this.contextMenu.MenuItems.Add(menuItem);
            }
        }

        void myMenuItemAddStoredProcedure_Click(object sender, EventArgs e)
        {
            // 
            Program.GetMain().SetCrudContext(this, OperationType.Create, ResourceType.StoredProcedure,
                "function() { \r\n \r\n}", this.CreateStoredProcedureAsync);
        }

        void myMenuItemCreateStoredProcedureFromFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            DialogResult dr = ofd.ShowDialog();

            if (dr == DialogResult.OK)
            {
                string filename = ofd.FileName;
                // 
                string text = System.IO.File.ReadAllText(filename);

                Program.GetMain().SetCrudContext(this, OperationType.Create, ResourceType.StoredProcedure, text, this.CreateStoredProcedureAsync);
            }
        }

        async void CreateStoredProcedureAsync(object resource, RequestOptions requestOptions)
        {
            try
            {
                StoredProcedure sp = resource as StoredProcedure;
                ResourceResponse<StoredProcedure> newsp;
                using (PerfStatus.Start("CreateStoredProcedure"))
                {
                    newsp = await this.client.CreateStoredProcedureAsync((this.Parent.Tag as DocumentCollection).GetLink(this.client), sp, requestOptions);
                }

                this.Nodes.Add(new ResourceNode(this.client, newsp.Resource, ResourceType.StoredProcedure));

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
                FeedResponse<StoredProcedure> sps;
                using (PerfStatus.Start("ReadStoredProcedure"))
                {
                    sps = this.client.ReadStoredProcedureFeedAsync((collnode.Tag as DocumentCollection).GetLink(this.client)).Result;
                }

                foreach (var sp in sps)
                {
                    ResourceNode node = new ResourceNode(client, sp, ResourceType.StoredProcedure);
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

    class UserDefinedFunctionNode : FeedNode
    {
        private DocumentClient client;
        private ContextMenu contextMenu = new ContextMenu();

        public UserDefinedFunctionNode(DocumentClient client)
        {
            this.Text = "UDFs";
            this.client = client;
            this.Nodes.Add(new TreeNode("fake"));
            this.Tag = "This represents the UserDefinedFunction feed. Right click to add UserDefinedFunction";
            this.ImageKey = "Feed";
            this.SelectedImageKey = "Feed";

            {
                MenuItem menuItem = new MenuItem("Create UserDefinedFunction");
                menuItem.Click += new EventHandler(myMenuItemCreateUDF_Click);
                this.contextMenu.MenuItems.Add(menuItem);
            }
            {
                MenuItem menuItem = new MenuItem("Create UserDefinedFunction from File");
                menuItem.Click += new EventHandler(myMenuItemCreateUDFFromFile_Click);
                this.contextMenu.MenuItems.Add(menuItem);
            }
            {
                MenuItem menuItem = new MenuItem("Refresh UserDefinedFunction feed");
                menuItem.Click += new EventHandler((sender, e) => Refresh(true));
                this.contextMenu.MenuItems.Add(menuItem);
            }
        }

        void myMenuItemCreateUDF_Click(object sender, EventArgs e)
        {
            // 
            Program.GetMain().SetCrudContext(this, OperationType.Create, ResourceType.UserDefinedFunction,
                "function() { \r\n \r\n}", this.CreateUDFAsync);
        }

        void myMenuItemCreateUDFFromFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            DialogResult dr = ofd.ShowDialog();

            if (dr == DialogResult.OK)
            {
                string filename = ofd.FileName;
                // 
                string text = System.IO.File.ReadAllText(filename);

                Program.GetMain().SetCrudContext(this, OperationType.Create, ResourceType.UserDefinedFunction, text, this.CreateUDFAsync);
            }
        }
        async void CreateUDFAsync(object resource, RequestOptions requestOptions)
        {
            try
            {
                UserDefinedFunction udf = resource as UserDefinedFunction;

                ResourceResponse<UserDefinedFunction> newudf;
                using (PerfStatus.Start("CreateUDF"))
                {
                    newudf = await this.client.CreateUserDefinedFunctionAsync((this.Parent.Tag as DocumentCollection).GetLink(this.client), udf, requestOptions);
                }

                this.Nodes.Add(new ResourceNode(this.client, newudf.Resource, ResourceType.UserDefinedFunction));

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
                FeedResponse<UserDefinedFunction> sps;
                using (PerfStatus.Start("ReadUdfFeed"))
                {
                    sps = this.client.ReadUserDefinedFunctionFeedAsync((collnode.Tag as DocumentCollection).GetLink(this.client)).Result;
                }

                foreach (var sp in sps)
                {
                    ResourceNode node = new ResourceNode(client, sp, ResourceType.UserDefinedFunction);
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

    class TriggerNode : FeedNode
    {
        private DocumentClient client;
        private ContextMenu contextMenu = new ContextMenu();

        public TriggerNode(DocumentClient client)
        {
            this.Text = "Triggers";
            this.client = client;
            this.Nodes.Add(new TreeNode("fake"));
            this.Tag = "This represents the Triggers feed. Right click to add Trigger";
            this.ImageKey = "Feed";
            this.SelectedImageKey = "Feed";

            {
                MenuItem menuItem = new MenuItem("Create Trigger");
                menuItem.Click += new EventHandler(myMenuItemCreateTrigger_Click);
                this.contextMenu.MenuItems.Add(menuItem);
            }
            {
                MenuItem menuItem = new MenuItem("Create Trigger from file");
                menuItem.Click += new EventHandler(myMenuItemCreateTriggerFromFile_Click);
                this.contextMenu.MenuItems.Add(menuItem);
            }
            {
                MenuItem menuItem = new MenuItem("Refresh Triggers feed");
                menuItem.Click += new EventHandler((sender, e) => Refresh(true));
                this.contextMenu.MenuItems.Add(menuItem);
            }
        }

        void myMenuItemCreateTriggerFromFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            DialogResult dr = ofd.ShowDialog();

            if (dr == DialogResult.OK)
            {
                string filename = ofd.FileName;
                // 
                string text = System.IO.File.ReadAllText(filename);

                Program.GetMain().SetCrudContext(this, OperationType.Create, ResourceType.Trigger, text, this.CreateTriggerAsync, new CommandContext() { IsCreateTrigger = true });
            }
        }

        void myMenuItemCreateTrigger_Click(object sender, EventArgs e)
        {
            // 
            Program.GetMain().SetCrudContext(this, OperationType.Create, ResourceType.Trigger,
                "function() { \r\n \r\n}", this.CreateTriggerAsync, new CommandContext() { IsCreateTrigger = true });
        }

        async void CreateTriggerAsync(object triggerObject, RequestOptions requestOptions)
        {
            try
            {
                Trigger trigger = triggerObject as Trigger;

                ResourceResponse<Trigger> newtrigger;
                using (PerfStatus.Start("CreateTrigger"))
                {
                    newtrigger = await this.client.CreateTriggerAsync((this.Parent.Tag as DocumentCollection).GetLink(this.client), trigger, requestOptions);
                }

                this.Nodes.Add(new ResourceNode(this.client, newtrigger.Resource, ResourceType.Trigger));

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
                FeedResponse<Trigger> sps;
                using (PerfStatus.Start("ReadTriggerFeed"))
                {
                    sps = this.client.ReadTriggerFeedAsync((collnode.Tag as DocumentCollection).GetLink(this.client)).Result;
                }

                foreach (var sp in sps)
                {
                    ResourceNode node = new ResourceNode(client, sp, ResourceType.Trigger);
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

    class UserNode : FeedNode
    {
        private DocumentClient client;
        private ContextMenu contextMenu = new ContextMenu();

        public UserNode(DocumentClient client)
        {
            this.Text = "Users";
            this.client = client;
            this.Nodes.Add(new TreeNode("fake"));
            this.Tag = "This represents the Users feed. Right click to add user";
            this.ImageKey = "User";
            this.SelectedImageKey = "User";

            {
                MenuItem menuItem = new MenuItem("Create User");
                menuItem.Click += new EventHandler(myMenuItemCreateUser_Click);
                this.contextMenu.MenuItems.Add(menuItem);
            }
            {
                MenuItem menuItem = new MenuItem("Refresh Users feed");
                menuItem.Click += new EventHandler((sender, e) => Refresh(true));
                this.contextMenu.MenuItems.Add(menuItem);
            }
        }

        void myMenuItemCreateUser_Click(object sender, EventArgs e)
        {
            dynamic d = new System.Dynamic.ExpandoObject();
            d.id = "Here is your user Id";
            string x = JsonConvert.SerializeObject(d, Newtonsoft.Json.Formatting.Indented);
            Program.GetMain().SetCrudContext(this, OperationType.Create, ResourceType.User, x, this.CreateUserAsync);
        }

        async void CreateUserAsync(object resource, RequestOptions requestOptions)
        {
            try
            {
                User user = (User)JsonConvert.DeserializeObject(resource as string, typeof(User));

                ResourceResponse<User> newUser;
                using (PerfStatus.Start("CreateUser"))
                {
                    newUser = await this.client.CreateUserAsync((this.Parent.Tag as Database).GetLink(this.client), user, requestOptions);
                }
                this.Nodes.Add(new ResourceNode(this.client, newUser.Resource, ResourceType.User));

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
                FeedResponse<User> sps;
                using (PerfStatus.Start("ReadUser"))
                {
                    sps = this.client.ReadUserFeedAsync((this.Parent.Tag as Database).GetLink(this.client)).Result;
                }
                foreach (var sp in sps)
                {
                    ResourceNode node = new ResourceNode(client, sp, ResourceType.User);
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

    class PermissionNode : FeedNode
    {
        private DocumentClient client;
        private ContextMenu contextMenu = new ContextMenu();

        public PermissionNode(DocumentClient client)
        {
            this.Text = "Permissions";
            this.client = client;
            this.Nodes.Add(new TreeNode("fake"));
            this.Tag = "This represents the Permissions feed. Right click to add permission";
            this.ImageKey = "Permission";
            this.SelectedImageKey = "Permission";

            {
                MenuItem menuItem = new MenuItem("Create Permission");
                menuItem.Click += new EventHandler(myMenuItemCreatePermission_Click);
                this.contextMenu.MenuItems.Add(menuItem);
            }
            {
                MenuItem menuItem = new MenuItem("Refresh Permissions feed");
                menuItem.Click += new EventHandler((sender, e) => Refresh(true));
                this.contextMenu.MenuItems.Add(menuItem);
            }
        }

        void myMenuItemCreatePermission_Click(object sender, EventArgs e)
        {
            Permission permission = new Permission();
            permission.Id = "Here is your permission Id";
            permission.PermissionMode = PermissionMode.Read;
            permission.ResourceLink = "your resource link";

            string x = permission.ToString();

            Program.GetMain().SetCrudContext(this, OperationType.Create, ResourceType.Permission, x, this.CreatePermissionAsync);
        }

        async void CreatePermissionAsync(object resource, RequestOptions requestOptions)
        {
            try
            {
                Permission permission = Resource.LoadFrom<Permission>(new MemoryStream(Encoding.UTF8.GetBytes(resource as string)));

                ResourceResponse<Permission> newtpermission;
                using (PerfStatus.Start("CreatePermission"))
                {
                    newtpermission = await this.client.CreatePermissionAsync((this.Parent.Tag as Resource).GetLink(this.client), permission, requestOptions);
                }
                this.Nodes.Add(new ResourceNode(this.client, newtpermission.Resource, ResourceType.Permission));

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
                FeedResponse<Permission> sps;
                using (PerfStatus.Start("ReadPermission"))
                {
                    sps = this.client.ReadPermissionFeedAsync((this.Parent.Tag as User).GetLink(this.client)).Result;
                }

                foreach (var sp in sps)
                {
                    ResourceNode node = new ResourceNode(client, sp, ResourceType.Permission);
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

    class ConflictNode : FeedNode
    {
        private DocumentClient client;
        private ContextMenu contextMenu = new ContextMenu();

        public ConflictNode(DocumentClient client)
        {
            this.Text = "Conflicts";
            this.client = client;
            this.Nodes.Add(new TreeNode("fake"));
            this.Tag = "This represents the Conflicts feed.";
            this.ImageKey = "Conflict";
            this.SelectedImageKey = "Conflict";

            {
                MenuItem menuItem = new MenuItem("Refresh Conflict feed");
                menuItem.Click += new EventHandler((sender, e) => Refresh(true));
                this.contextMenu.MenuItems.Add(menuItem);
            }
            {
                // Query conflicts currrently fail due to gateway
                MenuItem menuItem = new MenuItem("Query Conflict feed");
                menuItem.Click += new EventHandler(myMenuItemQueryConflicts_Click);
                this.contextMenu.MenuItems.Add(menuItem);
            }
        }

        override public void ShowContextMenu(TreeView treeview, Point p)
        {
            this.contextMenu.Show(treeview, p);
        }

        void myMenuItemQueryConflicts_Click(object sender, EventArgs e)
        {
            Program.GetMain().SetCrudContext(this, OperationType.Query, ResourceType.Conflict, "select * from c", this.QueryConflictAsync);
        }

        async void QueryConflictAsync(object resource, RequestOptions requestOptions)
        {
            try
            {
                string queryText = resource as string;
                // text is the querytext.
                FeedResponse<Database> r;
                using (PerfStatus.Start("QueryConflicts"))
                {
                    IDocumentQuery<dynamic> q = this.client.CreateConflictQuery((this.Parent.Tag as DocumentCollection).GetLink(this.client), queryText).AsDocumentQuery();
                    r = await q.ExecuteNextAsync<Database>();
                }

                // set the result window
                string text = null;
                if (r.Count > 1)
                {
                    text = string.Format(CultureInfo.InvariantCulture, "Returned {0} Conflict", r.Count);
                }
                else
                {
                    text = string.Format(CultureInfo.InvariantCulture, "Returned {0} Conflict", r.Count);
                }

                string jsonarray = "[";
                int index = 0;
                foreach (dynamic d in r)
                {
                    index++;
                    // currently Query.ToString() has Formatting.Indented, but the public release doesn't have yet.
                    jsonarray += d.ToString();

                    if (index == r.Count)
                    {
                        jsonarray += "]";
                    }
                    else
                    {
                        jsonarray += ",\r\n";
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
                FeedResponse<Conflict> feedConflicts;
                using (PerfStatus.Start("ReadConflictsFeed"))
                {
                    feedConflicts = this.client.ReadConflictFeedAsync((this.Parent.Tag as DocumentCollection).GetLink(this.client)).Result;
                }

                foreach (var sp in feedConflicts)
                {
                    ResourceNode node = new ResourceNode(client, sp, ResourceType.Conflict);
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

    class OfferNode : FeedNode
    {
        private DocumentClient client;
        private ContextMenu contextMenu = new ContextMenu();

        public OfferNode(DocumentClient client)
        {
            this.Text = "Offers";
            this.client = client;
            this.Nodes.Add(new TreeNode("fake"));
            this.Tag = "This represents the Offers feed.";
            this.ImageKey = "Offer";
            this.SelectedImageKey = "Offer";

            MenuItem menuItem = new MenuItem("Refresh Offer feed");
            menuItem.Click += new EventHandler((sender, e) => Refresh(true));
            this.contextMenu.MenuItems.Add(menuItem);
        }

        override public void ShowContextMenu(TreeView treeview, Point p)
        {
            this.contextMenu.Show(treeview, p);
        }

        void myMenuItemQueryOffers_Click(object sender, EventArgs e)
        {
            Program.GetMain().SetCrudContext(this, OperationType.Query, ResourceType.Offer, "select * from c", this.QueryOfferAsync);
        }

        async void QueryOfferAsync(object resource, RequestOptions requestOptions)
        {
            try
            {
                string queryText = resource as string;
                // text is the querytext.
                IDocumentQuery<dynamic> q = this.client.CreateOfferQuery(queryText).AsDocumentQuery();

                FeedResponse<Database> r;
                using (PerfStatus.Start("QueryOffer"))
                {
                    r = await q.ExecuteNextAsync<Database>();
                }
                // set the result window
                string text = null;
                if (r.Count > 1)
                {
                    text = string.Format(CultureInfo.InvariantCulture, "Returned {0} Offers", r.Count);
                }
                else
                {
                    text = string.Format(CultureInfo.InvariantCulture, "Returned {0} Offer", r.Count);
                }

                string jsonarray = "[";
                int index = 0;
                foreach (dynamic d in r)
                {
                    index++;
                    // currently Query.ToString() has Formatting.Indented, but the public release doesn't have yet.
                    jsonarray += d.ToString();

                    if (index == r.Count)
                    {
                        jsonarray += "]";
                    }
                    else
                    {
                        jsonarray += ",\r\n";
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
                FeedResponse<Offer> feedOffers = this.client.ReadOffersFeedAsync().Result;

                foreach (var sp in feedOffers)
                {
                    ResourceNode node = new ResourceNode(client, sp, ResourceType.Offer);
                    this.Nodes.Add(node);
                }
                Program.GetMain().SetResponseHeaders(feedOffers.ResponseHeaders);

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