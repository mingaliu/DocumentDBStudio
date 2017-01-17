using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Azure.DocumentDBStudio.CustomDocumentListDisplay;
using Microsoft.Azure.DocumentDBStudio.Helpers;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json;

namespace Microsoft.Azure.DocumentDBStudio
{
    class DocumentCollectionNode : FeedNode
    {
        private readonly DocumentClient _client;
        private readonly ContextMenu _contextMenu = new ContextMenu();
        private string _currentContinuation = null;
        private CommandContext _currentQueryCommandContext = null;
        private readonly string _databaseId;
        private readonly string _documentCollectionId;

        private readonly CustomDocumentListDisplayManager _customDocumentListDisplayManager = new CustomDocumentListDisplayManager();

        public DocumentCollectionNode(DocumentClient client, DocumentCollection coll, string databaseId)
        {
            _databaseId = databaseId;
            _documentCollectionId = coll.Id;
            Text = _documentCollectionId;
            Tag = coll;
            _client = client;
            ImageKey = "SystemFeed";
            SelectedImageKey = "SystemFeed";

            Nodes.Add(new StoredProcedureNode(_client));
            Nodes.Add(new UserDefinedFunctionNode(_client));
            Nodes.Add(new TriggerNode(_client));
            Nodes.Add(new ConflictNode(_client));

            AddMenuItem("Read DocumentCollection", myMenuItemReadDocumentCollection_Click);

            AddMenuItem("Replace DocumentCollection", myMenuItemReplaceDocumentCollection_Click);
            AddMenuItem("Delete DocumentCollection", myMenuItemDeleteDocumentCollection_Click);

            _contextMenu.MenuItems.Add("-");
            
            AddMenuItem("Create Document", myMenuItemCreateDocument_Click, Shortcut.CtrlN);
            AddMenuItem("Create Document with prefilled id", myMenuItemCreateDocumentWithId_Click, Shortcut.CtrlShiftN);
            AddMenuItem("Create Document From File...", myMenuItemCreateDocumentFromFile_Click);
            AddMenuItem("Create Multiple Documents From Folder...", myMenuItemCreateDocumentsFromFolder_Click);

            _contextMenu.MenuItems.Add("-");

            AddMenuItem("Refresh Documents feed", (sender, e) => Refresh(true), Shortcut.F5);
            AddMenuItem("Query Documents", myMenuItemQueryDocument_Click);

            _contextMenu.MenuItems.Add("-");
            AddMenuItem("Configure Document Listing settings...", myMenuConfigureDocumentListingDisplay_Click);

            _contextMenu.MenuItems.Add("-");
            AddMenuItem("Collapse all", myMenuItemCollapseAll_Click);

        }

        private void myMenuItemCollapseAll_Click(object sender, EventArgs e)
        {
            this.Collapse();
        }

        private void myMenuConfigureDocumentListingDisplay_Click(object sender, EventArgs e)
        {
            if (Nodes.Count == 0)
            {
                MessageBox.Show("No documents available, cannot configure display settings.", "Cannot configure display settings", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var query = _client.CreateDocumentQuery<Document>((Tag as DocumentCollection).GetLink(_client), "SELECT TOP 1 * FROM c");
            var doc = query.ToList().First();
            var docConverted = JsonConvert.DeserializeObject(DocumentHelper.RemoveInternalDocumentValues(JsonConvert.SerializeObject(doc)));

            var dlg = new CustomDocumentListDisplayConfigurationForm(_client.ServiceEndpoint.Host, _databaseId, _documentCollectionId, docConverted);
            
            var dr = dlg.ShowDialog();
            if (dr == DialogResult.OK)
            {
                Refresh(true);
            }
        }

        private MenuItem AddMenuItem(string menuItemText, EventHandler eventHandler, Shortcut shortcut = Shortcut.None)
        {
            var menuItem = new MenuItem(menuItemText);
            menuItem.Click += eventHandler;
            if (shortcut != Shortcut.None)
            {
                menuItem.Shortcut = shortcut;
            }

            _contextMenu.MenuItems.Add(menuItem);

            return menuItem;
        }

        async void myMenuItemReadDocumentCollection_Click(object sender, EventArgs eArgs)
        {
            try
            {
                ResourceResponse<DocumentCollection> rr;
                using (PerfStatus.Start("ReadDocumentCollection"))
                {
                    rr = await _client.ReadDocumentCollectionAsync(((Resource)Tag).GetLink(_client), Program.GetMain().GetRequestOptions());
                }
                // set the result window
                var json = JsonConvert.SerializeObject(rr.Resource, Formatting.Indented);

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
            var bodytext = Tag.ToString();
            var context = new CommandContext {IsDelete = true};
            Program.GetMain().SetCrudContext(this, OperationType.Delete, ResourceType.DocumentCollection, bodytext, DeleteDocumentCollectionAsync, context);
        }

        void myMenuItemReplaceDocumentCollection_Click(object sender, EventArgs e)
        {
            var bodytext = Tag.ToString();
            var context = new CommandContext();
            Program.GetMain().SetCrudContext(this, OperationType.Replace, ResourceType.DocumentCollection, bodytext, ReplaceDocumentCollectionAsync, context);
        }

        async void ReplaceDocumentCollectionAsync(object resource, RequestOptions requestOptions)
        {
            try
            {
                var originalCollection = (DocumentCollection)Tag;

                //Update collection if necessary
                var updatedCollection = resource as DocumentCollection;
                originalCollection.IndexingPolicy = (IndexingPolicy)updatedCollection.IndexingPolicy.Clone();

                ResourceResponse<DocumentCollection> response;
                using (PerfStatus.Start("ReplaceDocumentCollection"))
                {
                    response = await _client.ReplaceDocumentCollectionExAsync(originalCollection, requestOptions);
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
                var coll = (DocumentCollection)Tag;
                ResourceResponse<DocumentCollection> newcoll;
                using (PerfStatus.Start("DeleteDocumentCollection"))
                {
                    newcoll = await _client.DeleteDocumentCollectionAsync(coll.GetLink(_client), Program.GetMain().GetRequestOptions());
                }
                Program.GetMain().SetResultInBrowser(null, "Delete DocumentCollection succeed!", false, newcoll.ResponseHeaders);

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

        void myMenuItemCreateDocumentFromFile_Click(object sender, EventArgs e)
        {
            InvokeCreateDocumentFromFile();
        }

        public void InvokeCreateDocumentFromFile()
        {
            var ofd = new OpenFileDialog();
            var dr = ofd.ShowDialog();

            if (dr == DialogResult.OK)
            {
                var filename = ofd.FileName;
                // 
                var text = File.ReadAllText(filename);

                Program.GetMain().SetCrudContext(this, OperationType.Create, ResourceType.Document, text, CreateDocumentAsync);
            }
        }

        async void myMenuItemCreateDocumentsFromFolder_Click(object sender, EventArgs e)
        {
            await InvokeCreateDocumentsFromFolder();
        }

        public async Task InvokeCreateDocumentsFromFolder()
        {
            var ofd = new OpenFileDialog {Multiselect = true};

            var dr = ofd.ShowDialog();

            if (dr == DialogResult.OK)
            {
                var status = string.Format(CultureInfo.InvariantCulture, "Create {0} documents in collection\r\n",
                    ofd.FileNames.Length);
                // Read the files 
                foreach (var filename in ofd.FileNames)
                {
                    // right now assume every file is JSON content
                    var jsonText = File.ReadAllText(filename);
                    var fileRootName = Path.GetFileName(filename);

                    var document = JsonConvert.DeserializeObject(jsonText);

                    try
                    {
                        using (PerfStatus.Start("CreateDocument"))
                        {
                            var newdocument =
                                await
                                    _client.CreateDocumentAsync((Tag as DocumentCollection).GetLink(_client), document,
                                        Program.GetMain().GetRequestOptions());
                            status += string.Format(CultureInfo.InvariantCulture, "Succeed adding {0} \r\n", fileRootName);
                        }
                    }
                    catch (DocumentClientException ex)
                    {
                        status += string.Format(CultureInfo.InvariantCulture, "Failed adding {0}, statusCode={1} \r\n",
                            fileRootName, ex.StatusCode);
                    }
                    catch (Exception ex)
                    {
                        status += string.Format(CultureInfo.InvariantCulture, "Failed adding {0}, unknown exception \r\n",
                            fileRootName, ex.Message);
                    }

                    Program.GetMain().SetResultInBrowser(null, status, false);
                }
            }
        }

        void myMenuItemCreateDocument_Click(object sender, EventArgs e)
        {
            InvokeCreateDocument();
        }

        public void InvokeCreateDocument(dynamic d = null)
        {
            if (d == null)
            {
                d = new System.Dynamic.ExpandoObject();
                d.id = "Here is your Document Id";
            }
            string x = JsonConvert.SerializeObject(d, Formatting.Indented);
            Program.GetMain().SetCrudContext(this, OperationType.Create, ResourceType.Document, x, CreateDocumentAsync);
        }

        void myMenuItemCreateDocumentWithId_Click(object sender, EventArgs e)
        {
            InvokeCreatedDocumentWithId();
        }

        public void InvokeCreatedDocumentWithId()
        {
            dynamic d = new System.Dynamic.ExpandoObject();
            d.id = Guid.NewGuid();
            string x = JsonConvert.SerializeObject(d, Formatting.Indented);
            Program.GetMain().SetCrudContext(this, OperationType.Create, ResourceType.Document, x, CreateDocumentAsync);
        }

        void myMenuItemQueryDocument_Click(object sender, EventArgs e)
        {
            _currentQueryCommandContext = new CommandContext {IsFeed = true};

            // reset continuation token
            _currentContinuation = null;

            Program.GetMain().SetCrudContext(this, OperationType.Query, ResourceType.Document, "select * from c", QueryDocumentsAsync, _currentQueryCommandContext);
        }

        async void CreateDocumentAsync(object resource, RequestOptions requestOptions)
        {
            try
            {
                var document = JsonConvert.DeserializeObject(resource as string);
                var dc = Tag as DocumentCollection;
                var dcId = dc.Id;

                ResourceResponse<Document> newdocument;
                using (PerfStatus.Start("CreateDocument"))
                {
                    newdocument = await _client.CreateDocumentAsync(dc.GetLink(_client), document, requestOptions);
                }

                var hostName = _client.ServiceEndpoint.Host;
                var displayText = _customDocumentListDisplayManager.GetDisplayText(newdocument.Resource, hostName, dcId, _databaseId);

                Nodes.Add(
                    new ResourceNode(_client, newdocument.Resource, ResourceType.Document, dc.PartitionKey, displayText)
                );

                // set the result window
                var json = newdocument.Resource.ToString();
                json = DocumentHelper.RemoveInternalDocumentValues(json);

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

                q = CreateDocumentQuery(queryText, feedOptions);

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
                Nodes.Add(new StoredProcedureNode(_client));
                Nodes.Add(new UserDefinedFunctionNode(_client));
                Nodes.Add(new TriggerNode(_client));
                Nodes.Add(new ConflictNode(_client));

                FillWithChildren();
            }
        }

        public void FillWithChildren()
        {
            try
            {
                var docs = new List<dynamic>();
                NameValueCollection responseHeaders = null;
                var dc = (DocumentCollection)Tag;
                var dcId = dc.Id;

                using (PerfStatus.Start("ReadDocumentFeed"))
                {
                    var feedReader = CreateDocumentQuery("Select * from c", new FeedOptions {EnableCrossPartitionQuery = true});

                    while (feedReader.HasMoreResults && docs.Count() < 100)
                    {
                        FeedResponse<Document> response = feedReader.ExecuteNextAsync<Document>().Result;
                        docs.AddRange(response);

                        responseHeaders = response.ResponseHeaders;
                    }
                }

                var host = _client.ServiceEndpoint.Host;

                string customDocumentDisplayIdentifier;
                string sortField;
                bool reverseSort;
                var useCustom = _customDocumentListDisplayManager.GetCustomDocumentDisplayIdentifier(docs, host, dc.Id, _databaseId, out customDocumentDisplayIdentifier, out sortField, out reverseSort);

                DocumentHelper.SortDocuments(useCustom, docs, sortField, reverseSort);

                foreach (var doc in docs)
                {
                    if (useCustom)
                    {
                        var displayText = _customDocumentListDisplayManager.GetDisplayText(true, doc, customDocumentDisplayIdentifier);
                        var node = new ResourceNode(_client, doc, ResourceType.Document, dc.PartitionKey, displayText, dataBaseId: _databaseId, documentCollectionId: dcId);
                        Nodes.Add(node);
                    }
                    else
                    {
                        var node = new ResourceNode(_client, doc, ResourceType.Document, dc.PartitionKey, dataBaseId: _databaseId, documentCollectionId: dcId);
                        Nodes.Add(node); 
                    }
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

        private IDocumentQuery<dynamic> CreateDocumentQuery(string queryText, FeedOptions feedOptions)
        {
            return _client.CreateDocumentQuery((Tag as DocumentCollection).GetLink(_client), queryText, feedOptions).AsDocumentQuery();
        }

        public override void HandleNodeKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            var kv = keyEventArgs.KeyValue;
            var ctrl = keyEventArgs.Control;
            var shift = keyEventArgs.Shift;

            if (kv == 116) // F5
            {
                Refresh(true);
            }

            if (ctrl && kv == 78) // ctrl+n
            {
                InvokeCreateDocument();
            }

            if (ctrl && shift && kv == 78) // ctrl+n
            {
                InvokeCreatedDocumentWithId();
            }

        }

        public override void HandleNodeKeyPress(object sender, KeyPressEventArgs keyPressEventArgs)
        {
        }


    }  
}
