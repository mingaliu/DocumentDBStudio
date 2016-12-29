using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace Microsoft.Azure.DocumentDBStudio
{
    class TriggerNode : FeedNode
    {
        private readonly DocumentClient _client;
        private readonly ContextMenu _contextMenu = new ContextMenu();

        public TriggerNode(DocumentClient client)
        {
            Text = "Triggers";
            _client = client;
            Nodes.Add(new TreeNode("fake"));
            Tag = "This represents the Triggers feed. Right click to add Trigger";
            ImageKey = "Feed";
            SelectedImageKey = "Feed";

            {
                var menuItem = new MenuItem("Create Trigger");
                menuItem.Click += myMenuItemCreateTrigger_Click;
                _contextMenu.MenuItems.Add(menuItem);
            }
            {
                var menuItem = new MenuItem("Create Trigger from file");
                menuItem.Click += myMenuItemCreateTriggerFromFile_Click;
                _contextMenu.MenuItems.Add(menuItem);
            }
            {
                var menuItem = new MenuItem("Refresh Triggers feed");
                menuItem.Click += (sender, e) => Refresh(true);
                _contextMenu.MenuItems.Add(menuItem);
            }
        }

        void myMenuItemCreateTriggerFromFile_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            var dr = ofd.ShowDialog();

            if (dr == DialogResult.OK)
            {
                var filename = ofd.FileName;
                // 
                var text = System.IO.File.ReadAllText(filename);

                Program.GetMain().SetCrudContext(this, OperationType.Create, ResourceType.Trigger, text, CreateTriggerAsync, new CommandContext() { IsCreateTrigger = true });
            }
        }

        void myMenuItemCreateTrigger_Click(object sender, EventArgs e)
        {
            Program.GetMain().SetCrudContext(this, OperationType.Create, ResourceType.Trigger,
                "function() { \r\n \r\n}", CreateTriggerAsync, new CommandContext() { IsCreateTrigger = true });
        }

        async void CreateTriggerAsync(object triggerObject, RequestOptions requestOptions)
        {
            try
            {
                var trigger = triggerObject as Trigger;

                ResourceResponse<Trigger> newtrigger;
                using (PerfStatus.Start("CreateTrigger"))
                {
                    newtrigger = await _client.CreateTriggerAsync((Parent.Tag as DocumentCollection).GetLink(_client), trigger, requestOptions);
                }

                Nodes.Add(new ResourceNode(_client, newtrigger.Resource, ResourceType.Trigger));

                // set the result window
                var json = newtrigger.Resource.ToString();

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
                FeedResponse<Trigger> sps;
                using (PerfStatus.Start("ReadTriggerFeed"))
                {
                    sps = _client.ReadTriggerFeedAsync((collnode.Tag as DocumentCollection).GetLink(_client)).Result;
                }

                foreach (var sp in sps)
                {
                    var node = new ResourceNode(_client, sp, ResourceType.Trigger);
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
