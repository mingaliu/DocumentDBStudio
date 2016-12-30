using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
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

        public DocumentCollectionNode(DocumentClient client, DocumentCollection coll)
        {
            Text = coll.Id;
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
            
            AddMenuItem("Create Document", myMenuItemCreateDocument_Click);
            AddMenuItem("Create Document with prefilled id", myMenuItemCreateDocumentWithId_Click);
            AddMenuItem("Create Document From File", myMenuItemCreateDocumentFromFile_Click);
            AddMenuItem("Create Multiple Documents From Folder", myMenuItemCreateDocumentsFromFolder_Click);

            _contextMenu.MenuItems.Add("-");

            AddMenuItem("Refresh Documents feed", (sender, e) => Refresh(true));
            AddMenuItem("Query Documents", myMenuItemQueryDocument_Click);
        }

        private void AddMenuItem(string menuItemText, EventHandler eventHandler)
        {
            var menuItem = new MenuItem(menuItemText);
            menuItem.Click += eventHandler;
            _contextMenu.MenuItems.Add(menuItem);
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
            var ofd = new OpenFileDialog {Multiselect = true};

            var dr = ofd.ShowDialog();

            if (dr == DialogResult.OK)
            {
                var status = string.Format(CultureInfo.InvariantCulture, "Create {0} documents in collection\r\n", ofd.FileNames.Length);
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
                            var newdocument = await _client.CreateDocumentAsync((Tag as DocumentCollection).GetLink(_client), document, Program.GetMain().GetRequestOptions());
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
            string x = JsonConvert.SerializeObject(d, Formatting.Indented);
            Program.GetMain().SetCrudContext(this, OperationType.Create, ResourceType.Document, x, CreateDocumentAsync);
        }

        void myMenuItemCreateDocumentWithId_Click(object sender, EventArgs e)
        {
            // 
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

                ResourceResponse<Document> newdocument;
                using (PerfStatus.Start("CreateDocument"))
                {
                    newdocument = await _client.CreateDocumentAsync((Tag as DocumentCollection).GetLink(_client), document, requestOptions);
                }

                Nodes.Add(
                    new ResourceNode(_client, newdocument.Resource, ResourceType.Document, ((DocumentCollection)Tag).PartitionKey, DocumentHelper.GetDisplayText(newdocument.Resource))
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

                q = _client.CreateDocumentQuery((Tag as DocumentCollection).GetLink(_client), queryText, feedOptions).AsDocumentQuery();

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

                using (PerfStatus.Start("ReadDocumentFeed"))
                {
                    IDocumentQuery<dynamic> feedReader = _client.CreateDocumentQuery((Tag as DocumentCollection).GetLink(_client), "Select * from c", 
                        new FeedOptions { EnableCrossPartitionQuery = true }).AsDocumentQuery();

                    while (feedReader.HasMoreResults && docs.Count() < 100)
                    {
                        FeedResponse<Document> response = feedReader.ExecuteNextAsync<Document>().Result;
                        docs.AddRange(response);

                        responseHeaders = response.ResponseHeaders;
                    }
                }

                string customDocumentDisplayIdentifier;
                var useCustom = DocumentHelper.GetCustomDocumentDisplayIdentifier(docs, out customDocumentDisplayIdentifier);

                DocumentHelper.SortDocuments(useCustom, docs, customDocumentDisplayIdentifier);

                foreach (var doc in docs)
                {
                    if (useCustom)
                    {
                        var displayText = DocumentHelper.GetDisplayText(true, doc, customDocumentDisplayIdentifier);
                        var node = new ResourceNode(_client, doc, ResourceType.Document, ((DocumentCollection)Tag).PartitionKey, displayText);
                        Nodes.Add(node);
                    }
                    else
                    {
                        var node = new ResourceNode(_client, doc, ResourceType.Document, ((DocumentCollection) Tag).PartitionKey);
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
    }  
}
