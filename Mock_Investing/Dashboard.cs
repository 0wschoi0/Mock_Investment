using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mock_Investing
{
    public partial class Dashboard : Form
    {
        public Dashboard(string uid)
        {
            InitializeComponent();
        }
        private void butMy_Click(object sender, EventArgs e)
        {
            guna2TabControl1.SelectedIndex = 0;
        }

        private void butView_Click(object sender, EventArgs e)
        {
            guna2TabControl1.SelectedIndex = 1;
        }

        private void butFav_Click(object sender, EventArgs e)
        {
            guna2TabControl1.SelectedIndex = 2;
        }

        private void butTrans_Click(object sender, EventArgs e)
        {
            guna2TabControl1.SelectedIndex = 3;
        }



        private void butLogout_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

    }
}
