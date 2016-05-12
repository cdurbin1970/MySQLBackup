using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MySQL_Backup {
    public partial class ScheduleGUI : Form {
        public ScheduleGUI() {
            InitializeComponent();
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //   Find the config file location                                                   //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void buConfigLocation_Click(object sender, EventArgs e) {
            openFileDialog1.Filter = "Config Files (*.conf)|*.conf";
            openFileDialog1.InitialDirectory = @Directory.GetCurrentDirectory() + @"\configs";
            if (openFileDialog1.ShowDialog() == DialogResult.OK) {
                tbConfigFile.Text = openFileDialog1.FileName;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //   Add the item to the schedule                                                    //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void buAdd_Click(object sender, EventArgs e) {
            string type = "";
            string daystorun = "";
            // Go through each control and figure out which type they selected
            foreach (var control in groupBox2.Controls) {
                RadioButton radio = control as RadioButton;
                if (radio != null && radio.Checked) {
                    switch (radio.Text) { 
                        case "Run Once":
                            type = "Run Once";
                           break;
                        case "Daily" :
                           type = "Daily";
                           break;
                        case "Monthly":
                           type = "Monthly";
                           break;
                        case "Regular Intervals":
                           type = "Regular Intervals";
                           break;
                        case "Weekly" :
                           type = "Weekly";
                           break;    
                    }
                }
            }
            // Go through each control and figure out which days they selected
            foreach (var control in gbDays.Controls) {
                if (((CheckBox)control).Enabled) {
                    if (((CheckBox)control).Checked) {
                        daystorun = daystorun + "1";
                    }
                    else {
                        daystorun = daystorun + "0";
                    }
                }
            }
            string[] row = new string[] { tbScheduleName.Text, type, tbConfigFile.Text, dtpScheduler.Text,"NA", daystorun, dtpStart.Text, dtpEnd.Text};
            dgvSchedule.Rows.Add(row);           
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //   Remove the selected item from the schedule                                      //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////
        
        private void buRemove_Click(object sender, EventArgs e) {
            dgvSchedule.Rows.RemoveAt(dgvSchedule.CurrentRow.Index);
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //    A checkbox was changed                                                         //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void _CheckedChanged(object sender, EventArgs e) {
            if (sender == rbRegularIntervals || sender == rbDaily) {
                foreach (var control in gbDays.Controls) {
                    if (((CheckBox)control).Enabled) {
                        ((CheckBox)control).Enabled = false;
                    }
                    else {
                        ((CheckBox)control).Enabled = true;
                    } 
                }
                
            }
            if (sender == rbRegularIntervals) {
                dtpStart.Enabled = true;
                dtpEnd.Enabled = true;
            }
            else {
                dtpStart.Enabled = false;
                dtpEnd.Enabled = false;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //    Save the schedule                                                              //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void buSave_Click(object sender, EventArgs e) {

            dgvSchedule.CancelEdit();
            // Is there anything to save?
            if (dgvSchedule.NewRowIndex == 0) {
                utilityFunctions.displayErrorMessage("No schedule records to save.", "No Records", false);
                return;
            }

            List<string> rows = new List<string>();
            
            foreach (DataGridViewRow dgvr in dgvSchedule.Rows) {
                if (dgvr.Cells[0].Value != null) {
                    // Get the underlying datarow
                  rows.Add(dgvr.Cells[0].Value.ToString() +"," + "Test");
                }
            }
            try {
                File.WriteAllLines(Application.StartupPath + @"\schedule.conf", rows);
            }
            catch (Exception) {
                utilityFunctions.displayErrorMessage("Unable to save schedule config file.", "File Error", false);
                return;
            }

           /* // Does a schedule file exist? If not, create it.
            if (!File.Exists(Application.StartupPath + @"\schedule.conf")) {
                try {
                    File.Open(Application.StartupPath + @"\schedule.conf", FileMode.Create);
                }
                catch (Exception) {
                    utilityFunctions.displayErrorMessage("Unable to save schedule config file.", "File Error", false);
                    return;
                }               
            }
            // Open it up in truncate mode
            try {
                File.Open(Application.StartupPath + @"\schedule.conf", FileMode.Truncate);
            }
            catch (Exception) {
                utilityFunctions.displayErrorMessage("Unable to save schedule config file.", "File Error", false);
                return;
            }
            */
            
            
            
            
            
            
            // Write all the lines out, then close the file.
            try {
                //File.WriteAllLines("schedule.conf", rows);               
                utilityFunctions.displayInformationMessage("Schedule saved successfully. Stop and start the scheduler to read in any changes.", "Schedule Saved", false);
            }
            catch {
                utilityFunctions.displayErrorMessage("Error saving config file.", "Error Saving Config", false);
            }

        }

      






    }
}
