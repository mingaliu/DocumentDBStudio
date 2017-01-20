using System;
using System.Windows.Forms;
using Microsoft.Azure.DocumentDBStudio.Properties;

namespace Microsoft.Azure.DocumentDBStudio
{
    public partial class AppSettingsForm : Form
    {
        public AppSettingsForm()
        {
            InitializeComponent();
            cbExpandJson.Checked = Settings.Default.ExpandPrettyPrintJson;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Settings.Default.ExpandPrettyPrintJson = cbExpandJson.Checked;
            Settings.Default.Save();

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.None;
            Close();
        }

        private void cbExpandJson_CheckedChanged(object sender, EventArgs e)
        {
        }
    }
}
