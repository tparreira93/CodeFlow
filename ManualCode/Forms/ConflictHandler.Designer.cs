namespace CodeFlow
{
    partial class ConflictHandler
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConflictHandler));
            this.lstConflicts = new System.Windows.Forms.ListView();
            this.chCode = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnBack = new System.Windows.Forms.Button();
            this.btnUse = new System.Windows.Forms.Button();
            this.btnViewCode = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lstConflicts
            // 
            this.lstConflicts.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstConflicts.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chCode});
            this.lstConflicts.Location = new System.Drawing.Point(17, 16);
            this.lstConflicts.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.lstConflicts.Name = "lstConflicts";
            this.lstConflicts.Size = new System.Drawing.Size(872, 479);
            this.lstConflicts.TabIndex = 0;
            this.lstConflicts.UseCompatibleStateImageBehavior = false;
            this.lstConflicts.View = System.Windows.Forms.View.Details;
            this.lstConflicts.SelectedIndexChanged += new System.EventHandler(this.lstConflicts_SelectedIndexChanged);
            this.lstConflicts.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lstConflicts_MouseDoubleClick);
            // 
            // chCode
            // 
            this.chCode.Text = "Code";
            this.chCode.Width = 623;
            // 
            // btnBack
            // 
            this.btnBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBack.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnBack.Image = ((System.Drawing.Image)(resources.GetObject("btnBack.Image")));
            this.btnBack.Location = new System.Drawing.Point(791, 503);
            this.btnBack.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(100, 32);
            this.btnBack.TabIndex = 3;
            this.btnBack.Text = "Exit";
            this.btnBack.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // btnUse
            // 
            this.btnUse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUse.Image = ((System.Drawing.Image)(resources.GetObject("btnUse.Image")));
            this.btnUse.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUse.Location = new System.Drawing.Point(641, 503);
            this.btnUse.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnUse.Name = "btnUse";
            this.btnUse.Size = new System.Drawing.Size(141, 32);
            this.btnUse.TabIndex = 2;
            this.btnUse.Text = "Use selected";
            this.btnUse.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnUse.UseVisualStyleBackColor = true;
            this.btnUse.Click += new System.EventHandler(this.btnUse_Click);
            // 
            // btnViewCode
            // 
            this.btnViewCode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnViewCode.Image = ((System.Drawing.Image)(resources.GetObject("btnViewCode.Image")));
            this.btnViewCode.Location = new System.Drawing.Point(17, 503);
            this.btnViewCode.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnViewCode.Name = "btnViewCode";
            this.btnViewCode.Size = new System.Drawing.Size(128, 32);
            this.btnViewCode.TabIndex = 1;
            this.btnViewCode.Text = "View code";
            this.btnViewCode.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnViewCode.UseVisualStyleBackColor = true;
            this.btnViewCode.Click += new System.EventHandler(this.btnViewCode_Click);
            // 
            // ConflictHandler
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnBack;
            this.ClientSize = new System.Drawing.Size(907, 546);
            this.Controls.Add(this.btnViewCode);
            this.Controls.Add(this.btnUse);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.lstConflicts);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "ConflictHandler";
            this.Text = "Conflict viewer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConflictHandler_FormClosing);
            this.Load += new System.EventHandler(this.ConflictHandler_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lstConflicts;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Button btnUse;
        private System.Windows.Forms.Button btnViewCode;
        private System.Windows.Forms.ColumnHeader chCode;
    }
}