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
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.btnBack = new System.Windows.Forms.Button();
            this.btnUse = new System.Windows.Forms.Button();
            this.btnViewCode = new System.Windows.Forms.Button();
            this.btnMerge = new System.Windows.Forms.Button();
            this.clOper = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chCode = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clFileName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lstConflicts = new System.Windows.Forms.ListView();
            this.SuspendLayout();
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
            this.btnBack.Image = global::CodeFlowUI.Properties.Resources.Close_16xLG;
            this.btnBack.Location = new System.Drawing.Point(872, 516);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(60, 24);
            this.btnBack.TabIndex = 4;
            this.btnBack.Text = "Exit";
            this.btnBack.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // btnUse
            // 
            this.btnUse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUse.Image = global::CodeFlowUI.Properties.Resources.Checkmark_16x;
            this.btnUse.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUse.Location = new System.Drawing.Point(766, 516);
            this.btnUse.Name = "btnUse";
            this.btnUse.Size = new System.Drawing.Size(101, 24);
            this.btnUse.TabIndex = 3;
            this.btnUse.Text = "Use selected";
            this.btnUse.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnUse.UseVisualStyleBackColor = true;
            this.btnUse.Click += new System.EventHandler(this.btnUse_Click);
            // 
            // btnViewCode
            // 
            this.btnViewCode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnViewCode.Image = global::CodeFlowUI.Properties.Resources.PreviewWebTab_16x;
            this.btnViewCode.Location = new System.Drawing.Point(103, 515);
            this.btnViewCode.Name = "btnViewCode";
            this.btnViewCode.Size = new System.Drawing.Size(90, 24);
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
            this.btnMerge.Image = global::CodeFlowUI.Properties.Resources.PreviewWebTab_16x;
            this.btnMerge.Location = new System.Drawing.Point(13, 515);
            this.btnMerge.Name = "btnMerge";
            this.btnMerge.Size = new System.Drawing.Size(84, 24);
            this.btnMerge.TabIndex = 1;
            this.btnMerge.Text = "Merge";
            this.btnMerge.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnMerge.UseVisualStyleBackColor = true;
            this.btnMerge.Click += new System.EventHandler(this.btnMerge_Click);
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
            this.lstConflicts.Location = new System.Drawing.Point(13, 13);
            this.lstConflicts.MultiSelect = false;
            this.lstConflicts.Name = "lstConflicts";
            this.lstConflicts.Size = new System.Drawing.Size(921, 496);
            this.lstConflicts.SmallImageList = this.imageList1;
            this.lstConflicts.TabIndex = 0;
            this.lstConflicts.UseCompatibleStateImageBehavior = false;
            this.lstConflicts.View = System.Windows.Forms.View.Details;
            this.lstConflicts.SelectedIndexChanged += new System.EventHandler(this.lstConflicts_SelectedIndexChanged);
            this.lstConflicts.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lstConflicts_MouseDoubleClick);
            // 
            // ConflictForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnBack;
            this.ClientSize = new System.Drawing.Size(946, 547);
            this.Controls.Add(this.btnMerge);
            this.Controls.Add(this.btnViewCode);
            this.Controls.Add(this.btnUse);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.lstConflicts);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ConflictForm";
            this.Text = "Conflict viewer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConflictHandler_FormClosing);
            this.Load += new System.EventHandler(this.ConflictHandler_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Button btnUse;
        private System.Windows.Forms.Button btnViewCode;
        private System.Windows.Forms.Button btnMerge;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ColumnHeader clOper;
        private System.Windows.Forms.ColumnHeader chCode;
        private System.Windows.Forms.ColumnHeader clFileName;
        private System.Windows.Forms.ListView lstConflicts;
    }
}