using System;
using System.IO;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Net.Mail;
    
   public class utilityFunctions {       

        /// <summary>
        /// Displays an error message box.
        /// </summary>
        /// <param name="displayCancel">
        /// Should the MessageBox display a Cancel button as well as an OK button? 
        /// </param>
        /// <param name="caption">
        /// The MessageBox caption
        /// </param>
        /// <param name="errorMessage">
        /// Error message to display
        /// </param>
        /// <returns>
        /// Returns bool true if OK is clicked, otherwise it returns false.
        /// </returns> 
        public static bool displayErrorMessage(string errorMessage, string caption, bool displayCancel) {
            if (displayCancel) {
                if (System.Windows.Forms.MessageBox.Show(errorMessage, caption, MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == DialogResult.OK) {
                    return true;
                }
                else {
                    return false;
                }
            }
            else {
                System.Windows.Forms.MessageBox.Show(errorMessage, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);                
            }
            return true;
        }

        /// <summary>
        /// Displays an information message box.
        /// </summary>
        /// <param name="displayCancel">
        /// Should the MessageBox display a Cancel button as well as an OK button? 
        /// </param>
        /// <param name="caption">
        /// The MessageBox caption
        /// </param>
        /// <param name="message">
        /// Message to display
        /// </param>
        /// <returns>
        /// Returns bool true if OK is clicked, otherwise it returns false.
        /// </returns> 
        public static bool displayInformationMessage(string message, string caption, bool displayCancel) {
            if (displayCancel) {
                if (System.Windows.Forms.MessageBox.Show(message, caption, MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK) {
                    return true;
                }
                else {
                    return false;
                }
            }
            else {
                System.Windows.Forms.MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            return true;
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

            foreach (System.Windows.Forms.Control child in thisform.Controls) {
                if (child is System.Windows.Forms.GroupBox) {
                    foreach (System.Windows.Forms.Control tb in child.Controls) {
                        if (tb is System.Windows.Forms.TextBox || tb is System.Windows.Forms.ComboBox || child is MaskedTextBox) {
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
                if (child is System.Windows.Forms.TextBox || child is System.Windows.Forms.ComboBox || child is MaskedTextBox) {
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

    /// <summary>
    /// Scheduler error and result codes
    /// </summary>
    /// <param name="error">
    /// Error code
    /// </param>
    /// <returns>
    /// String type message
    /// </returns>
     
    public static string schedulerErrors(string error)
       {
           string message = "Unknown Result";
           switch (error)
           {
            case "0":
                   message = "Operation completed successfully";
                   break;
            case "267008":
                message = "Task is ready";
                break;
            case "267009":
                message = "Task is running";
                break;
            case "267010":
                message = "Task is disabled";
                break;
            case "267011":
                message = "Task has never run";
                break;
            case "267012":
                message = "Task has no more runs";
                break;
            case "267013":
                message = "Task is not scheduled";
                break ;
            case "267014":
                message = "Task terminated by user";
                break;
            case "267015":
                message = "Task has no valid triggers";
                break;
            case "267016":
                message = "No trigger run times";
                break;
            case "2147750665":
                message = "Trigger not found";
                break;
            case "2147750666":
                message = "Task not ready";
                break;
            case "2147750667":
                message = "Task not running";
                break;
            case "2147750668":
                message = "Scheduler service not installed";
                break;
            case "2147750669":
                message = "Cannot open task";
                break;
            case "2147750670":
                message = "Invalid task";
                break;
            case "2147750671":
                message = "Account information not set";
                break;
            case "2147750672":
                message = "Account name not found";
                break;
            case "2147750673":
                message = "Account database corrupted";
                break;
            case "2147750674":
                message = "No security services available";
                break;
            case "2147750675":
                message = "Unknown object version";
                break;
            case "2147750676":
                message = "Unsupported account options";
                break;
            case "2147750677":
                message = "Scheduler service not running";
                break;
            case "2147750678":
                message = "Malformed task XML";
                break;
            case "2147750679":
                message = "Task XML unexpected namespace";
                break;
            case "2147750680":
                message = "Invalid value in task XML";
                break;
            case "2147750681":
                message = "Missing node in task XML";
                break;
            case "2147750682":
                message = "Malformed task XML";
                break;
            case "267035":
                message = "Some triggers failed";
                break;
            case "267036":
                message = "Batch logon problems.";
                break;
            case "2147750685":
                message = "Task XML contains too many nodes";
                break;
            case "2147750686":
                message = "Schedule past end boundary";
                break;
            case "2147750687":
                message = "Task already running";
                break;
            case "2147750688":
                message = "User not logged in";
                break;
            case "2147750689":
                message = "Task image is corrupt";
                break;
            case "2147750690":
                message = "Scheduler service not available";
                break;
            case "2147750691":
                message = "Scheduler service too busy";
                break;
            case "2147750692":
                message = "Task was attempted but failed";
                break;
            case "267045":
                message = "Task has been queued";
                break;
            case "2147750694":
                message = "Task is disabled";
                break;
            case "2147750695":
                message = "Task not V1 compatible";
                break;
            case "2147750696":
                message = "Task cannot start on demand";
                break;
        }
        return message;
    }

   }