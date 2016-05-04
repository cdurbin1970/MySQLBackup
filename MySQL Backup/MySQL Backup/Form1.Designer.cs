namespace MySQL_Backup
{
    partial class frMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frMain));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearAllItemsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.rtbOutput = new System.Windows.Forms.RichTextBox();
            this.buTestConfig = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tbMySQLDumpOptions = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cbSendEmail = new System.Windows.Forms.CheckBox();
            this.tbFromAddress = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.buTestEmail = new System.Windows.Forms.Button();
            this.tbEmailAddress = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.tbSMTPServer = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.tbSMTPUserName = new System.Windows.Forms.TextBox();
            this.tbSMTPPassword = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.tbSMTPPort = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbDBDirs = new System.Windows.Forms.CheckBox();
            this.cbRemoveDumpFile = new System.Windows.Forms.CheckBox();
            this.cbCompress = new System.Windows.Forms.CheckBox();
            this.tbDaystoKeep = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.buSaveLocation = new System.Windows.Forms.Button();
            this.tbSaveLocation = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.cbSelectDatabases = new System.Windows.Forms.CheckBox();
            this.tbHostName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buDumpLocation = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.tbDumpLocation = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.tbUserName = new System.Windows.Forms.TextBox();
            this.clbDatabases = new System.Windows.Forms.CheckedListBox();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.buTestConnection = new System.Windows.Forms.Button();
            this.tbPort = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tsslCurrentConfig = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.menuStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(843, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripMenuItem1,
            this.quitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearAllItemsToolStripMenuItem});
            this.newToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("newToolStripMenuItem.Image")));
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newToolStripMenuItem.Size = new System.Drawing.Size(190, 26);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.clearConfigItems);
            // 
            // clearAllItemsToolStripMenuItem
            // 
            this.clearAllItemsToolStripMenuItem.Name = "clearAllItemsToolStripMenuItem";
            this.clearAllItemsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.N)));
            this.clearAllItemsToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.clearAllItemsToolStripMenuItem.Text = "Clear All Items";
            this.clearAllItemsToolStripMenuItem.Click += new System.EventHandler(this.clearConfigItems);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openToolStripMenuItem.Image")));
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(190, 26);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Enabled = false;
            this.saveToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripMenuItem.Image")));
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(190, 26);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveConfigFile);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveAsToolStripMenuItem.Image")));
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(190, 26);
            this.saveAsToolStripMenuItem.Text = "Save As";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveConfigFile);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(187, 6);
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("quitToolStripMenuItem.Image")));
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(190, 26);
            this.quitToolStripMenuItem.Text = "Quit";
            this.quitToolStripMenuItem.Click += new System.EventHandler(this.quitToolStripMenuItem_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(0, 27);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(843, 449);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.rtbOutput);
            this.tabPage1.Controls.Add(this.buTestConfig);
            this.tabPage1.Controls.Add(this.groupBox3);
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(835, 423);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Connection";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // rtbOutput
            // 
            this.rtbOutput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rtbOutput.Location = new System.Drawing.Point(417, 290);
            this.rtbOutput.Name = "rtbOutput";
            this.rtbOutput.ReadOnly = true;
            this.rtbOutput.Size = new System.Drawing.Size(410, 91);
            this.rtbOutput.TabIndex = 20;
            this.rtbOutput.TabStop = false;
            this.rtbOutput.Text = "";
            // 
            // buTestConfig
            // 
            this.buTestConfig.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buTestConfig.Location = new System.Drawing.Point(563, 387);
            this.buTestConfig.Margin = new System.Windows.Forms.Padding(2);
            this.buTestConfig.Name = "buTestConfig";
            this.buTestConfig.Size = new System.Drawing.Size(124, 25);
            this.buTestConfig.TabIndex = 25;
            this.buTestConfig.Text = "Test Configuration";
            this.buTestConfig.UseVisualStyleBackColor = true;
            this.buTestConfig.Click += new System.EventHandler(this.buTestConfig_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.tbMySQLDumpOptions);
            this.groupBox3.Controls.Add(this.label14);
            this.groupBox3.Location = new System.Drawing.Point(417, 212);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox3.Size = new System.Drawing.Size(412, 73);
            this.groupBox3.TabIndex = 18;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "MySQL Dump Options";
            // 
            // tbMySQLDumpOptions
            // 
            this.tbMySQLDumpOptions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbMySQLDumpOptions.Location = new System.Drawing.Point(11, 35);
            this.tbMySQLDumpOptions.Margin = new System.Windows.Forms.Padding(2);
            this.tbMySQLDumpOptions.Name = "tbMySQLDumpOptions";
            this.tbMySQLDumpOptions.Size = new System.Drawing.Size(396, 20);
            this.tbMySQLDumpOptions.TabIndex = 24;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(9, 18);
            this.label14.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(165, 13);
            this.label14.TabIndex = 0;
            this.label14.Text = "Additional Command Line Options";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cbSendEmail);
            this.groupBox2.Controls.Add(this.tbFromAddress);
            this.groupBox2.Controls.Add(this.label15);
            this.groupBox2.Controls.Add(this.buTestEmail);
            this.groupBox2.Controls.Add(this.tbEmailAddress);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.tbSMTPServer);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.tbSMTPUserName);
            this.groupBox2.Controls.Add(this.tbSMTPPassword);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.tbSMTPPort);
            this.groupBox2.Location = new System.Drawing.Point(417, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(412, 201);
            this.groupBox2.TabIndex = 17;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "E-Mail Server Information";
            // 
            // cbSendEmail
            // 
            this.cbSendEmail.AutoSize = true;
            this.cbSendEmail.Location = new System.Drawing.Point(18, 19);
            this.cbSendEmail.Name = "cbSendEmail";
            this.cbSendEmail.Size = new System.Drawing.Size(79, 17);
            this.cbSendEmail.TabIndex = 16;
            this.cbSendEmail.Text = "Send Email";
            this.cbSendEmail.UseVisualStyleBackColor = true;
            this.cbSendEmail.CheckedChanged += new System.EventHandler(this.cbSendEmail_CheckedChanged);
            // 
            // tbFromAddress
            // 
            this.tbFromAddress.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbFromAddress.Enabled = false;
            this.tbFromAddress.Location = new System.Drawing.Point(94, 159);
            this.tbFromAddress.Name = "tbFromAddress";
            this.tbFromAddress.Size = new System.Drawing.Size(177, 20);
            this.tbFromAddress.TabIndex = 22;
            this.tbFromAddress.Text = "MySQLBackup@mailserver.com";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(13, 162);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(74, 13);
            this.label15.TabIndex = 19;
            this.label15.Text = "From Address:";
            // 
            // buTestEmail
            // 
            this.buTestEmail.Enabled = false;
            this.buTestEmail.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buTestEmail.Location = new System.Drawing.Point(333, 162);
            this.buTestEmail.Name = "buTestEmail";
            this.buTestEmail.Size = new System.Drawing.Size(68, 29);
            this.buTestEmail.TabIndex = 23;
            this.buTestEmail.Text = "Test Mail";
            this.buTestEmail.UseVisualStyleBackColor = true;
            this.buTestEmail.Click += new System.EventHandler(this.buTestEmail_Click);
            // 
            // tbEmailAddress
            // 
            this.tbEmailAddress.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbEmailAddress.Enabled = false;
            this.tbEmailAddress.Location = new System.Drawing.Point(94, 130);
            this.tbEmailAddress.Name = "tbEmailAddress";
            this.tbEmailAddress.Size = new System.Drawing.Size(177, 20);
            this.tbEmailAddress.TabIndex = 21;
            this.tbEmailAddress.Text = "cdurbin@cdcomputersys.com";
            this.tbEmailAddress.Validating += new System.ComponentModel.CancelEventHandler(this.tbEmailAddress_Validating);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(7, 132);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(80, 13);
            this.label12.TabIndex = 16;
            this.label12.Text = "E-Mail Address:";
            // 
            // tbSMTPServer
            // 
            this.tbSMTPServer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbSMTPServer.Enabled = false;
            this.tbSMTPServer.Location = new System.Drawing.Point(93, 39);
            this.tbSMTPServer.Name = "tbSMTPServer";
            this.tbSMTPServer.Size = new System.Drawing.Size(178, 20);
            this.tbSMTPServer.TabIndex = 17;
            this.tbSMTPServer.Text = "mailserver.com";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(15, 41);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(72, 13);
            this.label7.TabIndex = 8;
            this.label7.Text = "Server Name:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(24, 67);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(63, 13);
            this.label8.TabIndex = 9;
            this.label8.Text = "User Name:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(31, 94);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(56, 13);
            this.label9.TabIndex = 10;
            this.label9.Text = "Password:";
            // 
            // tbSMTPUserName
            // 
            this.tbSMTPUserName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbSMTPUserName.Enabled = false;
            this.tbSMTPUserName.Location = new System.Drawing.Point(93, 65);
            this.tbSMTPUserName.Name = "tbSMTPUserName";
            this.tbSMTPUserName.Size = new System.Drawing.Size(178, 20);
            this.tbSMTPUserName.TabIndex = 19;
            this.tbSMTPUserName.Text = "username";
            // 
            // tbSMTPPassword
            // 
            this.tbSMTPPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbSMTPPassword.Enabled = false;
            this.tbSMTPPassword.Location = new System.Drawing.Point(93, 92);
            this.tbSMTPPassword.Name = "tbSMTPPassword";
            this.tbSMTPPassword.PasswordChar = '*';
            this.tbSMTPPassword.Size = new System.Drawing.Size(178, 20);
            this.tbSMTPPassword.TabIndex = 20;
            this.tbSMTPPassword.Text = "password";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(298, 41);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(29, 13);
            this.label10.TabIndex = 14;
            this.label10.Text = "Port:";
            // 
            // tbSMTPPort
            // 
            this.tbSMTPPort.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbSMTPPort.Enabled = false;
            this.tbSMTPPort.Location = new System.Drawing.Point(333, 38);
            this.tbSMTPPort.Name = "tbSMTPPort";
            this.tbSMTPPort.Size = new System.Drawing.Size(52, 20);
            this.tbSMTPPort.TabIndex = 18;
            this.tbSMTPPort.Text = "25";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbDBDirs);
            this.groupBox1.Controls.Add(this.cbRemoveDumpFile);
            this.groupBox1.Controls.Add(this.cbCompress);
            this.groupBox1.Controls.Add(this.tbDaystoKeep);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.buSaveLocation);
            this.groupBox1.Controls.Add(this.tbSaveLocation);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.cbSelectDatabases);
            this.groupBox1.Controls.Add(this.tbHostName);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.buDumpLocation);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.tbDumpLocation);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.tbUserName);
            this.groupBox1.Controls.Add(this.clbDatabases);
            this.groupBox1.Controls.Add(this.tbPassword);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.buTestConnection);
            this.groupBox1.Controls.Add(this.tbPort);
            this.groupBox1.Location = new System.Drawing.Point(8, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(403, 411);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "MySQL Server Information";
            // 
            // cbDBDirs
            // 
            this.cbDBDirs.AutoSize = true;
            this.cbDBDirs.Location = new System.Drawing.Point(282, 381);
            this.cbDBDirs.Name = "cbDBDirs";
            this.cbDBDirs.Size = new System.Drawing.Size(84, 17);
            this.cbDBDirs.TabIndex = 15;
            this.cbDBDirs.Text = "Use DB Dirs";
            this.cbDBDirs.UseVisualStyleBackColor = true;
            // 
            // cbRemoveDumpFile
            // 
            this.cbRemoveDumpFile.AutoSize = true;
            this.cbRemoveDumpFile.Location = new System.Drawing.Point(131, 381);
            this.cbRemoveDumpFile.Name = "cbRemoveDumpFile";
            this.cbRemoveDumpFile.Size = new System.Drawing.Size(143, 17);
            this.cbRemoveDumpFile.TabIndex = 14;
            this.cbRemoveDumpFile.Text = "Remove .SQL Dump File";
            this.cbRemoveDumpFile.UseVisualStyleBackColor = true;
            this.cbRemoveDumpFile.CheckedChanged += new System.EventHandler(this.cbRemoveDumpFile_CheckedChanged);
            // 
            // cbCompress
            // 
            this.cbCompress.AutoSize = true;
            this.cbCompress.Location = new System.Drawing.Point(13, 381);
            this.cbCompress.Name = "cbCompress";
            this.cbCompress.Size = new System.Drawing.Size(112, 17);
            this.cbCompress.TabIndex = 13;
            this.cbCompress.Text = "Compress Backup";
            this.cbCompress.UseVisualStyleBackColor = true;
            // 
            // tbDaystoKeep
            // 
            this.tbDaystoKeep.Location = new System.Drawing.Point(132, 355);
            this.tbDaystoKeep.Name = "tbDaystoKeep";
            this.tbDaystoKeep.Size = new System.Drawing.Size(32, 20);
            this.tbDaystoKeep.TabIndex = 12;
            this.tbDaystoKeep.Text = "7";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(52, 355);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(74, 13);
            this.label13.TabIndex = 19;
            this.label13.Text = "Days to Keep:";
            // 
            // buSaveLocation
            // 
            this.buSaveLocation.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buSaveLocation.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buSaveLocation.Location = new System.Drawing.Point(370, 328);
            this.buSaveLocation.Name = "buSaveLocation";
            this.buSaveLocation.Size = new System.Drawing.Size(26, 20);
            this.buSaveLocation.TabIndex = 11;
            this.buSaveLocation.Text = "...";
            this.buSaveLocation.UseVisualStyleBackColor = true;
            this.buSaveLocation.Click += new System.EventHandler(this.buSaveLocation_Click);
            // 
            // tbSaveLocation
            // 
            this.tbSaveLocation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbSaveLocation.Location = new System.Drawing.Point(132, 328);
            this.tbSaveLocation.Name = "tbSaveLocation";
            this.tbSaveLocation.Size = new System.Drawing.Size(232, 20);
            this.tbSaveLocation.TabIndex = 10;
            this.tbSaveLocation.Text = "backups\\";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(8, 330);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(119, 13);
            this.label11.TabIndex = 16;
            this.label11.Text = "Backup Save Location:";
            // 
            // cbSelectDatabases
            // 
            this.cbSelectDatabases.AutoSize = true;
            this.cbSelectDatabases.Location = new System.Drawing.Point(77, 123);
            this.cbSelectDatabases.Name = "cbSelectDatabases";
            this.cbSelectDatabases.Size = new System.Drawing.Size(124, 17);
            this.cbSelectDatabases.TabIndex = 5;
            this.cbSelectDatabases.Text = "Select All Databases";
            this.cbSelectDatabases.UseVisualStyleBackColor = true;
            this.cbSelectDatabases.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // tbHostName
            // 
            this.tbHostName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbHostName.Location = new System.Drawing.Point(77, 37);
            this.tbHostName.Name = "tbHostName";
            this.tbHostName.Size = new System.Drawing.Size(178, 20);
            this.tbHostName.TabIndex = 1;
            this.tbHostName.Text = "localhost";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Host Name:";
            // 
            // buDumpLocation
            // 
            this.buDumpLocation.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buDumpLocation.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buDumpLocation.Location = new System.Drawing.Point(370, 298);
            this.buDumpLocation.Name = "buDumpLocation";
            this.buDumpLocation.Size = new System.Drawing.Size(26, 20);
            this.buDumpLocation.TabIndex = 9;
            this.buDumpLocation.Text = "...";
            this.buDumpLocation.UseVisualStyleBackColor = true;
            this.buDumpLocation.Click += new System.EventHandler(this.buDumpLocation_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "User Name:";
            // 
            // tbDumpLocation
            // 
            this.tbDumpLocation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbDumpLocation.Location = new System.Drawing.Point(132, 298);
            this.tbDumpLocation.Name = "tbDumpLocation";
            this.tbDumpLocation.Size = new System.Drawing.Size(232, 20);
            this.tbDumpLocation.TabIndex = 8;
            this.tbDumpLocation.Text = "mysqldump.exe";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 93);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Password:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 300);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(120, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "MySQL Dump Location:";
            // 
            // tbUserName
            // 
            this.tbUserName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbUserName.Location = new System.Drawing.Point(77, 63);
            this.tbUserName.Name = "tbUserName";
            this.tbUserName.Size = new System.Drawing.Size(178, 20);
            this.tbUserName.TabIndex = 3;
            this.tbUserName.Text = "dbuser";
            // 
            // clbDatabases
            // 
            this.clbDatabases.FormattingEnabled = true;
            this.clbDatabases.Location = new System.Drawing.Point(77, 146);
            this.clbDatabases.Name = "clbDatabases";
            this.clbDatabases.Size = new System.Drawing.Size(178, 109);
            this.clbDatabases.Sorted = true;
            this.clbDatabases.TabIndex = 6;
            // 
            // tbPassword
            // 
            this.tbPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbPassword.Location = new System.Drawing.Point(77, 90);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.PasswordChar = '*';
            this.tbPassword.Size = new System.Drawing.Size(178, 20);
            this.tbPassword.TabIndex = 4;
            this.tbPassword.Text = "dbuser";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 146);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(61, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Databases:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(296, 40);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Port:";
            // 
            // buTestConnection
            // 
            this.buTestConnection.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buTestConnection.Location = new System.Drawing.Point(87, 261);
            this.buTestConnection.Name = "buTestConnection";
            this.buTestConnection.Size = new System.Drawing.Size(152, 23);
            this.buTestConnection.TabIndex = 7;
            this.buTestConnection.Text = "Get Database Names";
            this.buTestConnection.UseVisualStyleBackColor = true;
            this.buTestConnection.Click += new System.EventHandler(this.buTestConnection_Click);
            // 
            // tbPort
            // 
            this.tbPort.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbPort.Location = new System.Drawing.Point(331, 37);
            this.tbPort.Name = "tbPort";
            this.tbPort.Size = new System.Drawing.Size(52, 20);
            this.tbPort.TabIndex = 2;
            this.tbPort.Text = "3306";
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(835, 423);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Schedule";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslCurrentConfig,
            this.toolStripProgressBar1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 495);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(843, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tsslCurrentConfig
            // 
            this.tsslCurrentConfig.AutoSize = false;
            this.tsslCurrentConfig.Name = "tsslCurrentConfig";
            this.tsslCurrentConfig.Size = new System.Drawing.Size(625, 17);
            this.tsslCurrentConfig.Text = "No Config Selected";
            this.tsslCurrentConfig.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Enabled = false;
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(200, 16);
            this.toolStripProgressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.toolStripProgressBar1.Visible = false;
            // 
            // errorProvider1
            // 
            this.errorProvider1.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errorProvider1.ContainerControl = this;
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.Filter = "Config Files (*.conf)|*.conf";
            // 
            // folderBrowserDialog1
            // 
            this.folderBrowserDialog1.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // frMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(843, 517);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "frMain";
            this.Text = "MySQL Backup";
            this.Load += new System.EventHandler(this.frMain_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.TextBox tbUserName;
        private System.Windows.Forms.TextBox tbHostName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbPort;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button buTestConnection;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.CheckedListBox clbDatabases;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbDumpLocation;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button buDumpLocation;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox tbSMTPServer;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox tbSMTPUserName;
        private System.Windows.Forms.TextBox tbSMTPPassword;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox tbSMTPPort;
        private System.Windows.Forms.ToolStripStatusLabel tsslCurrentConfig;
        private System.Windows.Forms.ToolStripMenuItem clearAllItemsToolStripMenuItem;
        private System.Windows.Forms.CheckBox cbSelectDatabases;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button buSaveLocation;
        private System.Windows.Forms.TextBox tbSaveLocation;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox tbEmailAddress;
        private System.Windows.Forms.Button buTestEmail;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox tbDaystoKeep;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox tbMySQLDumpOptions;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Button buTestConfig;
        private System.Windows.Forms.RichTextBox rtbOutput;
        private System.Windows.Forms.CheckBox cbRemoveDumpFile;
        private System.Windows.Forms.CheckBox cbCompress;
        private System.Windows.Forms.CheckBox cbDBDirs;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.TextBox tbFromAddress;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.CheckBox cbSendEmail;
    }
}

