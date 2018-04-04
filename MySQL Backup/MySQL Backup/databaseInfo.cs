using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace MySQL_Backup {
    public partial class DatabaseInfo : Form {
        public DatabaseInfo() {
            InitializeComponent();
        }

        public void ShowDialog(string databaseName, string hostName,string userName,string passWord, string port) {
            label2.Text = databaseName;
            var mySqlConnect = UtilityFunctions.DbConnect("server=" + hostName + ";user id=" + userName + ";port=" + port + ";database=mysql;pooling=false;allow user variables=true;password=" + passWord);
            if (mySqlConnect.Ping()) {
                // Create our Lookup SELECT command
                var lookupSelectCmd = new MySqlCommand();
                // Create our Lookup command reader
                MySqlDataReader readLookupData;
                // Create our select command to get the records
                lookupSelectCmd.CommandText = "SELECT * FROM information_schema.TABLES where TABLE_SCHEMA='" + databaseName + "';";
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
                dgvTableInfo.Rows.Clear();
                while (readLookupData.Read()) {
                    dgvTableInfo.Rows.Add(readLookupData.GetValue(readLookupData.GetOrdinal("TABLE_NAME")).ToString(),
                        readLookupData.GetValue(readLookupData.GetOrdinal("TABLE_TYPE")).ToString(),
                        readLookupData.GetValue(readLookupData.GetOrdinal("ENGINE")).ToString(),
                        readLookupData.GetValue(readLookupData.GetOrdinal("VERSION")).ToString(),
                        readLookupData.GetValue(readLookupData.GetOrdinal("ROW_FORMAT")).ToString(),
                        readLookupData.GetValue(readLookupData.GetOrdinal("TABLE_ROWS")).ToString(),
                        readLookupData.GetValue(readLookupData.GetOrdinal("AVG_ROW_LENGTH")).ToString(),
                        readLookupData.GetValue(readLookupData.GetOrdinal("DATA_LENGTH")).ToString(),
                        readLookupData.GetValue(readLookupData.GetOrdinal("MAX_DATA_LENGTH")).ToString(),
                        readLookupData.GetValue(readLookupData.GetOrdinal("INDEX_LENGTH")).ToString(),
                        readLookupData.GetValue(readLookupData.GetOrdinal("DATA_FREE")).ToString(),
                        readLookupData.GetValue(readLookupData.GetOrdinal("AUTO_INCREMENT")).ToString(),
                        readLookupData.GetValue(readLookupData.GetOrdinal("CREATE_TIME")).ToString(),
                        readLookupData.GetValue(readLookupData.GetOrdinal("UPDATE_TIME")).ToString(),
                        readLookupData.GetValue(readLookupData.GetOrdinal("CHECK_TIME")).ToString(),
                        readLookupData.GetValue(readLookupData.GetOrdinal("TABLE_COLLATION")).ToString());                    
                }
                if (!UtilityFunctions.DbClose(mySqlConnect)) {
                    UtilityFunctions.DisplayMessage("error", "Could not close database connection.", "Error", false);
                }
            }            
            ShowDialog();
        }
    }
}
