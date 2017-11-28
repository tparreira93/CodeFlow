namespace CodeFlow
{
    partial class ExportForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExportForm));
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnCompare = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.lstCode = new System.Windows.Forms.ListView();
            this.clCode = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblServer = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblManual = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnConflict = new System.Windows.Forms.Button();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Image = global::CodeFlow.Properties.Resources.Close_16xLG;
            this.btnCancel.Location = new System.Drawing.Point(636, 522);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(64, 24);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Exit";
            this.btnCancel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnCompare
            // 
            this.btnCompare.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCompare.Image = global::CodeFlow.Properties.Resources.Compare_16x;
            this.btnCompare.Location = new System.Drawing.Point(122, 522);
            this.btnCompare.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnCompare.Name = "btnCompare";
            this.btnCompare.Size = new System.Drawing.Size(138, 24);
            this.btnCompare.TabIndex = 1;
            this.btnCompare.Text = "Compare and commit";
            this.btnCompare.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCompare.UseVisualStyleBackColor = true;
            this.btnCompare.Click += new System.EventHandler(this.btnCompare_Click);
            // 
            // btnExport
            // 
            this.btnExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExport.Image = global::CodeFlow.Properties.Resources.Upload_gray_16x;
            this.btnExport.Location = new System.Drawing.Point(268, 522);
            this.btnExport.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(85, 24);
            this.btnExport.TabIndex = 2;
            this.btnExport.Text = "Commit";
            this.btnExport.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // lstCode
            // 
            this.lstCode.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstCode.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clCode});
            this.lstCode.FullRowSelect = true;
            this.lstCode.Location = new System.Drawing.Point(12, 12);
            this.lstCode.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.lstCode.Name = "lstCode";
            this.lstCode.Size = new System.Drawing.Size(686, 504);
            this.lstCode.TabIndex = 7;
            this.lstCode.UseCompatibleStateImageBehavior = false;
            this.lstCode.View = System.Windows.Forms.View.Details;
            this.lstCode.SelectedIndexChanged += new System.EventHandler(this.lstCode_SelectedIndexChanged);
            this.lstCode.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lstCode_MouseDoubleClick);
            // 
            // clCode
            // 
            this.clCode.Tag = "clCode";
            this.clCode.Text = "Code";
            this.clCode.Width = 500;
            // 
            // statusStrip
            // 
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblServer,
            this.toolStripStatusLabel1,
            this.lblManual});
            this.statusStrip.Location = new System.Drawing.Point(0, 550);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(712, 22);
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
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(589, 17);
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
            this.btnConflict.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConflict.Image = global::CodeFlow.Properties.Resources.Conflict_16x;
            this.btnConflict.Location = new System.Drawing.Point(12, 522);
            this.btnConflict.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnConflict.Name = "btnConflict";
            this.btnConflict.Size = new System.Drawing.Size(102, 24);
            this.btnConflict.TabIndex = 9;
            this.btnConflict.Text = "View conflict";
            this.btnConflict.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnConflict.UseVisualStyleBackColor = true;
            this.btnConflict.Click += new System.EventHandler(this.btnConflict_Click);
            // 
            // ExportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(712, 572);
            this.Controls.Add(this.btnConflict);
            this.Controls.Add(this.lstCode);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.btnCompare);
            this.Controls.Add(this.btnCancel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "ExportForm";
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
        private System.Windows.Forms.Button btnCompare;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.ListView lstCode;
        private System.Windows.Forms.ColumnHeader clCode;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.Button btnConflict;
        private System.Windows.Forms.ToolStripStatusLabel lblServer;
        private System.Windows.Forms.ToolStripStatusLabel lblManual;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
    }
}