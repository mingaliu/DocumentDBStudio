﻿using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json;

namespace Microsoft.Azure.DocumentDBStudio
{
    class DatabaseAccountNode : FeedNode
    {
        private readonly DocumentClient _client;
        private readonly string _accountEndpoint;
        private readonly ContextMenu _contextMenu = new ContextMenu();

        public DatabaseAccountNode(string endpointName, DocumentClient client)
        {
            _accountEndpoint = endpointName;

            Text = endpointName;

            ImageKey = "DatabaseAccount";
            SelectedImageKey = "DatabaseAccount";

            _client = client;
            Tag = "This represents the DatabaseAccount. Right click to add Database";

            Nodes.Add(new OfferNode(_client));

            AddMenuItem("Read DatabaseAccount", myMenuItemReadDatabaseAccount_Click);
            AddMenuItem("Create Database", myMenuItemCreateDatabase_Click, Shortcut.CtrlN);
            AddMenuItem("Refresh Databases feed", (sender, e) => Refresh(true), Shortcut.F5);
            AddMenuItem("Query Database", myMenuItemQueryDatabase_Click, Shortcut.CtrlQ);

            _contextMenu.MenuItems.Add("-");

            AddMenuItem("Remove setting", myMenuItemRemoveDatabaseAccount_Click);
            AddMenuItem("Change setting", myMenuItemChangeSetting_Click);

            _contextMenu.MenuItems.Add("-");
            AddMenuItem("Collapse all", myMenuItemCollapseAll_Click);
        }

        private void myMenuItemCollapseAll_Click(object sender, EventArgs e)
        {
            this.Collapse();
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

        void myMenuItemChangeSetting_Click(object sender, EventArgs e)
        {
            Program.GetMain().ChangeAccountSettings(this, _accountEndpoint);
        }

        async void myMenuItemReadDatabaseAccount_Click(object sender, EventArgs eArgs)
        {
            try
            {
                DatabaseAccount databaseAccount;
                using (PerfStatus.Start("ReadDatabase"))
                {
                    databaseAccount = await _client.GetDatabaseAccountAsync();
                }
                // set the result window
                var json = JsonConvert.SerializeObject(databaseAccount, Formatting.Indented);

                Tag = databaseAccount;
                Program.GetMain().SetResultInBrowser(json, null, false);

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

        void myMenuItemCreateDatabase_Click(object sender, EventArgs e)
        {
            // 
            InvokeCreateDatabase();
        }

        private void InvokeCreateDatabase()
        {
            dynamic d = new System.Dynamic.ExpandoObject();
            d.id = "Here is your Database Id";
            string x = JsonConvert.SerializeObject(d, Formatting.Indented);
            Program.GetMain().SetCrudContext(this, OperationType.Create, ResourceType.Database, x, CreateDatabaseAsync);
        }

        void myMenuItemQueryDatabase_Click(object sender, EventArgs e)
        {
            InvokeQueryDatabase();
        }

        private void InvokeQueryDatabase()
        {
            Program.GetMain()
                .SetCrudContext(this, OperationType.Query, ResourceType.Database, "select * from c", QueryDatabasesAsync);
        }

        void myMenuItemRemoveDatabaseAccount_Click(object sender, EventArgs e)
        {
            // 
            Remove();
            Program.GetMain().RemoveAccountFromSettings(_accountEndpoint);
        }

        async void QueryDatabasesAsync(object resource, RequestOptions requestOptions)
        {
            try
            {
                var queryText = resource as string;

                FeedResponse<Database> r;
                using (PerfStatus.Start("QueryDatabase"))
                {
                    // text is the querytext.
                    var q = _client.CreateDatabaseQuery(queryText).AsDocumentQuery();
                    r = await q.ExecuteNextAsync<Database>();
                }

                // set the result window
                string text = null;
                text = string.Format(CultureInfo.InvariantCulture, "Returned {0} database(s)", r.Count);

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

        private async void FillWithChildren()
        {
            try
            {
                FeedResponse<Database> databases;
                using (PerfStatus.Start("ReadDatabaseFeed"))
                {
                    databases = await _client.ReadDatabaseFeedAsync();
                }

                //databases.Sort((first, second) => string.Compare(((Document)first).Id, ((Document)second).Id, StringComparison.Ordinal));
                //databases = databases.Sort()

                var dbNodeList = databases.Select(db => new DatabaseNode(_client, db)).ToList();
                dbNodeList.Sort((first, second) => string.Compare((first).Text, (second).Text, StringComparison.Ordinal));
                foreach (var databaseNode in dbNodeList)
                {
                    Nodes.Add(databaseNode);
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

                var offerNode = new OfferNode(_client);
                Nodes.Add(offerNode);

                FillWithChildren();
            }
        }

        async void CreateDatabaseAsync(object resource, RequestOptions requestOptions)
        {
            try
            {
                var text = resource as string;
                var db = (Database)JsonConvert.DeserializeObject(text, typeof(Database));

                ResourceResponse<Database> newdb;
                using (PerfStatus.Start("CreateDatabase"))
                {
                    newdb = await _client.CreateDatabaseAsync(db, requestOptions);
                }
                Nodes.Add(new DatabaseNode(_client, newdb.Resource));

                // set the result window
                var json = JsonConvert.SerializeObject(newdb.Resource, Formatting.Indented);

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

        public override void HandleNodeKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            var kv = keyEventArgs.KeyValue;
            var ctrl = keyEventArgs.Control;

            if (ctrl && kv == 78) // ctrl+n
            {
                InvokeCreateDatabase();
            }

            if (ctrl && kv == 81) // ctrl+q
            {
                InvokeQueryDatabase();
            }

            if (kv == 116) // F5
            {
                Refresh(true);
            }
        }

        public override void HandleNodeKeyPress(object sender, KeyPressEventArgs keyPressEventArgs)
        {
        }

    }
}
