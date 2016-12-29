using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace Microsoft.Azure.DocumentDBStudio
{
    class StoredProcedureNode : FeedNode
    {
        private readonly DocumentClient _client;
        private readonly ContextMenu _contextMenu = new ContextMenu();

        public StoredProcedureNode(DocumentClient client)
        {
            Text = "StoredProcedures";
            _client = client;
            Nodes.Add(new TreeNode("fake"));
            Tag = "This represents the StoredProcedure feed. Right click to add StoredProcedure";

            ImageKey = "Feed";
            SelectedImageKey = "Feed";

            {
                var menuItem = new MenuItem("Create StoredProcedure");
                menuItem.Click += myMenuItemAddStoredProcedure_Click;
                _contextMenu.MenuItems.Add(menuItem);
            }
            {
                var menuItem = new MenuItem("Create StoredProcedure From File");
                menuItem.Click += myMenuItemCreateStoredProcedureFromFile_Click;
                _contextMenu.MenuItems.Add(menuItem);
            }
            {
                var menuItem = new MenuItem("Refresh StoredProcedures feed");
                menuItem.Click += (sender, e) => Refresh(true);
                _contextMenu.MenuItems.Add(menuItem);
            }
        }

        void myMenuItemAddStoredProcedure_Click(object sender, EventArgs e)
        {
            Program.GetMain().SetCrudContext(this, OperationType.Create, ResourceType.StoredProcedure,
                "function() { \r\n \r\n}", CreateStoredProcedureAsync);
        }

        void myMenuItemCreateStoredProcedureFromFile_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            var dr = ofd.ShowDialog();

            if (dr == DialogResult.OK)
            {
                var filename = ofd.FileName;
                // 
                var text = System.IO.File.ReadAllText(filename);

                Program.GetMain().SetCrudContext(this, OperationType.Create, ResourceType.StoredProcedure, text, CreateStoredProcedureAsync);
            }
        }

        async void CreateStoredProcedureAsync(object resource, RequestOptions requestOptions)
        {
            try
            {
                var sp = resource as StoredProcedure;
                ResourceResponse<StoredProcedure> newsp;
                using (PerfStatus.Start("CreateStoredProcedure"))
                {
                    newsp = await _client.CreateStoredProcedureAsync((Parent.Tag as DocumentCollection).GetLink(_client), sp, requestOptions);
                }

                Nodes.Add(new ResourceNode(_client, newsp.Resource, ResourceType.StoredProcedure));

                // set the result window
                var json = newsp.Resource.ToString();

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
                FillWithChildren();
            }
        }

        public void FillWithChildren()
        {
            try
            {
                var collnode = (DocumentCollectionNode)Parent;
                FeedResponse<StoredProcedure> sps;
                using (PerfStatus.Start("ReadStoredProcedure"))
                {
                    sps = _client.ReadStoredProcedureFeedAsync((collnode.Tag as DocumentCollection).GetLink(_client)).Result;
                }

                foreach (var sp in sps)
                {
                    var node = new ResourceNode(_client, sp, ResourceType.StoredProcedure);
                    Nodes.Add(node);
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
}
