namespace CodeFlow
{
    partial class ConnectionForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConnectionForm));
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
            this.txtCheckoutPath = new System.Windows.Forms.TextBox();
            this.lblCheckout = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblServerName
            // 
            this.lblServerName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblServerName.AutoSize = true;
            this.lblServerName.Location = new System.Drawing.Point(17, 59);
            this.lblServerName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblServerName.Name = "lblServerName";
            this.lblServerName.Size = new System.Drawing.Size(50, 17);
            this.lblServerName.TabIndex = 2;
            this.lblServerName.Text = "Server";
            // 
            // lblDb
            // 
            this.lblDb.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDb.AutoSize = true;
            this.lblDb.Location = new System.Drawing.Point(17, 205);
            this.lblDb.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDb.Name = "lblDb";
            this.lblDb.Size = new System.Drawing.Size(69, 17);
            this.lblDb.TabIndex = 8;
            this.lblDb.Text = "Database";
            // 
            // lblUser
            // 
            this.lblUser.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblUser.AutoSize = true;
            this.lblUser.Location = new System.Drawing.Point(17, 110);
            this.lblUser.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(73, 17);
            this.lblUser.TabIndex = 4;
            this.lblUser.Text = "Username";
            // 
            // txtUsername
            // 
            this.txtUsername.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUsername.Location = new System.Drawing.Point(20, 131);
            this.txtUsername.Margin = new System.Windows.Forms.Padding(4);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(669, 22);
            this.txtUsername.TabIndex = 5;
            // 
            // lblPass
            // 
            this.lblPass.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPass.AutoSize = true;
            this.lblPass.Location = new System.Drawing.Point(17, 158);
            this.lblPass.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPass.Name = "lblPass";
            this.lblPass.Size = new System.Drawing.Size(69, 17);
            this.lblPass.TabIndex = 6;
            this.lblPass.Text = "Password";
            // 
            // txtPassword
            // 
            this.txtPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPassword.Location = new System.Drawing.Point(20, 179);
            this.txtPassword.Margin = new System.Windows.Forms.Padding(4);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(669, 22);
            this.txtPassword.TabIndex = 7;
            this.txtPassword.UseSystemPasswordChar = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.Image")));
            this.btnCancel.Location = new System.Drawing.Point(592, 407);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 31);
            this.btnCancel.TabIndex = 20;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
            this.btnSave.Location = new System.Drawing.Point(484, 407);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 31);
            this.btnSave.TabIndex = 19;
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
            this.cmbServers.Location = new System.Drawing.Point(20, 79);
            this.cmbServers.Margin = new System.Windows.Forms.Padding(4);
            this.cmbServers.Name = "cmbServers";
            this.cmbServers.Size = new System.Drawing.Size(669, 24);
            this.cmbServers.TabIndex = 3;
            this.cmbServers.SelectedIndexChanged += new System.EventHandler(this.cmbServers_SelectedIndexChanged);
            this.cmbServers.MouseClick += new System.Windows.Forms.MouseEventHandler(this.cmbServers_MouseClick);
            // 
            // cmbDb
            // 
            this.cmbDb.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbDb.FormattingEnabled = true;
            this.cmbDb.Location = new System.Drawing.Point(21, 226);
            this.cmbDb.Margin = new System.Windows.Forms.Padding(4);
            this.cmbDb.Name = "cmbDb";
            this.cmbDb.Size = new System.Drawing.Size(669, 24);
            this.cmbDb.TabIndex = 9;
            this.cmbDb.MouseClick += new System.Windows.Forms.MouseEventHandler(this.cmbDb_MouseClick);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.Image = ((System.Drawing.Image)(resources.GetObject("btnRefresh.Image")));
            this.btnRefresh.Location = new System.Drawing.Point(376, 407);
            this.btnRefresh.Margin = new System.Windows.Forms.Padding(4);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(100, 31);
            this.btnRefresh.TabIndex = 18;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnTry
            // 
            this.btnTry.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnTry.Image = ((System.Drawing.Image)(resources.GetObject("btnTry.Image")));
            this.btnTry.Location = new System.Drawing.Point(20, 407);
            this.btnTry.Margin = new System.Windows.Forms.Padding(4);
            this.btnTry.Name = "btnTry";
            this.btnTry.Size = new System.Drawing.Size(128, 31);
            this.btnTry.TabIndex = 16;
            this.btnTry.Text = "Try it!";
            this.btnTry.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnTry.UseVisualStyleBackColor = true;
            this.btnTry.Click += new System.EventHandler(this.btnTry_Click);
            // 
            // txtGenioUser
            // 
            this.txtGenioUser.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtGenioUser.Location = new System.Drawing.Point(20, 273);
            this.txtGenioUser.Margin = new System.Windows.Forms.Padding(4);
            this.txtGenioUser.Name = "txtGenioUser";
            this.txtGenioUser.Size = new System.Drawing.Size(669, 22);
            this.txtGenioUser.TabIndex = 11;
            // 
            // lblGenioUser
            // 
            this.lblGenioUser.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblGenioUser.AutoSize = true;
            this.lblGenioUser.Location = new System.Drawing.Point(17, 254);
            this.lblGenioUser.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblGenioUser.Name = "lblGenioUser";
            this.lblGenioUser.Size = new System.Drawing.Size(80, 17);
            this.lblGenioUser.TabIndex = 10;
            this.lblGenioUser.Text = "User Genio";
            // 
            // txtConfigName
            // 
            this.txtConfigName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtConfigName.Location = new System.Drawing.Point(21, 31);
            this.txtConfigName.Margin = new System.Windows.Forms.Padding(4);
            this.txtConfigName.Name = "txtConfigName";
            this.txtConfigName.Size = new System.Drawing.Size(669, 22);
            this.txtConfigName.TabIndex = 1;
            // 
            // lblConfigName
            // 
            this.lblConfigName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblConfigName.AutoSize = true;
            this.lblConfigName.Location = new System.Drawing.Point(17, 11);
            this.lblConfigName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblConfigName.Name = "lblConfigName";
            this.lblConfigName.Size = new System.Drawing.Size(131, 17);
            this.lblConfigName.TabIndex = 0;
            this.lblConfigName.Text = "Configuration name";
            // 
            // txtGenioPath
            // 
            this.txtGenioPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtGenioPath.Location = new System.Drawing.Point(20, 321);
            this.txtGenioPath.Margin = new System.Windows.Forms.Padding(4);
            this.txtGenioPath.Name = "txtGenioPath";
            this.txtGenioPath.Size = new System.Drawing.Size(669, 22);
            this.txtGenioPath.TabIndex = 13;
            // 
            // lblGenioPath
            // 
            this.lblGenioPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblGenioPath.AutoSize = true;
            this.lblGenioPath.Location = new System.Drawing.Point(17, 302);
            this.lblGenioPath.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblGenioPath.Name = "lblGenioPath";
            this.lblGenioPath.Size = new System.Drawing.Size(78, 17);
            this.lblGenioPath.TabIndex = 12;
            this.lblGenioPath.Text = "Genio path";
            // 
            // txtCheckoutPath
            // 
            this.txtCheckoutPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCheckoutPath.Location = new System.Drawing.Point(20, 369);
            this.txtCheckoutPath.Margin = new System.Windows.Forms.Padding(4);
            this.txtCheckoutPath.Name = "txtCheckoutPath";
            this.txtCheckoutPath.Size = new System.Drawing.Size(669, 22);
            this.txtCheckoutPath.TabIndex = 15;
            // 
            // lblCheckout
            // 
            this.lblCheckout.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCheckout.AutoSize = true;
            this.lblCheckout.Location = new System.Drawing.Point(17, 350);
            this.lblCheckout.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCheckout.Name = "lblCheckout";
            this.lblCheckout.Size = new System.Drawing.Size(99, 17);
            this.lblCheckout.TabIndex = 14;
            this.lblCheckout.Text = "Checkout path";
            // 
            // ConnectionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(703, 452);
            this.Controls.Add(this.txtCheckoutPath);
            this.Controls.Add(this.lblCheckout);
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
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConnectionForm";
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
        private System.Windows.Forms.TextBox txtCheckoutPath;
        private System.Windows.Forms.Label lblCheckout;
    }
}