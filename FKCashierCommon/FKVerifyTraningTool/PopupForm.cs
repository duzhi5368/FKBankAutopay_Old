using System;
using System.Windows.Forms;
//------------------------------------------------------------
namespace FKVerifyTraningTool
{
    public partial class PopupForm : Form
    {
        public PopupForm()
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
