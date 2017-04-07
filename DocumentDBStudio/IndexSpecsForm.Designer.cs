namespace Microsoft.Azure.DocumentDBStudio
{
    partial class IndexSpecsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbRange = new System.Windows.Forms.RadioButton();
            this.rbHash = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbString = new System.Windows.Forms.RadioButton();
            this.rbNumber = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.tbPrecision = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.rbSpatial = new System.Windows.Forms.RadioButton();
            this.rbPoint = new System.Windows.Forms.RadioButton();
            this.rbPolygon = new System.Windows.Forms.RadioButton();
            this.rbLineString = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbSpatial);
            this.groupBox1.Controls.Add(this.rbRange);
            this.groupBox1.Controls.Add(this.rbHash);
            this.groupBox1.Location = new System.Drawing.Point(16, 15);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Size = new System.Drawing.Size(123, 228);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Index Kind";
            // 
            // rbRange
            // 
            this.rbRange.AutoSize = true;
            this.rbRange.Location = new System.Drawing.Point(24, 66);
            this.rbRange.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rbRange.Name = "rbRange";
            this.rbRange.Size = new System.Drawing.Size(71, 21);
            this.rbRange.TabIndex = 2;
            this.rbRange.TabStop = true;
            this.rbRange.Text = "Range";
            this.rbRange.UseVisualStyleBackColor = true;
            // 
            // rbHash
            // 
            this.rbHash.AutoSize = true;
            this.rbHash.Location = new System.Drawing.Point(24, 23);
            this.rbHash.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rbHash.Name = "rbHash";
            this.rbHash.Size = new System.Drawing.Size(62, 21);
            this.rbHash.TabIndex = 1;
            this.rbHash.TabStop = true;
            this.rbHash.Text = "Hash";
            this.rbHash.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rbLineString);
            this.groupBox2.Controls.Add(this.rbPolygon);
            this.groupBox2.Controls.Add(this.rbPoint);
            this.groupBox2.Controls.Add(this.rbString);
            this.groupBox2.Controls.Add(this.rbNumber);
            this.groupBox2.Location = new System.Drawing.Point(172, 15);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Size = new System.Drawing.Size(123, 228);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Data Type";
            // 
            // rbString
            // 
            this.rbString.AutoSize = true;
            this.rbString.Location = new System.Drawing.Point(24, 66);
            this.rbString.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rbString.Name = "rbString";
            this.rbString.Size = new System.Drawing.Size(66, 21);
            this.rbString.TabIndex = 2;
            this.rbString.TabStop = true;
            this.rbString.Text = "String";
            this.rbString.UseVisualStyleBackColor = true;
            // 
            // rbNumber
            // 
            this.rbNumber.AutoSize = true;
            this.rbNumber.Checked = true;
            this.rbNumber.Location = new System.Drawing.Point(24, 23);
            this.rbNumber.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rbNumber.Name = "rbNumber";
            this.rbNumber.Size = new System.Drawing.Size(79, 21);
            this.rbNumber.TabIndex = 1;
            this.rbNumber.TabStop = true;
            this.rbNumber.Text = "Number";
            this.rbNumber.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(37, 262);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "Precision";
            // 
            // tbPrecision
            // 
            this.tbPrecision.Location = new System.Drawing.Point(112, 258);
            this.tbPrecision.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tbPrecision.Name = "tbPrecision";
            this.tbPrecision.Size = new System.Drawing.Size(132, 22);
            this.tbPrecision.TabIndex = 3;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(112, 297);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 28);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // rbSpatial
            // 
            this.rbSpatial.AutoSize = true;
            this.rbSpatial.Location = new System.Drawing.Point(24, 109);
            this.rbSpatial.Margin = new System.Windows.Forms.Padding(4);
            this.rbSpatial.Name = "rbSpatial";
            this.rbSpatial.Size = new System.Drawing.Size(72, 21);
            this.rbSpatial.TabIndex = 2;
            this.rbSpatial.TabStop = true;
            this.rbSpatial.Text = "Spatial";
            this.rbSpatial.UseVisualStyleBackColor = true;
            // 
            // rbPoint
            // 
            this.rbPoint.AutoSize = true;
            this.rbPoint.Location = new System.Drawing.Point(24, 109);
            this.rbPoint.Margin = new System.Windows.Forms.Padding(4);
            this.rbPoint.Name = "rbPoint";
            this.rbPoint.Size = new System.Drawing.Size(61, 21);
            this.rbPoint.TabIndex = 2;
            this.rbPoint.TabStop = true;
            this.rbPoint.Text = "Point";
            this.rbPoint.UseVisualStyleBackColor = true;
            // 
            // rbPolygon
            // 
            this.rbPolygon.AutoSize = true;
            this.rbPolygon.Location = new System.Drawing.Point(24, 152);
            this.rbPolygon.Margin = new System.Windows.Forms.Padding(4);
            this.rbPolygon.Name = "rbPolygon";
            this.rbPolygon.Size = new System.Drawing.Size(80, 21);
            this.rbPolygon.TabIndex = 2;
            this.rbPolygon.TabStop = true;
            this.rbPolygon.Text = "Polygon";
            this.rbPolygon.UseVisualStyleBackColor = true;
            // 
            // rbLineString
            // 
            this.rbLineString.AutoSize = true;
            this.rbLineString.Location = new System.Drawing.Point(24, 195);
            this.rbLineString.Margin = new System.Windows.Forms.Padding(4);
            this.rbLineString.Name = "rbLineString";
            this.rbLineString.Size = new System.Drawing.Size(93, 21);
            this.rbLineString.TabIndex = 2;
            this.rbLineString.TabStop = true;
            this.rbLineString.Text = "LineString";
            this.rbLineString.UseVisualStyleBackColor = true;
            // 
            // IndexSpecsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(317, 339);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.tbPrecision);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "IndexSpecsForm";
            this.Text = "Index Specs";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbRange;
        private System.Windows.Forms.RadioButton rbHash;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rbString;
        private System.Windows.Forms.RadioButton rbNumber;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbPrecision;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.RadioButton rbSpatial;
        private System.Windows.Forms.RadioButton rbLineString;
        private System.Windows.Forms.RadioButton rbPolygon;
        private System.Windows.Forms.RadioButton rbPoint;
    }
}