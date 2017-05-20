using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LligaPingPong
{
    public partial class FormEditMatch : Form
    {
        public ListViewItem match;

        public FormEditMatch()
        {
            InitializeComponent();
        }

        public FormEditMatch(ListViewItem match)
        {
            InitializeComponent();
            this.match = match;
        }

        private void FormEditMatch_Load(object sender, EventArgs e)
        {
            p1_Name.Text = match.SubItems[3].Text;
            p1_Points.Text = match.SubItems[4].Text;
            p2_Name.Text = match.SubItems[5].Text;
            p2_Points.Text = match.SubItems[6].Text;
            cbStatus.Text = match.SubItems[7].Text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            match.SubItems[4].Text = p1_Points.Text;
            match.SubItems[6].Text = p2_Points.Text;
            match.SubItems[7].Text = cbStatus.Text;
            this.Close();
        }
    }
}
