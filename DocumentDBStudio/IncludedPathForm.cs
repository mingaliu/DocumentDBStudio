using System;
using System.Windows.Forms;
using Microsoft.Azure.Documents;

namespace Microsoft.Azure.DocumentDBStudio
{
    public partial class IncludedPathForm : Form
    {
        private IncludedPath includedPath = null;

        public IncludedPathForm()
        {
            InitializeComponent();
        }

        public void SetIncludedPath(IncludedPath includedPath)
        {
            this.includedPath = includedPath;

            // init the path
            tbIncludedPathPath.Text = includedPath.Path;
            this.lbIndexes.Items.Clear();

            foreach (Index index in includedPath.Indexes)
            {
                this.lbIndexes.Items.Add(index);
            }
        }

        public IncludedPath IncludedPath
        {
            get { return this.includedPath; }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbIncludedPathPath.Text))
            {
                MessageBox.Show("Please input the valid path");
                this.DialogResult = DialogResult.None;
                return;
            }

            this.includedPath = new IncludedPath();

            includedPath.Path = tbIncludedPathPath.Text;

            foreach (object item in this.lbIndexes.Items)
            {
                Index index = item as Index;
                this.includedPath.Indexes.Add(index);
            }

            this.DialogResult = DialogResult.OK;
            return;
        }

        private void btnAddIndexSpec_Click(object sender, EventArgs e)
        {
            IndexSpecsForm dlg = new IndexSpecsForm();
            dlg.StartPosition = FormStartPosition.CenterParent;
            DialogResult dr = dlg.ShowDialog(this);
            if (dr == DialogResult.OK)
            {
                this.lbIndexes.Items.Add(dlg.Index);
            }
        }

        private void btnRemoveIndexSpec_Click(object sender, EventArgs e)
        {
            this.lbIndexes.Items.RemoveAt(this.lbIndexes.SelectedIndex);
        }

        private void btnEditIndexSpec_Click(object sender, EventArgs e)
        {
            Index index = this.lbIndexes.SelectedItem as Index;

            IndexSpecsForm dlg = new IndexSpecsForm();
            dlg.StartPosition = FormStartPosition.CenterParent;

            dlg.SetIndex(index);

            DialogResult dr = dlg.ShowDialog(this);
            if (dr == DialogResult.OK)
            {
                this.lbIndexes.Items[this.lbIndexes.SelectedIndex] = dlg.Index;
            }
        }

        private void lbIndexes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.lbIndexes.SelectedItem != null)
            {
                this.btnEditIndexSpec.Enabled = true;
                this.btnRemoveIndexSpec.Enabled = true;
            }
            else
            {
                this.btnEditIndexSpec.Enabled = false;
                this.btnRemoveIndexSpec.Enabled = false;
            }
        }
    }
}