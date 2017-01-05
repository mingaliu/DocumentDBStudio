namespace Microsoft.Azure.DocumentDBStudio
{
    partial class CustomDocumentListDisplayConfigurationForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomDocumentListDisplayConfigurationForm));
            this.label1 = new System.Windows.Forms.Label();
            this.tbDefaultGlobalFieldName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.gbCustom = new System.Windows.Forms.GroupBox();
            this.btnSettingToggle = new System.Windows.Forms.Button();
            this.lbSettingInfo = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lbExample = new System.Windows.Forms.Label();
            this.tbDisplayPattern = new System.Windows.Forms.TextBox();
            this.lblInfo2 = new System.Windows.Forms.Label();
            this.lblDisplayPattern = new System.Windows.Forms.Label();
            this.cbSortFields = new System.Windows.Forms.ComboBox();
            this.tbSortField = new System.Windows.Forms.TextBox();
            this.lblSortField = new System.Windows.Forms.Label();
            this.lblInfo1 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblCannotSaveHint = new System.Windows.Forms.Label();
            this.cbReverseSort = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.gbCustom.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 65);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(165, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Default field name (Global setting)";
            // 
            // tbDefaultGlobalFieldName
            // 
            this.tbDefaultGlobalFieldName.Location = new System.Drawing.Point(183, 63);
            this.tbDefaultGlobalFieldName.Name = "tbDefaultGlobalFieldName";
            this.tbDefaultGlobalFieldName.Size = new System.Drawing.Size(390, 20);
            this.tbDefaultGlobalFieldName.TabIndex = 1;
            this.tbDefaultGlobalFieldName.TextChanged += new System.EventHandler(this.tbDefaultGlobalFieldName_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(510, 39);
            this.label2.TabIndex = 2;
            this.label2.Text = resources.GetString("label2.Text");
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tbDefaultGlobalFieldName);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(11, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(590, 102);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Global setting for default field";
            // 
            // gbCustom
            // 
            this.gbCustom.Controls.Add(this.cbReverseSort);
            this.gbCustom.Controls.Add(this.btnSettingToggle);
            this.gbCustom.Controls.Add(this.lbSettingInfo);
            this.gbCustom.Controls.Add(this.label7);
            this.gbCustom.Controls.Add(this.lbExample);
            this.gbCustom.Controls.Add(this.tbDisplayPattern);
            this.gbCustom.Controls.Add(this.lblInfo2);
            this.gbCustom.Controls.Add(this.lblDisplayPattern);
            this.gbCustom.Controls.Add(this.cbSortFields);
            this.gbCustom.Controls.Add(this.tbSortField);
            this.gbCustom.Controls.Add(this.lblSortField);
            this.gbCustom.Controls.Add(this.lblInfo1);
            this.gbCustom.Location = new System.Drawing.Point(11, 120);
            this.gbCustom.Name = "gbCustom";
            this.gbCustom.Size = new System.Drawing.Size(590, 312);
            this.gbCustom.TabIndex = 4;
            this.gbCustom.TabStop = false;
            this.gbCustom.Text = "groupBox2";
            // 
            // btnSettingToggle
            // 
            this.btnSettingToggle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSettingToggle.Location = new System.Drawing.Point(15, 55);
            this.btnSettingToggle.Name = "btnSettingToggle";
            this.btnSettingToggle.Size = new System.Drawing.Size(113, 23);
            this.btnSettingToggle.TabIndex = 10;
            this.btnSettingToggle.Text = "btnSettingToggle";
            this.btnSettingToggle.UseVisualStyleBackColor = true;
            this.btnSettingToggle.Click += new System.EventHandler(this.btnSettingToggle_Click);
            // 
            // lbSettingInfo
            // 
            this.lbSettingInfo.AutoSize = true;
            this.lbSettingInfo.Location = new System.Drawing.Point(12, 28);
            this.lbSettingInfo.Name = "lbSettingInfo";
            this.lbSettingInfo.Size = new System.Drawing.Size(66, 13);
            this.lbSettingInfo.TabIndex = 9;
            this.lbSettingInfo.Text = "lbSettingInfo";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.label7.Location = new System.Drawing.Point(12, 277);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(80, 13);
            this.label7.TabIndex = 8;
            this.label7.Text = "Example output";
            // 
            // lbExample
            // 
            this.lbExample.AutoSize = true;
            this.lbExample.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbExample.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.lbExample.Location = new System.Drawing.Point(92, 277);
            this.lbExample.Name = "lbExample";
            this.lbExample.Size = new System.Drawing.Size(0, 13);
            this.lbExample.TabIndex = 7;
            // 
            // tbDisplayPattern
            // 
            this.tbDisplayPattern.Location = new System.Drawing.Point(95, 244);
            this.tbDisplayPattern.Name = "tbDisplayPattern";
            this.tbDisplayPattern.Size = new System.Drawing.Size(478, 20);
            this.tbDisplayPattern.TabIndex = 6;
            this.tbDisplayPattern.TextChanged += new System.EventHandler(this.tbDisplayPattern_TextChanged);
            // 
            // lblInfo2
            // 
            this.lblInfo2.AutoSize = true;
            this.lblInfo2.Location = new System.Drawing.Point(15, 183);
            this.lblInfo2.Name = "lblInfo2";
            this.lblInfo2.Size = new System.Drawing.Size(428, 52);
            this.lblInfo2.TabIndex = 5;
            this.lblInfo2.Text = resources.GetString("lblInfo2.Text");
            // 
            // lblDisplayPattern
            // 
            this.lblDisplayPattern.AutoSize = true;
            this.lblDisplayPattern.Location = new System.Drawing.Point(12, 246);
            this.lblDisplayPattern.Name = "lblDisplayPattern";
            this.lblDisplayPattern.Size = new System.Drawing.Size(77, 13);
            this.lblDisplayPattern.TabIndex = 4;
            this.lblDisplayPattern.Text = "Display pattern";
            // 
            // cbSortFields
            // 
            this.cbSortFields.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSortFields.FormattingEnabled = true;
            this.cbSortFields.Location = new System.Drawing.Point(298, 154);
            this.cbSortFields.Name = "cbSortFields";
            this.cbSortFields.Size = new System.Drawing.Size(183, 21);
            this.cbSortFields.TabIndex = 3;
            this.cbSortFields.SelectedIndexChanged += new System.EventHandler(this.cbSortFields_SelectedIndexChanged);
            // 
            // tbSortField
            // 
            this.tbSortField.Location = new System.Drawing.Point(95, 154);
            this.tbSortField.Name = "tbSortField";
            this.tbSortField.Size = new System.Drawing.Size(197, 20);
            this.tbSortField.TabIndex = 2;
            this.tbSortField.TextChanged += new System.EventHandler(this.tbSortField_TextChanged);
            // 
            // lblSortField
            // 
            this.lblSortField.AutoSize = true;
            this.lblSortField.Location = new System.Drawing.Point(12, 156);
            this.lblSortField.Name = "lblSortField";
            this.lblSortField.Size = new System.Drawing.Size(48, 13);
            this.lblSortField.TabIndex = 1;
            this.lblSortField.Text = "Sort field";
            // 
            // lblInfo1
            // 
            this.lblInfo1.AutoSize = true;
            this.lblInfo1.Location = new System.Drawing.Point(12, 95);
            this.lblInfo1.Name = "lblInfo1";
            this.lblInfo1.Size = new System.Drawing.Size(483, 39);
            this.lblInfo1.TabIndex = 0;
            this.lblInfo1.Text = resources.GetString("lblInfo1.Text");
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(526, 444);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 8;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(445, 444);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblCannotSaveHint
            // 
            this.lblCannotSaveHint.AutoSize = true;
            this.lblCannotSaveHint.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCannotSaveHint.Location = new System.Drawing.Point(13, 448);
            this.lblCannotSaveHint.Name = "lblCannotSaveHint";
            this.lblCannotSaveHint.Size = new System.Drawing.Size(112, 13);
            this.lblCannotSaveHint.TabIndex = 10;
            this.lblCannotSaveHint.Text = "lblCannotSaveHint";
            // 
            // cbReverseSort
            // 
            this.cbReverseSort.AutoSize = true;
            this.cbReverseSort.Location = new System.Drawing.Point(487, 155);
            this.cbReverseSort.Name = "cbReverseSort";
            this.cbReverseSort.Size = new System.Drawing.Size(86, 17);
            this.cbReverseSort.TabIndex = 11;
            this.cbReverseSort.Text = "Reverse sort";
            this.cbReverseSort.UseVisualStyleBackColor = true;
            this.cbReverseSort.CheckedChanged += new System.EventHandler(this.cbReverseSort_CheckedChanged);
            // 
            // CustomDocumentListDisplayConfigurationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(609, 475);
            this.ControlBox = false;
            this.Controls.Add(this.lblCannotSaveHint);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.gbCustom);
            this.Name = "CustomDocumentListDisplayConfigurationForm";
            this.Text = "CustomDocumentListDisplay";
            this.Load += new System.EventHandler(this.CustomDocumentListDisplayForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.gbCustom.ResumeLayout(false);
            this.gbCustom.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbDefaultGlobalFieldName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox gbCustom;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label lblInfo1;
        private System.Windows.Forms.Label lblSortField;
        private System.Windows.Forms.TextBox tbSortField;
        private System.Windows.Forms.ComboBox cbSortFields;
        private System.Windows.Forms.Label lblDisplayPattern;
        private System.Windows.Forms.TextBox tbDisplayPattern;
        private System.Windows.Forms.Label lblInfo2;
        private System.Windows.Forms.Label lbExample;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lbSettingInfo;
        private System.Windows.Forms.Button btnSettingToggle;
        private System.Windows.Forms.Label lblCannotSaveHint;
        private System.Windows.Forms.CheckBox cbReverseSort;
    }
}