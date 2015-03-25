using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Azure.Documents;

namespace Microsoft.Azure.DocumentDBStudio
{
    public partial class IndexingPathForm : Form
    {
        private IndexingPath indexingPath;

        public IndexingPathForm()
        {
            InitializeComponent();
        }

        public void SetIndexingPath(IndexingPath indexingPath)
        {
            this.indexingPath = indexingPath;

            // init the path
            tbIndexingPathPath.Text = indexingPath.Path;
            if (indexingPath.IndexType == IndexType.Hash)
            {
                rbHash.Checked = true;
            }
            else
            {
                rbRange.Checked = true;
            }

            if (indexingPath.NumericPrecision != null)
            {
                tbNumericPrecision.Text = indexingPath.NumericPrecision.ToString();
            }

            if (indexingPath.StringPrecision != null)
            {
                tbNumericPrecision.Text = indexingPath.StringPrecision.ToString();
            }
        }

        public IndexingPath IndexingPath
        {
            get {return this.indexingPath; }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbIndexingPathPath.Text))
            {
                MessageBox.Show("Please input the valid path");
                this.DialogResult = DialogResult.None;
                return;
            }

            int? StringPrecision = null;
            if (!string.IsNullOrEmpty(tbStringPrecision.Text))
            {
                int x;
                if ( Int32.TryParse(tbStringPrecision.Text, out x))
                {
                    StringPrecision = x;
                }
                else
                {
                    MessageBox.Show("Please input the valid precision");
                    this.DialogResult = DialogResult.None;
                    return;
                }
            }

            int? NumericPrecision = null;
            if (!string.IsNullOrEmpty(tbNumericPrecision.Text))
            {
                int x;
                if (Int32.TryParse(tbNumericPrecision.Text, out x))
                {
                    NumericPrecision = x;
                }
                else
                {
                    MessageBox.Show("Please input the valid precision");
                    this.DialogResult = DialogResult.None;
                    return;
                }
            }

            if (this.indexingPath == null)
            {
                this.indexingPath = new IndexingPath();
            }

            this.indexingPath.Path = tbIndexingPathPath.Text;

            if (rbHash.Checked)
            {
                this.indexingPath.IndexType = IndexType.Hash;
            }
            else
            {
                this.indexingPath.IndexType = IndexType.Range;
            }
            this.indexingPath.NumericPrecision = NumericPrecision;
            this.indexingPath.StringPrecision = StringPrecision;

        }
    }
}
