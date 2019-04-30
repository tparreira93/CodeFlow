namespace CodeFlowUI
{
    partial class ProfileForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProfileForm));
            this.lblServerName = new System.Windows.Forms.Label();
            this.lblDb = new System.Windows.Forms.Label();
            this.lblUser = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.lblPass = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.cmbServers = new System.Windows.Forms.ComboBox();
            this.cmbDb = new System.Windows.Forms.ComboBox();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnTry = new System.Windows.Forms.Button();
            this.txtGenioUser = new System.Windows.Forms.TextBox();
            this.lblGenioUser = new System.Windows.Forms.Label();
            this.txtConfigName = new System.Windows.Forms.TextBox();
            this.lblConfigName = new System.Windows.Forms.Label();
            this.txtGenioPath = new System.Windows.Forms.TextBox();
            this.lblGenioPath = new System.Windows.Forms.Label();
            this.chkProd = new System.Windows.Forms.CheckBox();
            this.btnRules = new System.Windows.Forms.Button();
            this.btnSelectFolder = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblServerName
            // 
            this.lblServerName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblServerName.AutoSize = true;
            this.lblServerName.Location = new System.Drawing.Point(13, 48);
            this.lblServerName.Name = "lblServerName";
            this.lblServerName.Size = new System.Drawing.Size(38, 13);
            this.lblServerName.TabIndex = 2;
            this.lblServerName.Text = "Server";
            // 
            // lblDb
            // 
            this.lblDb.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDb.AutoSize = true;
            this.lblDb.Location = new System.Drawing.Point(13, 167);
            this.lblDb.Name = "lblDb";
            this.lblDb.Size = new System.Drawing.Size(53, 13);
            this.lblDb.TabIndex = 8;
            this.lblDb.Text = "Database";
            // 
            // lblUser
            // 
            this.lblUser.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblUser.AutoSize = true;
            this.lblUser.Location = new System.Drawing.Point(13, 89);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(55, 13);
            this.lblUser.TabIndex = 4;
            this.lblUser.Text = "Username";
            // 
            // txtUsername
            // 
            this.txtUsername.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUsername.Location = new System.Drawing.Point(15, 106);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(503, 20);
            this.txtUsername.TabIndex = 2;
            // 
            // lblPass
            // 
            this.lblPass.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPass.AutoSize = true;
            this.lblPass.Location = new System.Drawing.Point(13, 128);
            this.lblPass.Name = "lblPass";
            this.lblPass.Size = new System.Drawing.Size(53, 13);
            this.lblPass.TabIndex = 6;
            this.lblPass.Text = "Password";
            // 
            // txtPassword
            // 
            this.txtPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPassword.Location = new System.Drawing.Point(15, 145);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(503, 20);
            this.txtPassword.TabIndex = 3;
            this.txtPassword.UseSystemPasswordChar = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Image = global::CodeFlowUI.Properties.Resources.Close_16xLG;
            this.btnCancel.Location = new System.Drawing.Point(444, 310);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 24);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Image = global::CodeFlowUI.Properties.Resources.save_16xLG;
            this.btnSave.Location = new System.Drawing.Point(363, 310);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 24);
            this.btnSave.TabIndex = 10;
            this.btnSave.Text = "Save";
            this.btnSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // cmbServers
            // 
            this.cmbServers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbServers.FormattingEnabled = true;
            this.cmbServers.Location = new System.Drawing.Point(15, 64);
            this.cmbServers.Name = "cmbServers";
            this.cmbServers.Size = new System.Drawing.Size(503, 21);
            this.cmbServers.TabIndex = 1;
            this.cmbServers.SelectedIndexChanged += new System.EventHandler(this.cmbServers_SelectedIndexChanged);
            this.cmbServers.MouseClick += new System.Windows.Forms.MouseEventHandler(this.cmbServers_MouseClick);
            // 
            // cmbDb
            // 
            this.cmbDb.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbDb.FormattingEnabled = true;
            this.cmbDb.Location = new System.Drawing.Point(16, 184);
            this.cmbDb.Name = "cmbDb";
            this.cmbDb.Size = new System.Drawing.Size(503, 21);
            this.cmbDb.TabIndex = 4;
            this.cmbDb.MouseClick += new System.Windows.Forms.MouseEventHandler(this.cmbDb_MouseClick);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.Image = global::CodeFlowUI.Properties.Resources.refresh_16xLG;
            this.btnRefresh.Location = new System.Drawing.Point(282, 310);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 24);
            this.btnRefresh.TabIndex = 9;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnTry
            // 
            this.btnTry.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnTry.Image = global::CodeFlowUI.Properties.Resources.UserTrigger_16x;
            this.btnTry.Location = new System.Drawing.Point(15, 310);
            this.btnTry.Name = "btnTry";
            this.btnTry.Size = new System.Drawing.Size(72, 24);
            this.btnTry.TabIndex = 8;
            this.btnTry.Text = "Try it!";
            this.btnTry.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnTry.UseVisualStyleBackColor = true;
            this.btnTry.Click += new System.EventHandler(this.btnTry_Click);
            // 
            // txtGenioUser
            // 
            this.txtGenioUser.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtGenioUser.Location = new System.Drawing.Point(15, 222);
            this.txtGenioUser.Name = "txtGenioUser";
            this.txtGenioUser.Size = new System.Drawing.Size(503, 20);
            this.txtGenioUser.TabIndex = 5;
            // 
            // lblGenioUser
            // 
            this.lblGenioUser.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblGenioUser.AutoSize = true;
            this.lblGenioUser.Location = new System.Drawing.Point(13, 206);
            this.lblGenioUser.Name = "lblGenioUser";
            this.lblGenioUser.Size = new System.Drawing.Size(60, 13);
            this.lblGenioUser.TabIndex = 10;
            this.lblGenioUser.Text = "User Genio";
            // 
            // txtConfigName
            // 
            this.txtConfigName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtConfigName.Location = new System.Drawing.Point(16, 25);
            this.txtConfigName.Name = "txtConfigName";
            this.txtConfigName.Size = new System.Drawing.Size(503, 20);
            this.txtConfigName.TabIndex = 0;
            // 
            // lblConfigName
            // 
            this.lblConfigName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblConfigName.AutoSize = true;
            this.lblConfigName.Location = new System.Drawing.Point(13, 9);
            this.lblConfigName.Name = "lblConfigName";
            this.lblConfigName.Size = new System.Drawing.Size(98, 13);
            this.lblConfigName.TabIndex = 0;
            this.lblConfigName.Text = "Configuration name";
            // 
            // txtGenioPath
            // 
            this.txtGenioPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtGenioPath.Location = new System.Drawing.Point(15, 261);
            this.txtGenioPath.Name = "txtGenioPath";
            this.txtGenioPath.Size = new System.Drawing.Size(422, 20);
            this.txtGenioPath.TabIndex = 6;
            // 
            // lblGenioPath
            // 
            this.lblGenioPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblGenioPath.AutoSize = true;
            this.lblGenioPath.Location = new System.Drawing.Point(13, 245);
            this.lblGenioPath.Name = "lblGenioPath";
            this.lblGenioPath.Size = new System.Drawing.Size(59, 13);
            this.lblGenioPath.TabIndex = 12;
            this.lblGenioPath.Text = "Genio path";
            // 
            // chkProd
            // 
            this.chkProd.AutoSize = true;
            this.chkProd.Location = new System.Drawing.Point(16, 287);
            this.chkProd.Name = "chkProd";
            this.chkProd.Size = new System.Drawing.Size(112, 17);
            this.chkProd.TabIndex = 7;
            this.chkProd.Text = "Production system";
            this.chkProd.UseVisualStyleBackColor = true;
            // 
            // btnRules
            // 
            this.btnRules.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRules.Image = global::CodeFlowUI.Properties.Resources.Rule_16x;
            this.btnRules.Location = new System.Drawing.Point(93, 310);
            this.btnRules.Name = "btnRules";
            this.btnRules.Size = new System.Drawing.Size(86, 24);
            this.btnRules.TabIndex = 13;
            this.btnRules.Text = "Rules";
            this.btnRules.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnRules.UseVisualStyleBackColor = true;
            this.btnRules.Click += new System.EventHandler(this.btnRules_Click);
            // 
            // btnSelectFolder
            // 
            this.btnSelectFolder.Image = global::CodeFlowUI.Properties.Resources.folder_Open_16xLG;
            this.btnSelectFolder.Location = new System.Drawing.Point(442, 258);
            this.btnSelectFolder.Name = "btnSelectFolder";
            this.btnSelectFolder.Size = new System.Drawing.Size(75, 24);
            this.btnSelectFolder.TabIndex = 14;
            this.btnSelectFolder.Text = "Select";
            this.btnSelectFolder.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSelectFolder.UseVisualStyleBackColor = true;
            this.btnSelectFolder.Click += new System.EventHandler(this.btnSelectFolder_Click);
            // 
            // ProfileForm
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(527, 343);
            this.Controls.Add(this.btnSelectFolder);
            this.Controls.Add(this.btnRules);
            this.Controls.Add(this.chkProd);
            this.Controls.Add(this.txtGenioPath);
            this.Controls.Add(this.lblGenioPath);
            this.Controls.Add(this.txtConfigName);
            this.Controls.Add(this.lblConfigName);
            this.Controls.Add(this.txtGenioUser);
            this.Controls.Add(this.lblGenioUser);
            this.Controls.Add(this.btnTry);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.cmbDb);
            this.Controls.Add(this.cmbServers);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.lblPass);
            this.Controls.Add(this.txtUsername);
            this.Controls.Add(this.lblUser);
            this.Controls.Add(this.lblDb);
            this.Controls.Add(this.lblServerName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProfileForm";
            this.Text = "Genio profile";
            this.Load += new System.EventHandler(this.ConnectionForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblServerName;
        private System.Windows.Forms.Label lblDb;
        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label lblPass;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.ComboBox cmbServers;
        private System.Windows.Forms.ComboBox cmbDb;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnTry;
        private System.Windows.Forms.TextBox txtGenioUser;
        private System.Windows.Forms.Label lblGenioUser;
        private System.Windows.Forms.TextBox txtConfigName;
        private System.Windows.Forms.Label lblConfigName;
        private System.Windows.Forms.TextBox txtGenioPath;
        private System.Windows.Forms.Label lblGenioPath;
        private System.Windows.Forms.CheckBox chkProd;
        private System.Windows.Forms.Button btnRules;
        private System.Windows.Forms.Button btnSelectFolder;
    }
}