using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cover_and_init
{
    public partial class Form_hand_measurement_button : Form
    {
        public Form_hand_measurement_button()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Common.is_hand_mode = true;
            this.Visible = false;
        }

        private void Form_hand_measurement_button_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                //Hide();
            }
        }
    }
}
