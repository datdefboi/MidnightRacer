using System;
using System.Windows.Forms;

namespace MidnightRacer
{
    public partial class RenameDialog : Form
    {
        public string UserName
        {
            get => nameBox.Text;
            set => nameBox.Text = value;
        }

        public RenameDialog() { InitializeComponent(); }
        private void button1_Click(object sender, EventArgs e)
        {
            Hide();
        }
    }
}