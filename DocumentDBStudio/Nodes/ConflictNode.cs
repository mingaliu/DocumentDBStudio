using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace Microsoft.Azure.DocumentDBStudio
{
    class ConflictNode : FeedNode
    {
        private readonly DocumentClient _client;
        private readonly ContextMenu _contextMenu = new ContextMenu();

        public ConflictNode(DocumentClient client)
        {
            Text = "Conflicts";
            _client = client;
            Nodes.Add(new TreeNode("fake"));
            Tag = "This represents the Conflicts feed.";
            ImageKey = "Conflict";
            SelectedImageKey = "Conflict";

            AddMenuItem("Refresh Conflict feed", (sender, e) => Refresh(true));
            // Query conflicts currrently fail due to gateway
            AddMenuItem("Query Conflict feed", myMenuItemQueryConflicts_Click);
        }

        private void AddMenuItem(string menuItemText, EventHandler eventHandler)
        {
            var menuItem = new MenuItem(menuItemText);
            menuItem.Click += eventHandler;
            _contextMenu.MenuItems.Add(menuItem);
        }

        public override void ShowContextMenu(TreeView treeview, Point p)
        {
            _contextMenu.Show(treeview, p);
        }

        void myMenuItemQueryConflicts_Click(object sender, EventArgs e)
        {
            Program.GetMain().SetCrudContext(this, OperationType.Query, ResourceType.Conflict, "select * from c", QueryConflictAsync);
        }

        async void QueryConflictAsync(object resource, RequestOptions requestOptions)
        {
            try
            {
                var queryText = resource as string;
                // text is the querytext.
                FeedResponse<Database> r;
                using (PerfStatus.Start("QueryConflicts"))
                {
                    var q = _client.CreateConflictQuery((Parent.Tag as DocumentCollection).GetLink(_client), queryText).AsDocumentQuery();
                    r = await q.ExecuteNextAsync<Database>();
                }

                // set the result window
                var text = string.Format(CultureInfo.InvariantCulture, "Returned {0} Conflict(s)", r.Count);

                var jsonarray = "[";
                var index = 0;
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

        public override void Refresh(bool forceRefresh)
        {
            if (forceRefresh || IsFirstTime)
            {
                IsFirstTime = false;
                Nodes.Clear();
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
                    feedConflicts = _client.ReadConflictFeedAsync((Parent.Tag as DocumentCollection).GetLink(_client)).Result;
                }

                foreach (var sp in feedConflicts)
                {
                    var node = new ResourceNode(_client, sp, ResourceType.Conflict);
                    Nodes.Add(node);
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

        public override void HandleNodeKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
        }

        public override void HandleNodeKeyPress(object sender, KeyPressEventArgs keyPressEventArgs)
        {
        }

    }
}
