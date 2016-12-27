using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace Microsoft.Azure.DocumentDBStudio
{
    class UserDefinedFunctionNode : FeedNode
    {
        private readonly DocumentClient _client;
        private readonly ContextMenu _contextMenu = new ContextMenu();

        public UserDefinedFunctionNode(DocumentClient client)
        {
            Text = "UDFs";
            _client = client;
            Nodes.Add(new TreeNode("fake"));
            Tag = "This represents the UserDefinedFunction feed. Right click to add UserDefinedFunction";
            ImageKey = "Feed";
            SelectedImageKey = "Feed";

            {
                var menuItem = new MenuItem("Create UserDefinedFunction");
                menuItem.Click += myMenuItemCreateUDF_Click;
                _contextMenu.MenuItems.Add(menuItem);
            }
            {
                var menuItem = new MenuItem("Create UserDefinedFunction from File");
                menuItem.Click += myMenuItemCreateUDFFromFile_Click;
                _contextMenu.MenuItems.Add(menuItem);
            }
            {
                var menuItem = new MenuItem("Refresh UserDefinedFunction feed");
                menuItem.Click += (sender, e) => Refresh(true);
                _contextMenu.MenuItems.Add(menuItem);
            }
        }

        void myMenuItemCreateUDF_Click(object sender, EventArgs e)
        {
            Program.GetMain().SetCrudContext(this, OperationType.Create, ResourceType.UserDefinedFunction,
                "function() { \r\n \r\n}", CreateUDFAsync);
        }

        void myMenuItemCreateUDFFromFile_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            var dr = ofd.ShowDialog();

            if (dr == DialogResult.OK)
            {
                var filename = ofd.FileName;
                // 
                var text = System.IO.File.ReadAllText(filename);

                Program.GetMain().SetCrudContext(this, OperationType.Create, ResourceType.UserDefinedFunction, text, CreateUDFAsync);
            }
        }
        async void CreateUDFAsync(object resource, RequestOptions requestOptions)
        {
            try
            {
                var udf = resource as UserDefinedFunction;

                ResourceResponse<UserDefinedFunction> newudf;
                using (PerfStatus.Start("CreateUDF"))
                {
                    newudf = await _client.CreateUserDefinedFunctionAsync((Parent.Tag as DocumentCollection).GetLink(_client), udf, requestOptions);
                }

                Nodes.Add(new ResourceNode(_client, newudf.Resource, ResourceType.UserDefinedFunction));

                // set the result window
                var json = newudf.Resource.ToString();

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
                FeedResponse<UserDefinedFunction> sps;
                using (PerfStatus.Start("ReadUdfFeed"))
                {
                    sps = _client.ReadUserDefinedFunctionFeedAsync((collnode.Tag as DocumentCollection).GetLink(_client)).Result;
                }

                foreach (var sp in sps)
                {
                    var node = new ResourceNode(_client, sp, ResourceType.UserDefinedFunction);
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
