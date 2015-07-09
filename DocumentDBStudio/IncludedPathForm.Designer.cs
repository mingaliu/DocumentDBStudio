namespace Microsoft.Azure.DocumentDBStudio
{
    partial class IncludedPathForm
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
            this.tbIncludedPathPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lbIndexes = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnAddIndexSpec = new System.Windows.Forms.Button();
            this.btnRemoveIndexSpec = new System.Windows.Forms.Button();
            this.btnEditIndexSpec = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tbIncludedPathPath
            // 
            this.tbIncludedPathPath.Location = new System.Drawing.Point(58, 25);
            this.tbIncludedPathPath.Name = "tbIncludedPathPath";
            this.tbIncludedPathPath.Size = new System.Drawing.Size(425, 20);
            this.tbIncludedPathPath.TabIndex = 0;
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
            // lbIndexes
            // 
            this.lbIndexes.FormattingEnabled = true;
            this.lbIndexes.Location = new System.Drawing.Point(58, 66);
            this.lbIndexes.Name = "lbIndexes";
            this.lbIndexes.ScrollAlwaysVisible = true;
            this.lbIndexes.Size = new System.Drawing.Size(425, 95);
            this.lbIndexes.TabIndex = 2;
            this.lbIndexes.SelectedIndexChanged += new System.EventHandler(this.lbIndexes_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Indexes";
            // 
            // btnAddIndexSpec
            // 
            this.btnAddIndexSpec.Location = new System.Drawing.Point(58, 167);
            this.btnAddIndexSpec.Name = "btnAddIndexSpec";
            this.btnAddIndexSpec.Size = new System.Drawing.Size(75, 23);
            this.btnAddIndexSpec.TabIndex = 4;
            this.btnAddIndexSpec.Text = "Add";
            this.btnAddIndexSpec.UseVisualStyleBackColor = true;
            this.btnAddIndexSpec.Click += new System.EventHandler(this.btnAddIndexSpec_Click);
            // 
            // btnRemoveIndexSpec
            // 
            this.btnRemoveIndexSpec.Enabled = false;
            this.btnRemoveIndexSpec.Location = new System.Drawing.Point(139, 167);
            this.btnRemoveIndexSpec.Name = "btnRemoveIndexSpec";
            this.btnRemoveIndexSpec.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveIndexSpec.TabIndex = 5;
            this.btnRemoveIndexSpec.Text = "Remove";
            this.btnRemoveIndexSpec.UseVisualStyleBackColor = true;
            this.btnRemoveIndexSpec.Click += new System.EventHandler(this.btnRemoveIndexSpec_Click);
            // 
            // btnEditIndexSpec
            // 
            this.btnEditIndexSpec.Enabled = false;
            this.btnEditIndexSpec.Location = new System.Drawing.Point(220, 167);
            this.btnEditIndexSpec.Name = "btnEditIndexSpec";
            this.btnEditIndexSpec.Size = new System.Drawing.Size(75, 23);
            this.btnEditIndexSpec.TabIndex = 6;
            this.btnEditIndexSpec.Text = "Edit";
            this.btnEditIndexSpec.UseVisualStyleBackColor = true;
            this.btnEditIndexSpec.Click += new System.EventHandler(this.btnEditIndexSpec_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(408, 211);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 7;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // IncludedPathForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(542, 246);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnEditIndexSpec);
            this.Controls.Add(this.btnRemoveIndexSpec);
            this.Controls.Add(this.btnAddIndexSpec);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lbIndexes);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbIncludedPathPath);
            this.Name = "IncludedPathForm";
            this.Text = "Included Path";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbIncludedPathPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox lbIndexes;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnAddIndexSpec;
        private System.Windows.Forms.Button btnRemoveIndexSpec;
        private System.Windows.Forms.Button btnEditIndexSpec;
        private System.Windows.Forms.Button btnSave;
    }
}