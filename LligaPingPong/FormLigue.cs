using System;
using System.Windows.Forms;

namespace LligaPingPong
{
    public partial class FormLigue : Form
    {

        public String leagueName;
        public String leagueStatus;

        public FormLigue()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormLigue_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            leagueName = textBox1.Text;
            leagueStatus = comboBox1.Text;
            this.Close();
        }
    }
}
