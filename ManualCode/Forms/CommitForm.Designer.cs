namespace CodeFlow
{
    partial class CommitForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CommitForm));
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnCommit = new System.Windows.Forms.Button();
            this.lstCode = new System.Windows.Forms.ListView();
            this.clOperation = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clCode = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clFile = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblServer = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblManual = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnConflict = new System.Windows.Forms.Button();
            this.lblSolutionVersion = new System.Windows.Forms.Label();
            this.lblProd = new System.Windows.Forms.Label();
            this.lblWarning = new System.Windows.Forms.Label();
            this.btnMerge = new System.Windows.Forms.Button();
            this.lblMerged = new System.Windows.Forms.Label();
            this.lblConflict = new System.Windows.Forms.Label();
            this.lblNotMerged = new System.Windows.Forms.Label();
            this.lblColors = new System.Windows.Forms.Label();
            this.lblDivis = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Image = global::CodeFlow.Properties.Resources.Close_16xLG;
            this.btnCancel.Location = new System.Drawing.Point(982, 592);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 24);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Exit";
            this.btnCancel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnCommit
            // 
            this.btnCommit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCommit.Enabled = false;
            this.btnCommit.Image = global::CodeFlow.Properties.Resources.Upload_gray_16x;
            this.btnCommit.Location = new System.Drawing.Point(12, 592);
            this.btnCommit.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnCommit.Name = "btnCommit";
            this.btnCommit.Size = new System.Drawing.Size(85, 24);
            this.btnCommit.TabIndex = 1;
            this.btnCommit.Text = "Commit";
            this.btnCommit.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCommit.UseVisualStyleBackColor = true;
            this.btnCommit.Click += new System.EventHandler(this.btnCommit_Click);
            // 
            // lstCode
            // 
            this.lstCode.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstCode.CheckBoxes = true;
            this.lstCode.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clOperation,
            this.clCode,
            this.clFile});
            this.lstCode.FullRowSelect = true;
            this.lstCode.GridLines = true;
            this.lstCode.Location = new System.Drawing.Point(12, 12);
            this.lstCode.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.lstCode.MultiSelect = false;
            this.lstCode.Name = "lstCode";
            this.lstCode.Size = new System.Drawing.Size(1042, 533);
            this.lstCode.SmallImageList = this.imageList1;
            this.lstCode.TabIndex = 0;
            this.lstCode.UseCompatibleStateImageBehavior = false;
            this.lstCode.View = System.Windows.Forms.View.Details;
            this.lstCode.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.lstCode_ItemChecked);
            this.lstCode.SelectedIndexChanged += new System.EventHandler(this.lstCode_SelectedIndexChanged);
            this.lstCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lstCode_KeyDown);
            this.lstCode.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lstCode_MouseDoubleClick);
            // 
            // clOperation
            // 
            this.clOperation.Text = "Operation";
            this.clOperation.Width = 171;
            // 
            // clCode
            // 
            this.clCode.Tag = "clCode";
            this.clCode.Text = "Code";
            this.clCode.Width = 603;
            // 
            // clFile
            // 
            this.clFile.Text = "File name";
            this.clFile.Width = 224;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Upload_gray_48x.png");
            this.imageList1.Images.SetKeyName(1, "StatusCriticalError_48x.png");
            this.imageList1.Images.SetKeyName(2, "VSO_Remove_16x.png");
            // 
            // statusStrip
            // 
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblServer,
            this.toolStripStatusLabel1,
            this.lblManual});
            this.statusStrip.Location = new System.Drawing.Point(0, 621);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1068, 22);
            this.statusStrip.TabIndex = 8;
            this.statusStrip.Text = "status";
            // 
            // lblServer
            // 
            this.lblServer.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblServer.ForeColor = System.Drawing.Color.MidnightBlue;
            this.lblServer.Name = "lblServer";
            this.lblServer.Size = new System.Drawing.Size(50, 17);
            this.lblServer.Text = "SERVER";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(945, 17);
            this.toolStripStatusLabel1.Spring = true;
            // 
            // lblManual
            // 
            this.lblManual.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblManual.ForeColor = System.Drawing.Color.Crimson;
            this.lblManual.Name = "lblManual";
            this.lblManual.Size = new System.Drawing.Size(58, 17);
            this.lblManual.Text = "MANUAL";
            // 
            // btnConflict
            // 
            this.btnConflict.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnConflict.Enabled = false;
            this.btnConflict.Image = global::CodeFlow.Properties.Resources.Conflict_16x;
            this.btnConflict.Location = new System.Drawing.Point(182, 592);
            this.btnConflict.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnConflict.Name = "btnConflict";
            this.btnConflict.Size = new System.Drawing.Size(98, 24);
            this.btnConflict.TabIndex = 4;
            this.btnConflict.Text = "View conflict";
            this.btnConflict.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnConflict.UseVisualStyleBackColor = true;
            this.btnConflict.Click += new System.EventHandler(this.btnConflict_Click);
            // 
            // lblSolutionVersion
            // 
            this.lblSolutionVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblSolutionVersion.AutoSize = true;
            this.lblSolutionVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSolutionVersion.Location = new System.Drawing.Point(8, 548);
            this.lblSolutionVersion.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblSolutionVersion.Name = "lblSolutionVersion";
            this.lblSolutionVersion.Size = new System.Drawing.Size(106, 13);
            this.lblSolutionVersion.TabIndex = 10;
            this.lblSolutionVersion.Text = "SOLUTION_INFO";
            // 
            // lblProd
            // 
            this.lblProd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblProd.AutoSize = true;
            this.lblProd.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProd.Location = new System.Drawing.Point(8, 568);
            this.lblProd.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblProd.Name = "lblProd";
            this.lblProd.Size = new System.Drawing.Size(122, 13);
            this.lblProd.TabIndex = 11;
            this.lblProd.Text = "PRODUCTION ENV.";
            // 
            // lblWarning
            // 
            this.lblWarning.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblWarning.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWarning.ForeColor = System.Drawing.Color.Red;
            this.lblWarning.Location = new System.Drawing.Point(683, 568);
            this.lblWarning.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblWarning.Name = "lblWarning";
            this.lblWarning.Size = new System.Drawing.Size(375, 15);
            this.lblWarning.TabIndex = 12;
            this.lblWarning.Text = "WARNING";
            this.lblWarning.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnMerge
            // 
            this.btnMerge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnMerge.Enabled = false;
            this.btnMerge.Image = global::CodeFlow.Properties.Resources.Join_16x;
            this.btnMerge.Location = new System.Drawing.Point(104, 592);
            this.btnMerge.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnMerge.Name = "btnMerge";
            this.btnMerge.Size = new System.Drawing.Size(70, 24);
            this.btnMerge.TabIndex = 3;
            this.btnMerge.Text = "Merge";
            this.btnMerge.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnMerge.UseVisualStyleBackColor = true;
            this.btnMerge.Click += new System.EventHandler(this.btnMerge_Click);
            // 
            // lblMerged
            // 
            this.lblMerged.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMerged.AutoSize = true;
            this.lblMerged.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMerged.ForeColor = System.Drawing.Color.DarkGreen;
            this.lblMerged.Location = new System.Drawing.Point(998, 548);
            this.lblMerged.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblMerged.Name = "lblMerged";
            this.lblMerged.Size = new System.Drawing.Size(60, 13);
            this.lblMerged.TabIndex = 14;
            this.lblMerged.Text = "MERGED";
            // 
            // lblConflict
            // 
            this.lblConflict.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblConflict.AutoSize = true;
            this.lblConflict.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblConflict.ForeColor = System.Drawing.Color.DarkRed;
            this.lblConflict.Location = new System.Drawing.Point(931, 548);
            this.lblConflict.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblConflict.Name = "lblConflict";
            this.lblConflict.Size = new System.Drawing.Size(67, 13);
            this.lblConflict.TabIndex = 15;
            this.lblConflict.Text = "CONFLICT";
            // 
            // lblNotMerged
            // 
            this.lblNotMerged.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNotMerged.AutoSize = true;
            this.lblNotMerged.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNotMerged.Location = new System.Drawing.Point(843, 548);
            this.lblNotMerged.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblNotMerged.Name = "lblNotMerged";
            this.lblNotMerged.Size = new System.Drawing.Size(90, 13);
            this.lblNotMerged.TabIndex = 16;
            this.lblNotMerged.Text = "NOT MERGED";
            // 
            // lblColors
            // 
            this.lblColors.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblColors.AutoSize = true;
            this.lblColors.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblColors.Location = new System.Drawing.Point(749, 548);
            this.lblColors.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblColors.Name = "lblColors";
            this.lblColors.Size = new System.Drawing.Size(93, 13);
            this.lblColors.TabIndex = 17;
            this.lblColors.Text = "Color schemes:";
            // 
            // lblDivis
            // 
            this.lblDivis.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDivis.AutoSize = true;
            this.lblDivis.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDivis.Location = new System.Drawing.Point(924, 547);
            this.lblDivis.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblDivis.Name = "lblDivis";
            this.lblDivis.Size = new System.Drawing.Size(10, 13);
            this.lblDivis.TabIndex = 18;
            this.lblDivis.Text = "|";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(991, 547);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(10, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "|";
            // 
            // CommitForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(1068, 643);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblDivis);
            this.Controls.Add(this.lblColors);
            this.Controls.Add(this.lblNotMerged);
            this.Controls.Add(this.lblConflict);
            this.Controls.Add(this.lblMerged);
            this.Controls.Add(this.btnMerge);
            this.Controls.Add(this.lblWarning);
            this.Controls.Add(this.lblProd);
            this.Controls.Add(this.lblSolutionVersion);
            this.Controls.Add(this.btnConflict);
            this.Controls.Add(this.lstCode);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.btnCommit);
            this.Controls.Add(this.btnCancel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "CommitForm";
            this.Text = "Commit";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ExportForm_FormClosing);
            this.Load += new System.EventHandler(this.ExportForm_Load);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnCommit;
        private System.Windows.Forms.ListView lstCode;
        private System.Windows.Forms.ColumnHeader clCode;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.Button btnConflict;
        private System.Windows.Forms.ToolStripStatusLabel lblServer;
        private System.Windows.Forms.ToolStripStatusLabel lblManual;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Label lblSolutionVersion;
        private System.Windows.Forms.Label lblProd;
        private System.Windows.Forms.Label lblWarning;
        private System.Windows.Forms.ColumnHeader clFile;
        private System.Windows.Forms.Button btnMerge;
        private System.Windows.Forms.Label lblMerged;
        private System.Windows.Forms.Label lblConflict;
        private System.Windows.Forms.Label lblNotMerged;
        private System.Windows.Forms.Label lblColors;
        private System.Windows.Forms.Label lblDivis;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ColumnHeader clOperation;
    }
}