namespace CodeFlowUI
{
    partial class ConflictForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConflictForm));
            this.lstConflicts = new System.Windows.Forms.ListView();
            this.clOper = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chCode = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clFileName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.btnBack = new System.Windows.Forms.Button();
            this.btnUse = new System.Windows.Forms.Button();
            this.btnViewCode = new System.Windows.Forms.Button();
            this.btnMerge = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lstConflicts
            // 
            this.lstConflicts.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstConflicts.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clOper,
            this.chCode,
            this.clFileName});
            this.lstConflicts.FullRowSelect = true;
            this.lstConflicts.GridLines = true;
            this.lstConflicts.Location = new System.Drawing.Point(17, 16);
            this.lstConflicts.Margin = new System.Windows.Forms.Padding(4);
            this.lstConflicts.MultiSelect = false;
            this.lstConflicts.Name = "lstConflicts";
            this.lstConflicts.Size = new System.Drawing.Size(1227, 610);
            this.lstConflicts.SmallImageList = this.imageList1;
            this.lstConflicts.TabIndex = 0;
            this.lstConflicts.UseCompatibleStateImageBehavior = false;
            this.lstConflicts.View = System.Windows.Forms.View.Details;
            this.lstConflicts.SelectedIndexChanged += new System.EventHandler(this.lstConflicts_SelectedIndexChanged);
            this.lstConflicts.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lstConflicts_MouseDoubleClick);
            // 
            // clOper
            // 
            this.clOper.Text = "Operation";
            this.clOper.Width = 187;
            // 
            // chCode
            // 
            this.chCode.Text = "Code";
            this.chCode.Width = 371;
            // 
            // clFileName
            // 
            this.clFileName.Text = "File name";
            this.clFileName.Width = 198;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Upload_gray_48x.png");
            this.imageList1.Images.SetKeyName(1, "StatusCriticalError_48x.png");
            this.imageList1.Images.SetKeyName(2, "VSO_Remove_16x.png");
            // 
            // btnBack
            // 
            this.btnBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBack.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnBack.Image = global::CodeFlow.Properties.Resources.Close_16xLG;
            this.btnBack.Location = new System.Drawing.Point(1163, 635);
            this.btnBack.Margin = new System.Windows.Forms.Padding(4);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(80, 30);
            this.btnBack.TabIndex = 4;
            this.btnBack.Text = "Exit";
            this.btnBack.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // btnUse
            // 
            this.btnUse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUse.Image = global::CodeFlow.Properties.Resources.Checkmark_16x;
            this.btnUse.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUse.Location = new System.Drawing.Point(1021, 635);
            this.btnUse.Margin = new System.Windows.Forms.Padding(4);
            this.btnUse.Name = "btnUse";
            this.btnUse.Size = new System.Drawing.Size(135, 30);
            this.btnUse.TabIndex = 3;
            this.btnUse.Text = "Use selected";
            this.btnUse.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnUse.UseVisualStyleBackColor = true;
            this.btnUse.Click += new System.EventHandler(this.btnUse_Click);
            // 
            // btnViewCode
            // 
            this.btnViewCode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnViewCode.Image = global::CodeFlow.Properties.Resources.PreviewWebTab_16x;
            this.btnViewCode.Location = new System.Drawing.Point(137, 634);
            this.btnViewCode.Margin = new System.Windows.Forms.Padding(4);
            this.btnViewCode.Name = "btnViewCode";
            this.btnViewCode.Size = new System.Drawing.Size(120, 30);
            this.btnViewCode.TabIndex = 2;
            this.btnViewCode.Text = "View code";
            this.btnViewCode.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnViewCode.UseVisualStyleBackColor = true;
            this.btnViewCode.Visible = false;
            this.btnViewCode.Click += new System.EventHandler(this.btnViewCode_Click);
            // 
            // btnMerge
            // 
            this.btnMerge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnMerge.Image = global::CodeFlow.Properties.Resources.PreviewWebTab_16x;
            this.btnMerge.Location = new System.Drawing.Point(17, 634);
            this.btnMerge.Margin = new System.Windows.Forms.Padding(4);
            this.btnMerge.Name = "btnMerge";
            this.btnMerge.Size = new System.Drawing.Size(112, 30);
            this.btnMerge.TabIndex = 1;
            this.btnMerge.Text = "Merge";
            this.btnMerge.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnMerge.UseVisualStyleBackColor = true;
            this.btnMerge.Click += new System.EventHandler(this.btnMerge_Click);
            // 
            // ConflictForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnBack;
            this.ClientSize = new System.Drawing.Size(1262, 673);
            this.Controls.Add(this.btnMerge);
            this.Controls.Add(this.btnViewCode);
            this.Controls.Add(this.btnUse);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.lstConflicts);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ConflictForm";
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
        private System.Windows.Forms.ColumnHeader clFileName;
        private System.Windows.Forms.Button btnMerge;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ColumnHeader clOper;
    }
}