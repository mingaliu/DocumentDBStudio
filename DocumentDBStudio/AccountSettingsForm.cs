using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Azure.DocumentDBStudio.Helpers;
using Microsoft.Azure.DocumentDBStudio.Properties;
using Microsoft.Azure.Documents.Client;

namespace Microsoft.Azure.DocumentDBStudio
{
    public partial class AccountSettingsForm : Form
    {
        public AccountSettingsForm()
        {
            InitializeComponent();
            AccountSettings = new AccountSettings();

        }

        internal string AccountEndpoint
        {
            get;
            set;
        }

        internal AccountSettings AccountSettings
        {
            get;
            set;
        }
        private void SettingsForm_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.AccountEndpoint))
            {
                // load from change settings.
                this.cbDevFabric.Enabled = false;
                tbAccountName.Enabled = false;
                tbAccountName.Text = this.AccountEndpoint;
                tbAccountSecret.Text = this.AccountSettings.MasterKey;

                if (this.AccountSettings.MasterKey == Constants.LocalEmulatorMasterkey)
                {
                    cbDevFabric.Checked = true;
                }

                if (this.AccountSettings.ConnectionMode == ConnectionMode.Gateway)
                {
                    radioButtonGateway.Checked = true;
                }
                else if (this.AccountSettings.Protocol == Protocol.Https)
                {
                    radioButtonDirectHttp.Checked = true;
                }
                else if (this.AccountSettings.Protocol == Protocol.Tcp)
                {
                    radioButtonDirectTcp.Checked = true;
                }

                cbNameBased.Checked = !this.AccountSettings.IsNameBased;
                tbAccountName.Text = this.AccountEndpoint;
                tbAccountSecret.Text = this.AccountSettings.MasterKey;
                this.cbEnableEndpointDiscovery.Checked = this.AccountSettings.EnableEndpointDiscovery;
            }
            else
            {
                radioButtonGateway.Checked = true;
                cbEnableEndpointDiscovery.Checked = false;

                this.cbNameBased.Checked = false;
                ApplyDevFabricSettings();
            }

            this.cbNameBased.Visible = true;
            this.cbDevFabric.Visible = true;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbAccountName.Text) || string.IsNullOrEmpty(tbAccountSecret.Text))
            {
                MessageBox.Show("Please input the valid account settings", Constants.ApplicationName);
                this.DialogResult = DialogResult.None;
            }
            this.AccountEndpoint = tbAccountName.Text;
            if (!this.cbEnableTokenLogin.Checked)
            {
                this.AccountSettings.MasterKey = tbAccountSecret.Text;
            }
            else
            {
                this.AccountSettings.DatabaseName = tbDbName.Text;
                ProcessTokensString(tbAccountSecret.Text, tbCollections.Text);
            }

            if (this.radioButtonGateway.Checked)
            {
                this.AccountSettings.ConnectionMode = ConnectionMode.Gateway;
            }
            else if (this.radioButtonDirectHttp.Checked)
            {
                this.AccountSettings.ConnectionMode = ConnectionMode.Direct;
                this.AccountSettings.Protocol = Protocol.Https;
            }
            else if (this.radioButtonDirectTcp.Checked)
            {
                this.AccountSettings.ConnectionMode = ConnectionMode.Direct;
                this.AccountSettings.Protocol = Protocol.Tcp;
            }

            this.AccountSettings.IsPersistedLocally = this.persistLocallyCheckBox.Checked;

            this.AccountSettings.EnableEndpointDiscovery = cbEnableEndpointDiscovery.Checked;
            this.AccountSettings.IsNameBased = !cbNameBased.Checked;

            Settings.Default.Save();
        }

        private void ProcessTokensString(string tokenText, string collections)
        {
            this.AccountSettings.collectionTokens = new List<KeyValuePair<string, string>>();
            string tokenBeggining = "type=resource";
            string[] collectionSplit = collections.Split(';');
            string[] tokens = tokenText.Split(new[] { tokenBeggining }, StringSplitOptions.RemoveEmptyEntries);
            var tokensAndCollections = tokens.Zip(collectionSplit, (token, collection) => new { Token = token, Collection = collection });
            foreach (var tokenCollection in tokensAndCollections)
            {
                string currentToken = String.Concat(tokenBeggining, tokenCollection.Token);
                this.AccountSettings.collectionTokens.Add(new KeyValuePair<string, string>(tokenCollection.Collection, currentToken));
            }
            this.AccountSettings.MasterKey = null;
        }

        private void cbDevFabric_CheckedChanged(object sender, EventArgs e)
        {
            ApplyDevFabricSettings();
        }

        private void cbEnableTokenLogin_CheckedChanged(object sender, EventArgs e)
        {
            ApplyResourceTokenSettings();
        }

        private void ApplyDevFabricSettings()
        {
            if (cbDevFabric.Checked)
            {
                tbAccountSecret.Enabled = false;
                tbAccountName.Text = Constants.LocalEmulatorEndpoint;
                tbAccountSecret.Text = Constants.LocalEmulatorMasterkey;
            }
            else
            {
                tbAccountSecret.Enabled = true;
                tbAccountName.Text = "";
                tbAccountSecret.Text = "";
            }
        }

        private void ApplyResourceTokenSettings()
        {
            if (cbEnableTokenLogin.Checked)
            {
                tbDbName.Visible = true;
                label1.Visible = true;
                tbCollections.Visible = true;
                label5.Visible = true;
            }
            else
            {
                tbDbName.Visible = false;
                label1.Visible = false;
                tbCollections.Visible = false;
                label5.Visible = false;
            }
        }
    }
}
