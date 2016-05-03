using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using iniReader;
using MySql.Data.MySqlClient;

using System.Windows;
using System.Windows.Controls;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading;
using System.ComponentModel;

using Encryption_Lib;
    
   public class utilityFunctions {       

        /// <summary>
        /// Displays an error message box.
        /// </summary>
        /// <returns>
        /// Returns bool true if OK is clicked, otherwise it returns false.
        /// </returns> 
        public static bool displayErrorMessage(string errorMessage, string caption) {
            if (System.Windows.Forms.MessageBox.Show(errorMessage, caption, MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == DialogResult.OK) {
                return true;
            }
            else {
                return false;
            }
        
        }
       
       /// <summary>
       /// Sends an email
       /// </summary>
       /// <returns>
       /// Returns string of Sent email. or the exception message if there is a failure.
       /// </returns>
       
        public static string SendEmail(string[] parameters) {
            // Create SMTP client
            SmtpClient client = new SmtpClient(parameters[0]);
            if (parameters[1] != "") {
                client.Port = Convert.ToInt16(parameters[1]);
            }           
            // Specify the message content.
            MailMessage message = new MailMessage(new MailAddress(parameters[5]), new MailAddress(parameters[4]));
            message.Body = parameters[7];
            message.Subject = parameters[6];
            if (parameters[2] != "" && parameters[3] != "") {
                client.Credentials = new System.Net.NetworkCredential(parameters[2],parameters[3]);
            }           
            // Try to send the message
            try {
                client.Send(message);
            }
            catch (SmtpException ex){
                message.Dispose();
                return ex.Message;
            }            
            // Clean up.
            message.Dispose();
            return "OK";
        }      
       
       /// <summary>
       /// Validates an email address
       /// </summary>
       /// <returns>
       /// Returns a boolean
       /// </returns>      
       
       public static bool ValidEmailAddress(string txtEmailID, out string errorMsg) {
           // Confirm that the e-mail address string is not empty. 
           if (txtEmailID.Length == 0) {
               errorMsg = "e-mail address is required to send email.";
               return true;
           }

           // Confirm that there is an "@" and a "." in the e-mail address, and in the correct order.
           if (txtEmailID.IndexOf("@") > -1) {
               if (txtEmailID.IndexOf(".", txtEmailID.IndexOf("@")) > txtEmailID.IndexOf("@")) {
                   errorMsg = "";
                   return true;
               }
           }
           errorMsg = "e-mail address must be valid e-mail address format.\n" +
              "For example 'someone@example.com' ";
           return false;
       }       

       /// <summary>
       /// Creates a connection to the MySQL database
       /// </summary>
       /// <returns>
       /// Returns a MySqlConnection object
       /// </returns>       
       
       public static MySqlConnection DBConnect(string connectionString) {         
                  
            // MySQL Connections
            MySqlConnection connection = null;
            
            // Connect to the databases.
            try {
                // Try to connect to the MySQL server
                connection = new MySqlConnection(connectionString);
                connection.Open();
            }
            catch (MySqlException ex) {
                // Catch any exceptions.
                System.Windows.Forms.MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);                
            }            
            return connection;
        }

        /// <summary>
        /// Closes a connection to the database server
        /// </summary>
        /// <param name="connectionObject">
        /// The database conenction to be closed
        /// </param>
        /// <returns>
        /// A boolean if the database conenction was closed or not.
        /// </returns>
        
       public static bool DBClose(MySqlConnection connectionObject) {
            try {
                connectionObject.Close();
                return true;
            }
            catch (MySqlException ex) {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Open the configuration file and reads it in to a Configuration object
        /// </summary>
        /// <returns>
        /// Returns a configuration object
        /// </returns>
        
        public static Configuration ConfigOpen( string configLocation) {
            StreamReader inireader = null;
            Configuration objinifile = null;

            // Get the path for our exe file so that it reads in the ini file no matter where we put it.
            try {
                inireader = new StreamReader(configLocation);
                objinifile = new Configuration(inireader);
                inireader.Close();
            }
            catch (Exception) {
                System.Windows.Forms.MessageBox.Show("Unable to read configuration file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            return objinifile;            
        }

        /// <summary>
        /// Updates specific text controls on a form to have a background color when they have focus.
        /// </summary>
        /// <param name="thisform">
        /// The form object that needs to be checked
        /// </param>
        /// <returns>
        /// none
        /// </returns>       

        public static void textupdate(Form thisform) {

            var lastColorSaved = System.Drawing.Color.Empty;


            foreach (System.Windows.Forms.Control child in thisform.Controls)
            {
                if (child is System.Windows.Forms.GroupBox)
                {
                    foreach (System.Windows.Forms.Control tb in child.Controls)
                    {
                        if (tb is System.Windows.Forms.TextBox || tb is System.Windows.Forms.ComboBox || child is MaskedTextBox)
                        {
                            tb.Enter += (s, e) =>
                            {
                                var control = (System.Windows.Forms.Control)s;
                                lastColorSaved = control.BackColor;
                                control.BackColor = System.Drawing.Color.LightYellow;
                            };
                            tb.Leave += (s, e) =>
                            {
                                ((System.Windows.Forms.Control)s).BackColor = lastColorSaved;
                            };
                        }
                    }
                }
                if (child is System.Windows.Forms.TextBox || child is System.Windows.Forms.ComboBox || child is MaskedTextBox)
                {
                    child.Enter += (s, e) =>
                    {
                        var control = (System.Windows.Forms.Control)s;
                        lastColorSaved = control.BackColor;
                        control.BackColor = System.Drawing.Color.LightYellow;
                    };
                    child.Leave += (s, e) =>
                    {
                        ((System.Windows.Forms.Control)s).BackColor = lastColorSaved;
                    };
                }
            }
        }

     /*  public static IEnumerable<T> FindVisualChildren<T>(DependencyObject obj) where T : DependencyObject {
            if (obj != null) {
                if (obj is T)
                    yield return obj as T;

                foreach (DependencyObject child in LogicalTreeHelper.GetChildren(obj).OfType<DependencyObject>())
                    foreach (T c in FindVisualChildren<T>(child))
                        yield return c;
            }
        }*/

        
   
   
   }