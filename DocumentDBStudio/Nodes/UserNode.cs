using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;

namespace Microsoft.Azure.DocumentDBStudio
{
    class UserNode : FeedNode
    {
        private readonly DocumentClient _client;
        private readonly ContextMenu _contextMenu = new ContextMenu();

        public UserNode(DocumentClient client)
        {
            Text = "Users";
            _client = client;
            Nodes.Add(new TreeNode("fake"));
            Tag = "This represents the Users feed. Right click to add user";
            ImageKey = "User";
            SelectedImageKey = "User";

            {
                var menuItem = new MenuItem("Create User");
                menuItem.Click += myMenuItemCreateUser_Click;
                _contextMenu.MenuItems.Add(menuItem);
            }
            {
                var menuItem = new MenuItem("Refresh Users feed");
                menuItem.Click += (sender, e) => Refresh(true);
                _contextMenu.MenuItems.Add(menuItem);
            }
        }

        void myMenuItemCreateUser_Click(object sender, EventArgs e)
        {
            dynamic d = new System.Dynamic.ExpandoObject();
            d.id = "Here is your user Id";
            string x = JsonConvert.SerializeObject(d, Formatting.Indented);
            Program.GetMain().SetCrudContext(this, OperationType.Create, ResourceType.User, x, CreateUserAsync);
        }

        async void CreateUserAsync(object resource, RequestOptions requestOptions)
        {
            try
            {
                var user = (User)JsonConvert.DeserializeObject(resource as string, typeof(User));

                ResourceResponse<User> newUser;
                using (PerfStatus.Start("CreateUser"))
                {
                    newUser = await _client.CreateUserAsync((Parent.Tag as Database).GetLink(_client), user, requestOptions);
                }
                Nodes.Add(new ResourceNode(_client, newUser.Resource, ResourceType.User));

                // set the result window
                var json = newUser.Resource.ToString();

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
                FeedResponse<User> sps;
                using (PerfStatus.Start("ReadUser"))
                {
                    sps = _client.ReadUserFeedAsync((Parent.Tag as Database).GetLink(_client)).Result;
                }
                foreach (var sp in sps)
                {
                    var node = new ResourceNode(_client, sp, ResourceType.User);
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
