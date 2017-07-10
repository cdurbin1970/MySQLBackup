using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32.TaskScheduler;

namespace MySQL_Backup {
    public partial class ScheduleGUI : Form {
        public ScheduleGUI() {
            InitializeComponent();
            // Update our text controls on the form.
            utilityFunctions.textupdate(this);
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //   Find the config file location                                                   //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void buConfigLocation_Click(object sender, EventArgs e) {
            openFileDialog1.Filter = "Config Files (*.xml)|*.xml";
            openFileDialog1.InitialDirectory = @Directory.GetCurrentDirectory() + @"\configs";
            openFileDialog1.FileName = "";
            if (openFileDialog1.ShowDialog() == DialogResult.OK) {
                tbConfigFile.Text = openFileDialog1.SafeFileName;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //   Remove the selected task                                                        //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void buRemove_Click(object sender, EventArgs e) {

            TaskService ts = new TaskService();
            // Get the selected task name
            string taskName = "MySQL Backup\\" + dgvSchedule.Rows[dgvSchedule.CurrentCell.RowIndex].Cells[0].Value;

            if (MessageBox.Show("Ok to remove " + taskName, "Remove Task", MessageBoxButtons.OKCancel,MessageBoxIcon.Question) == DialogResult.OK) {
                // Find the task name
                Microsoft.Win32.TaskScheduler.Task t = ts.GetTask(taskName);
                if (t == null) return;
                // Remove the task 
                ts.RootFolder.DeleteTask(taskName);   
            }
            LoadTasks();

        }

        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //    Save the task                                                                  //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void buSave_Click(object sender, EventArgs e) {

            if (tbScheduleName.Text != "" && tbConfigFile.Text != "") {
                try {
                    using (TaskService ts = new TaskService()) {
                        string taskName = "MySQL Backup\\" + tbScheduleName.Text;
                        // Create a new task
                        Task t = ts.AddTask(taskName,
                            new TimeTrigger()
                            {
                                StartBoundary = DateTime.Now + TimeSpan.FromHours(1),
                                Enabled = false
                            },
                            new ExecAction("mysqlbackupcommand.exe", tbConfigFile.Text, @Directory.GetCurrentDirectory()));
                        
                        // Edit task and re-register if user clicks Ok
                        TaskEditDialog editorForm = new TaskEditDialog(t, true, true);

                        if (editorForm.ShowDialog() == DialogResult.Cancel) {
                            ts.RootFolder.DeleteTask(taskName);
                        }
                    }
                }
                catch (Exception ex) {
                    utilityFunctions.displayErrorMessage(ex.Message, "Error", false);
                }
            }
            else {
                utilityFunctions.displayErrorMessage("Please supply a name and a config file before saving.","Save Error", false);
            }
            LoadTasks();
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //    Edit a task                                                                    //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void dgvSchedule_CellDoubleClick(object sender, DataGridViewCellEventArgs e) {

            try {
                using (TaskService ts = new TaskService()) {
                    string taskName = "MySQL Backup\\" + dgvSchedule.Rows[e.RowIndex].Cells["clJobName"].FormattedValue.ToString();
                    TaskFolder tf = ts.RootFolder;
                    TaskDefinition td = ts.GetTask(taskName).Definition;
                    Task t = ts.GetTask(taskName);
                    // Edit task and re-register if user clicks Ok
                    TaskEditDialog editorForm = new TaskEditDialog(t);
                    if (editorForm.ShowDialog() == DialogResult.OK) {
                        tf.RegisterTaskDefinition(taskName, td, TaskCreation.CreateOrUpdate, null, null,TaskLogonType.InteractiveToken, null);
                    }
                }
            }
            catch (Exception ex) {
                utilityFunctions.displayErrorMessage(ex.Message, "Error", false);
            }
            LoadTasks();
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //    Load the schedule form                                                         //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void ScheduleGUI_Load(object sender, EventArgs e) {
          LoadTasks(); 

        } //End of GUI load

        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //    Load tasks into the datagrid                                                   //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////
        /// 
        private void LoadTasks() {
            // Clear old tasks and load all in the folder
            dgvSchedule.Rows.Clear();
            dgvSchedule.Refresh();
            using (TaskService ts = new TaskService()) {
                TaskFolder folder = ts.GetFolder("MySQL Backup");
                TaskDefinition td = null;
               try {
                    foreach (Microsoft.Win32.TaskScheduler.Task t in folder.GetTasks()) {
                        td = t.Definition;
                        dgvSchedule.Rows.Add(t.Name, t.State, td.Triggers[0].TriggerType,td.Actions[0],  t.LastRunTime, t.NextRunTime, utilityFunctions.schedulerErrors(t.LastTaskResult.ToString()));
                    }
                }
                catch (Exception ex) {

                }
            }
        }
    }
}