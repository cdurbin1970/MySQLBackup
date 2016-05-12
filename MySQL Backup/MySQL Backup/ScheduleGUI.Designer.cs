namespace MySQL_Backup
{
    partial class ScheduleGUI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScheduleGUI));
            this.dgvSchedule = new System.Windows.Forms.DataGridView();
            this.gbScheduleDef = new System.Windows.Forms.GroupBox();
            this.gbRunTimes = new System.Windows.Forms.GroupBox();
            this.dtpEnd = new System.Windows.Forms.DateTimePicker();
            this.label5 = new System.Windows.Forms.Label();
            this.dtpStart = new System.Windows.Forms.DateTimePicker();
            this.gbDays = new System.Windows.Forms.GroupBox();
            this.cbSaturday = new System.Windows.Forms.CheckBox();
            this.cbSunday = new System.Windows.Forms.CheckBox();
            this.cbFriday = new System.Windows.Forms.CheckBox();
            this.cbMonday = new System.Windows.Forms.CheckBox();
            this.cbThursday = new System.Windows.Forms.CheckBox();
            this.cbTuesday = new System.Windows.Forms.CheckBox();
            this.cbWednesday = new System.Windows.Forms.CheckBox();
            this.buSave = new System.Windows.Forms.Button();
            this.buRemove = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbMonthly = new System.Windows.Forms.RadioButton();
            this.rbWeekly = new System.Windows.Forms.RadioButton();
            this.rbDaily = new System.Windows.Forms.RadioButton();
            this.rbRegularIntervals = new System.Windows.Forms.RadioButton();
            this.rbOnce = new System.Windows.Forms.RadioButton();
            this.buAdd = new System.Windows.Forms.Button();
            this.buConfigLocation = new System.Windows.Forms.Button();
            this.tbConfigFile = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.dtpScheduler = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.tbScheduleName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.clJobName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clConfigFile = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clNextRun = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clLastRunResult = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clDaysToRun = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clTimes = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSchedule)).BeginInit();
            this.gbScheduleDef.SuspendLayout();
            this.gbRunTimes.SuspendLayout();
            this.gbDays.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvSchedule
            // 
            this.dgvSchedule.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSchedule.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clJobName,
            this.clType,
            this.clConfigFile,
            this.clNextRun,
            this.clLastRunResult,
            this.clDaysToRun,
            this.clTimes});
            this.dgvSchedule.Location = new System.Drawing.Point(2, 1);
            this.dgvSchedule.Name = "dgvSchedule";
            this.dgvSchedule.ReadOnly = true;
            this.dgvSchedule.Size = new System.Drawing.Size(847, 299);
            this.dgvSchedule.TabIndex = 1;
            // 
            // gbScheduleDef
            // 
            this.gbScheduleDef.Controls.Add(this.gbRunTimes);
            this.gbScheduleDef.Controls.Add(this.gbDays);
            this.gbScheduleDef.Controls.Add(this.buSave);
            this.gbScheduleDef.Controls.Add(this.buRemove);
            this.gbScheduleDef.Controls.Add(this.label4);
            this.gbScheduleDef.Controls.Add(this.groupBox2);
            this.gbScheduleDef.Controls.Add(this.buAdd);
            this.gbScheduleDef.Controls.Add(this.buConfigLocation);
            this.gbScheduleDef.Controls.Add(this.tbConfigFile);
            this.gbScheduleDef.Controls.Add(this.label3);
            this.gbScheduleDef.Controls.Add(this.dtpScheduler);
            this.gbScheduleDef.Controls.Add(this.label2);
            this.gbScheduleDef.Controls.Add(this.tbScheduleName);
            this.gbScheduleDef.Controls.Add(this.label1);
            this.gbScheduleDef.Location = new System.Drawing.Point(2, 306);
            this.gbScheduleDef.Name = "gbScheduleDef";
            this.gbScheduleDef.Size = new System.Drawing.Size(847, 301);
            this.gbScheduleDef.TabIndex = 2;
            this.gbScheduleDef.TabStop = false;
            this.gbScheduleDef.Text = "Schedule Definition";
            // 
            // gbRunTimes
            // 
            this.gbRunTimes.Controls.Add(this.dtpEnd);
            this.gbRunTimes.Controls.Add(this.label5);
            this.gbRunTimes.Controls.Add(this.dtpStart);
            this.gbRunTimes.Location = new System.Drawing.Point(100, 210);
            this.gbRunTimes.Name = "gbRunTimes";
            this.gbRunTimes.Size = new System.Drawing.Size(436, 54);
            this.gbRunTimes.TabIndex = 24;
            this.gbRunTimes.TabStop = false;
            this.gbRunTimes.Text = "Run Between Times";
            // 
            // dtpEnd
            // 
            this.dtpEnd.CustomFormat = "hh:mm tt";
            this.dtpEnd.Enabled = false;
            this.dtpEnd.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpEnd.Location = new System.Drawing.Point(233, 19);
            this.dtpEnd.Name = "dtpEnd";
            this.dtpEnd.ShowUpDown = true;
            this.dtpEnd.Size = new System.Drawing.Size(104, 20);
            this.dtpEnd.TabIndex = 2;
            this.dtpEnd.Value = new System.DateTime(2016, 5, 9, 23, 59, 0, 0);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(201, 25);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(25, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "and";
            // 
            // dtpStart
            // 
            this.dtpStart.CustomFormat = "hh:mm tt";
            this.dtpStart.Enabled = false;
            this.dtpStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpStart.Location = new System.Drawing.Point(85, 19);
            this.dtpStart.Name = "dtpStart";
            this.dtpStart.ShowUpDown = true;
            this.dtpStart.Size = new System.Drawing.Size(104, 20);
            this.dtpStart.TabIndex = 0;
            this.dtpStart.Value = new System.DateTime(2016, 5, 9, 0, 0, 0, 0);
            // 
            // gbDays
            // 
            this.gbDays.Controls.Add(this.cbSaturday);
            this.gbDays.Controls.Add(this.cbSunday);
            this.gbDays.Controls.Add(this.cbFriday);
            this.gbDays.Controls.Add(this.cbMonday);
            this.gbDays.Controls.Add(this.cbThursday);
            this.gbDays.Controls.Add(this.cbTuesday);
            this.gbDays.Controls.Add(this.cbWednesday);
            this.gbDays.Location = new System.Drawing.Point(100, 155);
            this.gbDays.Name = "gbDays";
            this.gbDays.Size = new System.Drawing.Size(436, 48);
            this.gbDays.TabIndex = 23;
            this.gbDays.TabStop = false;
            this.gbDays.Text = "Days to run";
            // 
            // cbSaturday
            // 
            this.cbSaturday.AutoSize = true;
            this.cbSaturday.Enabled = false;
            this.cbSaturday.Location = new System.Drawing.Point(305, 19);
            this.cbSaturday.Name = "cbSaturday";
            this.cbSaturday.Size = new System.Drawing.Size(42, 17);
            this.cbSaturday.TabIndex = 22;
            this.cbSaturday.Text = "Sat";
            this.cbSaturday.UseVisualStyleBackColor = true;
            // 
            // cbSunday
            // 
            this.cbSunday.AutoSize = true;
            this.cbSunday.Enabled = false;
            this.cbSunday.Location = new System.Drawing.Point(7, 19);
            this.cbSunday.Name = "cbSunday";
            this.cbSunday.Size = new System.Drawing.Size(45, 17);
            this.cbSunday.TabIndex = 16;
            this.cbSunday.Text = "Sun";
            this.cbSunday.UseVisualStyleBackColor = true;
            // 
            // cbFriday
            // 
            this.cbFriday.AutoSize = true;
            this.cbFriday.Enabled = false;
            this.cbFriday.Location = new System.Drawing.Point(263, 19);
            this.cbFriday.Name = "cbFriday";
            this.cbFriday.Size = new System.Drawing.Size(37, 17);
            this.cbFriday.TabIndex = 21;
            this.cbFriday.Text = "Fri";
            this.cbFriday.UseVisualStyleBackColor = true;
            // 
            // cbMonday
            // 
            this.cbMonday.AutoSize = true;
            this.cbMonday.Enabled = false;
            this.cbMonday.Location = new System.Drawing.Point(57, 19);
            this.cbMonday.Name = "cbMonday";
            this.cbMonday.Size = new System.Drawing.Size(47, 17);
            this.cbMonday.TabIndex = 17;
            this.cbMonday.Text = "Mon";
            this.cbMonday.UseVisualStyleBackColor = true;
            // 
            // cbThursday
            // 
            this.cbThursday.AutoSize = true;
            this.cbThursday.Enabled = false;
            this.cbThursday.Location = new System.Drawing.Point(213, 19);
            this.cbThursday.Name = "cbThursday";
            this.cbThursday.Size = new System.Drawing.Size(45, 17);
            this.cbThursday.TabIndex = 20;
            this.cbThursday.Text = "Thu";
            this.cbThursday.UseVisualStyleBackColor = true;
            // 
            // cbTuesday
            // 
            this.cbTuesday.AutoSize = true;
            this.cbTuesday.Enabled = false;
            this.cbTuesday.Location = new System.Drawing.Point(109, 19);
            this.cbTuesday.Name = "cbTuesday";
            this.cbTuesday.Size = new System.Drawing.Size(45, 17);
            this.cbTuesday.TabIndex = 18;
            this.cbTuesday.Text = "Tue";
            this.cbTuesday.UseVisualStyleBackColor = true;
            // 
            // cbWednesday
            // 
            this.cbWednesday.AutoSize = true;
            this.cbWednesday.Enabled = false;
            this.cbWednesday.Location = new System.Drawing.Point(159, 19);
            this.cbWednesday.Name = "cbWednesday";
            this.cbWednesday.Size = new System.Drawing.Size(49, 17);
            this.cbWednesday.TabIndex = 19;
            this.cbWednesday.Text = "Wed";
            this.cbWednesday.UseVisualStyleBackColor = true;
            // 
            // buSave
            // 
            this.buSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buSave.Location = new System.Drawing.Point(765, 264);
            this.buSave.Name = "buSave";
            this.buSave.Size = new System.Drawing.Size(75, 23);
            this.buSave.TabIndex = 15;
            this.buSave.Text = "Save";
            this.buSave.UseVisualStyleBackColor = true;
            this.buSave.Click += new System.EventHandler(this.buSave_Click);
            // 
            // buRemove
            // 
            this.buRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buRemove.Location = new System.Drawing.Point(684, 264);
            this.buRemove.Name = "buRemove";
            this.buRemove.Size = new System.Drawing.Size(75, 23);
            this.buRemove.TabIndex = 14;
            this.buRemove.Text = "Remove";
            this.buRemove.UseVisualStyleBackColor = true;
            this.buRemove.Click += new System.EventHandler(this.buRemove_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 55);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(82, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "Schedule Type:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rbMonthly);
            this.groupBox2.Controls.Add(this.rbWeekly);
            this.groupBox2.Controls.Add(this.rbDaily);
            this.groupBox2.Controls.Add(this.rbRegularIntervals);
            this.groupBox2.Controls.Add(this.rbOnce);
            this.groupBox2.Location = new System.Drawing.Point(100, 46);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(436, 77);
            this.groupBox2.TabIndex = 12;
            this.groupBox2.TabStop = false;
            // 
            // rbMonthly
            // 
            this.rbMonthly.AutoSize = true;
            this.rbMonthly.Location = new System.Drawing.Point(248, 19);
            this.rbMonthly.Name = "rbMonthly";
            this.rbMonthly.Size = new System.Drawing.Size(62, 17);
            this.rbMonthly.TabIndex = 4;
            this.rbMonthly.Text = "Monthly";
            this.rbMonthly.UseVisualStyleBackColor = true;
            // 
            // rbWeekly
            // 
            this.rbWeekly.AutoSize = true;
            this.rbWeekly.Location = new System.Drawing.Point(141, 43);
            this.rbWeekly.Name = "rbWeekly";
            this.rbWeekly.Size = new System.Drawing.Size(61, 17);
            this.rbWeekly.TabIndex = 3;
            this.rbWeekly.Text = "Weekly";
            this.rbWeekly.UseVisualStyleBackColor = true;
            // 
            // rbDaily
            // 
            this.rbDaily.AutoSize = true;
            this.rbDaily.Location = new System.Drawing.Point(141, 19);
            this.rbDaily.Name = "rbDaily";
            this.rbDaily.Size = new System.Drawing.Size(48, 17);
            this.rbDaily.TabIndex = 2;
            this.rbDaily.Text = "Daily";
            this.rbDaily.UseVisualStyleBackColor = true;
            this.rbDaily.CheckedChanged += new System.EventHandler(this._CheckedChanged);
            // 
            // rbRegularIntervals
            // 
            this.rbRegularIntervals.AutoSize = true;
            this.rbRegularIntervals.Location = new System.Drawing.Point(6, 42);
            this.rbRegularIntervals.Name = "rbRegularIntervals";
            this.rbRegularIntervals.Size = new System.Drawing.Size(105, 17);
            this.rbRegularIntervals.TabIndex = 1;
            this.rbRegularIntervals.Text = "Regular Intervals";
            this.rbRegularIntervals.UseVisualStyleBackColor = true;
            this.rbRegularIntervals.CheckedChanged += new System.EventHandler(this._CheckedChanged);
            // 
            // rbOnce
            // 
            this.rbOnce.AutoSize = true;
            this.rbOnce.Checked = true;
            this.rbOnce.Location = new System.Drawing.Point(6, 19);
            this.rbOnce.Name = "rbOnce";
            this.rbOnce.Size = new System.Drawing.Size(74, 17);
            this.rbOnce.TabIndex = 0;
            this.rbOnce.TabStop = true;
            this.rbOnce.Text = "Run Once";
            this.rbOnce.UseVisualStyleBackColor = true;
            // 
            // buAdd
            // 
            this.buAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buAdd.Location = new System.Drawing.Point(603, 264);
            this.buAdd.Name = "buAdd";
            this.buAdd.Size = new System.Drawing.Size(75, 23);
            this.buAdd.TabIndex = 11;
            this.buAdd.Text = "Add";
            this.buAdd.UseVisualStyleBackColor = true;
            this.buAdd.Click += new System.EventHandler(this.buAdd_Click);
            // 
            // buConfigLocation
            // 
            this.buConfigLocation.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buConfigLocation.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buConfigLocation.Location = new System.Drawing.Point(542, 20);
            this.buConfigLocation.Name = "buConfigLocation";
            this.buConfigLocation.Size = new System.Drawing.Size(26, 20);
            this.buConfigLocation.TabIndex = 10;
            this.buConfigLocation.Text = "...";
            this.buConfigLocation.UseVisualStyleBackColor = true;
            this.buConfigLocation.Click += new System.EventHandler(this.buConfigLocation_Click);
            // 
            // tbConfigFile
            // 
            this.tbConfigFile.Location = new System.Drawing.Point(333, 19);
            this.tbConfigFile.Name = "tbConfigFile";
            this.tbConfigFile.Size = new System.Drawing.Size(203, 20);
            this.tbConfigFile.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(267, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Config File:";
            // 
            // dtpScheduler
            // 
            this.dtpScheduler.CustomFormat = "MM/dd/yyyy hh:mm tt";
            this.dtpScheduler.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpScheduler.Location = new System.Drawing.Point(100, 129);
            this.dtpScheduler.Name = "dtpScheduler";
            this.dtpScheduler.Size = new System.Drawing.Size(148, 20);
            this.dtpScheduler.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(35, 129);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Start Time:";
            // 
            // tbScheduleName
            // 
            this.tbScheduleName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbScheduleName.Location = new System.Drawing.Point(100, 20);
            this.tbScheduleName.Name = "tbScheduleName";
            this.tbScheduleName.Size = new System.Drawing.Size(147, 20);
            this.tbScheduleName.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Schedule Name:";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // clJobName
            // 
            this.clJobName.HeaderText = "Job Name";
            this.clJobName.Name = "clJobName";
            this.clJobName.ReadOnly = true;
            this.clJobName.Width = 150;
            // 
            // clType
            // 
            this.clType.HeaderText = "Type";
            this.clType.Name = "clType";
            this.clType.ReadOnly = true;
            // 
            // clConfigFile
            // 
            this.clConfigFile.HeaderText = "Config File";
            this.clConfigFile.Name = "clConfigFile";
            this.clConfigFile.ReadOnly = true;
            this.clConfigFile.Width = 285;
            // 
            // clNextRun
            // 
            this.clNextRun.HeaderText = "Next Run";
            this.clNextRun.Name = "clNextRun";
            this.clNextRun.ReadOnly = true;
            this.clNextRun.Width = 140;
            // 
            // clLastRunResult
            // 
            this.clLastRunResult.HeaderText = "Last Run Result";
            this.clLastRunResult.Name = "clLastRunResult";
            this.clLastRunResult.ReadOnly = true;
            this.clLastRunResult.Width = 110;
            // 
            // clDaysToRun
            // 
            this.clDaysToRun.HeaderText = "Days To Run";
            this.clDaysToRun.Name = "clDaysToRun";
            this.clDaysToRun.ReadOnly = true;
            this.clDaysToRun.Visible = false;
            // 
            // clTimes
            // 
            this.clTimes.HeaderText = "Times";
            this.clTimes.Name = "clTimes";
            this.clTimes.ReadOnly = true;
            this.clTimes.Visible = false;
            // 
            // ScheduleGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(854, 611);
            this.Controls.Add(this.gbScheduleDef);
            this.Controls.Add(this.dgvSchedule);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ScheduleGUI";
            this.Text = "Schedule Editor";
            ((System.ComponentModel.ISupportInitialize)(this.dgvSchedule)).EndInit();
            this.gbScheduleDef.ResumeLayout(false);
            this.gbScheduleDef.PerformLayout();
            this.gbRunTimes.ResumeLayout(false);
            this.gbRunTimes.PerformLayout();
            this.gbDays.ResumeLayout(false);
            this.gbDays.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvSchedule;
        private System.Windows.Forms.GroupBox gbScheduleDef;
        private System.Windows.Forms.DateTimePicker dtpScheduler;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbScheduleName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbConfigFile;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buConfigLocation;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button buAdd;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rbMonthly;
        private System.Windows.Forms.RadioButton rbWeekly;
        private System.Windows.Forms.RadioButton rbDaily;
        private System.Windows.Forms.RadioButton rbRegularIntervals;
        private System.Windows.Forms.RadioButton rbOnce;
        private System.Windows.Forms.Button buRemove;
        private System.Windows.Forms.Button buSave;
        private System.Windows.Forms.CheckBox cbSaturday;
        private System.Windows.Forms.CheckBox cbFriday;
        private System.Windows.Forms.CheckBox cbThursday;
        private System.Windows.Forms.CheckBox cbWednesday;
        private System.Windows.Forms.CheckBox cbTuesday;
        private System.Windows.Forms.CheckBox cbMonday;
        private System.Windows.Forms.CheckBox cbSunday;
        private System.Windows.Forms.GroupBox gbRunTimes;
        private System.Windows.Forms.GroupBox gbDays;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DateTimePicker dtpStart;
        private System.Windows.Forms.DateTimePicker dtpEnd;
        private System.Windows.Forms.DataGridViewTextBoxColumn clJobName;
        private System.Windows.Forms.DataGridViewTextBoxColumn clType;
        private System.Windows.Forms.DataGridViewTextBoxColumn clConfigFile;
        private System.Windows.Forms.DataGridViewTextBoxColumn clNextRun;
        private System.Windows.Forms.DataGridViewTextBoxColumn clLastRunResult;
        private System.Windows.Forms.DataGridViewTextBoxColumn clDaysToRun;
        private System.Windows.Forms.DataGridViewTextBoxColumn clTimes;
    }
}