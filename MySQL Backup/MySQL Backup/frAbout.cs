using System;
using System.Windows.Forms;

namespace MySQL_Backup {
    public partial class frAbout : Form {
        public frAbout() {
            InitializeComponent();
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        //                                                                                   //
        //    Close the window                                                               //
        //                                                                                   //
        ///////////////////////////////////////////////////////////////////////////////////////

        private void button1_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}
