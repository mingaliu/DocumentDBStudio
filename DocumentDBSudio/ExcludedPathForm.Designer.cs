namespace Microsoft.Azure.DocumentDBStudio
{
    partial class ExcludedPathForm
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
            this.btnSaveExcludedPath = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.tbExcludedPath = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnSaveExcludedPath
            // 
            this.btnSaveExcludedPath.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSaveExcludedPath.Location = new System.Drawing.Point(119, 73);
            this.btnSaveExcludedPath.Name = "btnSaveExcludedPath";
            this.btnSaveExcludedPath.Size = new System.Drawing.Size(75, 23);
            this.btnSaveExcludedPath.TabIndex = 0;
            this.btnSaveExcludedPath.Text = "OK";
            this.btnSaveExcludedPath.UseVisualStyleBackColor = true;
            this.btnSaveExcludedPath.Click += new System.EventHandler(this.btnSaveExcludedPath_Click);
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Location = new System.Drawing.Point(200, 73);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // tbExcludedPath
            // 
            this.tbExcludedPath.Location = new System.Drawing.Point(24, 29);
            this.tbExcludedPath.Name = "tbExcludedPath";
            this.tbExcludedPath.Size = new System.Drawing.Size(248, 20);
            this.tbExcludedPath.TabIndex = 2;
            // 
            // ExcludedPath
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 108);
            this.Controls.Add(this.tbExcludedPath);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnSaveExcludedPath);
            this.Name = "ExcludedPath";
            this.Text = "ExcludedPath";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSaveExcludedPath;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox tbExcludedPath;
    }
}