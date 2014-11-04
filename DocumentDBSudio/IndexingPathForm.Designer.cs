namespace Microsoft.Azure.DocumentDBStudio
{
    partial class IndexingPathForm
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
            this.tbIndexingPathPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbRange = new System.Windows.Forms.RadioButton();
            this.rbHash = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.tbNumericPrecision = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbStringPrecision = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbIndexingPathPath
            // 
            this.tbIndexingPathPath.Location = new System.Drawing.Point(58, 25);
            this.tbIndexingPathPath.Name = "tbIndexingPathPath";
            this.tbIndexingPathPath.Size = new System.Drawing.Size(280, 20);
            this.tbIndexingPathPath.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Path";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbRange);
            this.groupBox1.Controls.Add(this.rbHash);
            this.groupBox1.Location = new System.Drawing.Point(16, 63);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(116, 76);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "IndexType";
            // 
            // rbRange
            // 
            this.rbRange.AutoSize = true;
            this.rbRange.Location = new System.Drawing.Point(25, 43);
            this.rbRange.Name = "rbRange";
            this.rbRange.Size = new System.Drawing.Size(57, 17);
            this.rbRange.TabIndex = 1;
            this.rbRange.Text = "Range";
            this.rbRange.UseVisualStyleBackColor = true;
            // 
            // rbHash
            // 
            this.rbHash.AutoSize = true;
            this.rbHash.Checked = true;
            this.rbHash.Location = new System.Drawing.Point(25, 19);
            this.rbHash.Name = "rbHash";
            this.rbHash.Size = new System.Drawing.Size(50, 17);
            this.rbHash.TabIndex = 0;
            this.rbHash.TabStop = true;
            this.rbHash.Text = "Hash";
            this.rbHash.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(143, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "NumericPrecision";
            // 
            // tbNumericPrecision
            // 
            this.tbNumericPrecision.Location = new System.Drawing.Point(285, 67);
            this.tbNumericPrecision.Name = "tbNumericPrecision";
            this.tbNumericPrecision.Size = new System.Drawing.Size(53, 20);
            this.tbNumericPrecision.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(143, 106);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "StringPrecision";
            // 
            // tbStringPrecision
            // 
            this.tbStringPrecision.Location = new System.Drawing.Point(285, 106);
            this.tbStringPrecision.Name = "tbStringPrecision";
            this.tbStringPrecision.Size = new System.Drawing.Size(53, 20);
            this.tbStringPrecision.TabIndex = 5;
            // 
            // btnSave
            // 
            this.btnSave.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSave.Location = new System.Drawing.Point(175, 171);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 7;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(263, 171);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // IndexingPathForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(357, 220);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbStringPrecision);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbNumericPrecision);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbIndexingPathPath);
            this.Name = "IndexingPathForm";
            this.Text = "IndexingPath";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbIndexingPathPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbRange;
        private System.Windows.Forms.RadioButton rbHash;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbNumericPrecision;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbStringPrecision;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
    }
}