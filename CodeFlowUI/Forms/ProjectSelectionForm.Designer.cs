namespace CodeFlowUI
{
    partial class ProjectSelectionForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProjectSelectionForm));
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnAnalyze = new System.Windows.Forms.Button();
            this.chkSavedFiles = new System.Windows.Forms.CheckBox();
            this.treeProjects = new System.Windows.Forms.TreeView();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.cancelAnal = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Image = global::CodeFlowUI.Properties.Resources.Close_16xLG;
            this.btnCancel.Location = new System.Drawing.Point(538, 469);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(81, 24);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnAnalyze
            // 
            this.btnAnalyze.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAnalyze.Image = global::CodeFlowUI.Properties.Resources.StartupProject_16x;
            this.btnAnalyze.Location = new System.Drawing.Point(454, 469);
            this.btnAnalyze.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnAnalyze.Name = "btnAnalyze";
            this.btnAnalyze.Size = new System.Drawing.Size(80, 24);
            this.btnAnalyze.TabIndex = 2;
            this.btnAnalyze.Text = "Analyze";
            this.btnAnalyze.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnAnalyze.UseVisualStyleBackColor = true;
            this.btnAnalyze.Click += new System.EventHandler(this.btnAnalyze_Click);
            // 
            // chkSavedFiles
            // 
            this.chkSavedFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkSavedFiles.AutoSize = true;
            this.chkSavedFiles.Location = new System.Drawing.Point(9, 474);
            this.chkSavedFiles.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.chkSavedFiles.Name = "chkSavedFiles";
            this.chkSavedFiles.Size = new System.Drawing.Size(100, 17);
            this.chkSavedFiles.TabIndex = 1;
            this.chkSavedFiles.Text = "Only saved files";
            this.chkSavedFiles.UseVisualStyleBackColor = true;
            this.chkSavedFiles.CheckedChanged += new System.EventHandler(this.chkSavedFiles_CheckedChanged);
            // 
            // treeProjects
            // 
            this.treeProjects.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeProjects.CheckBoxes = true;
            this.treeProjects.Location = new System.Drawing.Point(9, 10);
            this.treeProjects.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.treeProjects.Name = "treeProjects";
            this.treeProjects.Size = new System.Drawing.Size(610, 455);
            this.treeProjects.TabIndex = 0;
            this.treeProjects.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeProjects_AfterCheck);
            // 
            // statusStrip
            // 
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cancelAnal,
            this.toolProgress});
            this.statusStrip.Location = new System.Drawing.Point(0, 500);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Padding = new System.Windows.Forms.Padding(10, 0, 1, 0);
            this.statusStrip.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.statusStrip.Size = new System.Drawing.Size(628, 25);
            this.statusStrip.TabIndex = 4;
            this.statusStrip.Text = "statusStrip1";
            // 
            // cancelAnal
            // 
            this.cancelAnal.Enabled = false;
            this.cancelAnal.Image = global::CodeFlowUI.Properties.Resources.Cancel_16x;
            this.cancelAnal.Name = "cancelAnal";
            this.cancelAnal.Size = new System.Drawing.Size(20, 20);
            this.cancelAnal.Click += new System.EventHandler(this.cancelAnal_Click);
            // 
            // toolProgress
            // 
            this.toolProgress.Enabled = false;
            this.toolProgress.Name = "toolProgress";
            this.toolProgress.Size = new System.Drawing.Size(75, 19);
            // 
            // ProjectSelectionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(628, 525);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.treeProjects);
            this.Controls.Add(this.chkSavedFiles);
            this.Controls.Add(this.btnAnalyze);
            this.Controls.Add(this.btnCancel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "ProjectSelectionForm";
            this.Text = "Select project";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ProjectSelectionForm_FormClosing);
            this.Load += new System.EventHandler(this.SelectionProjectForm_LoadAsync);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnAnalyze;
        private System.Windows.Forms.CheckBox chkSavedFiles;
        private System.Windows.Forms.TreeView treeProjects;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel cancelAnal;
        private System.Windows.Forms.ToolStripProgressBar toolProgress;
    }
}