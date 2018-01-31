using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Encryption_Lib;
using AMS.Profile;
using Common.Logging;
using Quartz;

/*
 ITEMS Completed
 * Code cleanup. 
 * Added a try catch block for the show databases command under the buTestConnection_Click function.
 */

namespace MySQL_Backup {
    public partial class frMain : Form {

        // Universal key used for encryption and decryption functions
        // Key must be 256bit or 32bytes
        // If key is changed after a config file is saved, the passwords will not be decrypted properly.
        string encryptionKey = "LjjS5PDETyN98JZDgGtdPlhbk7-fBCVN";

        // Not the best way to do this, but trying to get status flags from processes running in a shell/seperate 
        // thread is not easy
        string error = String.Empty;
        int processID = 0;
        bool processTerminated = false;
        // The current config version
        string config_version = "1.0";
        // OS
        string OS = string.Empty;
        
        public frMain() {
            
            InitializeComponent();            
            // Update our text controls on the form.
            utilityFunctions.textupdate(this);       
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //    Form Load Routine                                                              //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void frMain_Load(object sender, EventArgs e) {
            // Create the ToolTip and associate with the Form container.
            ToolTip toolTip1 = new ToolTip();

            // Set up the delays for the ToolTip.
            toolTip1.AutoPopDelay = 5000;
            toolTip1.InitialDelay = 1000;
            toolTip1.ReshowDelay = 500;
            // Force the ToolTip text to be displayed whether or not the form is active.
            toolTip1.ShowAlways = true;

            // Set up the ToolTip text for the Button and Checkbox.
            toolTip1.SetToolTip(this.cbDBDirs, @"Store the backups in individual directories named after the database.");
            toolTip1.SetToolTip(this.cbCompress, @"Should the dump files be zipped after backup?");
            toolTip1.SetToolTip(this.cbRemoveDumpFile, @"Should the .sql file be removed after the backup is complete?");
            toolTip1.SetToolTip(this.tbHostName, @"Enter the host name or IP address of the MySQL database server.");
            toolTip1.SetToolTip(this.tbUserName, @"Enter the MySQL username.");
            toolTip1.SetToolTip(this.tbPassword, @"Enter the password of the MySQL user.");
            toolTip1.SetToolTip(this.tbPort, @"Enter the port for the MySQL server (Default is 3306).");
            toolTip1.SetToolTip(this.cbSelectDatabases, @"Click to select or unselect all the listed databases.");
            toolTip1.SetToolTip(this.clbDatabases, @"Select the databases you would like to backup.");
            toolTip1.SetToolTip(this.tbDumpLocation, @"Directory where mysqldump.exe is located.");
            toolTip1.SetToolTip(this.tbMySQLDumpOptions, @"Extra options to supply to mysqldump during the backup.");
            toolTip1.SetToolTip(this.tbSaveLocation, @"Directory where the backup file(s) will be saved.");
            toolTip1.SetToolTip(this.tbDaystoKeep, @"How many days worth of backups should we keep?");
            toolTip1.SetToolTip(this.cbSendEmail, @"Should I send emails after the backup is complete?");
            toolTip1.SetToolTip(this.tbSMTPServer, @"The host name or IP address of the SMTP server.");
            toolTip1.SetToolTip(this.tbSMTPUserName, @"SMTP server user name.");
            toolTip1.SetToolTip(this.tbSMTPPassword, @"SMTP server user's password.");
            toolTip1.SetToolTip(this.tbSMTPPort, @"Port for the SMTP server (Default is 25).");
            toolTip1.SetToolTip(this.tbEmailAddress, @"Email address to send reports to.");
            toolTip1.SetToolTip(this.tbFromAddress, @"The from email address.");
            toolTip1.SetToolTip(this.buTestConfig, @"The test will backup all databases in the list that are checked.");
            
            // Create our fileinfo object
            var objFileInfo = new FileInfo(Application.ExecutablePath);
            // To get the lastwrite time of this file
            var dtCreationDate = objFileInfo.LastWriteTime;
            // Set our Application Title
            Text = Application.ProductName + " v" + Application.ProductVersion + " build " + dtCreationDate.ToString("MMddyy");

            int p = (int)Environment.OSVersion.Platform;
            if ((p == 4) || (p == 6) || (p == 128)) {
                toolStripStatusLabel1.Text = "OS: Linux";
                OS = "Linux";
            }
            else {
                toolStripStatusLabel1.Text = "OS: Windows";
                OS = "Windows";
            }

        }

        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //    Exit the application                                                           //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void quitToolStripMenuItem_Click(object sender, EventArgs e) {
            if (MessageBox.Show("Exit MySQL Backup?", "Exit", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.OK) {
                Application.Exit();
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //    Get Database Names                                                             //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void buGetDatabaseNames_Click(object sender, EventArgs e) {
            errorProvider1.Clear();
            tbHostName.BackColor = Color.White;
            tbUserName.BackColor = Color.White;
            tbPassword.BackColor = Color.White;
            tbPort.BackColor = Color.White;

            if (tbHostName.Text != String.Empty && tbUserName.Text != String.Empty && tbPassword.Text != String.Empty && tbPort.Text != String.Empty) {
                // Setup our connection variables
                MySqlConnection MySQLConnect = utilityFunctions.DBConnect("server=" + tbHostName.Text + ";user id=" + tbUserName.Text + ";port=" + tbPort.Text + ";database=mysql;pooling=false;allow user variables=true;password=" + tbPassword.Text);
                if (MySQLConnect.Ping()) {
                    // Create our Lookup SELECT command
                    var lookupSelectCmd = new MySqlCommand();
                    // Create our Lookup command reader
                    MySqlDataReader readLookupData = null;
                    // Create our select command to get the records
                    lookupSelectCmd.CommandText = "SHOW DATABASES;";
                    // Set the lookup SELECT command connection
                    lookupSelectCmd.Connection = MySQLConnect;
                    try {
                        readLookupData = lookupSelectCmd.ExecuteReader();
                    }
                    catch (MySqlException ex) {
                        utilityFunctions.displayErrorMessage(ex.Message, "Error", false);
                        utilityFunctions.DBClose(MySQLConnect);
                        return;
                    }
                    clbDatabases.Items.Clear();
                    while (readLookupData.Read()) {
                        clbDatabases.Items.Add(readLookupData.GetValue(readLookupData.GetOrdinal("Database")).ToString());
                    }
                   utilityFunctions.DBClose(MySQLConnect);
                }
            }
            else {               
                if (tbPort.Text == String.Empty) {
                    errorProvider1.SetError(tbPort, "Please enter the MySQL connection port.");
                    tbPort.BackColor = Color.LightPink;
                    tbPort.Focus();
                }
                if (tbPassword.Text == String.Empty) {
                    errorProvider1.SetError(tbPassword, "Please enter the MySQL user password.");
                    tbPassword.BackColor = Color.LightPink;
                    tbPassword.Focus();
                }
                if (tbUserName.Text == String.Empty) {
                    errorProvider1.SetError(tbUserName, "Please enter the MySQL user name.");
                    tbUserName.BackColor = Color.LightPink;
                    tbUserName.Focus();
                }
                if (tbHostName.Text == String.Empty) {
                    errorProvider1.SetError(tbHostName, "Please enter the MySQL host to connect to.");
                    tbHostName.BackColor = Color.LightPink;
                    tbHostName.Focus();
                }
                utilityFunctions.displayErrorMessage("Please enter the  information before trying to connect!", "Error",false);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //    Open dialog to find MySQL dump                                            //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void buDumpLocation_Click(object sender, EventArgs e) {
            openFileDialog1.FileName = string.Empty;
            if (OS == "Windows") {
                openFileDialog1.Filter = "MySQL Dump (.exe)|*.exe";
            }
            else {
                openFileDialog1.Filter = "MySQL Dump|mysqldump";
            }

            if (openFileDialog1.ShowDialog() == DialogResult.OK) {
                tbDumpLocation.Text = openFileDialog1.FileName;
            }
        }        

        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //    Save config file based on sender                                               //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void saveConfigFile(object sender, EventArgs e) {
            // Do some checking for errors and report them.
            if (tbDumpLocation.Text == String.Empty) {
                if (!utilityFunctions.displayErrorMessage("MySQLDump.exe location is empty.\nThis program depends on MySQLDump.exe to perform the backup.\nContinuing with the save will result in a config file that does not work.","Error",true)) {
                    return;
                }
            }
            string fileNameToSave = String.Empty;
            // Shouldn't be possible to click this item without the name being present,
            // but we'll check anyways.
            if (sender == saveToolStripMenuItem && tsslCurrentConfig.Text == String.Empty) {
                return;
            }
            else if (sender == saveToolStripMenuItem && tsslCurrentConfig.Text != String.Empty)  {
                fileNameToSave = tsslCurrentConfig.Text;
            }            
            if(sender == saveAsToolStripMenuItem) {
                saveFileDialog1.Filter = "XML Config (.xml)|*.xml";
                saveFileDialog1.InitialDirectory = @Directory.GetCurrentDirectory() + @"\configs";
                saveFileDialog1.FileName = tbHostName.Text + ".xml";            
                if (saveFileDialog1.ShowDialog() == DialogResult.OK) {
                    fileNameToSave = saveFileDialog1.FileName;
                }
                else {
                    return;
                }
            }  
            // iterate through all the databases listed and decide which list to put them on.
            List<string> checkedDatabases = new List<string>();
            List<string> uncheckedDatabases = new List<string>();
            for (int x = 0; x < clbDatabases.Items.Count; x++) {
                if (clbDatabases.GetItemChecked(x)) {
                    checkedDatabases.Add((string)clbDatabases.Items[x]);                    
                }
                else {
                    uncheckedDatabases.Add((string)clbDatabases.Items[x]);
                }
            }
            // encrypt the sql and smtp passwords
            string SQLPassword = String.Empty;
            if (tbPassword.Text != String.Empty) {
                SQLPassword = Convert.ToBase64String(AESGCM.SimpleEncrypt(Encoding.UTF8.GetBytes(tbPassword.Text), Encoding.UTF8.GetBytes(encryptionKey)));
            }
            else {
                SQLPassword = String.Empty;
            }
            string SMTPPassword = String.Empty;
            if (tbSMTPPassword.Text != String.Empty) {
                SMTPPassword = Convert.ToBase64String(AESGCM.SimpleEncrypt(Encoding.UTF8.GetBytes(tbSMTPPassword.Text), Encoding.UTF8.GetBytes(encryptionKey)));
            }
            else {
                SMTPPassword = String.Empty;
            }
            // Open our .xml file and save the entries to it.
            try {
                Xml profile = new Xml(fileNameToSave);
                profile.SetValue("General", "Config Version", "1.0");
                if (rbWindows.Checked) {
                    profile.SetValue("General", "OS", "Windows");
                }
                else {
                    profile.SetValue("General", "OS", "Linux");
                }
                profile.SetValue("General", "Server", tbHostName.Text);
                profile.SetValue("General", "Port", tbPort.Text);
                profile.SetValue("General", "Username", tbUserName.Text);
                profile.SetValue("General", "SQLPassword", SQLPassword);
                profile.SetValue("General", "MySQLDump", tbDumpLocation.Text);
                profile.SetValue("General", "MySQLDumpOptions", tbMySQLDumpOptions.Text);
                profile.SetValue("General", "SaveLocation", tbSaveLocation.Text);
                profile.SetValue("General", "DaysToKeep",tbDaystoKeep.Text);
                profile.SetValue("General", "CompressBackup",cbCompress.CheckState);
                profile.SetValue("General", "RemoveDumpFile",cbRemoveDumpFile.CheckState);
                profile.SetValue("General", "DBDirs",cbDBDirs.CheckState);
                profile.SetValue("General", "SendEMail",cbSendEmail.CheckState);
                profile.SetValue("General", "SMTPServer",tbSMTPServer.Text);
                profile.SetValue("General", "SMTPPort",tbSMTPPort.Text);
                profile.SetValue("General", "SMTPUsername",tbSMTPUserName.Text);
                profile.SetValue("General", "SMTPPassword", SMTPPassword);
                profile.SetValue("General", "EmailAddress",tbEmailAddress.Text);
                profile.SetValue("General", "FromAddress",tbFromAddress.Text);
                profile.SetValue("Databases", "CheckedDatabases", string.Join(",",checkedDatabases));
                profile.SetValue("Databases", "unCheckedDatabases", string.Join(",",uncheckedDatabases));
                tsslCurrentConfig.Text = fileNameToSave;
                saveToolStripMenuItem.Enabled = true;
                utilityFunctions.displayInformationMessage("Config file saved successfully.", "Config Saved", false);
            }
            catch {
                utilityFunctions.displayErrorMessage("Error saving config file.", "Error Saving Config",false);
            }
       }   

        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //    Open a config file                                                             //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void openToolStripMenuItem_Click(object sender, EventArgs e) {
            openFileDialog1.FileName = String.Empty;
            openFileDialog1.Filter = "Config Files (*.xml)|*.xml";
            openFileDialog1.InitialDirectory = @Directory.GetCurrentDirectory() + @"\configs";
            if (openFileDialog1.ShowDialog() == DialogResult.OK) {                
                //clear everything out to make sure we're putting in correct items.                
                clearConfigItems(clearAllItemsToolStripMenuItem, e);
                //Load our config file in and assign the correct places.                
                Xml profile = new Xml(openFileDialog1.FileName);
                // Check to make sure we're using the correct version of the config file.
                try {
                    if (profile.GetValue("General", "Config Version").ToString() != config_version) {
                        utilityFunctions.displayErrorMessage("This config file appears to be for a different version of MySQL Backup.", "Config Error", false);
                        return;
                    }
                }
                catch (Exception) {
                    utilityFunctions.displayErrorMessage("This appears to be an invalid config file.", "Config Error", false);
                    return;
                }

                if (profile.GetValue("General", "OS").ToString() != OS) {
                    utilityFunctions.displayErrorMessage("This config file appears to be for a different OS than what you are running on. Please adjust the config for this OS.", "Config Error", false);
                }
                if (profile.GetValue("General", "OS").ToString() == "Windows") {
                    rbWindows.Checked = true;
                }
                else {
                    rbLinux.Checked = true;
                }

                // Load our values, fail if something is missing.
                    try {
                    tbHostName.Text = profile.GetValue("General", "Server").ToString();
                    tbPort.Text = profile.GetValue("General", "Port").ToString();
                    tbUserName.Text = profile.GetValue("General", "Username").ToString();
                    if (profile.GetValue("General", "SQLPassword").ToString() != String.Empty) {
                        tbPassword.Text = AESGCM.SimpleDecrypt(profile.GetValue("General", "SQLPassword").ToString(), Encoding.UTF8.GetBytes(encryptionKey));
                    }
                    else {
                        tbPassword.Text = String.Empty;
                    }
                    tbDumpLocation.Text = profile.GetValue("General", "MySQLDump").ToString();
                    tbMySQLDumpOptions.Text = profile.GetValue("General", "MySQLDumpOptions").ToString();
                    tbSaveLocation.Text = profile.GetValue("General", "SaveLocation").ToString();
                    tbDaystoKeep.Text = profile.GetValue("General", "DaysToKeep").ToString();
                    if (profile.GetValue("General", "CompressBackup").ToString() == "Checked") {
                        cbCompress.Checked = true;
                    }
                    else {
                        cbCompress.Checked = false;
                    }
                    if (profile.GetValue("General", "RemoveDumpFile").ToString() == "Checked") {
                        cbRemoveDumpFile.Checked = true;
                    }
                    else {
                        cbRemoveDumpFile.Checked = false;
                    }
                    if (profile.GetValue("General", "DBDirs").ToString() == "Checked") {
                        cbDBDirs.Checked = true;
                    }
                    else {
                        cbDBDirs.Checked = false;
                    }
                    if (profile.GetValue("General", "SendEMail").ToString() == "Checked") {
                        cbSendEmail.Checked = true;
                    }
                    else {
                        cbSendEmail.Checked = false;
                    }
                    tbSMTPServer.Text = profile.GetValue("General", "SMTPServer").ToString();
                    tbSMTPPort.Text = profile.GetValue("General", "SMTPPort").ToString();
                    tbSMTPUserName.Text = profile.GetValue("General", "SMTPUsername").ToString();
                    if (profile.GetValue("General", "SMTPPassword").ToString() != String.Empty) {
                        tbSMTPPassword.Text = AESGCM.SimpleDecrypt(profile.GetValue("General", "SMTPPassword").ToString(), Encoding.UTF8.GetBytes(encryptionKey));
                    }
                    else {
                        tbSMTPPassword.Text = String.Empty;
                    }
                    tbEmailAddress.Text = profile.GetValue("General", "EmailAddress").ToString();
                    tbFromAddress.Text = profile.GetValue("General", "FromAddress").ToString();

                    if (profile.GetValue("Databases", "CheckedDatabases").ToString() != String.Empty) {
                        // get the list of checked DBs
                        string[] checkedDBList = profile.GetValue("Databases", "CheckedDatabases").ToString().Split(',');
                        // iterate through each one and add it to the list checked.
                        for (int x = 0; x < checkedDBList.Length; x++) {
                            clbDatabases.Items.Add(checkedDBList[x], true);
                        }
                    }
                    if (profile.GetValue("Databases", "unCheckedDatabases").ToString() != String.Empty) {
                        // get the list of unchecked DBs
                        string[] unCheckedDBList = profile.GetValue("Databases", "unCheckedDatabases").ToString().Split(',');
                        // iterate through each one and add it to the list unchecked.
                        for (int x = 0; x < unCheckedDBList.Length; x++) {
                            clbDatabases.Items.Add(unCheckedDBList[x], false);
                        }
                    }
                }
                catch (Exception) {
                    utilityFunctions.displayErrorMessage("There was a problem parsing the config file.\n Not all entries were found and it is doubtful it will work properly.", "Config Error", false);
                    return;
                }
                saveToolStripMenuItem.Enabled = true;
                tsslCurrentConfig.Text = "Config File: " + openFileDialog1.SafeFileName;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //    Create a new config file - clear items based on sender                         //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void clearConfigItems(object sender, EventArgs e) {
            
            if (sender == newToolStripMenuItem) {
                saveToolStripMenuItem.Enabled = false;
                tbHostName.Text = String.Empty;
                tbUserName.Text = String.Empty;
                tbPassword.Text = String.Empty;
                tbPort.Text = String.Empty;
                tsslCurrentConfig.Text = "Config File: None";
                clbDatabases.Items.Clear();
                tbDumpLocation.Text = String.Empty;
                tbMySQLDumpOptions.Text = String.Empty;
                tbSaveLocation.Text = String.Empty;
                tbDaystoKeep.Text = String.Empty;
                rtbOutput.Text = String.Empty;
            }
            else if (sender == clearAllItemsToolStripMenuItem) {
                saveToolStripMenuItem.Enabled = false;
                tbHostName.Text = String.Empty;
                tbUserName.Text = String.Empty;
                tbPassword.Text = String.Empty;
                tbPort.Text = String.Empty;
                tsslCurrentConfig.Text = "Config File: None";
                clbDatabases.Items.Clear();
                tbDumpLocation.Text = String.Empty;
                tbMySQLDumpOptions.Text = String.Empty;
                tbSaveLocation.Text = String.Empty;
                tbDaystoKeep.Text = String.Empty;
                cbSendEmail.Checked = false;
                tbSMTPServer.Text = String.Empty;
                tbSMTPUserName.Text = String.Empty;
                tbSMTPPassword.Text = String.Empty;
                tbSMTPPort.Text = String.Empty;
                tbEmailAddress.Text = String.Empty;
                tbFromAddress.Text = String.Empty;
                rtbOutput.Text = String.Empty;
            }
        }        

        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //    Select and Unselect all databases                                              //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void cbSelectDatabases_CheckedChanged(object sender, EventArgs e) {
            if (cbSelectDatabases.CheckState == CheckState.Checked) {
                for (int x = 0; x < clbDatabases.Items.Count; x++) {
                    clbDatabases.SetItemChecked(x, true);
                }
                cbSelectDatabases.Text = "Unselect All Databases";
            }
            else if (cbSelectDatabases.CheckState == CheckState.Unchecked) {
                for (int x = 0; x < clbDatabases.Items.Count; x++) {
                    clbDatabases.SetItemChecked(x, false);
                }
                cbSelectDatabases.Text = "Select All Databases";
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //    Select save location                                                           //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void buSaveLocation_Click(object sender, EventArgs e) {            
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK) {
                tbSaveLocation.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //    Validate the email address                                                     //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void tbEmailAddress_Validating(object sender, CancelEventArgs e) {
            errorProvider1.Clear();
            string errorMsg;
            if (!utilityFunctions.ValidEmailAddress(tbEmailAddress.Text, out errorMsg)) {
                // Cancel the event and select the text to be corrected by the user.
                e.Cancel = true;
                tbEmailAddress.Select(0, tbEmailAddress.Text.Length);
                // Set the ErrorProvider error with the text to display.  
                errorProvider1.SetError(tbEmailAddress, errorMsg);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //    Send a test email                                                              //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void buTestEmail_Click(object sender, EventArgs e) {
            errorProvider1.Clear();
            tbSMTPServer.BackColor = Color.White;
            tbFromAddress.BackColor = Color.White;
            tbEmailAddress.BackColor = Color.White;
                        
            if (tbSMTPServer.Text !=String.Empty && tbEmailAddress.Text != String.Empty && tbFromAddress.Text !=String.Empty) {   
                string message = utilityFunctions.SendEmail(new string[] { tbSMTPServer.Text, tbSMTPPort.Text, tbSMTPUserName.Text,tbSMTPPassword.Text, tbEmailAddress.Text, tbFromAddress.Text, "MySQLBackup Test Message", "This is a test message." });
                if (message == "OK") {
                    utilityFunctions.displayInformationMessage("Email sent successfully.", "Email Sent", false);
                }
                else {
                    utilityFunctions.displayErrorMessage(message, "Error", false);
                }                
            }
            else {
                
                if (tbEmailAddress.Text == String.Empty) {
                    errorProvider1.SetError(tbEmailAddress, "Please enter an email address.");
                    tbEmailAddress.BackColor = Color.LightPink;
                    tbEmailAddress.Focus();
                }
                if (tbFromAddress.Text == String.Empty) {
                    errorProvider1.SetError(tbFromAddress, "Please enter a from email address.");
                    tbFromAddress.BackColor = Color.LightPink;
                    tbFromAddress.Focus();
                }
                if (tbSMTPServer.Text == String.Empty) {
                    errorProvider1.SetError(tbSMTPServer, "Please enter the SMTP server name.");
                    tbSMTPServer.BackColor = Color.LightPink;
                    tbSMTPServer.Focus();
                }
                utilityFunctions.displayErrorMessage("Please enter the information before trying to send a test email!", "Error",false);
            }
        
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //    Test the current config                                                        //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void buTestConfig_Click(object sender, EventArgs e) {
            // To make this a quicker process, we could probably open up a zip archive and dump the output directly to it.
            // I chose to create the dump file in a temp directory, create a zip file if asked, move the dump file into it, delete the dump file 
            // if appropriate then move the file into the save location.

            string dbSaveDirectory = String.Empty;
            DateTime now = DateTime.Now;
            string dateStamp = now.ToString("yyyy_MM_dd_HH_mm_ss_");
  
            rtbOutput.Text = String.Empty;

            string tempDir = Directory.GetCurrentDirectory();
            if (OS == "Windows") {
                tempDir = tempDir + @"\temp\";
            }
            else {
                tempDir = tempDir + @"/temp/";
            }

            List<string> dbNames = new List<string>();
            for (int x = 0; x < clbDatabases.Items.Count; x++) {
                if (clbDatabases.GetItemChecked(x)) {
                    dbNames.Add((string)clbDatabases.Items[x]);
                }
            }

            if (!dbNames.Any()) {
                utilityFunctions.displayErrorMessage("Please select one database from the list before trying to test.", "Database Selection Error", false);
                return;
            }

            foreach (string dbName in dbNames) {

                // Check to see if we should be using individual dirs for each database
                if (cbDBDirs.Checked) {
                    if (OS == "Windows") {
                        dbSaveDirectory = tbSaveLocation.Text + @"\" + dbName + @"\";
                    }
                    else {
                        dbSaveDirectory = tbSaveLocation.Text + @"/" + dbName + @"/";
                    }
                }
                else {
                    if (OS == "Windows") {
                        dbSaveDirectory = tbSaveLocation.Text + @"\";
                    }
                    else {
                        dbSaveDirectory = tbSaveLocation.Text + @"/";
                    }
                    
                }

                //put it all together, notice that were using the --result-file command line. This allows us to not have to use a streamwriter with redirected output.
                //string cmd = String.Format("-h{0} -u{1} -p{2} --opt --databases {3} --result-file={4}", tbHostName.Text, tbUserName.Text, tbPassword.Text, dbName, tbSaveLocation.Text + "/" + dbName + "/" + now.ToString("yyyy_MM_dd_HH_mm_ss_") + dbName + ".sql");
                // Check to see if a folder location exists. Which is usually backuplocation/dbname/date_dbname.sql
                if (!Directory.Exists(dbSaveDirectory)) {
                    try {
                        Directory.CreateDirectory(dbSaveDirectory);
                    }
                    catch (IOException) {
                        rtbOutput.Text = "Could not create save directory, aborting backup.";
                        return;
                    }
                }

                // Check for our temp directory
                if (!Directory.Exists(tempDir)) {
                    try {
                        Directory.CreateDirectory(tempDir);
                    }
                    catch (IOException) {
                        rtbOutput.Text = "Could not create temp directory and it does not exist, aborting backup.";
                        return;
                    }
                }
                
                //Creata a temporary file to hold the sqlpassword. Otherwise, mysqldump exists with a warning about using a password on the command line.
                //[mysqldump]
                //user=username
                //password=password
                if (Directory.Exists(tempDir)) {
                    try {
                        string[] lines = {"[mysqldump]", "user=" + tbUserName.Text, "password=" + tbPassword.Text};
                        File.WriteAllLines(@tempDir + ".sqlpasswd", lines);
                    }
                    catch (IOException){
                        rtbOutput.Text = "Could not create sql password file and it does not exist, aborting backup.";
                        return;
                    }
                }
                // Make the cancel button visible
                buCancelTest.Visible = true;

                // Create a string array to hold all of our parameters in.
                string[] parameters =
                {
                    tbHostName.Text, tempDir + ".sqlpasswd", tbMySQLDumpOptions.Text, dbName, tbDumpLocation.Text, dateStamp, tempDir
                };

                //utilityFunctions.displayInformationMessage("Command Line Used: " + parameters[5] + " " + String.Format("-h{0} -u{1} -p[password] {2} --databases {3} --result-file={4}", parameters[0], parameters[1], parameters[3], parameters[4], parameters[7] + parameters[6] + parameters[4] + ".sql\n"),"Message", false);
                rtbOutput.Text = rtbOutput.Text + "Command Line Used: " + parameters[4] + " " + String.Format(" --defaults-extra-file={1} -h{0} {2} --databases {3} --result-file={4}",parameters[0], parameters[1], parameters[2], parameters[3], parameters[6] + parameters[5] + parameters[3] + ".sql\n");
                
                //Create a background worker to do the actual backup in.           
                BackgroundWorker backupThread = new BackgroundWorker();
                backupThread.WorkerReportsProgress = false;
                backupThread.WorkerSupportsCancellation = false;
                backupThread.DoWork += backupDataBase;
                backupThread.RunWorkerAsync(parameters);

                // Start the control to show it is doing something
                toolStripProgressBar1.Visible = true;
                toolStripProgressBar1.Enabled = true;

                // Wait while the thread completes, doevents so the form is not frozen.
                while (backupThread.IsBusy) {
                    Application.DoEvents();
                }

                // Check to see if we return anything other than OK.
                if (error != "OK") {
                    rtbOutput.Text = error;
                    toolStripProgressBar1.Visible = false;
                    toolStripProgressBar1.Enabled = false;
                    if (cbSendEmail.Checked && tbSMTPServer.Text != String.Empty && tbEmailAddress.Text != String.Empty && tbFromAddress.Text != String.Empty) {
                        string message = utilityFunctions.SendEmail(new string[]
                        {
                            tbSMTPServer.Text, tbSMTPPort.Text, tbSMTPUserName.Text, tbSMTPPassword.Text, tbEmailAddress.Text, tbFromAddress.Text, "MySQL Backup Notification", "Test backup failed for database " + dbName + " on server " + tbHostName.Text + ".\n\n" + rtbOutput.Text
                        });
                    }
                    // Make the cancel button invisible
                    buCancelTest.Visible = false;
                    return;
                }
                
                //Dump was completed, remove the crendentials file we created
                try {
                    File.Delete(@tempDir + ".sqlpasswd");
                }
                catch (IOException ex) {
                    error = ex.Message;
                }      
                
                // Make the cancel button invisible
                buCancelTest.Visible = false;
                rtbOutput.Text = rtbOutput.Text + "Dump of " + dbName + " Complete.\n";

                if (cbCompress.Checked) {
                    // create a background worker to do the zip.
                    BackgroundWorker zipThread = new BackgroundWorker();
                    zipThread.WorkerReportsProgress = false;
                    zipThread.WorkerSupportsCancellation = false;
                    zipThread.DoWork += zipDataBase;
                    zipThread.RunWorkerAsync(parameters);

                    // Wait while the thread completes, doevents so the form is not frozen.
                    while (zipThread.IsBusy) {
                        Application.DoEvents();
                    }
                    if (error == "OK") {
                        rtbOutput.Text = rtbOutput.Text + "Zip of " + dbName + " Complete.\n";
                    }
                    else {
                        rtbOutput.Text = rtbOutput.Text + error;
                        toolStripProgressBar1.Visible = false;
                        toolStripProgressBar1.Enabled = false;
                        if (cbSendEmail.Checked && tbSMTPServer.Text != String.Empty && tbEmailAddress.Text != String.Empty && tbFromAddress.Text != String.Empty) {
                            string message = utilityFunctions.SendEmail(new string[]
                            {
                                tbSMTPServer.Text, tbSMTPPort.Text, tbSMTPUserName.Text, tbSMTPPassword.Text, tbEmailAddress.Text, tbFromAddress.Text, "MySQL Backup Notification", "Test backup failed for database " + dbName + " on server " + tbHostName.Text + ".\n\n" + rtbOutput.Text
                            });
                        }
                        return;
                    }
                }

                if (cbRemoveDumpFile.Checked) {
                    // Remove the original .sql file and only store the .zip
                    try {
                        File.Delete(tempDir + dateStamp + dbName + ".sql");
                    }
                    catch (IOException ex) {
                        utilityFunctions.displayErrorMessage("Unable to delete the .SQL dump file.", "Error", false);
                    }
                }

                // Everything appears to be OK so far. We're going to move the file from our temp directory to the save directory.
                try {
                    // Did we create a .zip file?
                    if (cbCompress.Checked) {
                        File.Move(tempDir + dateStamp + dbName + ".zip", dbSaveDirectory + dateStamp + dbName + ".zip");
                    }
                    // Did we delete the .sql dump file?
                    if (!cbRemoveDumpFile.Checked) {
                        File.Move(tempDir + dateStamp + dbName + ".sql", dbSaveDirectory + dateStamp + dbName + ".sql");
                    }
                }
                catch (IOException ex) {
                    utilityFunctions.displayErrorMessage(ex.Message, "error", false);
                    rtbOutput.Text = rtbOutput.Text + "Unable to move file(s) to save directory.\n";
                    toolStripProgressBar1.Visible = false;
                    toolStripProgressBar1.Enabled = false;
                    if (cbSendEmail.Checked && tbSMTPServer.Text != String.Empty && tbEmailAddress.Text != String.Empty && tbFromAddress.Text != String.Empty) {
                        string message = utilityFunctions.SendEmail(new string[]
                        {
                            tbSMTPServer.Text, tbSMTPPort.Text, tbSMTPUserName.Text, tbSMTPPassword.Text, tbEmailAddress.Text, tbFromAddress.Text, "MySQL Backup Notification", "Test backup failed for database " + dbName + " on server " + tbHostName.Text + ".\n\n" + rtbOutput.Text
                        });
                    }
                    return;
                }

                rtbOutput.Text = rtbOutput.Text + "Backup of: " + dbName + " Complete.\n";
                rtbOutput.Text = rtbOutput.Text + "Backup saved to: " + dbSaveDirectory + "\n";

                rtbOutput.Text = rtbOutput.Text + "Total Backup Time: " + DateTime.Now.Subtract(now).TotalSeconds.ToString("########") + " Seconds.\n";

                toolStripProgressBar1.Visible = false;
                toolStripProgressBar1.Enabled = false;
            }

            if (cbSendEmail.Checked && tbSMTPServer.Text != String.Empty && tbEmailAddress.Text != String.Empty && tbFromAddress.Text != String.Empty) {
                string message = utilityFunctions.SendEmail(new string[]
                {
                    tbSMTPServer.Text, tbSMTPPort.Text, tbSMTPUserName.Text, tbSMTPPassword.Text, tbEmailAddress.Text, tbFromAddress.Text, "MySQL Backup Notification", "Test backup completed for the selected database(s) on server " + tbHostName.Text + ".\n\n" + rtbOutput.Text
                });
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //    Database backup thread                                                         //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void backupDataBase(object sender, DoWorkEventArgs e) {
            
            // Parameters
            /*
                0=MySQL Host
                1=MySQL sqlpasswd file location
                2=MySQL Dump Options
                3=Database Name(s)
                4=mysqldump Location
                5=Date Stamp (filename prepend)
                6=Temp Dir Location 
             */
            
            Object[] arg = e.Argument as Object[];
            error = String.Empty;                                   
            //create start info...
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.FileName = (string)arg[4];
            //put it all together, notice that were using the --result-file command line. This allows us to not have to use a streamwriter with redirected output.
            startInfo.Arguments = String.Format("--defaults-extra-file=\"{1}\" -h{0} {2} --databases {3} --result-file=\"{4}\"", (string)arg[0], (string)arg[1], (string)arg[2], (string)arg[3], (string)arg[6] + (string)arg[5] + (string)arg[3] + ".sql");
            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardInput = false;
            startInfo.RedirectStandardOutput = false; //we do not need to redirect the standard output to a StreamWriter
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            startInfo.ErrorDialog = false;
            //create the process...
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo = startInfo;
            //start the process...
            try {
                proc.Start();
            }
            catch (Exception ex) {
                utilityFunctions.displayErrorMessage(ex.Message, "Error", false);
                return;
            }
            // Get the process ID so we can kill it if necessary.
            processID = proc.Id;
            error = proc.StandardError.ReadToEnd();
                   
            if (proc.ExitCode == 1 || processTerminated) {
                error = "mysqldump.exe process was terminated.";
            }
            //close the process...
            proc.Close();            
            if (error != String.Empty)  {
                // Cleanup the empty file 
                try {
                    File.Delete((string)arg[7] + (string)arg[6] + (string)arg[4] + ".sql");
                }
                catch (IOException ex) {
                    error = ex.Message;
                }                
                e.Cancel = true;
                return;
            }
            //MySQLDump.
            error =  "OK";
            e.Cancel = true;
            return;
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //    Database zip thread                                                            //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void zipDataBase(object sender, DoWorkEventArgs e){
            // Parameters
            /*
                0=MySQL Host
                1=MySQL sqlpasswd file location
                2=MySQL Dump Options
                3=Database Name(s)
                4=mysqldump Location
                5=Date Stamp (filename prepend)
                6=Temp Dir Location 
             */
            
            error = String.Empty;
            Object[] arg = e.Argument as Object[];
                       
            try {
                // Now were going to zip the file up.            
                ZipArchive zip = ZipFile.Open((string)arg[6] + (string)arg[5] +  (string)arg[3] + ".zip", ZipArchiveMode.Create);
                zip.CreateEntryFromFile((string)arg[6] + (string)arg[5] + (string)arg[3] + ".sql", (string)arg[5] + (string)arg[3] + ".sql");
                zip.Dispose();
            }
            catch (IOException ex) {
                error = "Zip process failed: " + ex.Message + "\n";                
                e.Cancel = true;
                return;
            }
            error = "OK";
            e.Cancel = true;
            return;
        }
        
        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //    Check Boxes not correct                                                        //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void cbRemoveDumpFile_CheckedChanged(object sender, EventArgs e) {
            if (cbRemoveDumpFile.Checked && !cbCompress.Checked) {
                if (utilityFunctions.displayErrorMessage("Checking this and not checking the Compress Backup box will result in your backup file being deleted!\nThis is designed to let you keep both the original dump file and the .zip file.", "File Deletion Warning",true) == false){
                    cbRemoveDumpFile.Checked = false;
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //    Send Email Checkbox                                                            //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void cbSendEmail_CheckedChanged(object sender, EventArgs e) {
            if (cbSendEmail.Checked) {
                tbSMTPServer.Enabled = true;
                tbSMTPPassword.Enabled = true;
                tbSMTPPort.Enabled = true;
                tbSMTPUserName.Enabled = true;
                tbFromAddress.Enabled = true;
                tbEmailAddress.Enabled = true;
                buTestEmail.Enabled = true;
            }
            else if (!cbSendEmail.Checked) {
                tbSMTPServer.Enabled = false;
                tbSMTPPassword.Enabled = false;
                tbSMTPPort.Enabled = false;
                tbSMTPUserName.Enabled = false;
                tbFromAddress.Enabled = false;
                tbEmailAddress.Enabled = false;
                buTestEmail.Enabled = false;
            }

        }

        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //    Cancel the running mysqldump process                                           //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void buCancelTest_Click(object sender, EventArgs e) {

            buCancelTest.Visible = false;  
            Process p = Process.GetProcessById(processID);
            if (p != null || !p.HasExited) {
                p.Kill();
                processTerminated = true;                
            }             
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //    Edit the backup schedules                                                      //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void editScheduleToolStripMenuItem_Click(object sender, EventArgs e) {
            //ScheduleGUI scheduleForm = new ScheduleGUI();
            //scheduleForm.ShowDialog();
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //   Show the about dialog                                                           //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e) {
            frAbout about = new frAbout();
            about.ShowDialog();
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //    Open dialog to find MySQL exe                                                  //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void buRestoreMySQLRequestor_Click(object sender, EventArgs e) {
                openFileDialog1.Filter = "MySQL (.exe)|*.exe";
                if (openFileDialog1.ShowDialog() == DialogResult.OK) {
                    tbRestoreMySQLLocation.Text = openFileDialog1.FileName;
                }
           }

        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //    Start the job scheduler                                                        //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void startSchedulerToolStripMenuItem_Click(object sender, EventArgs e) {
            if (startSchedulerToolStripMenuItem.Text == "Start Scheduler") {
                startSchedulerToolStripMenuItem.CheckState = CheckState.Checked;
                startSchedulerToolStripMenuItem.Text = "Stop Scheduler";
                toolStripStatusLabel2.Text = "Scheduler: Running";

                var schedule = new scheduler();
                schedule.Run();



            }
            else {
                startSchedulerToolStripMenuItem.CheckState = CheckState.Unchecked;
                startSchedulerToolStripMenuItem.Text = "Start Scheduler";
                toolStripStatusLabel2.Text = "Scheduler: Stopped";
            }
        }
    }
} 