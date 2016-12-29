using System;
using System.Windows.Forms;
using Microsoft.Azure.Documents;
using System.Globalization;

namespace Microsoft.Azure.DocumentDBStudio
{
    public partial class IndexSpecsForm : Form
    {
        private Index index = null;

        public IndexSpecsForm()
        {
            InitializeComponent();
        }

        public void SetIndex(Index index)
        {
            this.index = index;

            if (index.Kind == IndexKind.Hash)
            {
                this.rbHash.Checked = true;
                if (((HashIndex)index).DataType == DataType.Number)
                {
                    this.rbNumber.Checked = true;
                }
                else
                {
                    this.rbString.Checked = true;
                }

                this.tbPrecision.Text = ((HashIndex)index).Precision.HasValue ? ((HashIndex)index).Precision.Value.ToString(CultureInfo.InvariantCulture) : string.Empty; 
            }
            else
            {
                this.rbRange.Checked = true;
                if (((RangeIndex)index).DataType == DataType.Number)
                {
                    this.rbNumber.Checked = true;
                }
                else
                {
                    this.rbString.Checked = true;
                }

                this.tbPrecision.Text = ((RangeIndex)index).Precision.HasValue ? ((RangeIndex)index).Precision.Value.ToString(CultureInfo.InvariantCulture) : string.Empty;
            }
        }

        public Index Index
        {
            get { return this.index; }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            short? precision = null;
            if (!string.IsNullOrEmpty(this.tbPrecision.Text))
            {
                short precisionValue;
                if (short.TryParse(this.tbPrecision.Text, out precisionValue))
                {
                    precision = precisionValue;
                }
                else
                {
                    MessageBox.Show("Please enter a valid precision value.");
                    this.DialogResult = DialogResult.None;
                    return;
                }
            }

            if (this.rbHash.Checked)
            {
                this.index = new HashIndex(this.rbNumber.Checked ? DataType.Number : DataType.String) { Precision = precision };
            }
            else
            {
                this.index = new RangeIndex(this.rbNumber.Checked ? DataType.Number : DataType.String) { Precision = precision };
            }

            this.DialogResult = DialogResult.OK;
            return;
        }
    }
}
