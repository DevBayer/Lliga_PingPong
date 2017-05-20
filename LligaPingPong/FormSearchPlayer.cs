using System;
using System.Windows.Forms;
using Firebase.Database;
using Firebase.Database.Query;
using System.Linq;

namespace LligaPingPong
{
    public partial class FormSearchPlayer : Form
    {
        FirebaseClient client;
        Manager m;
        public System.Collections.Generic.List<Player> players = new System.Collections.Generic.List<Player>();

        public FormSearchPlayer()
        {
            InitializeComponent();
        }

        private async void FormSearchPlayer_Load(object sender, EventArgs e)
        {
            m = new Manager();
            client = m.client();
            var players = await client.Child("players").OnceAsync<Player>();

            ListViewItem[] list = new ListViewItem[players.Count];
            int row = 0;
            foreach (var player in players)
            {
                list[row] = new ListViewItem(player.Key);
                list[row].SubItems.Add(player.Object.Dni);
                list[row].SubItems.Add(player.Object.Name);
                row++;
            }

            listView1.Items.AddRange(list);
            listView1.Refresh();

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count >= 1)
            {
                foreach (ListViewItem item in listView1.SelectedItems)
                {
                    Player p = await m.GetPlayer(item.SubItems[0].Text);
                    players.Add(p);
                }
            }
            this.Close();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var players = await client.Child("players").OrderBy("Dni").StartAt(textBox1.Text).OnceAsync<Player>();
            listView1.Items.Clear();
            ListViewItem[] list = new ListViewItem[players.Count];
            int row = 0;
            foreach (var player in players)
            {
                list[row] = new ListViewItem(player.Key);
                list[row].SubItems.Add(player.Object.Dni);
                list[row].SubItems.Add(player.Object.Name);
                row++;
            }

            listView1.Items.AddRange(list);
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
