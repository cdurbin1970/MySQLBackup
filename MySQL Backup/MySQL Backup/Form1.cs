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

/*
 ITEMS Completed
 * Code cleanup. 
 * Added a try catch block for the show databases command under the buTestConnection_Click function.
 */

namespace MySQL_Backup {
    public partial class FrMain : Form { 
        // The current config version
        private const string ConfigVersion = "1.0";
        // Not the best way to do this, but trying to get status flags from processes running in a shell/seperate 
        // thread is not easy
        private string _error = string.Empty;
        private int _processId;
        private bool _processTerminated;
        // OS
        private string _os = string.Empty;
        // Config file location
        private string _configLocation = string.Empty;

        public FrMain() {
            InitializeComponent();
            // Update our text controls on the form.
            UtilityFunctions.Textupdate(this);
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //    Form Load Routine                                                              //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void FrMain_Load(object sender, EventArgs e) {
            // Create the ToolTip and associate with the Form container.
            var toolTip1 = new ToolTip() {
                // Set up the delays for the ToolTip.
                AutoPopDelay = 5000,
                InitialDelay = 1000,
                ReshowDelay = 500,
                // Force the ToolTip text to be displayed whether or not the form is active.
                ShowAlways = true
            };
            // Set up the ToolTip text for the Button and Checkbox.
            toolTip1.SetToolTip(cbDBDirs, @"Store the backups in individual directories named after the database.");
            toolTip1.SetToolTip(cbCompress, @"Should the dump files be zipped after backup?");
            toolTip1.SetToolTip(cbRemoveDumpFile, @"Should the .sql file be removed after the backup is complete?");
            toolTip1.SetToolTip(tbHostName, @"Enter the host name or IP address of the MySQL database server.");
            toolTip1.SetToolTip(tbUserName, @"Enter the MySQL username.");
            toolTip1.SetToolTip(tbPassword, @"Enter the password of the MySQL user.");
            toolTip1.SetToolTip(tbPort, @"Enter the port for the MySQL server (Default is 3306).");
            toolTip1.SetToolTip(cbSelectDatabases, @"Click to select or unselect all the listed databases.");
            toolTip1.SetToolTip(clbDatabases, @"Select the databases you would like to backup.");
            toolTip1.SetToolTip(tbDumpLocation, @"Directory where mysqldump.exe is located.");
            toolTip1.SetToolTip(tbMySQLDumpOptions, @"Extra options to supply to mysqldump during the backup.");
            toolTip1.SetToolTip(tbSaveLocation, @"Directory where the backup file(s) will be saved.");
            toolTip1.SetToolTip(tbDaystoKeep, @"How many days worth of backups should we keep?");
            toolTip1.SetToolTip(cbSendEmail, @"Should I send emails after the backup is complete?");
            toolTip1.SetToolTip(tbSMTPServer, @"The host name or IP address of the SMTP server.");
            toolTip1.SetToolTip(tbSMTPUserName, @"SMTP server user name.");
            toolTip1.SetToolTip(tbSMTPPassword, @"SMTP server user's password.");
            toolTip1.SetToolTip(tbSMTPPort, @"Port for the SMTP server (Default is 25).");
            toolTip1.SetToolTip(tbEmailAddress, @"Email address to send reports to.");
            toolTip1.SetToolTip(tbFromAddress, @"The from email address.");
            toolTip1.SetToolTip(buTestConfig, @"The test will backup all databases in the list that are checked.");
            // Create our fileinfo object
            var objFileInfo = new FileInfo(Application.ExecutablePath);
            //MessageBox.Show(Path.GetDirectoryName(Application.ExecutablePath));
            // To get the lastwrite time of this file
            var dtCreationDate = objFileInfo.LastWriteTime;
            // Set our Application Title
            Text = Application.ProductName + " v" + Application.ProductVersion + " build " + dtCreationDate.ToString("MMddyy");
            var p = (int)Environment.OSVersion.Platform;
            if (p == 4 || p == 6 || p == 128) {
                toolStripStatusLabel1.Text = "OS: Linux";
                _os = "Linux";
                cbCompress.Enabled = false;
            }
            else { 
                toolStripStatusLabel1.Text = "OS: Windows";
                _os = "Windows";
            }
            // Load the program config file.
            var profile = _os == "Windows" ? new Xml(Path.GetDirectoryName(Application.ExecutablePath) + @"\mysqlbackupconfig.xml") : new Xml(Path.GetDirectoryName(Application.ExecutablePath) + @"/mysqlbackupconfig.xml");
            // Check to make sure we're using the correct version of the config file.
            try {
                if (profile.GetValue("General", "Config Version").ToString() != ConfigVersion) {
                    UtilityFunctions.DisplayMessage("error","This config file appears to be for a different version of MySQL Backup.", "Config Error", false);
                    return;
                }
            }
            catch (Exception) {
                UtilityFunctions.DisplayMessage("error","Cannot open the main config file. A new one will be created when you exit the program.", "Config Error", false);
                return;
            }
            try {
                tbRestoreMySQLLocation.Text = profile.GetValue("General", "MySQLLocation").ToString();
            }
            catch (Exception) {
                UtilityFunctions.DisplayMessage("error","There was a problem parsing the main config file.\n Not all entries were found and it is doubtful it will work properly.", "Config Error", false);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //    Exit the application                                                           //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void QuitToolStripMenuItem_Click(object sender, EventArgs e) {
            if (UtilityFunctions.DisplayMessage("question","Exit MySQL Backup?","Exit",true)) {
                Application.Exit();
            }
        }

        private void FrMain_FormClosing(object sender, FormClosingEventArgs e) {
            // Open our .xml file and save the entries to it.
            try {
                var profile = _os == "Windows" ? new Xml(Path.GetDirectoryName(Application.ExecutablePath) + @"\mysqlbackupconfig.xml") : new Xml(Path.GetDirectoryName(Application.ExecutablePath) + @"/mysqlbackupconfig.xml");                
                profile.SetValue("General", "Config Version", "1.0");
                profile.SetValue("General", "MySQLLocation", tbRestoreMySQLLocation.Text);            
            }
            catch {
                UtilityFunctions.DisplayMessage("error","Error saving main config file.","Error Saving Main Config", false);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //    Get Database Names                                                             //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void BuGetDatabaseNames_Click(object sender, EventArgs e) {
            errorProvider1.Clear();
            tbHostName.BackColor = Color.White;
            tbUserName.BackColor = Color.White;
            tbPassword.BackColor = Color.White;
            tbPort.BackColor = Color.White;
            if (tbHostName.Text != string.Empty && tbUserName.Text != string.Empty && tbPassword.Text != string.Empty && tbPort.Text != string.Empty) {
                // Setup our connection variables
                var mySqlConnect = UtilityFunctions.DbConnect("server=" + tbHostName.Text + ";user id=" + tbUserName.Text + ";port=" + tbPort.Text + ";database=mysql;pooling=false;allow user variables=true;password=" + tbPassword.Text);
                if (!mySqlConnect.Ping()) return;
                //if (mySqlConnect.Ping()) {
                // Create our Lookup SELECT command
                var lookupSelectCmd = new MySqlCommand();
                // Create our Lookup command reader
                MySqlDataReader readLookupData;
                // Create our select command to get the records
                lookupSelectCmd.CommandText = "SHOW DATABASES;";
                // Set the lookup SELECT command connection
                lookupSelectCmd.Connection = mySqlConnect;
                try {
                    readLookupData = lookupSelectCmd.ExecuteReader();
                }
                catch (MySqlException ex) {
                    UtilityFunctions.DisplayMessage("error",ex.Message, "Error", false);
                    if (!UtilityFunctions.DbClose(mySqlConnect)) {
                        UtilityFunctions.DisplayMessage("error", "Could not close database connection.", "Error", false);
                    }
                    return;
                }
                clbDatabases.Items.Clear();
                while (readLookupData.Read()) {
                    clbDatabases.Items.Add(readLookupData.GetValue(readLookupData.GetOrdinal("Database")).ToString(),false);
                }
                if (!UtilityFunctions.DbClose(mySqlConnect)) {
                    UtilityFunctions.DisplayMessage("error", "Could not close database connection.", "Error", false);
                }
            }
            else {
                if (tbPort.Text == string.Empty) {
                    errorProvider1.SetError(tbPort, "Please enter the MySQL connection port.");
                    tbPort.BackColor = Color.LightPink;
                    tbPort.Focus();
                }
                if (tbPassword.Text == string.Empty) {
                    errorProvider1.SetError(tbPassword, "Please enter the MySQL user password.");
                    tbPassword.BackColor = Color.LightPink;
                    tbPassword.Focus();
                }
                if (tbUserName.Text == string.Empty) {
                    errorProvider1.SetError(tbUserName, "Please enter the MySQL user name.");
                    tbUserName.BackColor = Color.LightPink;
                    tbUserName.Focus();
                }
                if (tbHostName.Text == string.Empty) {
                    errorProvider1.SetError(tbHostName, "Please enter the MySQL host to connect to.");
                    tbHostName.BackColor = Color.LightPink;
                    tbHostName.Focus();
                }
                UtilityFunctions.DisplayMessage("error","Please enter the  information before trying to connect!", "Error", false);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //    Open a config file                                                             //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e) {
            openFileDialog1.FileName = string.Empty;
            openFileDialog1.Filter = "Config Files (*.xml)|*.xml";
            if (_os == "Windows") {
                if (!Directory.Exists(Path.GetDirectoryName(Application.ExecutablePath) + @"\configs")) {
                    Directory.CreateDirectory(Path.GetDirectoryName(Application.ExecutablePath) + @"\configs");
                }
                openFileDialog1.InitialDirectory = Path.GetDirectoryName(Application.ExecutablePath) + @"\configs";
            }
            else {              
                if (!Directory.Exists(Path.GetDirectoryName(Application.ExecutablePath) + @"/configs")) {
                    Directory.CreateDirectory(Path.GetDirectoryName(Application.ExecutablePath) + @"/configs");
                }
                openFileDialog1.InitialDirectory = Path.GetDirectoryName(Application.ExecutablePath) + @"/configs";
            }

            if (openFileDialog1.ShowDialog() != DialogResult.OK) return;
            //clear everything out to make sure we're putting in correct items.                
            ClearConfigItems(clearAllItemsToolStripMenuItem, e);
            //Load our config file in and assign the correct places.                
            var profile = new Xml(openFileDialog1.FileName);
            // Check to make sure we're using the correct version of the config file.
            try {
                if (profile.GetValue("General", "Config Version").ToString() != ConfigVersion) {
                    UtilityFunctions.DisplayMessage("error","This config file appears to be for a different version of MySQL Backup.", "Config Error", false);
                    return;
                }
            }
            catch (Exception) {
                UtilityFunctions.DisplayMessage("error","This appears to be an invalid config file.", "Config Error", false);
                return;
            }
            if (profile.GetValue("General", "OS").ToString() != _os) {
                UtilityFunctions.DisplayMessage("error","This config file appears to be for a different OS than what you are running on. Please adjust the config for this OS.", "Config Error", false);
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
                tbPassword.Text = profile.GetValue("General", "SQLPassword").ToString() != string.Empty ? AESGCM.SimpleDecrypt(profile.GetValue("General", "SQLPassword").ToString(), Encoding.UTF8.GetBytes(AESGCM.Sha256(tbHostName.Text))) : string.Empty;
                tbDumpLocation.Text = profile.GetValue("General", "MySQLDump").ToString();
                tbMySQLDumpOptions.Text = profile.GetValue("General", "MySQLDumpOptions").ToString();
                tbSaveLocation.Text = profile.GetValue("General", "SaveLocation").ToString();
                tbDaystoKeep.Text = profile.GetValue("General", "DaysToKeep").ToString();
                cbCompress.Checked = (profile.GetValue("General", "CompressBackup").ToString() == "Checked") ? true: false;
                cbRemoveDumpFile.Checked = (profile.GetValue("General", "RemoveDumpFile").ToString() == "Checked") ? true : false;
                cbDBDirs.Checked = (profile.GetValue("General", "DBDirs").ToString() == "Checked") ? true : false;
                cbSendEmail.Checked = (profile.GetValue("General", "SendEMail").ToString() == "Checked") ? true : false;
                tbSMTPServer.Text = profile.GetValue("General", "SMTPServer").ToString();
                tbSMTPPort.Text = profile.GetValue("General", "SMTPPort").ToString();
                tbSMTPUserName.Text = profile.GetValue("General", "SMTPUsername").ToString();
                tbSMTPPassword.Text = (profile.GetValue("General", "SMTPPassword").ToString() != string.Empty) ? AESGCM.SimpleDecrypt(profile.GetValue("General", "SMTPPassword").ToString(), Encoding.UTF8.GetBytes(AESGCM.Sha256(tbHostName.Text))) : string.Empty;
                tbEmailAddress.Text = profile.GetValue("General", "EmailAddress").ToString();
                tbFromAddress.Text = profile.GetValue("General", "FromAddress").ToString();
                if (profile.GetValue("Databases", "CheckedDatabases").ToString() != string.Empty) {
                    // get the list of checked DBs
                    var checkedDbList = profile.GetValue("Databases", "CheckedDatabases").ToString().Split(',');
                    // iterate through each one and add it to the list checked.
                    foreach (var cDBname in checkedDbList) {
                        clbDatabases.Items.Add(cDBname, true);
                    }
                }
                if (profile.GetValue("Databases", "unCheckedDatabases").ToString() != string.Empty) {
                    // get the list of unchecked DBs
                    var unCheckedDbList = profile.GetValue("Databases", "unCheckedDatabases").ToString().Split(',');
                    // iterate through each one and add it to the list unchecked.
                    foreach (var uDBname in unCheckedDbList) {
                        clbDatabases.Items.Add(uDBname, false);
                    }
                }
            }
            catch (Exception) {
                UtilityFunctions.DisplayMessage("error","There was a problem parsing the config file.\n Not all entries were found and it is doubtful it will work properly.", "Config Error", false);
                return;
            }
            saveToolStripMenuItem.Enabled = true;
            tsslCurrentConfig.Text = openFileDialog1.SafeFileName;
            _configLocation = Path.GetDirectoryName(openFileDialog1.FileName);
            tsmiDatabaseInfo.Enabled = true;
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //    Save config file based on sender                                               //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void SaveConfigFile(object sender, EventArgs e) { 
            // Do some checking for errors and report them.
            if (tbDumpLocation.Text == string.Empty) {
                if (!UtilityFunctions.DisplayMessage("error","MySQLDump location is empty.\nThis program depends on MySQLDump to perform the backup.\nContinuing with the save will result in a config file that does not work.", "Error", true)) {
                    return;
                }
            }
            var fileNameToSave = string.Empty;
            // Shouldn't be possible to click this item without the name being present,
            // but we'll check anyways.
            if (sender == saveToolStripMenuItem && tsslCurrentConfig.Text == string.Empty) {
                return;
            }
            if (sender == saveToolStripMenuItem && tsslCurrentConfig.Text != string.Empty) {
                if (_os == "Windows") {
                    fileNameToSave = _configLocation + @"\" + tsslCurrentConfig.Text;
                }
                else {
                    fileNameToSave = _configLocation + @"/" + tsslCurrentConfig.Text;
                }
            }
            if (sender == saveAsToolStripMenuItem) {
                saveFileDialog1.Filter = "XML Config (.xml)|*.xml";
                saveFileDialog1.InitialDirectory = Path.GetDirectoryName(Application.ExecutablePath) + @"\configs";
                saveFileDialog1.FileName = tbHostName.Text + ".xml";
                if (saveFileDialog1.ShowDialog() == DialogResult.OK) {
                    fileNameToSave = saveFileDialog1.FileName;
                }
                else {
                    return;
                }
            }
            // iterate through all the databases listed and decide which list to put them on.
            var checkedDatabases = new List<string>();
            var uncheckedDatabases = new List<string>();
            for (var x = 0; x < clbDatabases.Items.Count; x++) {
                if (clbDatabases.GetItemChecked(x)) {
                    checkedDatabases.Add((string)clbDatabases.Items[x]);
                }
                else {
                    uncheckedDatabases.Add((string)clbDatabases.Items[x]);
                }
            }
            // encrypt the sql and smtp passwords
            var sqlPassword = (tbPassword.Text != string.Empty) ? AESGCM.SimpleEncrypt(tbPassword.Text, Encoding.UTF8.GetBytes(AESGCM.Sha256(tbHostName.Text))) : string.Empty;
            var smtpPassword = (tbSMTPPassword.Text != string.Empty) ? AESGCM.SimpleEncrypt(tbSMTPPassword.Text, Encoding.UTF8.GetBytes(AESGCM.Sha256(tbHostName.Text))) : string.Empty;
            // Open our .xml file and save the entries to it.
            try {
                var profile = new Xml(fileNameToSave);
                profile.SetValue("General", "Config Version", "1.0");
                profile.SetValue("General", "OS", rbWindows.Checked ? "Windows" : "Linux");
                profile.SetValue("General", "Server", tbHostName.Text);
                profile.SetValue("General", "Port", tbPort.Text);
                profile.SetValue("General", "Username", tbUserName.Text);
                profile.SetValue("General", "SQLPassword", sqlPassword);
                profile.SetValue("General", "MySQLDump", tbDumpLocation.Text);
                profile.SetValue("General", "MySQLDumpOptions", tbMySQLDumpOptions.Text);
                profile.SetValue("General", "SaveLocation", tbSaveLocation.Text);
                profile.SetValue("General", "DaysToKeep", tbDaystoKeep.Text);
                profile.SetValue("General", "CompressBackup", cbCompress.CheckState);
                profile.SetValue("General", "RemoveDumpFile", cbRemoveDumpFile.CheckState);
                profile.SetValue("General", "DBDirs", cbDBDirs.CheckState);
                profile.SetValue("General", "SendEMail", cbSendEmail.CheckState);
                profile.SetValue("General", "SMTPServer", tbSMTPServer.Text);
                profile.SetValue("General", "SMTPPort", tbSMTPPort.Text);
                profile.SetValue("General", "SMTPUsername", tbSMTPUserName.Text);
                profile.SetValue("General", "SMTPPassword", smtpPassword);
                profile.SetValue("General", "EmailAddress", tbEmailAddress.Text);
                profile.SetValue("General", "FromAddress", tbFromAddress.Text);
                profile.SetValue("Databases", "CheckedDatabases", string.Join(",", checkedDatabases));
                profile.SetValue("Databases", "unCheckedDatabases", string.Join(",", uncheckedDatabases));
                tsslCurrentConfig.Text = Path.GetFileName(fileNameToSave);
                saveToolStripMenuItem.Enabled = true;
                UtilityFunctions.DisplayMessage("information","Config file saved successfully.", "Config Saved", false);
            }
            catch {
                UtilityFunctions.DisplayMessage("error","Error saving config file.", "Error Saving Config", false);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //    Create a new config file - clear items based on sender                         //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void ClearConfigItems(object sender, EventArgs e) {
            if (sender == newToolStripMenuItem) {
                saveToolStripMenuItem.Enabled = false;
                tbHostName.Text = string.Empty;
                tbUserName.Text = string.Empty;
                tbPassword.Text = string.Empty;
                tbPort.Text = string.Empty;
                tsslCurrentConfig.Text = "None";
                tsmiDatabaseInfo.Enabled = false;
                clbDatabases.Items.Clear();
                cbCompress.Checked = false;
                cbDBDirs.Checked = false;
                cbRemoveDumpFile.Checked = false;
                tbDumpLocation.Text = string.Empty;
                tbMySQLDumpOptions.Text = string.Empty;
                tbSaveLocation.Text = string.Empty;
                tbDaystoKeep.Text = string.Empty;
                rtbOutput.Text = string.Empty;
            }
            else if (sender == clearAllItemsToolStripMenuItem) {
                saveToolStripMenuItem.Enabled = false;
                tbHostName.Text = string.Empty;
                tbUserName.Text = string.Empty;
                tbPassword.Text = string.Empty;
                tbPort.Text = string.Empty;
                tsslCurrentConfig.Text = "None";
                tsmiDatabaseInfo.Enabled = false;
                clbDatabases.Items.Clear();
                cbCompress.Checked = false;
                cbDBDirs.Checked = false;
                cbRemoveDumpFile.Checked = false;
                tbDumpLocation.Text = string.Empty;
                tbMySQLDumpOptions.Text = string.Empty;
                tbSaveLocation.Text = string.Empty;
                tbDaystoKeep.Text = string.Empty;
                cbSendEmail.Checked = false;
                tbSMTPServer.Text = string.Empty;
                tbSMTPUserName.Text = string.Empty;
                tbSMTPPassword.Text = string.Empty;
                tbSMTPPort.Text = string.Empty;
                tbEmailAddress.Text = string.Empty;
                tbFromAddress.Text = string.Empty;
                rtbOutput.Text = string.Empty;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //    Select and Unselect all databases                                              //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void CbSelectDatabases_CheckedChanged(object sender, EventArgs e) {
            switch (cbSelectDatabases.CheckState) {
                case CheckState.Checked:
                    for (var x = 0; x < clbDatabases.Items.Count; x++) {
                        clbDatabases.SetItemChecked(x, true);
                    }
                    cbSelectDatabases.Text = "Unselect All Databases";
                    break;
                case CheckState.Unchecked:
                    for (var x = 0; x < clbDatabases.Items.Count; x++) {
                        clbDatabases.SetItemChecked(x, false);
                    }
                    cbSelectDatabases.Text = "Select All Databases";
                    break;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //    Select save location                                                           //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void BuSaveLocation_Click(object sender, EventArgs e) {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK) {
                tbSaveLocation.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //    Validate the email address                                                     //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void TbEmailAddress_Validating(object sender, CancelEventArgs e) {
            errorProvider1.Clear();
            if (UtilityFunctions.ValidEmailAddress(tbEmailAddress.Text, out var errorMsg)) return;
            // Cancel the event and select the text to be corrected by the user.
            e.Cancel = true;
            tbEmailAddress.Select(0, tbEmailAddress.Text.Length);
            // Set the ErrorProvider error with the text to display.  
            errorProvider1.SetError(tbEmailAddress, errorMsg);
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //    Send a test email                                                              //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void BuTestEmail_Click(object sender, EventArgs e) {
            errorProvider1.Clear();
            tbSMTPServer.BackColor = Color.White;
            tbFromAddress.BackColor = Color.White;
            tbEmailAddress.BackColor = Color.White;
            if (tbSMTPServer.Text != string.Empty && tbEmailAddress.Text != string.Empty && tbFromAddress.Text != string.Empty) {
                var message = UtilityFunctions.SendEmail(new[] { tbSMTPServer.Text, tbSMTPPort.Text, tbSMTPUserName.Text, tbSMTPPassword.Text, tbEmailAddress.Text, tbFromAddress.Text, "MySQLBackup Test Message", "This is a test message." });
                if (message == "OK") {
                    UtilityFunctions.DisplayMessage("information","Email sent successfully.", "Email Sent", false);
                }
                else {
                    UtilityFunctions.DisplayMessage("error",message, "Error", false);
                }
            }
            else {
                if (tbEmailAddress.Text == string.Empty) {
                    errorProvider1.SetError(tbEmailAddress, "Please enter an email address.");
                    tbEmailAddress.BackColor = Color.LightPink;
                    tbEmailAddress.Focus();
                }
                if (tbFromAddress.Text == string.Empty) {
                    errorProvider1.SetError(tbFromAddress, "Please enter a from email address.");
                    tbFromAddress.BackColor = Color.LightPink;
                    tbFromAddress.Focus();
                }
                if (tbSMTPServer.Text == string.Empty) {
                    errorProvider1.SetError(tbSMTPServer, "Please enter the SMTP server name.");
                    tbSMTPServer.BackColor = Color.LightPink;
                    tbSMTPServer.Focus();
                }
                UtilityFunctions.DisplayMessage("error","Please enter the information before trying to send a test email!", "Error", false);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //    Test the current config                                                        //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void BuTestConfig_Click(object sender, EventArgs e) {
            // To make this a quicker process, we could probably open up a zip archive and dump the output directly to it.
            // I choose to create the dump file in a temp directory, create a zip file if asked, move the dump file into it, delete the dump file 
            // if appropriate then move the file into the save location.
            var now = DateTime.Now;
            var dateStamp = now.ToString("yyyy_MM_dd_HH_mm_ss_");
            rtbOutput.Text = string.Empty;
            string tempDir; 
            if (_os == "Windows") {
                tempDir = Path.GetDirectoryName(Application.ExecutablePath) + @"\temp\";
            }
            else {
                tempDir = Path.GetDirectoryName(Application.ExecutablePath) + @"/temp/";
            }
            var dbNames = clbDatabases.Items.Cast<object>().Where((t, x) => clbDatabases.GetItemChecked(x)).Cast<string>().ToList();
            if (!dbNames.Any()) {
                UtilityFunctions.DisplayMessage("error","Please select one database from the list before trying to test.", "Database Selection Error", false);
                return;
            }
            foreach (var dbName in dbNames) {
                string dbSaveDirectory;
                // Check to see if we should be using individual dirs for each database
                if (cbDBDirs.Checked) {
                    if (_os == "Windows") {
                        dbSaveDirectory = tbSaveLocation.Text + @"\" + dbName + @"\";
                    }
                    else {
                        dbSaveDirectory = tbSaveLocation.Text + @"/" + dbName + @"/";
                    }
                }
                else {
                    if (_os == "Windows") {
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
                        string[] lines = { "[mysqldump]", "user=" + tbUserName.Text, "password=" + tbPassword.Text };
                        File.WriteAllLines(tempDir + ".sqlpasswd", lines);
                    }
                    catch (IOException) {
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
                rtbOutput.Text = rtbOutput.Text + "Command Line Used: " + parameters[4] + " " + string.Format(" --defaults-extra-file={1} -h{0} {2} --databases {3} --result-file={4}", parameters[0], parameters[1], parameters[2], parameters[3], parameters[6] + parameters[5] + parameters[3] + ".sql\n");
                //Create a background worker to do the actual backup in.           
                BackgroundWorker backupThread = new BackgroundWorker() {
                    WorkerReportsProgress = false,
                    WorkerSupportsCancellation = false
                };
                backupThread.DoWork += BackupDataBase;
                backupThread.RunWorkerAsync(parameters);
                // Start the control to show it is doing something
                toolStripProgressBar1.Visible = true;
                toolStripProgressBar1.Enabled = true;
                // Wait while the thread completes, doevents so the form is not frozen.
                while (backupThread.IsBusy) {
                    Application.DoEvents();
                }
                // Check to see if we return anything other than OK.
                if (_error != "OK") {
                    rtbOutput.Text = _error;
                    toolStripProgressBar1.Visible = false;
                    toolStripProgressBar1.Enabled = false;
                    if (cbSendEmail.Checked && tbSMTPServer.Text != string.Empty && tbEmailAddress.Text != string.Empty && tbFromAddress.Text != string.Empty) {
                        UtilityFunctions.SendEmail(new []
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
                    File.Delete(tempDir + ".sqlpasswd");
                }
                catch (IOException ex) {
                    _error = ex.Message;
                }
                // Make the cancel button invisible
                buCancelTest.Visible = false;
                rtbOutput.Text = rtbOutput.Text + "Dump of " + dbName + " Complete.\n";
                if (cbCompress.Checked) {
                    // create a background worker to do the zip.
                    var zipThread = new BackgroundWorker() {
                        WorkerReportsProgress = false,
                        WorkerSupportsCancellation = false
                    };
                    zipThread.DoWork += ZipDataBase;
                    zipThread.RunWorkerAsync(parameters);
                    // Wait while the thread completes, doevents so the form is not frozen.
                    while (zipThread.IsBusy) {
                        Application.DoEvents();
                    }
                    if (_error == "OK") {
                        rtbOutput.Text = rtbOutput.Text + "Zip of " + dbName + " Complete.\n";
                    }
                    else {
                        rtbOutput.Text = rtbOutput.Text + _error;
                        toolStripProgressBar1.Visible = false;
                        toolStripProgressBar1.Enabled = false;
                        if (cbSendEmail.Checked && tbSMTPServer.Text != string.Empty && tbEmailAddress.Text != string.Empty && tbFromAddress.Text != string.Empty) {
                            UtilityFunctions.SendEmail(new []
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
                    catch (IOException) {
                        UtilityFunctions.DisplayMessage("error","Unable to delete the .SQL dump file.", "Error", false);
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
                    UtilityFunctions.DisplayMessage("error",ex.Message, "error", false);
                    rtbOutput.Text = rtbOutput.Text + "Unable to move file(s) to save directory.\n";
                    toolStripProgressBar1.Visible = false;
                    toolStripProgressBar1.Enabled = false;
                    if (cbSendEmail.Checked && tbSMTPServer.Text != string.Empty && tbEmailAddress.Text != string.Empty && tbFromAddress.Text != string.Empty) {
                        UtilityFunctions.SendEmail(new []
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
            if (cbSendEmail.Checked && tbSMTPServer.Text != string.Empty && tbEmailAddress.Text != string.Empty && tbFromAddress.Text != string.Empty) {
                UtilityFunctions.SendEmail(new []
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

        private void BackupDataBase(object sender, DoWorkEventArgs e) {
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
            var arg = e.Argument as object[];
            _error = string.Empty;
            //create start info...
            if (arg != null) {
                var startInfo = new ProcessStartInfo() {
                    FileName = (string)arg[4],
                    //put it all together, notice that were using the --result-file command line. This allows us to not have to use a streamwriter with redirected output.
                    Arguments = string.Format("--defaults-extra-file=\"{1}\" -h{0} {2} --databases {3} --result-file=\"{4}\"", (string)arg[0], (string)arg[1], (string)arg[2], (string)arg[3], (string)arg[6] + (string)arg[5] + (string)arg[3] + ".sql"),
                    RedirectStandardError = true,
                    RedirectStandardInput = false,
                    RedirectStandardOutput = false, //we do not need to redirect the standard output to a StreamWriter
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    ErrorDialog = false
                };
                //create the process...
                var proc = new Process() {
                    StartInfo = startInfo
                };
                //start the process...
                try {
                    proc.Start();
                }
                catch (Exception ex) {
                    UtilityFunctions.DisplayMessage("error",ex.Message, "Error", false);
                    return;
                }
                // Get the process ID so we can kill it if necessary.
                _processId = proc.Id;
                _error = proc.StandardError.ReadToEnd();
                if (proc.ExitCode == 1 || _processTerminated) {
                    _error = "mysqldump.exe process was terminated.";
                }
                //close the process...
                proc.Close();
            }

            if (_error != string.Empty) {
                // Cleanup the empty file 
                try {
                    if (arg != null) File.Delete((string) arg[6] + (string) arg[5] + (string) arg[3] + ".sql");
                }
                catch (Exception ex) {
                    _error = ex.Message;
                }
                e.Cancel = true;
                return;
            }
            //MySQLDump.
            _error = "OK";
            e.Cancel = true;
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //    Database zip thread                                                            //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void ZipDataBase(object sender, DoWorkEventArgs e) {
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
            _error = string.Empty;
            var arg = e.Argument as object[];
            try {
                // Now were going to zip the file up.            
                if (arg != null) {
                    var zip = ZipFile.Open((string)arg[6] + (string)arg[5] + (string)arg[3] + ".zip", ZipArchiveMode.Create);
                    zip.CreateEntryFromFile((string)arg[6] + (string)arg[5] + (string)arg[3] + ".sql", (string)arg[5] + (string)arg[3] + ".sql");
                    zip.Dispose();
                }
            }
            catch (IOException ex) {
                _error = "Zip process failed: " + ex.Message + "\n";
                e.Cancel = true;
                return;
            }
            _error = "OK";
            e.Cancel = true;
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //    Cancel the running mysqldump process                                           //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void BuCancelTest_Click(object sender, EventArgs e) {
            buCancelTest.Visible = false;
            var p = Process.GetProcessById(_processId);
            if (p.HasExited) return;
            p.Kill();
            _processTerminated = true;
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //    Check Boxes not correct                                                        //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void CbRemoveDumpFile_CheckedChanged(object sender, EventArgs e) {
            if (!cbRemoveDumpFile.Checked || cbCompress.Checked) return;
            if (UtilityFunctions.DisplayMessage("error","Checking this and not checking the Compress Backup box will result in your backup file being deleted!\nThis is designed to let you keep both the original dump file and the .zip file.", "File Deletion Warning", true) == false) {
                cbRemoveDumpFile.Checked = false;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //    Send Email Checkbox                                                            //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void CbSendEmail_CheckedChanged(object sender, EventArgs e) {
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
        //   Show the about dialog                                                           //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e) {
            var about = new frAbout();
            about.ShowDialog();
        }
        
        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //    Open dialog to find MySQL dump                                                 //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void BuDumpLocation_Click(object sender, EventArgs e) { 
            openFileDialog1.FileName = string.Empty;
            openFileDialog1.Filter = (_os == "Windows") ? "MySQL Dump (.exe)|*.exe" : "MySQL Dump|mysqldump";
            if (openFileDialog1.ShowDialog() == DialogResult.OK) {
                tbDumpLocation.Text = openFileDialog1.FileName;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //    Edit the backup schedules                                                      //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void EditScheduleToolStripMenuItem_Click(object sender, EventArgs e) {
            //ScheduleGUI scheduleForm = new ScheduleGUI();
            //scheduleForm.ShowDialog();
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //    Start the job scheduler                                                        //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void StartSchedulerToolStripMenuItem_Click(object sender, EventArgs e) {
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

        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //    Database Information                                                           //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void TsmiDatabaseInfo_Click(object sender, EventArgs e) {
            // Display the database information form
            DatabaseInfo databaseInfoForm = new DatabaseInfo();            
            databaseInfoForm.ShowDialog(clbDatabases.SelectedItem.ToString(), tbHostName.Text, tbUserName.Text, tbPassword.Text, tbPort.Text);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------//
        // RESTORE TAB ROUTINES
        //-----------------------------------------------------------------------------------------------------------------------------------------//

        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //    Open dialog to find MySQL exe                                                  //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void BuRestoreMySQLRequestor_Click(object sender, EventArgs e) {
            openFileDialog1.FileName = string.Empty;
            openFileDialog1.Filter = (_os == "Windows") ? "MySQL (.exe)|mysql.exe" : "MySQL|mysql";
            if (openFileDialog1.ShowDialog() == DialogResult.OK) {
                tbRestoreMySQLLocation.Text = openFileDialog1.FileName;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //    Open dialog to open dump file                                                  //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void BuRestoreFileLocationRequestor_Click(object sender, EventArgs e) {
            openFileDialog1.FileName = string.Empty;
            openFileDialog1.Filter = "SQL File (.sql)|*.sql|ZIP Files (.zip)|*.zip";
            if (openFileDialog1.ShowDialog() == DialogResult.OK) { 
                tbRestoreFileLocation.Text = openFileDialog1.FileName;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //    Restore a file to a database                                                   //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void BuRestoreDatabase_Click(object sender, EventArgs e) {
            errorProvider1.Clear();
            rtbRestoreProgress.Text = string.Empty;
            //string restoreFile = tbRestoreFileLocation.Text;
            tbRestoreHostName.BackColor = Color.White;
            tbRestoreUserName.BackColor = Color.White;
            tbRestorePassword.BackColor = Color.White;
            tbRestorePort.BackColor = Color.White;
            if (tbRestoreHostName.Text != string.Empty && tbRestoreUserName.Text != string.Empty && tbRestorePassword.Text != string.Empty && tbRestorePort.Text != string.Empty) {
                string tempDir;
                // Setup our connection variables
                var mySqlConnect = UtilityFunctions.DbConnect("server=" + tbRestoreHostName.Text + ";user id=" + tbRestoreUserName.Text + ";port=" + tbRestorePort.Text + ";database=mysql;pooling=false;allow user variables=true;password=" + tbRestorePassword.Text);
                if (mySqlConnect.State == System.Data.ConnectionState.Open) {  
                    UtilityFunctions.DbClose(mySqlConnect);
                }
                else {
                    return;
                }
                //Looks like we could connect to the database server with the supplied credentials.
                // Now were going to unzip the file.  
                if (_os == "Windows") {
                    tempDir = Path.GetDirectoryName(Application.ExecutablePath) + @"\temp\";
                }
                else {
                    tempDir = Path.GetDirectoryName(Application.ExecutablePath) + @"/temp/";
                }
                // Create a string array to hold all of our parameters in.
                string[] parameters =
                {
                    tbRestoreFileLocation.Text,tempDir,"no",tbRestoreHostName.Text,tbRestorePort.Text,tbRestoreUserName.Text,tbRestorePassword.Text,tbRestoreMySQLLocation.Text,tbRestoreCommandLine.Text
                };
                // Start the control to show it is doing something
                toolStripProgressBar1.Visible = true;
                toolStripProgressBar1.Enabled = true;
                // What is the file we've been given? Is it a zip? If so, we need to unpack it.
                if (Path.GetExtension(parameters[0]) == ".zip") {
                    //was the file zipped? Change element to yes.
                    parameters[2] = "yes";
                    // create a background worker to do the unzip.
                    var unzipThread = new BackgroundWorker() {
                        WorkerReportsProgress = false,
                        WorkerSupportsCancellation = false
                    };
                    unzipThread.DoWork += UnzipDataBase;
                    unzipThread.RunWorkerAsync(parameters);
                    // Wait while the thread completes, doevents so the form is not frozen.
                    while (unzipThread.IsBusy) {
                        Application.DoEvents();
                    }
                    if (_error == "OK") {
                        rtbRestoreProgress.Text = rtbRestoreProgress.Text + "unZip of " + Path.GetFileNameWithoutExtension(parameters[0]) + " Complete.\n";                        
                    }
                    else {
                        rtbRestoreProgress.Text = rtbRestoreProgress.Text + _error;
                        toolStripProgressBar1.Visible = false;
                        toolStripProgressBar1.Enabled = false;                        
                        return;
                    }
                    // Set the restore file name to the newly unzipped file.
                    parameters[0] = tempDir + Path.GetFileNameWithoutExtension(parameters[0]) + ".sql";
                }
                //utilityFunctions.displayInformationMessage("Command Line Used: " + parameters[5] + " " + String.Format("-h{0} -u{1} -p[password] {2} --databases {3} --result-file={4}", parameters[0], parameters[1], parameters[3], parameters[4], parameters[7] + parameters[6] + parameters[4] + ".sql\n"),"Message", false);
                rtbRestoreProgress.Text = rtbRestoreProgress.Text + "Command Line Used: " + parameters[4] + " " + string.Format(" --defaults-extra-file={1} -h{0} {2} --databases {3} --result-file={4}", parameters[0], parameters[1], parameters[2], parameters[3], parameters[6] + parameters[5] + parameters[3] + ".sql\n");
                //Create a background worker to do the actual backup in.           
                var restoreThread = new BackgroundWorker()
                {
                    WorkerReportsProgress = false,
                    WorkerSupportsCancellation = false
                };
                restoreThread.DoWork += RestoreDataBase;
                restoreThread.RunWorkerAsync(parameters);
                // Start the control to show it is doing something
                toolStripProgressBar1.Visible = true;
                toolStripProgressBar1.Enabled = true;
                // Wait while the thread completes, doevents so the form is not frozen.
                while (restoreThread.IsBusy) {
                    Application.DoEvents();
                }
                // Check to see if we return anything other than OK.
                if (_error != "OK") {
                    rtbOutput.Text = _error;
                    toolStripProgressBar1.Visible = false;
                    toolStripProgressBar1.Enabled = false;
                    // Make the cancel button invisible
                    buCancelRestore.Visible = false;
                    return;
                }
                //Dump was completed, remove the crendentials file we created
                /*try
                {
                    File.Delete(@tempDir + ".sqlpasswd");
                }
                catch (IOException ex)
                {
                    error = ex.Message;
                }*/
                // Make the cancel button invisible
                buCancelTest.Visible = false;
                //rtbOutput.Text = rtbOutput.Text + "Dump of " + dbName + " Complete.\n";



                /*parameters
                // The file has been passed down or unzipped and stored in the restoreFile variable. Time to restore it.
                parame =
                {
                    restoreFile,tempDir
                };*/

            }
            else {
                if (tbRestorePort.Text == string.Empty) {
                    errorProvider1.SetError(tbRestorePort, "Please enter the MySQL connection port.");
                    tbRestorePort.BackColor = Color.LightPink;
                    tbRestorePort.Focus();
                }
                if (tbRestorePassword.Text == string.Empty) {
                    errorProvider1.SetError(tbRestorePassword, "Please enter the MySQL user password.");
                    tbRestorePassword.BackColor = Color.LightPink;
                    tbRestorePassword.Focus();
                }
                if (tbRestoreUserName.Text == string.Empty) {
                    errorProvider1.SetError(tbRestoreUserName, "Please enter the MySQL user name.");
                    tbRestoreUserName.BackColor = Color.LightPink;
                    tbRestoreUserName.Focus();
                }
                if (tbRestoreHostName.Text == string.Empty) {
                    errorProvider1.SetError(tbRestoreHostName, "Please enter the MySQL host to connect to.");
                    tbRestoreHostName.BackColor = Color.LightPink;
                    tbRestoreHostName.Focus();
                }
                UtilityFunctions.DisplayMessage("error","Please enter the information before trying to restore a database!", "Error", false);
            }
            toolStripProgressBar1.Visible = false;
            toolStripProgressBar1.Enabled = false;
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //    Database unzip thread                                                          //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void UnzipDataBase(object sender, DoWorkEventArgs e) {
            // Parameters
            /*
               0=file path
               1=Temp Dir Location
               2=zipped or not
               3=HostName
               4=Port
               5=UserName
               6=Password
               7=MySQL Locationt
               8=CommandLine
             */
            _error = string.Empty;
            var arg = e.Argument as object[];
            try {
                // Now were going to unzip the file.            
                if (arg != null) ZipFile.ExtractToDirectory((string) arg[0], (string) arg[1]);
            }
            catch (IOException ex) {
                _error = "unZip process failed: " + ex.Message + "\n";
                e.Cancel = true;
                return;
            }
            _error = "OK";
            e.Cancel = true;
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //    Database restore thread                                                        //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void RestoreDataBase(object sender, DoWorkEventArgs e) {
            // Parameters
            /*
               0=file path
               1=Temp Dir Location
               2=zipped or not
               3=HostName
               4=Port
               5=UserName
               6=Password
               7=MySQL Locationt
               8=CommandLine
             */
            var arg = e.Argument as object[];
            _error = string.Empty;
            //create start info...
            if (arg != null) {
                var startInfo = new ProcessStartInfo() {
                    FileName = (string)arg[4],
                    //put it all together, notice that were using the --result-file command line. This allows us to not have to use a streamwriter with redirected output.
                    Arguments = string.Format("--defaults-extra-file=\"{1}\" -h{0} {2} --databases {3} --result-file=\"{4}\"", (string)arg[0], (string)arg[1], (string)arg[2], (string)arg[3], (string)arg[6] + (string)arg[5] + (string)arg[3] + ".sql"),
                    RedirectStandardError = true,
                    RedirectStandardInput = false,
                    RedirectStandardOutput = false, //we do not need to redirect the standard output to a StreamWriter
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    ErrorDialog = false
                };
                //create the process...
                var proc = new Process() {
                    StartInfo = startInfo
                };
                //start the process...
                try {
                    proc.Start();
                }
                catch (Exception ex) {
                    UtilityFunctions.DisplayMessage("error",ex.Message, "Error", false);
                    return;
                }
                // Get the process ID so we can kill it if necessary.
                _processId = proc.Id;
                _error = proc.StandardError.ReadToEnd();
                if (proc.ExitCode == 1 || _processTerminated) {
                    _error = "mysqldump.exe process was terminated.";
                }
                //close the process...
                proc.Close();
            }

            if (_error != string.Empty) {
                // Cleanup the empty file 
                try {
                    if (arg != null) File.Delete((string) arg[6] + (string) arg[5] + (string) arg[3] + ".sql");
                }
                catch (Exception ex) {
                    _error = ex.Message;
                }
                e.Cancel = true;
                return;
            }
            //MySQLDump.
            _error = "OK";
            e.Cancel = true;
        }
    }
} 