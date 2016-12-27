using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace Microsoft.Azure.DocumentDBStudio
{
    class OfferNode : FeedNode
    {
        private readonly DocumentClient _client;
        private readonly ContextMenu _contextMenu = new ContextMenu();

        public OfferNode(DocumentClient client)
        {
            Text = "Offers";
            _client = client;
            Nodes.Add(new TreeNode("fake"));
            Tag = "This represents the Offers feed.";
            ImageKey = "Offer";
            SelectedImageKey = "Offer";

            var menuItem = new MenuItem("Refresh Offer feed");
            menuItem.Click += (sender, e) => Refresh(true);
            _contextMenu.MenuItems.Add(menuItem);
        }

        public override void ShowContextMenu(TreeView treeview, Point p)
        {
            _contextMenu.Show(treeview, p);
        }

        void myMenuItemQueryOffers_Click(object sender, EventArgs e)
        {
            Program.GetMain().SetCrudContext(this, OperationType.Query, ResourceType.Offer, "select * from c", QueryOfferAsync);
        }

        async void QueryOfferAsync(object resource, RequestOptions requestOptions)
        {
            try
            {
                var queryText = resource as string;
                // text is the querytext.
                var q = _client.CreateOfferQuery(queryText).AsDocumentQuery();

                FeedResponse<Database> r;
                using (PerfStatus.Start("QueryOffer"))
                {
                    r = await q.ExecuteNextAsync<Database>();
                }
                // set the result window
                string text = null;
                if (r.Count > 1)
                {
                    text = string.Format(CultureInfo.InvariantCulture, "Returned {0} Offers", r.Count);
                }
                else
                {
                    text = string.Format(CultureInfo.InvariantCulture, "Returned {0} Offer", r.Count);
                }

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
                var feedOffers = _client.ReadOffersFeedAsync().Result;

                foreach (var sp in feedOffers)
                {
                    var node = new ResourceNode(_client, sp, ResourceType.Offer);
                    Nodes.Add(node);
                }
                Program.GetMain().SetResponseHeaders(feedOffers.ResponseHeaders);

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
