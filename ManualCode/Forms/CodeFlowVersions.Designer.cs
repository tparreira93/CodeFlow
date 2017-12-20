namespace CodeFlow.Forms
{
    partial class CodeFlowChangesForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CodeFlowChangesForm));
            this.lstChanges = new System.Windows.Forms.ListView();
            this.clVersion = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.lblVersion = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lstChanges
            // 
            this.lstChanges.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstChanges.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clVersion,
            this.clDescription});
            this.lstChanges.FullRowSelect = true;
            this.lstChanges.GridLines = true;
            this.lstChanges.Location = new System.Drawing.Point(16, 38);
            this.lstChanges.Margin = new System.Windows.Forms.Padding(4);
            this.lstChanges.MultiSelect = false;
            this.lstChanges.Name = "lstChanges";
            this.lstChanges.Size = new System.Drawing.Size(1140, 673);
            this.lstChanges.SmallImageList = this.imageList1;
            this.lstChanges.TabIndex = 0;
            this.lstChanges.UseCompatibleStateImageBehavior = false;
            this.lstChanges.View = System.Windows.Forms.View.Details;
            // 
            // clVersion
            // 
            this.clVersion.Text = "Version";
            this.clVersion.Width = 64;
            // 
            // clDescription
            // 
            this.clDescription.Text = "Description";
            this.clDescription.Width = 1022;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "VSO_AddCircle_16x.png");
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVersion.Location = new System.Drawing.Point(16, 15);
            this.lblVersion.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(76, 17);
            this.lblVersion.TabIndex = 1;
            this.lblVersion.Text = "VERSION";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Image = global::CodeFlow.Properties.Resources.Close_16xLG;
            this.btnCancel.Location = new System.Drawing.Point(1057, 719);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 30);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Exit";
            this.btnCancel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // CodeFlowChangesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1173, 761);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.lstChanges);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "CodeFlowChangesForm";
            this.Text = "CodeFlow updates";
            this.Load += new System.EventHandler(this.CodeFlowChanges_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lstChanges;
        private System.Windows.Forms.ColumnHeader clDescription;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.ColumnHeader clVersion;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ImageList imageList1;
    }
}