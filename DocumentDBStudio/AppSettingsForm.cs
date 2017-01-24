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
            cbDocumentTreeCount.SelectedIndex = cbDocumentTreeCount.FindStringExact(Settings.Default.DocumentTreeCount.ToString());
            
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Settings.Default.ExpandPrettyPrintJson = cbExpandJson.Checked;
            Settings.Default.DocumentTreeCount = Convert.ToInt32(cbDocumentTreeCount.SelectedItem);
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
