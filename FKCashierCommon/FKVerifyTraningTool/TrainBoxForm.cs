using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
//------------------------------------------------------------
namespace FKVerifyTraningTool
{
    public partial class TrainBoxForm : Form
    {
        public TrainBoxForm()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
                Close();
        }

        public string GetTextBoxString()
        {
            return textBox1.Text;
        }
    }
}
