﻿namespace Microsoft.Azure.DocumentDBStudio
{
    partial class AccountSettingsForm
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
            this.label4 = new System.Windows.Forms.Label();
            this.tbAccountSecret = new System.Windows.Forms.TextBox();
            this.tbAccountName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.cbDevFabric = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonDirectTcp = new System.Windows.Forms.RadioButton();
            this.radioButtonDirectHttp = new System.Windows.Forms.RadioButton();
            this.radioButtonGateway = new System.Windows.Forms.RadioButton();
            this.cbNameBased = new System.Windows.Forms.CheckBox();
            this.cbEnableEndpointDiscovery = new System.Windows.Forms.CheckBox();
            this.persistLocallyCheckBox = new System.Windows.Forms.CheckBox();
            this.cbEnableTokenLogin = new System.Windows.Forms.CheckBox();
            this.tbDbName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tbCollections = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 31);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(395, 17);
            this.label4.TabIndex = 21;
            this.label4.Text = "Please enter you Azure DocumentDB endpoint and secretkey";
            // 
            // tbAccountSecret
            // 
            this.tbAccountSecret.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbAccountSecret.Location = new System.Drawing.Point(192, 98);
            this.tbAccountSecret.Margin = new System.Windows.Forms.Padding(4);
            this.tbAccountSecret.Name = "tbAccountSecret";
            this.tbAccountSecret.Size = new System.Drawing.Size(455, 22);
            this.tbAccountSecret.TabIndex = 2;
            // 
            // tbAccountName
            // 
            this.tbAccountName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbAccountName.Location = new System.Drawing.Point(192, 64);
            this.tbAccountName.Margin = new System.Windows.Forms.Padding(4);
            this.tbAccountName.Name = "tbAccountName";
            this.tbAccountName.Size = new System.Drawing.Size(455, 22);
            this.tbAccountName.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 98);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(104, 17);
            this.label3.TabIndex = 20;
            this.label3.Text = "AccountSecret:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 64);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(119, 17);
            this.label2.TabIndex = 19;
            this.label2.Text = "AccountEndpoint:";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(419, 391);
            this.btnOK.Margin = new System.Windows.Forms.Padding(4);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(100, 28);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(548, 391);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 28);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // cbDevFabric
            // 
            this.cbDevFabric.AllowDrop = true;
            this.cbDevFabric.AutoSize = true;
            this.cbDevFabric.Location = new System.Drawing.Point(19, 349);
            this.cbDevFabric.Margin = new System.Windows.Forms.Padding(4);
            this.cbDevFabric.Name = "cbDevFabric";
            this.cbDevFabric.Size = new System.Drawing.Size(147, 21);
            this.cbDevFabric.TabIndex = 25;
            this.cbDevFabric.Text = "Use local emulator";
            this.cbDevFabric.UseVisualStyleBackColor = true;
            this.cbDevFabric.CheckedChanged += new System.EventHandler(this.cbDevFabric_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonDirectTcp);
            this.groupBox1.Controls.Add(this.radioButtonDirectHttp);
            this.groupBox1.Controls.Add(this.radioButtonGateway);
            this.groupBox1.Location = new System.Drawing.Point(24, 205);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(501, 59);
            this.groupBox1.TabIndex = 26;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Connection";
            // 
            // radioButtonDirectTcp
            // 
            this.radioButtonDirectTcp.AutoSize = true;
            this.radioButtonDirectTcp.Location = new System.Drawing.Point(348, 23);
            this.radioButtonDirectTcp.Margin = new System.Windows.Forms.Padding(4);
            this.radioButtonDirectTcp.Name = "radioButtonDirectTcp";
            this.radioButtonDirectTcp.Size = new System.Drawing.Size(97, 21);
            this.radioButtonDirectTcp.TabIndex = 2;
            this.radioButtonDirectTcp.TabStop = true;
            this.radioButtonDirectTcp.Text = "Direct TCP";
            this.radioButtonDirectTcp.UseVisualStyleBackColor = true;
            // 
            // radioButtonDirectHttp
            // 
            this.radioButtonDirectHttp.AutoSize = true;
            this.radioButtonDirectHttp.Location = new System.Drawing.Point(172, 23);
            this.radioButtonDirectHttp.Margin = new System.Windows.Forms.Padding(4);
            this.radioButtonDirectHttp.Name = "radioButtonDirectHttp";
            this.radioButtonDirectHttp.Size = new System.Drawing.Size(116, 21);
            this.radioButtonDirectHttp.TabIndex = 1;
            this.radioButtonDirectHttp.TabStop = true;
            this.radioButtonDirectHttp.Text = "Direct HTTPS";
            this.radioButtonDirectHttp.UseVisualStyleBackColor = true;
            // 
            // radioButtonGateway
            // 
            this.radioButtonGateway.AutoSize = true;
            this.radioButtonGateway.Location = new System.Drawing.Point(21, 23);
            this.radioButtonGateway.Margin = new System.Windows.Forms.Padding(4);
            this.radioButtonGateway.Name = "radioButtonGateway";
            this.radioButtonGateway.Size = new System.Drawing.Size(84, 21);
            this.radioButtonGateway.TabIndex = 0;
            this.radioButtonGateway.TabStop = true;
            this.radioButtonGateway.Text = "Gateway";
            this.radioButtonGateway.UseVisualStyleBackColor = true;
            // 
            // cbNameBased
            // 
            this.cbNameBased.AllowDrop = true;
            this.cbNameBased.AutoSize = true;
            this.cbNameBased.Checked = true;
            this.cbNameBased.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbNameBased.Location = new System.Drawing.Point(19, 286);
            this.cbNameBased.Margin = new System.Windows.Forms.Padding(4);
            this.cbNameBased.Name = "cbNameBased";
            this.cbNameBased.Size = new System.Drawing.Size(147, 21);
            this.cbNameBased.TabIndex = 27;
            this.cbNameBased.Text = "Use legacy selflink";
            this.cbNameBased.UseVisualStyleBackColor = true;
            // 
            // cbEnableEndpointDiscovery
            // 
            this.cbEnableEndpointDiscovery.AllowDrop = true;
            this.cbEnableEndpointDiscovery.AutoSize = true;
            this.cbEnableEndpointDiscovery.Location = new System.Drawing.Point(192, 349);
            this.cbEnableEndpointDiscovery.Margin = new System.Windows.Forms.Padding(4);
            this.cbEnableEndpointDiscovery.Name = "cbEnableEndpointDiscovery";
            this.cbEnableEndpointDiscovery.Size = new System.Drawing.Size(215, 21);
            this.cbEnableEndpointDiscovery.TabIndex = 28;
            this.cbEnableEndpointDiscovery.Text = "Automatic endpoint discovery";
            this.cbEnableEndpointDiscovery.UseVisualStyleBackColor = true;
            // 
            // persistLocallyCheckBox
            // 
            this.persistLocallyCheckBox.AutoSize = true;
            this.persistLocallyCheckBox.Checked = true;
            this.persistLocallyCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.persistLocallyCheckBox.Location = new System.Drawing.Point(192, 286);
            this.persistLocallyCheckBox.Margin = new System.Windows.Forms.Padding(4);
            this.persistLocallyCheckBox.Name = "persistLocallyCheckBox";
            this.persistLocallyCheckBox.Size = new System.Drawing.Size(197, 21);
            this.persistLocallyCheckBox.TabIndex = 29;
            this.persistLocallyCheckBox.Text = "Persist account info locally";
            this.persistLocallyCheckBox.UseVisualStyleBackColor = true;
            // 
            // cbEnableTokenLogin
            // 
            this.cbEnableTokenLogin.AllowDrop = true;
            this.cbEnableTokenLogin.AutoSize = true;
            this.cbEnableTokenLogin.Location = new System.Drawing.Point(419, 286);
            this.cbEnableTokenLogin.Margin = new System.Windows.Forms.Padding(4);
            this.cbEnableTokenLogin.Name = "cbEnableTokenLogin";
            this.cbEnableTokenLogin.Size = new System.Drawing.Size(164, 21);
            this.cbEnableTokenLogin.TabIndex = 30;
            this.cbEnableTokenLogin.Text = "Use Resource Token";
            this.cbEnableTokenLogin.UseVisualStyleBackColor = true;
            this.cbEnableTokenLogin.CheckedChanged += new System.EventHandler(this.cbEnableTokenLogin_CheckedChanged);
            // 
            // tbDbName
            // 
            this.tbDbName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbDbName.Location = new System.Drawing.Point(192, 133);
            this.tbDbName.Margin = new System.Windows.Forms.Padding(4);
            this.tbDbName.Name = "tbDbName";
            this.tbDbName.Size = new System.Drawing.Size(455, 22);
            this.tbDbName.TabIndex = 31;
            this.tbDbName.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 133);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(112, 17);
            this.label1.TabIndex = 32;
            this.label1.Text = "Database name:";
            this.label1.Visible = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(21, 169);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(80, 17);
            this.label5.TabIndex = 33;
            this.label5.Text = "Collections:";
            this.label5.Visible = false;
            // 
            // tbCollections
            // 
            this.tbCollections.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbCollections.Location = new System.Drawing.Point(192, 169);
            this.tbCollections.Margin = new System.Windows.Forms.Padding(4);
            this.tbCollections.Name = "tbCollections";
            this.tbCollections.Size = new System.Drawing.Size(455, 22);
            this.tbCollections.TabIndex = 34;
            this.tbCollections.Visible = false;
            // 
            // AccountSettingsForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(664, 434);
            this.Controls.Add(this.tbCollections);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbDbName);
            this.Controls.Add(this.cbEnableTokenLogin);
            this.Controls.Add(this.persistLocallyCheckBox);
            this.Controls.Add(this.cbEnableEndpointDiscovery);
            this.Controls.Add(this.cbNameBased);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.cbDevFabric);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbAccountSecret);
            this.Controls.Add(this.tbAccountName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "AccountSettingsForm";
            this.Text = "Account Settings";
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label label4;
        public System.Windows.Forms.TextBox tbAccountSecret;
        public System.Windows.Forms.TextBox tbAccountName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox cbDevFabric;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButtonGateway;
        private System.Windows.Forms.RadioButton radioButtonDirectTcp;
        private System.Windows.Forms.RadioButton radioButtonDirectHttp;
        private System.Windows.Forms.CheckBox cbNameBased;
        private System.Windows.Forms.CheckBox cbEnableEndpointDiscovery;
        private System.Windows.Forms.CheckBox persistLocallyCheckBox;
        private System.Windows.Forms.CheckBox cbEnableTokenLogin;
        public System.Windows.Forms.TextBox tbDbName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        public System.Windows.Forms.TextBox tbCollections;
    }
}