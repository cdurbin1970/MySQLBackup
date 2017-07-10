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
            this.clJobName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clTrigger = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clConfigFile = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clLastRun = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clNextRun = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clLastRunResult = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gbScheduleDef = new System.Windows.Forms.GroupBox();
            this.buSave = new System.Windows.Forms.Button();
            this.buRemove = new System.Windows.Forms.Button();
            this.buConfigLocation = new System.Windows.Forms.Button();
            this.tbConfigFile = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbScheduleName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSchedule)).BeginInit();
            this.gbScheduleDef.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvSchedule
            // 
            this.dgvSchedule.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvSchedule.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSchedule.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clJobName,
            this.clStatus,
            this.clTrigger,
            this.clConfigFile,
            this.clLastRun,
            this.clNextRun,
            this.clLastRunResult});
            this.dgvSchedule.Location = new System.Drawing.Point(2, 1);
            this.dgvSchedule.Name = "dgvSchedule";
            this.dgvSchedule.ReadOnly = true;
            this.dgvSchedule.Size = new System.Drawing.Size(1087, 299);
            this.dgvSchedule.TabIndex = 1;
            this.dgvSchedule.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSchedule_CellDoubleClick);
            // 
            // clJobName
            // 
            this.clJobName.HeaderText = "Job Name";
            this.clJobName.Name = "clJobName";
            this.clJobName.ReadOnly = true;
            this.clJobName.Width = 150;
            // 
            // clStatus
            // 
            this.clStatus.HeaderText = "Status";
            this.clStatus.Name = "clStatus";
            this.clStatus.ReadOnly = true;
            // 
            // clTrigger
            // 
            this.clTrigger.HeaderText = "Trigger";
            this.clTrigger.Name = "clTrigger";
            this.clTrigger.ReadOnly = true;
            this.clTrigger.Width = 200;
            // 
            // clConfigFile
            // 
            this.clConfigFile.HeaderText = "Config File";
            this.clConfigFile.Name = "clConfigFile";
            this.clConfigFile.ReadOnly = true;
            this.clConfigFile.Width = 200;
            // 
            // clLastRun
            // 
            this.clLastRun.HeaderText = "Last Run";
            this.clLastRun.Name = "clLastRun";
            this.clLastRun.ReadOnly = true;
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
            this.clLastRunResult.Width = 150;
            // 
            // gbScheduleDef
            // 
            this.gbScheduleDef.Controls.Add(this.buSave);
            this.gbScheduleDef.Controls.Add(this.buRemove);
            this.gbScheduleDef.Controls.Add(this.buConfigLocation);
            this.gbScheduleDef.Controls.Add(this.tbConfigFile);
            this.gbScheduleDef.Controls.Add(this.label3);
            this.gbScheduleDef.Controls.Add(this.tbScheduleName);
            this.gbScheduleDef.Controls.Add(this.label1);
            this.gbScheduleDef.Location = new System.Drawing.Point(2, 306);
            this.gbScheduleDef.MinimumSize = new System.Drawing.Size(1087, 76);
            this.gbScheduleDef.Name = "gbScheduleDef";
            this.gbScheduleDef.Size = new System.Drawing.Size(1087, 76);
            this.gbScheduleDef.TabIndex = 2;
            this.gbScheduleDef.TabStop = false;
            this.gbScheduleDef.Text = "Schedule Definition";
            // 
            // buSave
            // 
            this.buSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buSave.Location = new System.Drawing.Point(673, 19);
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
            this.buRemove.Location = new System.Drawing.Point(754, 19);
            this.buRemove.Name = "buRemove";
            this.buRemove.Size = new System.Drawing.Size(75, 23);
            this.buRemove.TabIndex = 14;
            this.buRemove.Text = "Remove";
            this.buRemove.UseVisualStyleBackColor = true;
            this.buRemove.Click += new System.EventHandler(this.buRemove_Click);
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
            // ScheduleGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(1096, 391);
            this.Controls.Add(this.gbScheduleDef);
            this.Controls.Add(this.dgvSchedule);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ScheduleGUI";
            this.Text = "Schedule Editor";
            this.Load += new System.EventHandler(this.ScheduleGUI_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSchedule)).EndInit();
            this.gbScheduleDef.ResumeLayout(false);
            this.gbScheduleDef.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvSchedule;
        private System.Windows.Forms.GroupBox gbScheduleDef;
        private System.Windows.Forms.TextBox tbScheduleName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbConfigFile;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buConfigLocation;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button buRemove;
        private System.Windows.Forms.Button buSave;
        private System.Windows.Forms.DataGridViewTextBoxColumn clJobName;
        private System.Windows.Forms.DataGridViewTextBoxColumn clStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn clTrigger;
        private System.Windows.Forms.DataGridViewTextBoxColumn clConfigFile;
        private System.Windows.Forms.DataGridViewTextBoxColumn clLastRun;
        private System.Windows.Forms.DataGridViewTextBoxColumn clNextRun;
        private System.Windows.Forms.DataGridViewTextBoxColumn clLastRunResult;
    }
}