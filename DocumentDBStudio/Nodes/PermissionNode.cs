using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace Microsoft.Azure.DocumentDBStudio
{
    class PermissionNode : FeedNode
    {
        private readonly DocumentClient _client;
        private readonly ContextMenu _contextMenu = new ContextMenu();

        public PermissionNode(DocumentClient client)
        {
            Text = "Permissions";
            _client = client;
            Nodes.Add(new TreeNode("fake"));
            Tag = "This represents the Permissions feed. Right click to add permission";
            ImageKey = "Permission";
            SelectedImageKey = "Permission";

            AddMenuItem("Create Permission", myMenuItemCreatePermission_Click);
            AddMenuItem("Refresh Permissions feed", (sender, e) => Refresh(true));
        }

        private void AddMenuItem(string menuItemText, EventHandler eventHandler)
        {
            var menuItem = new MenuItem(menuItemText);
            menuItem.Click += eventHandler;
            _contextMenu.MenuItems.Add(menuItem);
        }

        void myMenuItemCreatePermission_Click(object sender, EventArgs e)
        {
            var permission = new Permission
            {
                Id = "Here is your permission Id",
                PermissionMode = PermissionMode.Read,
                ResourceLink = "your resource link"
            };

            var bodytext = permission.ToString();

            Program.GetMain().SetCrudContext(this, OperationType.Create, ResourceType.Permission, bodytext, CreatePermissionAsync);
        }

        async void CreatePermissionAsync(object resource, RequestOptions requestOptions)
        {
            try
            {
                var permission = JsonSerializable.LoadFrom<Permission>(new MemoryStream(Encoding.UTF8.GetBytes(resource as string)));

                ResourceResponse<Permission> newtpermission;
                using (PerfStatus.Start("CreatePermission"))
                {
                    newtpermission = await _client.CreatePermissionAsync((Parent.Tag as Resource).GetLink(_client), permission, requestOptions);
                }
                Nodes.Add(new ResourceNode(_client, newtpermission.Resource, ResourceType.Permission));

                // set the result window
                var json = newtpermission.Resource.ToString();

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
                FeedResponse<Permission> sps;
                using (PerfStatus.Start("ReadPermission"))
                {
                    sps = _client.ReadPermissionFeedAsync((Parent.Tag as User).GetLink(_client)).Result;
                }

                foreach (var sp in sps)
                {
                    var node = new ResourceNode(_client, sp, ResourceType.Permission);
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
