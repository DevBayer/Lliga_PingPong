using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LligaPingPong
{
    public partial class Form1 : Form
    {

        Manager manager = new Manager();
        DataTable t = new DataTable();

        public Form1()
        {
            InitializeComponent();
            dataGridView2.DataSource = t;
            dataGridView1.RowTemplate.Height = 100;
            runLeague();

        }
            private void runLeague()
        {
            Manager man = new Manager();
            man.ObservablePlayers(dataGridView1);
            man.ObservableLeagues(dataGridView2);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void panePlayerAvatar_Click(object sender, EventArgs e)
        {

        }

        private async void button3_Click(object sender, EventArgs e)
        {
            var window = new FormLigue();
            window.StartPosition = FormStartPosition.CenterParent;
            window.ShowDialog(this);
            if (window.leagueName.Length > 0)
            {
                League l = new League();
                l.Name = window.leagueName;
                l.Status = window.leagueStatus;
                await manager.AddLeague(l);
            }
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private async void button5_Click(object sender, EventArgs e)
        {
            var window = new FormAddPlayer();
            window.StartPosition = FormStartPosition.CenterParent;
            window.ShowDialog(this);
            if (window.playerName.Length > 0 && window.playerDNI.Length > 0)
            {
                Player p = new Player();
                p.Name = window.playerName;
                p.Dni = window.playerDNI;
                p.Status = window.playerStatus;
                p.Photo = Path.GetFileName(window.photoPlayer);
                p.AbsolutePhoto = window.photoPlayer;
                await manager.AddPlayer(p);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private async void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            var dataIndexNo = dataGridView1.Rows[e.RowIndex].Index.ToString();
            string cellValue = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();


            Player p = await manager.GetPlayer(cellValue.ToString());
            var window = new FormEditPlayer(p);
            window.StartPosition = FormStartPosition.CenterParent;
            window.ShowDialog(this);
            if (window.playerName.Length > 0 && window.playerDNI.Length > 0)
            {
                p.Name = window.playerName;
                p.Dni = window.playerDNI;
                p.Status = window.playerStatus;
                p.Photo = Path.GetFileName(window.photoPlayer);
                p.AbsolutePhoto = window.photoPlayer;
                await manager.UpdatePlayer(p);
            }

        }

        private async void dataGridView2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            var dataIndexNo = dataGridView2.Rows[e.RowIndex].Index.ToString();
            string cellValue = dataGridView2.Rows[e.RowIndex].Cells[0].Value.ToString();


            League l = await manager.GetLeague(cellValue.ToString());
            var window = new FormLigueDetails(l);
            window.StartPosition = FormStartPosition.CenterParent;
            window.ShowDialog(this);

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
