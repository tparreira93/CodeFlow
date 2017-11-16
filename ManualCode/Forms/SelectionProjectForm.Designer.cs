namespace CodeFlow.Forms
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnAnalyze = new System.Windows.Forms.Button();
            this.chkSavedFiles = new System.Windows.Forms.CheckBox();
            this.treeProjects = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(578, 340);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(56, 26);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnAnalyze
            // 
            this.btnAnalyze.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAnalyze.Location = new System.Drawing.Point(518, 340);
            this.btnAnalyze.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnAnalyze.Name = "btnAnalyze";
            this.btnAnalyze.Size = new System.Drawing.Size(56, 26);
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
            this.chkSavedFiles.Location = new System.Drawing.Point(9, 340);
            this.chkSavedFiles.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.chkSavedFiles.Name = "chkSavedFiles";
            this.chkSavedFiles.Size = new System.Drawing.Size(100, 17);
            this.chkSavedFiles.TabIndex = 3;
            this.chkSavedFiles.Text = "Only saved files";
            this.chkSavedFiles.UseVisualStyleBackColor = true;
            this.chkSavedFiles.CheckedChanged += new System.EventHandler(this.chkSavedFiles_CheckedChanged);
            // 
            // treeProjects
            // 
            this.treeProjects.CheckBoxes = true;
            this.treeProjects.Location = new System.Drawing.Point(9, 10);
            this.treeProjects.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.treeProjects.Name = "treeProjects";
            this.treeProjects.Size = new System.Drawing.Size(626, 326);
            this.treeProjects.TabIndex = 4;
            // 
            // ProjectSelectionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(644, 375);
            this.Controls.Add(this.treeProjects);
            this.Controls.Add(this.chkSavedFiles);
            this.Controls.Add(this.btnAnalyze);
            this.Controls.Add(this.btnCancel);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "ProjectSelectionForm";
            this.Text = "Select project";
            this.Load += new System.EventHandler(this.SelectionProjectForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnAnalyze;
        private System.Windows.Forms.CheckBox chkSavedFiles;
        private System.Windows.Forms.TreeView treeProjects;
    }
}