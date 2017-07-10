using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
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
    public partial class frMain : Form {

        // Universal key used for encryption and decryption functions
        // Key must be 256bit or 32bytes
        // If key is changed after a config file is saved, the passwords will not be decrypted properly.
        string encryptionKey = "LjjS5PDETyN98JZDgGtdPlhbk7-fBCVN";

        // Not the best way to do this, but trying to get status flags from processes running in a shell/seperate 
        // thread is not easy
        string error = "";
        int processID = 0;
        bool processTerminated = false;
        // The current config version
        string config_version = "1.0";

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
            toolTip1.SetToolTip(this.buTestConfig, @"The test will use the first checked database in the list.");
            
            // Create our fileinfo object
            var objFileInfo = new FileInfo(Application.ExecutablePath);
            // To get the lastwrite time of this file
            var dtCreationDate = objFileInfo.LastWriteTime;
            // Set our Application Title
            Text = Application.ProductName + " v" + Application.ProductVersion + " build " + dtCreationDate.ToString("MMddyy");
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

        private void buTestConnection_Click(object sender, EventArgs e) {
            errorProvider1.Clear();
            tbHostName.BackColor = Color.White;
            tbUserName.BackColor = Color.White;
            tbPassword.BackColor = Color.White;
            tbPort.BackColor = Color.White;


            if (tbHostName.Text != "" && tbUserName.Text != "" && tbPassword.Text != "" && tbPort.Text != "") {
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
                if (tbPort.Text == "") {
                    errorProvider1.SetError(tbPort, "Please enter the MySQL connection port.");
                    tbPort.BackColor = Color.LightPink;
                    tbPort.Focus();
                }
                if (tbPassword.Text == "") {
                    errorProvider1.SetError(tbPassword, "Please enter the MySQL user password.");
                    tbPassword.BackColor = Color.LightPink;
                    tbPassword.Focus();
                }
                if (tbUserName.Text == "") {
                    errorProvider1.SetError(tbUserName, "Please enter the MySQL user name.");
                    tbUserName.BackColor = Color.LightPink;
                    tbUserName.Focus();
                }
                if (tbHostName.Text == "") {
                    errorProvider1.SetError(tbHostName, "Please enter the MySQL host to connect to.");
                    tbHostName.BackColor = Color.LightPink;
                    tbHostName.Focus();
                }
                utilityFunctions.displayErrorMessage("Please enter the  information before trying to connect!", "Error",false);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //    Open dialog to find MySQL dump exe                                             //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void buDumpLocation_Click(object sender, EventArgs e) {
            openFileDialog1.Filter = "MySQL Dump (.exe)|*.exe";
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
            if (tbDumpLocation.Text == "") {
                if (!utilityFunctions.displayErrorMessage("MySQLDump.exe location is empty.\nThis program depends on MySQLDump.exe to perform the backup.\nContinuing with the save will result in a config file that does not work.","Error",true)) {
                    return;
                }
            }
            string fileNameToSave = "";
            // Shouldn't be possible to click this item without the name being present,
            // but we'll check anyways.
            if (sender == saveToolStripMenuItem && tsslCurrentConfig.Text == "") {
                return;
            }
            else if (sender == saveToolStripMenuItem && tsslCurrentConfig.Text != "")  {
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
            string SQLPassword = "";
            if (tbPassword.Text != "") {
                SQLPassword = Convert.ToBase64String(AESGCM.SimpleEncrypt(Encoding.UTF8.GetBytes(tbPassword.Text), Encoding.UTF8.GetBytes(encryptionKey)));
            }
            else {
                SQLPassword = "";
            }
            string SMTPPassword = "";
            if (tbSMTPPassword.Text != "") {
                SMTPPassword = Convert.ToBase64String(AESGCM.SimpleEncrypt(Encoding.UTF8.GetBytes(tbSMTPPassword.Text), Encoding.UTF8.GetBytes(encryptionKey)));
            }
            else {
                SMTPPassword = "";
            }
            // Open our .xml file and save the entries to it.
            try {
                Xml profile = new Xml(fileNameToSave);
                profile.SetValue("General", "Config Version", "1.0");
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
                // Load our values, fail if something is missing.
                try {
                    tbHostName.Text = profile.GetValue("General", "Server").ToString();
                    tbPort.Text = profile.GetValue("General", "Port").ToString();
                    tbUserName.Text = profile.GetValue("General", "Username").ToString();
                    if (profile.GetValue("General", "SQLPassword").ToString() != "") {
                        tbPassword.Text = AESGCM.SimpleDecrypt(profile.GetValue("General", "SQLPassword").ToString(), Encoding.UTF8.GetBytes(encryptionKey));
                    }
                    else {
                        tbPassword.Text = "";
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
                    if (profile.GetValue("General", "SMTPPassword").ToString() != "") {
                        tbSMTPPassword.Text = AESGCM.SimpleDecrypt(profile.GetValue("General", "SMTPPassword").ToString(), Encoding.UTF8.GetBytes(encryptionKey));
                    }
                    else {
                        tbSMTPPassword.Text = "";
                    }
                    tbEmailAddress.Text = profile.GetValue("General", "EmailAddress").ToString();
                    tbFromAddress.Text = profile.GetValue("General", "FromAddress").ToString();

                    if (profile.GetValue("Databases", "CheckedDatabases").ToString() != "") {
                        // get the list of checked DBs
                        string[] checkedDBList = profile.GetValue("Databases", "CheckedDatabases").ToString().Split(',');
                        // iterate through each one and add it to the list checked.
                        for (int x = 0; x < checkedDBList.Length; x++) {
                            clbDatabases.Items.Add(checkedDBList[x], true);
                        }
                    }
                    if (profile.GetValue("Databases", "unCheckedDatabases").ToString() != "") {
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
                tsslCurrentConfig.Text = openFileDialog1.FileName;
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
                tbHostName.Text = "";
                tbUserName.Text = "";
                tbPassword.Text = "";
                tbPort.Text = "";
                tsslCurrentConfig.Text = "No Config Selected";
                clbDatabases.Items.Clear();
                tbDumpLocation.Text = "";
                tbMySQLDumpOptions.Text = "";
                tbSaveLocation.Text = "";
                tbDaystoKeep.Text = "";
                rtbOutput.Text = "";
            }
            else if (sender == clearAllItemsToolStripMenuItem) {
                saveToolStripMenuItem.Enabled = false;
                tbHostName.Text = "";
                tbUserName.Text = "";
                tbPassword.Text = "";
                tbPort.Text = "";
                tsslCurrentConfig.Text = "No Config Selected";
                clbDatabases.Items.Clear();
                tbDumpLocation.Text = "";
                tbMySQLDumpOptions.Text = "";
                tbSaveLocation.Text = "";
                tbDaystoKeep.Text = "";
                cbSendEmail.Checked = false;
                tbSMTPServer.Text = "";
                tbSMTPUserName.Text = "";
                tbSMTPPassword.Text = "";
                tbSMTPPort.Text = "";
                tbEmailAddress.Text = "";
                tbFromAddress.Text = "";
                rtbOutput.Text = "";
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
                        
            if (tbSMTPServer.Text !="" && tbEmailAddress.Text != "" && tbFromAddress.Text !="") {   
                string message = utilityFunctions.SendEmail(new string[] { tbSMTPServer.Text, tbSMTPPort.Text, tbSMTPUserName.Text,tbSMTPPassword.Text, tbEmailAddress.Text, tbFromAddress.Text, "MySQLBackup Test Message", "This is a test message." });
                if (message == "OK") {
                    utilityFunctions.displayInformationMessage("Email sent successfully.", "Email Sent", false);
                }
                else {
                    utilityFunctions.displayErrorMessage(message, "Error", false);
                }                
            }
            else {
                
                if (tbEmailAddress.Text == "") {
                    errorProvider1.SetError(tbEmailAddress, "Please enter an email address.");
                    tbEmailAddress.BackColor = Color.LightPink;
                    tbEmailAddress.Focus();
                }
                if (tbFromAddress.Text == "") {
                    errorProvider1.SetError(tbFromAddress, "Please enter a from email address.");
                    tbFromAddress.BackColor = Color.LightPink;
                    tbFromAddress.Focus();
                }
                if (tbSMTPServer.Text == "") {
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
            // Since this is a test, we're only going to do one backup.
            // To make this a quicker process, we could probably open up a zip archive and dump the output directly to it.
            // I chose to create the dump file in a temp directory, create a zip file if asked, move the dump file into it, delete the dump file 
            // if appropriate then move the file into the save location.

            string dbName = "";
            string dbSaveDirectory = "";
            DateTime now = DateTime.Now;
            string dateStamp = now.ToString("yyyy_MM_dd_HH_mm_ss_");
  
            rtbOutput.Text = "";

            //Get the first database name from the list that has been checked. We'll use that.
            for (int x = 0; x < clbDatabases.Items.Count; x++) {
                if (clbDatabases.GetItemChecked(x)) {
                    dbName = (string)clbDatabases.Items[x];
                    break;
                }
            }

            if (dbName == "") {
                utilityFunctions.displayErrorMessage("Please select one database from the list before trying to test.", "Database Selection Error", false);
                return;
            }

            // Check to see if we should be using individual dirs for each database
            if (cbDBDirs.Checked) {
                dbSaveDirectory = tbSaveLocation.Text + @"\" + dbName + @"\";
            }
            else {
                dbSaveDirectory = tbSaveLocation.Text + @"\";
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
            if (!Directory.Exists(@"temp\")) {
                try {
                    Directory.CreateDirectory(@"temp\");
                }
                catch (IOException) {
                    rtbOutput.Text = "Could not create temp directory and it does not exist, aborting backup.";
                    return;
                }
            }
            // Make the cancel button visible
            buCancelTest.Visible = true;

            // Create a string array to hold all of our parameters in.
            string[] parameters = { tbHostName.Text, tbUserName.Text, tbPassword.Text, tbMySQLDumpOptions.Text, dbName, tbDumpLocation.Text, dateStamp };

            rtbOutput.Text = "Command Line Used: " + parameters[5] + " " + String.Format("-h{0} -u{1} -p[password] {2} --databases {3} --result-file={4}", parameters[0], parameters[1], parameters[3], parameters[4], @"temp\" + parameters[6] + parameters[4] + ".sql\n");
                
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
                if (cbSendEmail.Checked && tbSMTPServer.Text != "" && tbEmailAddress.Text != "" && tbFromAddress.Text != "") {
                    string message = utilityFunctions.SendEmail(new string[] { tbSMTPServer.Text, tbSMTPPort.Text, tbSMTPUserName.Text, tbSMTPPassword.Text, tbEmailAddress.Text, tbFromAddress.Text, "MySQL Backup Notification", "Test backup failed for database " + dbName + " on server " + tbHostName.Text + ".\n\n" + rtbOutput.Text });
                }
                // Make the cancel button invisible
                buCancelTest.Visible = false;
                return;
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
                    if (cbSendEmail.Checked && tbSMTPServer.Text != "" && tbEmailAddress.Text != "" && tbFromAddress.Text != "") {
                        string message = utilityFunctions.SendEmail(new string[] { tbSMTPServer.Text, tbSMTPPort.Text, tbSMTPUserName.Text, tbSMTPPassword.Text, tbEmailAddress.Text, tbFromAddress.Text, "MySQL Backup Notification", "Test backup failed for database " + dbName + " on server " + tbHostName.Text + ".\n\n" + rtbOutput.Text });
                    }                   
                    return;
                }              
            }

            if (cbRemoveDumpFile.Checked) {
                // Remove the original .sql file and only store the .zip
                try {
                    File.Delete(@"temp\" + dateStamp + dbName + ".sql");
                }
                catch (IOException ex) {
                    utilityFunctions.displayErrorMessage("Unable to delete the .SQL dump file.", "Error", false);
                }
            }

            // Everything appears to be OK so far. We're going to move the file from our temp directory to the save directory.
            try {
                // Did we create a .zip file?
                if (cbCompress.Checked) {
                    File.Move(@"temp\" + dateStamp + dbName + ".zip", dbSaveDirectory + dateStamp + dbName + ".zip");
                }
                // Did we delete the .sql dump file?
                if (!cbRemoveDumpFile.Checked) {
                    File.Move(@"temp\" + dateStamp + dbName + ".sql", dbSaveDirectory + dateStamp + dbName + ".sql");
                }                
            }
            catch (IOException ex) {
                utilityFunctions.displayErrorMessage(ex.Message, "error", false);
                rtbOutput.Text = rtbOutput.Text + "Unable to move file(s) to save directory.\n";
                toolStripProgressBar1.Visible = false;
                toolStripProgressBar1.Enabled = false;
                if (cbSendEmail.Checked && tbSMTPServer.Text != "" && tbEmailAddress.Text != "" && tbFromAddress.Text != "") {
                    string message = utilityFunctions.SendEmail(new string[] { tbSMTPServer.Text, tbSMTPPort.Text, tbSMTPUserName.Text, tbSMTPPassword.Text, tbEmailAddress.Text, tbFromAddress.Text, "MySQL Backup Notification", "Test backup failed for database " + dbName + " on server " + tbHostName.Text + ".\n\n" + rtbOutput.Text });
                }                
                return;
            }

            rtbOutput.Text = rtbOutput.Text + "Backup of: " + dbName + " Complete.\n";
            rtbOutput.Text = rtbOutput.Text + "Backup saved to: " + dbSaveDirectory + "\n";
            
            rtbOutput.Text = rtbOutput.Text + "Total Backup Time: " + DateTime.Now.Subtract(now).TotalSeconds.ToString("########") + " Seconds.\n";
            
            toolStripProgressBar1.Visible = false;
            toolStripProgressBar1.Enabled = false;            

            if (cbSendEmail.Checked && tbSMTPServer.Text != "" && tbEmailAddress.Text != "" && tbFromAddress.Text != "") {
                string message = utilityFunctions.SendEmail(new string[] { tbSMTPServer.Text, tbSMTPPort.Text, tbSMTPUserName.Text, tbSMTPPassword.Text, tbEmailAddress.Text, tbFromAddress.Text, "MySQL Backup Notification", "Test backup complete for database " + dbName + " on server " + tbHostName.Text + ".\n\n" + rtbOutput.Text });
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //    Database backup thread                                                         //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void backupDataBase(object sender, DoWorkEventArgs e) {
            
            // Parameters
            // 0 = MySQL Host
            // 1 = MySQL User
            // 2 = MySQL Password
            // 3 = MySQL Dump options
            // 4 = Database Name
            // 5 = mysqldump.exe location
            // 6 = filename prepend
            Object[] arg = e.Argument as Object[];
            error = "";                                   
            //create start info...
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.FileName = (string)arg[5];
            //put it all together, notice that were using the --result-file command line. This allows us to not have to use a streamwriter with redirected output.
            startInfo.Arguments = String.Format("-h{0} -u{1} -p{2} {3} --databases {4} --result-file={5}", (string)arg[0], (string)arg[1], (string)arg[2], (string)arg[3], (string)arg[4], @"temp\" + (string)arg[6] + (string)arg[4] + ".sql");
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
            if (error != "")  {
                // Cleanup the empty file 
                try {
                    File.Delete(@"temp\" + (string)arg[6] + (string)arg[4] + ".sql");
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
            // 0 = MySQL Host
            // 1 = MySQL User
            // 2 = MySQL Password
            // 3 = MySQL Dump options
            // 4 = Database Name
            // 5 = mysqldump.exe location
            // 6 = filename prepend
            error = "";
            Object[] arg = e.Argument as Object[];
                       
            try {
                // Now were going to zip the file up.            
                ZipArchive zip = ZipFile.Open(@"temp\" + (string)arg[6] +  (string)arg[4] + ".zip", ZipArchiveMode.Create);
                zip.CreateEntryFromFile(@"temp\" + (string)arg[6] + (string)arg[4] + ".sql", (string)arg[6] + (string)arg[4] + ".sql");
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
            ScheduleGUI scheduleForm = new ScheduleGUI();
            scheduleForm.ShowDialog();
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
    }
}   
