using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json;

namespace Microsoft.Azure.DocumentDBStudio
{
    class DatabaseNode : FeedNode
    {
        private readonly DocumentClient _client;
        private readonly ContextMenu _contextMenu = new ContextMenu();
        private string _currentContinuation = null;
        private CommandContext _currentQueryCommandContext = null;

        public DatabaseNode(DocumentClient localclient, Database db)
        {
            Text = db.Id;
            Tag = db;
            _client = localclient;
            ImageKey = "SystemFeed";
            SelectedImageKey = "SystemFeed";

            Nodes.Add(new UserNode(_client));

            AddMenuItem("Read Database", myMenuItemReadDatabase_Click);
            AddMenuItem("Delete Database", myMenuItemDeleteDatabase_Click);

            _contextMenu.MenuItems.Add("-");

            AddMenuItem("Create DocumentCollection", myMenuItemCreateDocumentCollection_Click);
            AddMenuItem("Refresh DocumentCollections Feed", (sender, e) => Refresh(true));
            AddMenuItem("Query DocumentCollections", myMenuItemQueryDocumentCollection_Click);
        }

        private void AddMenuItem(string menuItemText, EventHandler eventHandler)
        {
            var menuItem = new MenuItem(menuItemText);
            menuItem.Click += eventHandler;
            _contextMenu.MenuItems.Add(menuItem);
        }

        async void myMenuItemReadDatabase_Click(object sender, EventArgs eArgs)
        {
            try
            {
                ResourceResponse<Database> database;
                using (PerfStatus.Start("ReadDatabase"))
                {
                    database = await _client.ReadDatabaseAsync(((Database)Tag).GetLink(_client), Program.GetMain().GetRequestOptions());
                }
                // set the result window
                var json = JsonConvert.SerializeObject(database.Resource, Formatting.Indented);

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
            _currentQueryCommandContext = new CommandContext();
            _currentQueryCommandContext.IsFeed = true;

            // reset continuation token
            _currentContinuation = null;

            Program.GetMain().SetCrudContext(this, OperationType.Query, ResourceType.Document, "select * from c", QueryDocumentCollectionsAsync, _currentQueryCommandContext);
        }

        async void QueryDocumentCollectionsAsync(object resource, RequestOptions requestOptions)
        {
            try
            {
                var queryText = resource as string;
                // text is the querytext.
                IDocumentQuery<dynamic> q = null;

                var feedOptions = Program.GetMain().GetFeedOptions();

                if (requestOptions == null)
                {
                    // requestOptions = null means it is from the next page. We only attempt to continue using the RequestContinuation for next page button
                    if (!string.IsNullOrEmpty(_currentContinuation) && string.IsNullOrEmpty(feedOptions.RequestContinuation))
                    {
                        feedOptions.RequestContinuation = _currentContinuation;
                    }
                }

                q = _client.CreateDocumentCollectionQuery((Tag as Database).GetLink(_client), queryText, feedOptions).AsDocumentQuery();

                var sw = Stopwatch.StartNew();

                FeedResponse<dynamic> r;
                using (PerfStatus.Start("QueryDocument"))
                {
                    r = await q.ExecuteNextAsync();
                }
                sw.Stop();
                _currentContinuation = r.ResponseContinuation;
                _currentQueryCommandContext.HasContinuation = !string.IsNullOrEmpty(_currentContinuation);
                _currentQueryCommandContext.QueryStarted = true;

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

                var jsonarray = "[";
                var index = 0;
                foreach (var d in r)
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
                Program.GetMain().SetNextPageVisibility(_currentQueryCommandContext);
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
            var bodytext = Tag.ToString();
            var context = new CommandContext();
            context.IsDelete = true;
            Program.GetMain().SetCrudContext(this, OperationType.Delete, ResourceType.Database, bodytext, DeleteDatabaseAsync, context);
        }

        void myMenuItemCreateDocumentCollection_Click(object sender, EventArgs e)
        {
            dynamic d = new System.Dynamic.ExpandoObject();
            d.id = "Here is your DocumentCollection Id";

            string x = JsonConvert.SerializeObject(d, Formatting.Indented);
            Program.GetMain().SetCrudContext(this, OperationType.Create, ResourceType.DocumentCollection, x, CreateDocumentCollectionAsync);
        }

        public override void ShowContextMenu(TreeView treeview, Point p)
        {
            _contextMenu.Show(treeview, p);
        }

        public override void Refresh(bool forceRefresh)
        {
            if (forceRefresh || IsFirstTime)
            {
                IsFirstTime = false;
                Nodes.Clear();

                Nodes.Add(new UserNode(_client));

                FillWithChildren();
            }
        }

        async void CreateDocumentCollectionAsync(object resource, RequestOptions requestOptions)
        {
            try
            {
                var coll = resource as DocumentCollection;
                var db = (Database)Tag;
                var dbId = db.Id;
                ResourceResponse<DocumentCollection> newcoll;
                using (PerfStatus.Start("CreateDocumentCollection"))
                {
                    newcoll = await _client.CreateDocumentCollectionAsync(db.GetLink(_client), coll, requestOptions);
                }

                // set the result window
                var json = JsonConvert.SerializeObject(newcoll.Resource, Formatting.Indented);

                Program.GetMain().SetResultInBrowser(json, null, false, newcoll.ResponseHeaders);

                Nodes.Add(new DocumentCollectionNode(_client, newcoll.Resource, dbId));
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
                var db = (Database)Tag;
                ResourceResponse<Database> newdb;
                using (PerfStatus.Start("DeleteDatabase"))
                {
                    newdb = await _client.DeleteDatabaseAsync(db.GetLink(_client), requestOptions);
                }

                Program.GetMain().SetResultInBrowser(null, "Delete database succeed!", false, newdb.ResponseHeaders);

                Remove();
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

        public async void FillWithChildren()
        {
            try
            {
                FeedResponse<DocumentCollection> colls;
                var db = (Database) Tag;
                var dbId = db.Id;
                using (PerfStatus.Start("ReadDocumentCollectionFeed"))
                {
                    colls = await _client.ReadDocumentCollectionFeedAsync(db.GetLink(_client));
                }

                foreach (var coll in colls)
                {
                    var node = new DocumentCollectionNode(_client, coll, dbId);
                    Nodes.Add(node);
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
}
