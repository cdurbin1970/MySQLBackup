using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using MySql.Data.Common;
using iniReader;
using Encryption_Lib;


namespace MySQL_Backup {
    public partial class frMain : Form {

        // Universal key used for encryption and decryption functions
        // Key must be 256bit or 32bytes
        // If key is changed after a config file is saved, the passwords will not be decrypted properly.
        string encryptionKey = "LjjS5PDETyN98JZDgGtdPlhbk7-fBCVN";

        string error = "";
             
        public frMain() {
            InitializeComponent();
            // Update our text controls on the form.
                    
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
            toolTip1.SetToolTip(this.cbDBDirs, "Store the backups in individual directories named after the database.");
            
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
                    MySqlDataReader readLookupData;
                    // Create our select command to get the records
                    lookupSelectCmd.CommandText = "SHOW DATABASES;";
                    // Set the lookup SELECT command connection
                    lookupSelectCmd.Connection = MySQLConnect;
                    readLookupData = lookupSelectCmd.ExecuteReader();
                    clbDatabases.Items.Clear();
                    while (readLookupData.Read()) {
                        clbDatabases.Items.Add(readLookupData.GetValue(readLookupData.GetOrdinal("Database")).ToString());
                    }
                   utilityFunctions.DBClose(MySQLConnect);
                }
            }
            else {               
                if (tbPort.Text == "") {
                    errorProvider1.SetError(tbPort, "Please enter the connection port.");
                    tbPort.BackColor = Color.LightPink;
                    tbPort.Focus();
                }
                if (tbPassword.Text == "") {
                    errorProvider1.SetError(tbPassword, "Please enter the user password.");
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
                MessageBox.Show("Please enter the  information before trying to connect!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //    Open dialog to find MySQL dump exe                                             //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void buDumpLocation_Click(object sender, EventArgs e) {
            openFileDialog1.Filter = "MySQL Dump (.exe)|*.exe";
            DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) {
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
                if (!utilityFunctions.displayErrorMessage("MySQLDump.exe location is empty.\nThis program depends on MySQLDump.exe to perform the backup.\nContinuing with the save will result in a config file that does not work.","Error")) {
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
                saveFileDialog1.InitialDirectory = @Directory.GetCurrentDirectory() + "\\configs";
                saveFileDialog1.FileName = tbHostName.Text + ".conf";            
                if (saveFileDialog1.ShowDialog() == DialogResult.OK) {
                    fileNameToSave = saveFileDialog1.FileName;
                }
                else {
                    return;
                }
            }  
            // iterate through all the databases listed and decide which list to put them on.
            string checkedDatabases = "";
            string uncheckedDatabases = "";
            for (int x = 0; x < clbDatabases.Items.Count; x++) {
                if (clbDatabases.GetItemChecked(x)) {
                    checkedDatabases = checkedDatabases + (string)clbDatabases.Items[x];
                    if (x != clbDatabases.Items.Count) {
                        checkedDatabases = checkedDatabases + ",";
                    }
                }
                else {
                    uncheckedDatabases = uncheckedDatabases + (string)clbDatabases.Items[x];
                    if (x != clbDatabases.Items.Count) {
                        uncheckedDatabases = uncheckedDatabases + ",";
                    }
                }
            }
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
            
            string[] lines = { "[General]", "Server = " + tbHostName.Text, "Port = " + tbPort.Text, "Username = " + tbUserName.Text, "SQLPassword = " + SQLPassword, "MySQLDump = " + tbDumpLocation.Text,"MySQLDumpOptions = " + tbMySQLDumpOptions.Text, "SaveLocation = " + tbSaveLocation.Text 
                                ,"DaysToKeep = " + tbDaystoKeep.Text, "CompressBackup = " + cbCompress.CheckState,"RemoveDumpFile = " + cbRemoveDumpFile.CheckState,"DBDirs = " + cbDBDirs.CheckState,"SendEMail = " + cbSendEmail.CheckState, "SMTPServer = " + tbSMTPServer.Text, "SMTPPort = " + tbSMTPPort.Text, "SMTPUsername = " + tbSMTPUserName.Text, "SMTPPassword = " + SMTPPassword, "EmailAddress = " + tbEmailAddress.Text,"FromAddress = " + tbFromAddress.Text, "[Databases]", "CheckedDatabases = " + checkedDatabases.TrimEnd(','), "unCheckedDatabases = " + uncheckedDatabases.TrimEnd(',')};
            // Write all the lines out, then close the file.
            try {
                File.WriteAllLines(fileNameToSave, lines);
                tsslCurrentConfig.Text = fileNameToSave;
                saveToolStripMenuItem.Enabled = true;
                MessageBox.Show("Config file saved successfully.", "Config Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch {
                utilityFunctions.displayErrorMessage("Error saving config file.", "Error Saving Config");
            }
       }   

        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //    Open a config file                                                             //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void openToolStripMenuItem_Click(object sender, EventArgs e) {
            openFileDialog1.Filter = "Config Files (*.conf)|*.conf";
            openFileDialog1.InitialDirectory = @Directory.GetCurrentDirectory() + "\\configs";
            if (openFileDialog1.ShowDialog() == DialogResult.OK) {                
                //clear everything out to make sure we're putting in correct items.                
                clearConfigItems(clearAllItemsToolStripMenuItem, e);
                //Load our config file in and assign the correct places.                
                Configuration objinifile = utilityFunctions.ConfigOpen(openFileDialog1.FileName);
                if (objinifile != null) {
                    tbHostName.Text = objinifile.GetValue("General", "Server");
                    tbUserName.Text = objinifile.GetValue("General", "Username");
                    if (objinifile.GetValue("General", "SQLPassword").ToString() != "") {
                        tbPassword.Text = AESGCM.SimpleDecrypt(objinifile.GetValue("General", "SQLPassword").ToString(), Encoding.UTF8.GetBytes(encryptionKey));
                    }
                    else {
                        tbPassword.Text = "";
                    }                    
                    tbPort.Text = objinifile.GetValue("General", "Port");
                    tbDumpLocation.Text = objinifile.GetValue("General", "MySQLDump");
                    tbMySQLDumpOptions.Text = objinifile.GetValue("General", "MySQLDumpOptions");
                    tbSaveLocation.Text = objinifile.GetValue("General", "SaveLocation");
                    tbDaystoKeep.Text = objinifile.GetValue("General", "DaysToKeep");
                    if (objinifile.GetValue("General", "CompressBackup") == "Checked") {
                        cbCompress.Checked = true;
                    }
                    else {
                        cbCompress.Checked = false;
                    }
                    if (objinifile.GetValue("General", "RemoveDumpFile") == "Checked") {
                        cbRemoveDumpFile.Checked = true;
                    }
                    else {
                        cbRemoveDumpFile.Checked = false;
                    }
                    if (objinifile.GetValue("General", "DBDirs") == "Checked") {
                        
                        cbDBDirs.Checked = true;
                    }
                    else {
                        cbDBDirs.Checked = false;
                    }
                    if (objinifile.GetValue("General", "SendEMail") == "Checked") {

                       cbSendEmail.Checked = true;
                    }
                    else {
                        cbSendEmail.Checked = false;
                    }

                    tbSMTPServer.Text = objinifile.GetValue("General", "SMTPServer");
                    tbSMTPPort.Text = objinifile.GetValue("General", "SMTPPort");
                    tbSMTPUserName.Text = objinifile.GetValue("General", "SMTPUsername");
                    if (objinifile.GetValue("General", "SMTPPassword").ToString() != "") {
                        tbSMTPPassword.Text = AESGCM.SimpleDecrypt(objinifile.GetValue("General", "SMTPPassword").ToString(), Encoding.UTF8.GetBytes(encryptionKey));
                    }
                    else {
                        tbSMTPPassword.Text = "";
                    }
                    tbEmailAddress.Text = objinifile.GetValue("General", "EmailAddress");
                    tbFromAddress.Text = objinifile.GetValue("General", "FromAddress");

                    if (objinifile.GetValue("Databases", "CheckedDatabases").ToString() != "") {
                        // get the list of checked DBs
                        string[] checkedDBList = objinifile.GetValue("Databases", "CheckedDatabases").ToString().Split(',');
                        // iterate through each one and add it to the list checked.
                        for (int x = 0; x < checkedDBList.Length; x++) {
                            clbDatabases.Items.Add(checkedDBList[x], true);
                        }
                    }
                    if (objinifile.GetValue("Databases", "unCheckedDatabases").ToString() != "") {
                        // get the list of unchecked DBs
                        string[] unCheckedDBList = objinifile.GetValue("Databases", "unCheckedDatabases").ToString().Split(',');
                        // iterate through each one and add it to the list unchecked.
                        for (int x = 0; x < unCheckedDBList.Length; x++) {
                            clbDatabases.Items.Add(unCheckedDBList[x], false);
                        }
                    }
                    saveToolStripMenuItem.Enabled = true;
                    tsslCurrentConfig.Text = openFileDialog1.FileName;
                }
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

        private void checkBox1_CheckedChanged(object sender, EventArgs e) {

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
                    MessageBox.Show("Email sent successfully.", "Email Sent", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else {
                    utilityFunctions.displayErrorMessage(message, "Error");
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
                MessageBox.Show("Please enter the information before trying to send a test email!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            // I chose to create the dump file, create a zip file, move the dump file into it and then delete the dump file.

            string dbName = "";
            string dbSaveDirectory = "";
            DateTime now = DateTime.Now;

            rtbOutput.Text = "";

            //Get the first database name from the list that has been checked. We'll use that.
            for (int x = 0; x < clbDatabases.Items.Count; x++) {
                if (clbDatabases.GetItemChecked(x)) {
                    dbName = (string)clbDatabases.Items[x];
                    break;
                }
            }

            if (dbName == "") {
                utilityFunctions.displayErrorMessage("Please select one database from the list before trying to test.", "Database Selection Error");
                return;
            }

            // Check to see if we should be using individual dirs for each database
            if (cbDBDirs.Checked) {
                dbSaveDirectory = tbSaveLocation.Text + "/" + dbName + "/";
            }
            else {
                dbSaveDirectory = tbSaveLocation.Text + "/";
            }

            //put it all together, notice that were using the --result-file command line. This allows us to not have to use a streamwriter with redirected output.
            //string cmd = String.Format("-h{0} -u{1} -p{2} --opt --databases {3} --result-file={4}", tbHostName.Text, tbUserName.Text, tbPassword.Text, dbName, tbSaveLocation.Text + "/" + dbName + "/" + now.ToString("yyyy_MM_dd_HH_mm_ss_") + dbName + ".sql");
            // Check to see if a folder location exists. Which is usually backuplocation/dbname/date_dbname.sql
            if (!Directory.Exists(dbSaveDirectory)) {
                try {
                    Directory.CreateDirectory(dbSaveDirectory);
                }
                catch (Exception ex) {
                    rtbOutput.Text = "Could not create directory, aborting backup.";
                    return;
                }
            }

            // Create a string array to hold all of our parameters in.
            string[] parameters = { tbHostName.Text, tbUserName.Text, tbPassword.Text, tbMySQLDumpOptions.Text, dbName, dbSaveDirectory, tbDumpLocation.Text, now.ToString("yyyy_MM_dd_HH_mm_ss_") };

            //Create a thread to do the actual backup in.           
            Thread backupThread = new Thread(() => backupDataBase(parameters));
            backupThread.IsBackground = true;
            backupThread.Name = "backupThread";
            backupThread.Start();

            // Start the control to show it is doing something
            toolStripProgressBar1.Visible = true;
            toolStripProgressBar1.Enabled = true;
            
            // Wait while the thread completes, doevents so the form is not frozen.
            while (backupThread.IsAlive) {
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
                return;
            }

            rtbOutput.Text = "Dump of " + dbName + " Complete.\n";
         
            if (cbCompress.Checked) {
                error = "";
                //Create a thread to do the actual backup in.           
                Thread zipThread = new Thread(() => zipDataBase(parameters));
                zipThread.IsBackground = true;
                zipThread.Name = "zipThread";
                zipThread.Start();

                // Wait while the thread completes, doevents so the form is not frozen.
                while (zipThread.IsAlive) {
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
                    File.Delete(dbSaveDirectory + now.ToString("yyyy_MM_dd_HH_mm_ss_") + dbName + ".sql");
                }
                catch (IOException) {

                }
            }

            rtbOutput.Text = rtbOutput.Text + "Backup of " + dbName + " Complete.\n";
            rtbOutput.Text = rtbOutput.Text + "Backup saved to " + dbSaveDirectory + "\n";
            
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

        private void backupDataBase(string[] parameters) {
            
            error = "";                                   
            //create start info...
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.FileName = parameters[6];
            //put it all together, notice that were using the --result-file command line. This allows us to not have to use a streamwriter with redirected output.
            startInfo.Arguments = String.Format("-h{0} -u{1} -p{2} {3} --databases {4} --result-file={5}", parameters[0], parameters[1], parameters[2], parameters[3], parameters[4], parameters[5] + parameters[7] + parameters[4] + ".sql");
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
            proc.Start();
            error = proc.StandardError.ReadToEnd();
            //wait for the process to finish...
            proc.WaitForExit();
            //close the process...
            proc.Close();
            if (error != "") {
                // Cleanup the empty file 
                try {
                    File.Delete(parameters[5] + parameters[7] + parameters[4] + ".sql");
                }
                catch (IOException ex) {
                    error = ex.Message;
                }
                return; 
            }
            //MySQLDump.
            error =  "OK";            
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //    Database zip thread                                                            //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void zipDataBase(string[] parameters) {
            try {
                // Now were going to zip the file up.            
                ZipArchive zip = ZipFile.Open(parameters[5] + parameters[7] + parameters[4] + ".zip", ZipArchiveMode.Create);
                zip.CreateEntryFromFile(parameters[5] + parameters[7] + parameters[4] + ".sql", parameters[7] + parameters[4] + ".sql");
                zip.Dispose();
            }
            catch (IOException ex) {
                error = "Zip process failed: " + ex.Message + "\n"; ;
                return;
            }
            error = "OK";
        }
        
        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //    Check Boxes not correct                                                        //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void cbRemoveDumpFile_CheckedChanged(object sender, EventArgs e) {
            if (cbRemoveDumpFile.Checked && !cbCompress.Checked) {
                if (utilityFunctions.displayErrorMessage("Checking this and not checking the Compress Backup box will result in your backup file being deleted!\nThis is designed to let you keep both the original dump file and the .zip file.", "File Deletion Warning") == false){
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
        
 




    }
}   
