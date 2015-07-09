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
            this.rbHash = new System.Windows.Forms.RadioButton();
            this.rbRange = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbString = new System.Windows.Forms.RadioButton();
            this.rbNumber = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.tbPrecision = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbRange);
            this.groupBox1.Controls.Add(this.rbHash);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(92, 87);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Index Kind";
            // 
            // rbHash
            // 
            this.rbHash.AutoSize = true;
            this.rbHash.Location = new System.Drawing.Point(18, 19);
            this.rbHash.Name = "rbHash";
            this.rbHash.Size = new System.Drawing.Size(50, 17);
            this.rbHash.TabIndex = 1;
            this.rbHash.TabStop = true;
            this.rbHash.Text = "Hash";
            this.rbHash.UseVisualStyleBackColor = true;
            // 
            // rbRange
            // 
            this.rbRange.AutoSize = true;
            this.rbRange.Location = new System.Drawing.Point(18, 54);
            this.rbRange.Name = "rbRange";
            this.rbRange.Size = new System.Drawing.Size(57, 17);
            this.rbRange.TabIndex = 2;
            this.rbRange.TabStop = true;
            this.rbRange.Text = "Range";
            this.rbRange.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rbString);
            this.groupBox2.Controls.Add(this.rbNumber);
            this.groupBox2.Location = new System.Drawing.Point(129, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(92, 87);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Data Type";
            // 
            // rbString
            // 
            this.rbString.AutoSize = true;
            this.rbString.Location = new System.Drawing.Point(18, 54);
            this.rbString.Name = "rbString";
            this.rbString.Size = new System.Drawing.Size(52, 17);
            this.rbString.TabIndex = 2;
            this.rbString.TabStop = true;
            this.rbString.Text = "String";
            this.rbString.UseVisualStyleBackColor = true;
            // 
            // rbNumber
            // 
            this.rbNumber.AutoSize = true;
            this.rbNumber.Checked = true;
            this.rbNumber.Location = new System.Drawing.Point(18, 19);
            this.rbNumber.Name = "rbNumber";
            this.rbNumber.Size = new System.Drawing.Size(62, 17);
            this.rbNumber.TabIndex = 1;
            this.rbNumber.TabStop = true;
            this.rbNumber.Text = "Number";
            this.rbNumber.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 122);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Precision";
            // 
            // tbPrecision
            // 
            this.tbPrecision.Location = new System.Drawing.Point(83, 119);
            this.tbPrecision.Name = "tbPrecision";
            this.tbPrecision.Size = new System.Drawing.Size(100, 20);
            this.tbPrecision.TabIndex = 3;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(83, 156);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // IndexSpecsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(238, 191);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.tbPrecision);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
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
    }
}