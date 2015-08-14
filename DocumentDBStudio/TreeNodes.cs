//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using Newtonsoft.Json.Linq;
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
    using Newtonsoft.Json.Serialization;
    using System.Globalization;

    internal class AccountSettings
    {
        public string MasterKey;
        public ConnectionMode ConnectionMode;
        public Protocol Protocol;
        public bool IsNameBased;
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
        private ContextMenu contextMenu = new ContextMenu();

        public DatabaseAccountNode(string endpointName, DocumentClient client)
        {
            this.accountEndpoint = endpointName;

            this.Text = endpointName;

            this.ImageKey = "DatabaseAccount";
            this.SelectedImageKey = "DatabaseAccount";

            this.client = client;
            this.Tag = "This represents the DatabaseAccount. Right click to add Database";

            this.Nodes.Add(new OffersNode(this.client));

            MenuItem myMenuItem = new MenuItem("Create Database");
            myMenuItem.Click += new EventHandler(myMenuItemAddDatabase_Click);
            this.contextMenu.MenuItems.Add(myMenuItem);

            MenuItem myMenuItem1 = new MenuItem("Refresh Databases feed");
            myMenuItem1.Click += new EventHandler((sender, e) => Refresh(true));
            this.contextMenu.MenuItems.Add(myMenuItem1);

            MenuItem myMenuItem4 = new MenuItem("Query Database");
            myMenuItem4.Click += new EventHandler(myMenuItemQueryDatabase_Click);
            this.contextMenu.MenuItems.Add(myMenuItem4);

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
            Program.GetMain().SetCrudContext(this, "Create database", false, x, this.AddDatabase);
        }

        void myMenuItemQueryDatabase_Click(object sender, EventArgs e)
        {
            Program.GetMain().SetCrudContext(this, "Query Database",
                false, "select * from c", this.QueryDatabases);
        }

        void myMenuItemRemoveDatabaseAccount_Click(object sender, EventArgs e)
        {
            // 
            this.Remove();
            Program.GetMain().RemoveAccountFromSettings(this.accountEndpoint);
        }

        async void QueryDatabases(string queryText, object optional)
        {
            try
            {
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
                this.Tag = await client.GetDatabaseAccountAsync();

                FeedResponse<Documents.Database> databases;
                using (PerfStatus.Start("ReadDatabaseFeed"))
                {
                    databases = await this.client.ReadDatabaseFeedAsync();
                }

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

                this.Nodes.Add(new OffersNode(this.client));

                FillWithChildren();
            }
        }

        async void AddDatabase(string text, object optional)
        {
            try
            {
                Documents.Database db = (Documents.Database)JsonConvert.DeserializeObject(text, typeof(Documents.Database));

                ResourceResponse<Documents.Database> newdb;
                using (PerfStatus.Start("CreateDatabase"))
                {
                     newdb = await this.client.CreateDatabaseAsync(db, Program.GetMain().GetRequestOptions());
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

        public DatabaseNode(DocumentClient localclient, Documents.Database db)
        {
            this.Text = db.Id;
            this.Tag = db;
            this.client = localclient;
            this.ImageKey = "SystemFeed";
            this.SelectedImageKey = "SystemFeed";

            this.Nodes.Add(new UsersNode(this.client));

            MenuItem myMenuItem3 = new MenuItem("Read Database");
            myMenuItem3.Click += new EventHandler(myMenuItemReadDatabase_Click);
            this.contextMenu.MenuItems.Add(myMenuItem3);

            MenuItem myMenuItem = new MenuItem("Delete Database");
            myMenuItem.Click += new EventHandler(myMenuItemDeleteDatabase_Click);
            this.contextMenu.MenuItems.Add(myMenuItem);

            this.contextMenu.MenuItems.Add("-");

            MenuItem myMenuItem2 = new MenuItem("Create DocumentCollection");
            myMenuItem2.Click += new EventHandler(myMenuItemAddDocumentCollection_Click);
            this.contextMenu.MenuItems.Add(myMenuItem2);
            MenuItem myMenuItem4 = new MenuItem("Refresh DocumentCollections Feed");
            myMenuItem4.Click += new EventHandler((sender, e) => Refresh(true));
            this.contextMenu.MenuItems.Add(myMenuItem4);

        }
        public DocumentClient DocumentClient
        {
            get { return this.client; }
        }

        async void myMenuItemReadDatabase_Click(object sender, EventArgs eArgs)
        {
            try
            {
                ResourceResponse<Documents.Database> database;
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

        void myMenuItemDeleteDatabase_Click(object sender, EventArgs e)
        {
            string x = this.Tag.ToString();
            CommandContext context = new CommandContext();
            context.IsDelete = true;
            Program.GetMain().SetCrudContext(this, "Delete database", false, x, this.DeleteDatabase, context);
        }

        void myMenuItemAddDocumentCollection_Click(object sender, EventArgs e)
        {
            dynamic d = new System.Dynamic.ExpandoObject();
            d.id = "Here is your DocumentCollection Id";

            string x = JsonConvert.SerializeObject(d, Newtonsoft.Json.Formatting.Indented);
            Program.GetMain().SetCrudContext(this, "Create documentCollection", false, x, this.AddDocumentCollection);
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

        async void AddDocumentCollection(string text, object optional)
        {
            try
            {
                DocumentCollection coll = optional as DocumentCollection;
                Documents.Database db = (Documents.Database)this.Tag;
                ResourceResponse<Documents.DocumentCollection> newcoll;
                using (PerfStatus.Start("CreateDocumentCollection"))
                {
                    newcoll = await this.client.CreateDocumentCollectionAsync(db.GetLink(this.client), coll, Program.GetMain().GetRequestOptions(true));
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

        async void DeleteDatabase(string text, object optional)
        {
            try
            {
                Documents.Database db = (Documents.Database)this.Tag;
                ResourceResponse<Documents.Database> newdb;
                using (PerfStatus.Start("DeleteDatabase"))
                {
                    newdb = await this.client.DeleteDatabaseAsync(db.GetLink(this.client), Program.GetMain().GetRequestOptions());
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
                FeedResponse<Documents.DocumentCollection> colls;
                using (PerfStatus.Start("ReadDocumentCollectionFeed"))
                {
                    colls = await this.client.ReadDocumentCollectionFeedAsync(((Documents.Database)this.Tag).GetLink(this.client));
                }

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
        private string currentContinuation = null;
        private CommandContext currentQueryCommandContext = null;
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

            MenuItem myMenuItem5 = new MenuItem("Read DocumentCollection");
            myMenuItem5.Click += new EventHandler(myMenuItemReadDocumentCollection_Click);
            this.contextMenu.MenuItems.Add(myMenuItem5);

            MenuItem myMenuItem3 = new MenuItem("Replace DocumentCollection");
            myMenuItem3.Click += new EventHandler(myMenuItemUpdateDocumentCollection_Click);
            this.contextMenu.MenuItems.Add(myMenuItem3);

            MenuItem myMenuItem6 = new MenuItem("Delete DocumentCollection");
            myMenuItem6.Click += new EventHandler(myMenuItemDeleteDocumentCollection_Click);
            this.contextMenu.MenuItems.Add(myMenuItem6);

            this.contextMenu.MenuItems.Add("-");

            MenuItem myMenuItem = new MenuItem("Create Document");
            myMenuItem.Click += new EventHandler(myMenuItemAddDocument_Click);
            this.contextMenu.MenuItems.Add(myMenuItem);
            MenuItem myMenuItem9 = new MenuItem("Create Document From File");
            myMenuItem9.Click += new EventHandler(myMenuItemAddDocumentFromFile_Click);
            this.contextMenu.MenuItems.Add(myMenuItem9);
            MenuItem myMenuItem4 = new MenuItem("Create Multiple Documents From Folder");
            myMenuItem4.Click += new EventHandler(myMenuItemAddDocumentsFromFolder_Click);
            this.contextMenu.MenuItems.Add(myMenuItem4);
            MenuItem myMenuItem1 = new MenuItem("Refresh Documents feed");
            myMenuItem1.Click += new EventHandler((sender, e) => Refresh(true));
            this.contextMenu.MenuItems.Add(myMenuItem1);
            MenuItem myMenuItem2 = new MenuItem("Query Documents");
            myMenuItem2.Click += new EventHandler(myMenuItemQueryDocument_Click);
            this.contextMenu.MenuItems.Add(myMenuItem2);
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
            Program.GetMain().SetCrudContext(this, "Delete DocumentCollection", false, x, this.DeleteDocumentCollection, context);
        }

        void myMenuItemUpdateDocumentCollection_Click(object sender, EventArgs e)
        {
            string x = this.Tag.ToString();
            CommandContext context = new CommandContext();
            Program.GetMain().SetCrudContext(this, "Replace DocumentCollection", false, x, this.UpdateDocumentCollection, context);
        }

        async void UpdateDocumentCollection(string text, object optional)
        {
            try
            {
                RequestOptions requestionOptions = Program.GetMain().GetRequestOptions(true);

                // #1: Update offer if necessary
                Documents.DocumentCollection coll = (Documents.DocumentCollection)this.Tag;

                // Find the offer object corresponding to the current offer.
                IQueryable<Offer> offerQuery = from offer in client.CreateOfferQuery()
                                               where offer.ResourceLink == coll.SelfLink
                                               select offer;
                IDocumentQuery<Offer> offerDocDBQuery = offerQuery.AsDocumentQuery();

                List<Offer> queryResults = new List<Offer>();

                while (offerDocDBQuery.HasMoreResults)
                {
                    queryResults.AddRange(await offerDocDBQuery.ExecuteNextAsync<Offer>());
                }

                // change the Offer type of the document collection 
                Offer offerToReplace = queryResults[0];
                if (requestionOptions.OfferType != null && string.Compare(offerToReplace.OfferType, requestionOptions.OfferType, StringComparison.Ordinal) != 0)
                {
                    offerToReplace.OfferType = requestionOptions.OfferType;
                    ResourceResponse<Offer> replaceResponse;
                    using (PerfStatus.Start("ReplaceOffer"))
                    {
                        replaceResponse = await client.ReplaceOfferAsync(offerToReplace);
                    }
                }

                // #2: Update collection if necessary
                DocumentCollection collToChange = optional as DocumentCollection;
                collToChange.IndexingPolicy = (IndexingPolicy)coll.IndexingPolicy.Clone();

                ResourceResponse<DocumentCollection> response;
                using (PerfStatus.Start("ReplaceDocumentCollection"))
                {
                    response = await client.ReplaceDocumentCollectionExAsync(coll, requestionOptions);
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
        async void DeleteDocumentCollection(string text, object optional)
        {
            try
            {
                Documents.DocumentCollection coll = (Documents.DocumentCollection)this.Tag;
                ResourceResponse<Documents.DocumentCollection> newcoll;
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

        void myMenuItemAddDocumentFromFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            DialogResult dr = ofd.ShowDialog();

            if (dr == DialogResult.OK)
            {
                string filename = ofd.FileName;
                // 
                string text = System.IO.File.ReadAllText(filename);

                Program.GetMain().SetCrudContext(this, "Create document", false, text, this.AddDocument);
            }
        }

        async void myMenuItemAddDocumentsFromFolder_Click(object sender, EventArgs e)
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
                            ResourceResponse<Documents.Document> newdocument = await this.client.CreateDocumentAsync((this.Tag as Documents.DocumentCollection).GetLink(this.client), document, Program.GetMain().GetRequestOptions());
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

        void myMenuItemAddDocument_Click(object sender, EventArgs e)
        {
            // 
            dynamic d = new System.Dynamic.ExpandoObject();
            d.id = "Here is your Document Id";
            string x = JsonConvert.SerializeObject(d, Newtonsoft.Json.Formatting.Indented);
            Program.GetMain().SetCrudContext(this, "Create document", false, x, this.AddDocument);
        }


        void myMenuItemQueryDocument_Click(object sender, EventArgs e)
        {
            this.currentQueryCommandContext = new CommandContext();
            this.currentQueryCommandContext.IsFeed = true;

            // reset continuation token
            this.currentContinuation = null;

            Program.GetMain().SetCrudContext(this, string.Format(CultureInfo.InvariantCulture, "Query Documents from Collection {0}", (this.Tag as Documents.DocumentCollection).Id),
                false, "select * from c", this.QueryDocuments, this.currentQueryCommandContext);

        }


        async void AddDocument(string text, object optional)
        {
            try
            {
                object document = JsonConvert.DeserializeObject(text);

                ResourceResponse<Documents.Document> newdocument;
                using (PerfStatus.Start("CreateDocument"))
                {
                    newdocument = await this.client.CreateDocumentAsync((this.Tag as Documents.DocumentCollection).GetLink(this.client), document, Program.GetMain().GetRequestOptions());
                }

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

        async void QueryDocuments(string queryText, object optional)
        {
            try
            {
                // text is the querytext.
                IDocumentQuery<dynamic> q = null;

                FeedOptions feedOptions = Program.GetMain().GetFeedOptions();

                if (!string.IsNullOrEmpty(this.currentContinuation) && string.IsNullOrEmpty(feedOptions.RequestContinuation))
                {
                    feedOptions.RequestContinuation = this.currentContinuation;
                }

                q = this.client.CreateDocumentQuery((this.Tag as Documents.DocumentCollection).GetLink(this.client), queryText, feedOptions).AsDocumentQuery();

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
                    text = string.Format(CultureInfo.InvariantCulture, "Returned {0} documents in {1} ms", r.Count, sw.ElapsedMilliseconds);
                }
                else
                {
                    text = string.Format(CultureInfo.InvariantCulture, "Returned {0} document in {1} ms", r.Count, sw.ElapsedMilliseconds);
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
                FeedResponse<dynamic> docs;
                using (PerfStatus.Start("ReadDocumentFeed"))
                {
                     docs = this.client.ReadDocumentFeedAsync(((Documents.DocumentCollection)this.Tag).GetLink(this.client)).Result;
                }

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
            if (this.resourceType != ResourceType.Offer)
            {
                this.Text = (document as Documents.Resource).Id;
            }
            else
            {
                Offer offer = document as Offer;
                this.Text = offer.OfferType + "_" + offer.GetPropertyValue<String>("offerResourceId");
            }
            this.Tag = document;
            this.client = client;

            MenuItem myMenuItem0 = new MenuItem("Read " + this.resourceType.ToString());
            myMenuItem0.Click += new EventHandler(myMenuItemRead_Click);
            this.contextMenu.MenuItems.Add(myMenuItem0);

            if (this.resourceType != ResourceType.Conflict && this.resourceType != ResourceType.Offer)
            {
                MenuItem myMenuItem1 = new MenuItem("Replace " + this.resourceType.ToString());
                myMenuItem1.Click += new EventHandler(myMenuItemUpdate_Click);
                this.contextMenu.MenuItems.Add(myMenuItem1);
            }

            if (this.resourceType != ResourceType.Offer)
            {
                MenuItem myMenuItem = new MenuItem("Delete " + this.resourceType.ToString());
                myMenuItem.Click += new EventHandler(myMenuItemDelete_Click);
                this.contextMenu.MenuItems.Add(myMenuItem);
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

                MenuItem myMenuItem2 = new MenuItem("Download media");
                myMenuItem2.Click += new EventHandler(myMenuItemDownloadMedia_Click);
                this.contextMenu.MenuItems.Add(myMenuItem2);

                MenuItem myMenuItem3 = new MenuItem("Render media");
                myMenuItem3.Click += new EventHandler(myMenuItemRenderMedia_Click);
                this.contextMenu.MenuItems.Add(myMenuItem3);
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

                MenuItem myMenuItem3 = new MenuItem("Create attachment");
                myMenuItem3.Click += new EventHandler(myMenuItemAttachment_Click);
                this.contextMenu.MenuItems.Add(myMenuItem3);

                MenuItem myMenuItem4 = new MenuItem("Create attachment from file");
                myMenuItem4.Click += new EventHandler(myMenuItemAttachmentFromFile_Click);
                this.contextMenu.MenuItems.Add(myMenuItem4);
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
                Program.GetMain().SetCrudContext(this, "Replace " + this.resourceType.ToString(), true, (this.Tag as Documents.StoredProcedure).Body, this.UpdateNode);
            }
            else if (this.resourceType == ResourceType.Trigger)
            {
                Program.GetMain().SetCrudContext(this, "Replace " + this.resourceType.ToString(), true, (this.Tag as Documents.Trigger).Body, this.UpdateNode);
            }
            else if (this.resourceType == ResourceType.UserDefinedFunction)
            {
                Program.GetMain().SetCrudContext(this, "Replace " + this.resourceType.ToString(), true, (this.Tag as Documents.UserDefinedFunction).Body, this.UpdateNode);
            }
            else
            {
                string x = this.Tag.ToString();
                Program.GetMain().SetCrudContext(this, "Replace " + this.resourceType.ToString(), false, x, this.UpdateNode);
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
                    ResourceResponse<Document> rr;
                    using (PerfStatus.Start("ReadDocument"))
                    {
                        rr = await this.client.ReadDocumentAsync(((Resource)this.Tag).GetLink(this.client), Program.GetMain().GetRequestOptions());
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
                    using (PerfStatus.Start("ReadAttachment"))
                    {
                        rr = await this.client.ReadAttachmentAsync(((Resource)this.Tag).GetLink(this.client), Program.GetMain().GetRequestOptions());
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

        void myMenuItemAttachment_Click(object sender, EventArgs e)
        {
            Documents.Attachment attachment = new Documents.Attachment();
            attachment.Id = "Here is your attachment Id";
            attachment.ContentType = "application-content-type";
            attachment.MediaLink = "internal link or Azure blob or Amazon S3 link";

            string x = attachment.ToString();
            Program.GetMain().SetCrudContext(this, "Create attachment for this document " + this.resourceType.ToString(), false, x, this.AddAttachment);
        }

        async void myMenuItemRenderMedia_Click(object sender, EventArgs eventArg)
        {
            string appTempPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DocumentDBStudio");
            string guidFileName = Guid.NewGuid().ToString();
            string fileName;

            // let's guess the contentype.
            Documents.Attachment attachment = this.Tag as Attachment;
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
            Documents.Attachment attachment = this.Tag as Attachment;

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
                string filename = ofd.FileName;
                // 
                // todo: present the dialog for Slug name and Content type
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
                            Slug = Path.GetFileName(ofd.FileName)
                        };

                        ResourceResponse<Documents.Attachment> rr;
                        using (PerfStatus.Start("CreateAttachment"))
                        {
                             rr = await this.client.CreateAttachmentAsync((this.Tag as Documents.Document).GetLink(this.client) + "/attachments",
                                       stream, options);
                        }
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
            Program.GetMain().SetCrudContext(this, "Execute " + this.resourceType.ToString() + " " +
                (this.Tag as Documents.Resource).Id, false,
                "Here is the input parameters to the storedProcedure. Input each parameter as one line without quotation mark.", this.ExecuteStoredProcedure);
        }

        void myMenuItemDelete_Click(object sender, EventArgs e)
        {
            string x = this.Tag.ToString();
            CommandContext context = new CommandContext();
            context.IsDelete = true;
            Program.GetMain().SetCrudContext(this, "Delete " + this.resourceType.ToString(), false, x, this.DeleteNode, context);
        }

        async void AddAttachment(string text, object optional)
        {
            try
            {
                Documents.Attachment attachment = (Documents.Attachment)JsonConvert.DeserializeObject(text, typeof(Documents.Attachment));

                ResourceResponse<Documents.Attachment> rr;
                using (PerfStatus.Start("CreateAttachment"))
                {
                    rr = await this.client.CreateAttachmentAsync((this.Tag as Documents.Resource).GetLink(this.client),
                                        attachment, Program.GetMain().GetRequestOptions());
                }
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

        async void ExecuteStoredProcedure(string text, object optional)
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
                     rr = await this.client.ExecuteStoredProcedureAsync<dynamic>((this.Tag as Documents.Resource).GetLink(this.client),
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

        async void UpdateNode(string text, object optionalObject)
        {
            string optional = optionalObject as string;
            try
            {
                string json = null;
                if (this.resourceType == ResourceType.Document)
                {
                    Documents.Document doc = (Documents.Document)JsonConvert.DeserializeObject(text, typeof(Documents.Document));
                    doc.SetReflectedPropertyValue("AltLink", (this.Tag as Document).GetAltLink());
                    ResourceResponse<Documents.Document> rr;
                    using (PerfStatus.Start("ReplaceDocument"))
                    {
                         rr = await this.client.ReplaceDocumentAsync(doc.GetLink(this.client), doc, Program.GetMain().GetRequestOptions());
                    }
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
                    ResourceResponse<Documents.StoredProcedure> rr;
                    using (PerfStatus.Start("ReplaceStoredProcedure"))
                    {
                         rr = await this.client.ReplaceStoredProcedureExAsync(sp, Program.GetMain().GetRequestOptions());
                    }
                    json = rr.Resource.ToString();
                    this.Tag = rr.Resource;
                    this.Text = rr.Resource.Id;
                    // set the result window
                    Program.GetMain().SetResultInBrowser(json, null, false, rr.ResponseHeaders);
                }
                else if (this.resourceType == ResourceType.User)
                {
                    Documents.User sp = (Documents.User)JsonConvert.DeserializeObject(text, typeof(Documents.User));
                    sp.SetReflectedPropertyValue("AltLink", (this.Tag as User).GetAltLink());
                    ResourceResponse<Documents.User> rr;
                    using (PerfStatus.Start("ReplaceUser"))
                    {
                        rr = await this.client.ReplaceUserExAsync(sp, Program.GetMain().GetRequestOptions());
                    }
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
                    ResourceResponse<Documents.Trigger> rr;
                    using (PerfStatus.Start("ReplaceTrigger"))
                    {
                         rr = await this.client.ReplaceTriggerExAsync(sp, Program.GetMain().GetRequestOptions());
                    }
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
                    ResourceResponse<Documents.UserDefinedFunction> rr;
                    using (PerfStatus.Start("ReplaceUDF"))
                    {
                         rr = await this.client.ReplaceUserDefinedFunctionExAsync(sp, Program.GetMain().GetRequestOptions());
                    }
                    json = rr.Resource.ToString();
                    this.Tag = rr.Resource;
                    this.Text = rr.Resource.Id;
                    // set the result window
                    Program.GetMain().SetResultInBrowser(json, null, false, rr.ResponseHeaders);
                }
                else if (this.resourceType == ResourceType.Permission)
                {
                    Documents.Permission sp = Documents.Resource.LoadFrom<Documents.Permission>(new MemoryStream(Encoding.UTF8.GetBytes(text)));
                    sp.SetReflectedPropertyValue("AltLink", (this.Tag as Permission).GetAltLink());
                    ResourceResponse<Documents.Permission> rr;
                    using (PerfStatus.Start("ReplacePermission"))
                    {
                        rr = await this.client.ReplacePermissionExAsync(sp, Program.GetMain().GetRequestOptions());
                    }
                    json = rr.Resource.ToString();
                    this.Tag = rr.Resource;
                    this.Text = rr.Resource.Id;
                    // set the result window
                    Program.GetMain().SetResultInBrowser(json, null, false, rr.ResponseHeaders);
                }
                else if (this.resourceType == ResourceType.Attachment)
                {
                    Documents.Attachment sp = (Documents.Attachment)JsonConvert.DeserializeObject(text, typeof(Documents.Attachment));
                    sp.SetReflectedPropertyValue("AltLink", (this.Tag as Attachment).GetAltLink());
                    ResourceResponse<Documents.Attachment> rr;
                    using (PerfStatus.Start("ReplaceAttachment"))
                    {
                        rr = await this.client.ReplaceAttachmentExAsync(sp, Program.GetMain().GetRequestOptions());
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

        async void DeleteNode(string text, object optional)
        {
            try
            {
                if (this.resourceType == ResourceType.Document)
                {
                    Documents.Document doc = (Documents.Document)this.Tag;
                    ResourceResponse<Documents.Document> rr;
                    using (PerfStatus.Start("DeleteDocument"))
                    {
                        rr = await this.client.DeleteDocumentAsync(doc.GetLink(this.client), Program.GetMain().GetRequestOptions());
                    }
                    Program.GetMain().SetResultInBrowser(null, "Delete Document succeed!", false, rr.ResponseHeaders);

                }
                else if (this.resourceType == ResourceType.StoredProcedure)
                {
                    Documents.StoredProcedure sp = (Documents.StoredProcedure)this.Tag;
                    ResourceResponse<Documents.StoredProcedure> rr;
                    using (PerfStatus.Start("DeleteStoredProcedure"))
                    {
                        rr = await this.client.DeleteStoredProcedureAsync(sp.GetLink(this.client), Program.GetMain().GetRequestOptions());
                    }
                    Program.GetMain().SetResultInBrowser(null, "Delete StoredProcedure succeed!", false, rr.ResponseHeaders);
                }
                else if (this.resourceType == ResourceType.User)
                {
                    Documents.User sp = (Documents.User)this.Tag;
                    ResourceResponse<Documents.User> rr;
                    using (PerfStatus.Start("DeleteUser"))
                    {
                        rr = await this.client.DeleteUserAsync(sp.GetLink(this.client), Program.GetMain().GetRequestOptions());
                    }
                    Program.GetMain().SetResultInBrowser(null, "Delete User succeed!", false, rr.ResponseHeaders);
                }
                else if (this.resourceType == ResourceType.Trigger)
                {
                    Documents.Trigger sp = (Documents.Trigger)this.Tag;
                    ResourceResponse<Documents.Trigger> rr;
                    using (PerfStatus.Start("DeleteTrigger"))
                    {
                        rr = await this.client.DeleteTriggerAsync(sp.GetLink(this.client), Program.GetMain().GetRequestOptions());
                    }
                    Program.GetMain().SetResultInBrowser(null, "Delete Trigger succeed!", false, rr.ResponseHeaders);
                }
                else if (this.resourceType == ResourceType.UserDefinedFunction)
                {
                    Documents.UserDefinedFunction sp = (Documents.UserDefinedFunction)this.Tag;
                    ResourceResponse<Documents.UserDefinedFunction> rr;
                    using (PerfStatus.Start("DeleteUDF"))
                    {
                        rr = await this.client.DeleteUserDefinedFunctionAsync(sp.GetLink(this.client), Program.GetMain().GetRequestOptions());
                    }
                    Program.GetMain().SetResultInBrowser(null, "Delete UserDefinedFunction succeed!", false, rr.ResponseHeaders);
                }
                else if (this.resourceType == ResourceType.Permission)
                {
                    Documents.Permission sp = (Documents.Permission)this.Tag;
                    ResourceResponse<Documents.Permission> rr;
                    using (PerfStatus.Start("DeletePermission"))
                    {
                        rr = await this.client.DeletePermissionAsync(sp.GetLink(this.client), Program.GetMain().GetRequestOptions());
                    }
                    Program.GetMain().SetResultInBrowser(null, "Delete Permission succeed!", false, rr.ResponseHeaders);
                }
                else if (this.resourceType == ResourceType.Attachment)
                {
                    Documents.Attachment sp = (Documents.Attachment)this.Tag;
                    ResourceResponse<Documents.Attachment> rr;
                    using (PerfStatus.Start("DeleteAttachment"))
                    {
                        rr = await this.client.DeleteAttachmentAsync(sp.GetLink(this.client), Program.GetMain().GetRequestOptions());
                    }
                    Program.GetMain().SetResultInBrowser(null, "Delete Attachment succeed!", false, rr.ResponseHeaders);
                }
                else if (this.resourceType == ResourceType.Conflict)
                {
                    Documents.Conflict sp = (Documents.Conflict)this.Tag;
                    ResourceResponse<Documents.Conflict> rr;
                    using (PerfStatus.Start("DeleteConlict"))
                    {
                        rr = await this.client.DeleteConflictAsync(sp.GetLink(this.client), Program.GetMain().GetRequestOptions());
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
                FeedResponse<Documents.Attachment> attachments;
                using (PerfStatus.Start("ReadAttachmentFeed"))
                {
                     attachments = this.client.ReadAttachmentFeedAsync((this.Tag as Documents.Document).GetLink(this.client)).Result;
                }
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

            MenuItem myMenuItem = new MenuItem("Create StoredProcedure");
            myMenuItem.Click += new EventHandler(myMenuItemAddStoredProcedure_Click);
            this.contextMenu.MenuItems.Add(myMenuItem);
            MenuItem myMenuItem2 = new MenuItem("Create StoredProcedure From File");
            myMenuItem2.Click += new EventHandler(myMenuItemAddStoredProcedureFromFile_Click);
            this.contextMenu.MenuItems.Add(myMenuItem2);

            MenuItem myMenuItem1 = new MenuItem("Refresh StoredProcedures feed");
            myMenuItem1.Click += new EventHandler((sender, e) => Refresh(true));
            this.contextMenu.MenuItems.Add(myMenuItem1);

        }

        void myMenuItemAddStoredProcedure_Click(object sender, EventArgs e)
        {
            // 
            Program.GetMain().SetCrudContext(this, string.Format(CultureInfo.InvariantCulture, "Create StoredProcedure in collection {0}", (this.Parent.Tag as Documents.DocumentCollection).Id),
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

                Program.GetMain().SetCrudContext(this, "Add StoredProcedure", false, text, this.AddStoredProcedure);
            }
        }

        async void AddStoredProcedure(string body, object idobject)
        {
            string id = idobject as string;
            try
            {
                Documents.StoredProcedure sp = new Documents.StoredProcedure();
                sp.Body = body;
                sp.Id = id;

                ResourceResponse<Documents.StoredProcedure> newsp;
                using (PerfStatus.Start("CreateStoredProcedure"))
                {
                    newsp = await this.client.CreateStoredProcedureAsync((this.Parent.Tag as Documents.DocumentCollection).GetLink(this.client), sp, Program.GetMain().GetRequestOptions());
                }

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
                FeedResponse<Documents.StoredProcedure> sps;
                using (PerfStatus.Start("ReadStoredProcedure"))
                {
                    sps = this.client.ReadStoredProcedureFeedAsync((collnode.Tag as Documents.DocumentCollection).GetLink(this.client)).Result;
                }

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

            MenuItem myMenuItem = new MenuItem("Create UserDefinedFunction");
            myMenuItem.Click += new EventHandler(myMenuItemAddUDF_Click);
            this.contextMenu.MenuItems.Add(myMenuItem);
            MenuItem myMenuItem2 = new MenuItem("Create UserDefinedFunction from File");
            myMenuItem2.Click += new EventHandler(myMenuItemAddUDFFromFile_Click);
            this.contextMenu.MenuItems.Add(myMenuItem2);
            MenuItem myMenuItem1 = new MenuItem("Refresh UserDefinedFunction feed");
            myMenuItem1.Click += new EventHandler((sender, e) => Refresh(true));
            this.contextMenu.MenuItems.Add(myMenuItem1);
        }

        void myMenuItemAddUDF_Click(object sender, EventArgs e)
        {
            // 
            Program.GetMain().SetCrudContext(this, string.Format(CultureInfo.InvariantCulture, "Add UserDefinedFunction in collection {0}", (this.Parent.Tag as Documents.DocumentCollection).Id),
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

                Program.GetMain().SetCrudContext(this, "Create UDF", false, text, this.AddUDF);
            }
        }
        async void AddUDF(string body, object idObject)
        {
            string id = idObject as string;
            try
            {
                Documents.UserDefinedFunction udf = new Documents.UserDefinedFunction();
                udf.Body = body;
                udf.Id = id;

                ResourceResponse<Documents.UserDefinedFunction> newudf;
                using (PerfStatus.Start("CreateUDF"))
                {
                    newudf = await this.client.CreateUserDefinedFunctionAsync((this.Parent.Tag as Documents.DocumentCollection).GetLink(this.client), udf, Program.GetMain().GetRequestOptions());
                }

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
                FeedResponse<Documents.UserDefinedFunction> sps;
                using (PerfStatus.Start("ReadUdfFeed"))
                {
                    sps = this.client.ReadUserDefinedFunctionFeedAsync((collnode.Tag as Documents.DocumentCollection).GetLink(this.client)).Result;
                }

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

            MenuItem myMenuItem = new MenuItem("Create Trigger");
            myMenuItem.Click += new EventHandler(myMenuItemAddTrigger_Click);
            this.contextMenu.MenuItems.Add(myMenuItem);
            MenuItem myMenuItem2 = new MenuItem("Create Trigger from file");
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

                Program.GetMain().SetCrudContext(this, "Create trigger", false, text, this.AddTrigger, new CommandContext() { IsCreateTrigger = true });
            }
        }

        void myMenuItemAddTrigger_Click(object sender, EventArgs e)
        {
            // 
            Program.GetMain().SetCrudContext(this, string.Format(CultureInfo.InvariantCulture, "Create trigger in collection {0}", (this.Parent.Tag as Documents.DocumentCollection).Id),
                true, "function() { \r\n \r\n}", this.AddTrigger, new CommandContext() { IsCreateTrigger = true });
        }

        async void AddTrigger(string body, object triggerObject)
        {
            try
            {
                Trigger trigger = triggerObject as Trigger;

                ResourceResponse<Documents.Trigger> newtrigger;
                using (PerfStatus.Start("CreateTrigger"))
                {
                    newtrigger = await this.client.CreateTriggerAsync((this.Parent.Tag as Documents.DocumentCollection).GetLink(this.client), trigger, Program.GetMain().GetRequestOptions());
                }

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
                FeedResponse<Documents.Trigger> sps;
                using (PerfStatus.Start("ReadTriggerFeed"))
                {
                     sps = this.client.ReadTriggerFeedAsync((collnode.Tag as Documents.DocumentCollection).GetLink(this.client)).Result;
                }

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

            MenuItem myMenuItem = new MenuItem("Create User");
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
            Program.GetMain().SetCrudContext(this, string.Format(CultureInfo.InvariantCulture, "Create user in database {0}", (this.Parent.Tag as Documents.Database).Id),
                false, x, this.AddUser);
        }

        async void AddUser(string body, object id)
        {
            try
            {
                Documents.User user = (Documents.User)JsonConvert.DeserializeObject(body, typeof(Documents.User));

                ResourceResponse<Documents.User> newUser;
                using (PerfStatus.Start("CreateUser"))
                {
                    newUser = await this.client.CreateUserAsync((this.Parent.Tag as Documents.Database).GetLink(this.client), user, Program.GetMain().GetRequestOptions());
                }
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
                FeedResponse<Documents.User> sps;
                using (PerfStatus.Start("ReadUser"))
                {
                    sps = this.client.ReadUserFeedAsync((this.Parent.Tag as Documents.Database).GetLink(this.client)).Result;
                }
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

            MenuItem myMenuItem = new MenuItem("Create Permission");
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

            Program.GetMain().SetCrudContext(this, string.Format(CultureInfo.InvariantCulture, "Create permission for user {0}", (this.Parent.Tag as Documents.Resource).Id),
                false, x, this.AddPermission);
        }

        async void AddPermission(string body, object id)
        {
            try
            {
                Documents.Permission permission = Documents.Resource.LoadFrom<Documents.Permission>(new MemoryStream(Encoding.UTF8.GetBytes(body)));

                ResourceResponse<Documents.Permission> newtpermission;
                using (PerfStatus.Start("CreatePermission"))
                {
                     newtpermission = await this.client.CreatePermissionAsync((this.Parent.Tag as Documents.Resource).GetLink(this.client), permission, Program.GetMain().GetRequestOptions());
                }
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
                FeedResponse<Documents.Permission> sps;
                using (PerfStatus.Start("ReadPermission"))
                {
                    sps = this.client.ReadPermissionFeedAsync((this.Parent.Tag as Documents.User).GetLink(this.client)).Result;
                }

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

            // Query conflicts currrently fail due to gateway
            MenuItem myMenuItem2 = new MenuItem("Query Conflict feed");
            myMenuItem2.Click += new EventHandler(myMenuItemQueryConflicts_Click);
            this.contextMenu.MenuItems.Add(myMenuItem2);
        }

        override public void ShowContextMenu(TreeView treeview, Point p)
        {
            this.contextMenu.Show(treeview, p);
        }

        void myMenuItemQueryConflicts_Click(object sender, EventArgs e)
        {
            Program.GetMain().SetCrudContext(this, "Query Conflicts",
                false, "select * from c", this.QueryConflicts);
        }

        async void QueryConflicts(string queryText, object optional)
        {
            try
            {
                // text is the querytext.
                FeedResponse<Database> r;
                using (PerfStatus.Start("QueryConflicts"))
                {
                    IDocumentQuery<dynamic> q = this.client.CreateConflictQuery((this.Parent.Tag as Documents.DocumentCollection).GetLink(this.client), queryText).AsDocumentQuery();
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
                FeedResponse<Documents.Conflict> feedConflicts;
                using (PerfStatus.Start("ReadConflictsFeed"))
                {
                    feedConflicts = this.client.ReadConflictFeedAsync((this.Parent.Tag as Documents.DocumentCollection).GetLink(this.client)).Result;
                }

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

    class OffersNode : FeedNode
    {
        private DocumentClient client;
        private ContextMenu contextMenu = new ContextMenu();

        public OffersNode(DocumentClient client)
        {
            this.Text = "Offers";
            this.client = client;
            this.Nodes.Add(new TreeNode("fake"));
            this.Tag = "This represents the Offers feed.";
            this.ImageKey = "Offer";
            this.SelectedImageKey = "Offer";

            MenuItem myMenuItem1 = new MenuItem("Refresh Offer feed");
            myMenuItem1.Click += new EventHandler((sender, e) => Refresh(true));
            this.contextMenu.MenuItems.Add(myMenuItem1);
        }

        override public void ShowContextMenu(TreeView treeview, Point p)
        {
            this.contextMenu.Show(treeview, p);
        }

        void myMenuItemQueryOffers_Click(object sender, EventArgs e)
        {
            Program.GetMain().SetCrudContext(this, "Query Offers",
                false, "select * from c", this.QueryOffers);
        }

        async void QueryOffers(string queryText, object optional)
        {
            try
            {
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
                FeedResponse<Documents.Offer> feedOffers = this.client.ReadOffersFeedAsync().Result;

                foreach (var sp in feedOffers)
                {
                    DocumentNode node = new DocumentNode(client, sp, ResourceType.Offer);
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