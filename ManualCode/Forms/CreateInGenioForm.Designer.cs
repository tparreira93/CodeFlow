namespace CodeFlow.Forms
{
    partial class CreateInGenioForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateInGenioForm));
            this.cmbPlataform = new System.Windows.Forms.ComboBox();
            this.chkSystem = new System.Windows.Forms.CheckBox();
            this.cmbModule = new System.Windows.Forms.ComboBox();
            this.cmbFeature = new System.Windows.Forms.ComboBox();
            this.chkInhibt = new System.Windows.Forms.CheckBox();
            this.txtFile = new System.Windows.Forms.TextBox();
            this.txtOrder = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.cmbType = new System.Windows.Forms.ComboBox();
            this.txtParam = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.rtCode = new System.Windows.Forms.RichTextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnCreate = new System.Windows.Forms.Button();
            this.status = new System.Windows.Forms.StatusStrip();
            this.lblProfile = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblProd = new System.Windows.Forms.Label();
            this.lblSolutionVersion = new System.Windows.Forms.Label();
            this.lblWarning = new System.Windows.Forms.Label();
            this.btnSelect = new System.Windows.Forms.Button();
            this.status.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmbPlataform
            // 
            this.cmbPlataform.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPlataform.FormattingEnabled = true;
            this.cmbPlataform.Location = new System.Drawing.Point(15, 26);
            this.cmbPlataform.Name = "cmbPlataform";
            this.cmbPlataform.Size = new System.Drawing.Size(112, 21);
            this.cmbPlataform.Sorted = true;
            this.cmbPlataform.TabIndex = 0;
            this.cmbPlataform.SelectedIndexChanged += new System.EventHandler(this.cmbPlataform_SelectedIndexChanged);
            // 
            // chkSystem
            // 
            this.chkSystem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkSystem.AutoSize = true;
            this.chkSystem.Location = new System.Drawing.Point(510, 28);
            this.chkSystem.Name = "chkSystem";
            this.chkSystem.Size = new System.Drawing.Size(60, 17);
            this.chkSystem.TabIndex = 3;
            this.chkSystem.Text = "System";
            this.chkSystem.UseVisualStyleBackColor = true;
            this.chkSystem.CheckedChanged += new System.EventHandler(this.chkSystem_CheckedChanged);
            // 
            // cmbModule
            // 
            this.cmbModule.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbModule.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbModule.FormattingEnabled = true;
            this.cmbModule.Location = new System.Drawing.Point(576, 26);
            this.cmbModule.Name = "cmbModule";
            this.cmbModule.Size = new System.Drawing.Size(88, 21);
            this.cmbModule.Sorted = true;
            this.cmbModule.TabIndex = 4;
            // 
            // cmbFeature
            // 
            this.cmbFeature.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbFeature.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFeature.FormattingEnabled = true;
            this.cmbFeature.Location = new System.Drawing.Point(669, 25);
            this.cmbFeature.Name = "cmbFeature";
            this.cmbFeature.Size = new System.Drawing.Size(121, 21);
            this.cmbFeature.Sorted = true;
            this.cmbFeature.TabIndex = 5;
            // 
            // chkInhibt
            // 
            this.chkInhibt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkInhibt.AutoSize = true;
            this.chkInhibt.Location = new System.Drawing.Point(791, 28);
            this.chkInhibt.Name = "chkInhibt";
            this.chkInhibt.Size = new System.Drawing.Size(54, 17);
            this.chkInhibt.TabIndex = 6;
            this.chkInhibt.Text = "Inhibit";
            this.chkInhibt.UseVisualStyleBackColor = true;
            // 
            // txtFile
            // 
            this.txtFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFile.Location = new System.Drawing.Point(851, 28);
            this.txtFile.Name = "txtFile";
            this.txtFile.Size = new System.Drawing.Size(80, 20);
            this.txtFile.TabIndex = 7;
            // 
            // txtOrder
            // 
            this.txtOrder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOrder.Location = new System.Drawing.Point(938, 28);
            this.txtOrder.Name = "txtOrder";
            this.txtOrder.Size = new System.Drawing.Size(70, 20);
            this.txtOrder.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Plataform";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(130, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Type";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(573, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Module";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(669, 10);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Feature";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(849, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(23, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "File";
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(933, 10);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(33, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Order";
            // 
            // cmbType
            // 
            this.cmbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbType.FormattingEnabled = true;
            this.cmbType.Location = new System.Drawing.Point(132, 26);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(168, 21);
            this.cmbType.Sorted = true;
            this.cmbType.TabIndex = 1;
            // 
            // txtParam
            // 
            this.txtParam.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtParam.Location = new System.Drawing.Point(340, 27);
            this.txtParam.Name = "txtParam";
            this.txtParam.Size = new System.Drawing.Size(168, 20);
            this.txtParam.TabIndex = 2;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(338, 10);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(60, 13);
            this.label7.TabIndex = 16;
            this.label7.Text = "Parameters";
            // 
            // rtCode
            // 
            this.rtCode.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtCode.Location = new System.Drawing.Point(15, 54);
            this.rtCode.Name = "rtCode";
            this.rtCode.Size = new System.Drawing.Size(991, 522);
            this.rtCode.TabIndex = 9;
            this.rtCode.Text = "";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Image = global::CodeFlow.Properties.Resources.Close_16xLG;
            this.btnCancel.Location = new System.Drawing.Point(936, 595);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(69, 24);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnCreate
            // 
            this.btnCreate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCreate.Image = global::CodeFlow.Properties.Resources.AddFile_16x;
            this.btnCreate.Location = new System.Drawing.Point(852, 595);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(79, 24);
            this.btnCreate.TabIndex = 10;
            this.btnCreate.Text = "Create";
            this.btnCreate.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // status
            // 
            this.status.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.status.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblProfile});
            this.status.Location = new System.Drawing.Point(0, 622);
            this.status.Name = "status";
            this.status.Size = new System.Drawing.Size(1016, 22);
            this.status.TabIndex = 21;
            this.status.Text = "statusStrip1";
            // 
            // lblProfile
            // 
            this.lblProfile.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProfile.ForeColor = System.Drawing.Color.MidnightBlue;
            this.lblProfile.Name = "lblProfile";
            this.lblProfile.Size = new System.Drawing.Size(53, 17);
            this.lblProfile.Text = "PROFILE";
            // 
            // lblProd
            // 
            this.lblProd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblProd.AutoSize = true;
            this.lblProd.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProd.Location = new System.Drawing.Point(13, 600);
            this.lblProd.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblProd.Name = "lblProd";
            this.lblProd.Size = new System.Drawing.Size(122, 13);
            this.lblProd.TabIndex = 23;
            this.lblProd.Text = "PRODUCTION ENV.";
            // 
            // lblSolutionVersion
            // 
            this.lblSolutionVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblSolutionVersion.AutoSize = true;
            this.lblSolutionVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSolutionVersion.Location = new System.Drawing.Point(13, 580);
            this.lblSolutionVersion.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblSolutionVersion.Name = "lblSolutionVersion";
            this.lblSolutionVersion.Size = new System.Drawing.Size(106, 13);
            this.lblSolutionVersion.TabIndex = 22;
            this.lblSolutionVersion.Text = "SOLUTION_INFO";
            // 
            // lblWarning
            // 
            this.lblWarning.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblWarning.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWarning.ForeColor = System.Drawing.Color.Red;
            this.lblWarning.Location = new System.Drawing.Point(543, 577);
            this.lblWarning.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblWarning.Name = "lblWarning";
            this.lblWarning.Size = new System.Drawing.Size(464, 15);
            this.lblWarning.TabIndex = 24;
            this.lblWarning.Text = "WARNING";
            this.lblWarning.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnSelect
            // 
            this.btnSelect.Image = global::CodeFlow.Properties.Resources.ArrangeSelection_16x;
            this.btnSelect.Location = new System.Drawing.Point(304, 26);
            this.btnSelect.Margin = new System.Windows.Forms.Padding(2);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(30, 22);
            this.btnSelect.TabIndex = 25;
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // CreateInGenioForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(1016, 644);
            this.Controls.Add(this.btnSelect);
            this.Controls.Add(this.lblWarning);
            this.Controls.Add(this.lblProd);
            this.Controls.Add(this.lblSolutionVersion);
            this.Controls.Add(this.status);
            this.Controls.Add(this.btnCreate);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.rtCode);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtParam);
            this.Controls.Add(this.cmbType);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtOrder);
            this.Controls.Add(this.txtFile);
            this.Controls.Add(this.chkInhibt);
            this.Controls.Add(this.cmbFeature);
            this.Controls.Add(this.cmbModule);
            this.Controls.Add(this.chkSystem);
            this.Controls.Add(this.cmbPlataform);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CreateInGenioForm";
            this.Text = "Create in Genio";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CreateInGenioForm_FormClosing);
            this.Load += new System.EventHandler(this.CreateInGenioForm_Load);
            this.status.ResumeLayout(false);
            this.status.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbPlataform;
        private System.Windows.Forms.CheckBox chkSystem;
        private System.Windows.Forms.ComboBox cmbModule;
        private System.Windows.Forms.ComboBox cmbFeature;
        private System.Windows.Forms.CheckBox chkInhibt;
        private System.Windows.Forms.TextBox txtFile;
        private System.Windows.Forms.TextBox txtOrder;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cmbType;
        private System.Windows.Forms.TextBox txtParam;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.RichTextBox rtCode;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.StatusStrip status;
        private System.Windows.Forms.ToolStripStatusLabel lblProfile;
        private System.Windows.Forms.Label lblProd;
        private System.Windows.Forms.Label lblSolutionVersion;
        private System.Windows.Forms.Label lblWarning;
        private System.Windows.Forms.Button btnSelect;
    }
}