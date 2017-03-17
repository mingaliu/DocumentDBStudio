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

            switch(index.Kind)
            {
                case IndexKind.Hash:
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
                    break;
                case IndexKind.Range:
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
                    break;
                case IndexKind.Spatial:
                    {
                        this.rbSpatial.Checked = true;
                        switch(((SpatialIndex)index).DataType)
                        {
                            case DataType.Number: rbNumber.Checked=true; break;
                            case DataType.String : rbString.Checked=true; break;
                            case DataType.LineString: rbLineString.Checked=true; break;
                            case DataType.Point: rbPoint.Checked=true; break;
                            case DataType.Polygon: rbPolygon.Checked=true; break;
                        }
                    }
                    break;
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
            else if (this.rbRange.Checked)
            {
                this.index = new RangeIndex(this.rbNumber.Checked ? DataType.Number : DataType.String) { Precision = precision };
            }
            else
            {
                DataType target = DataType.String;
                if (rbNumber.Checked) target = DataType.Number;
                if (rbString.Checked) target = DataType.String ;
                if (rbPoint.Checked) target = DataType.Point;
                if (rbPolygon.Checked) target = DataType.Polygon;
                if (rbLineString.Checked) target = DataType.LineString;
                this.index = new SpatialIndex(target);
            }

            this.DialogResult = DialogResult.OK;
            return;
        }
    }
}
