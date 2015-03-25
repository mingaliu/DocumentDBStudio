using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using Microsoft.Azure.DocumentDBStudio.Properties;
using Microsoft.Azure.Documents.Client;

namespace Microsoft.Azure.DocumentDBStudio
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
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
            if (!string.IsNullOrEmpty(AccountEndpoint))
            {
                tbAccountName.Enabled = false;
                tbAccountName.Text = AccountEndpoint;
                tbAccountSecret.Text = AccountSettings.MasterKey;

                if (AccountEndpoint == Constants.LocalEmulatorEndpoint)
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
            }
            else
            {
                radioButtonGateway.Checked = true;
            }

            ApplyDevFabricSettings();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (cbDevFabric.Checked)
            {
                this.AccountEndpoint = Constants.LocalEmulatorEndpoint;
                this.AccountSettings.MasterKey = Constants.LocalEmulatorMasterkey;
            }
            else
            {
                if (string.IsNullOrEmpty(tbAccountName.Text) ||
                string.IsNullOrEmpty(tbAccountSecret.Text))
                {
                    MessageBox.Show("Please input the valid account settings", Constants.ApplicationName);
                    this.DialogResult = DialogResult.None;
                }
                this.AccountEndpoint = tbAccountName.Text;
                this.AccountSettings.MasterKey = tbAccountSecret.Text;
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

            Settings.Default.Save();
        }

        private void cbDevFabric_CheckedChanged(object sender, EventArgs e)
        {
            ApplyDevFabricSettings();
        }
        private void ApplyDevFabricSettings()
        {
            if (cbDevFabric.Checked)
            {
                tbAccountName.Enabled = false;
                tbAccountSecret.Enabled = false;
            }
            else
            {
                tbAccountName.Enabled = true;
                tbAccountSecret.Enabled = true;
            }
        }
    }
}
