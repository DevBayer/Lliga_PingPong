using Firebase.Database.Query;
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
    public partial class FormLigueDetails : Form
    {
        Manager m;
        League league;
        public FormLigueDetails(League league)
        {
            InitializeComponent();
            this.m = new Manager();
            this.league = league;
            txtLeagueName.Text = league.Name;
            cbStatus.Text = league.Status;
        }


        private void panePlayerAvatar_Click(object sender, EventArgs e)
        {

        }

        private async Task drawMatchesManager()
        {
            listView2.Items.Clear();
            List<Round> rounds = await m.getLeagueMatches(league);
            if (rounds.Count > 0)
            {
                paneInitializeMatches.Visible = false;
            }
            string style = "";
            style += ".container {position: relative; width: 100 %;max-width: 960px;margin: 0 auto;padding: 0 20px;box-sizing: border-box;}";
            style += ".column,.columns {width: 100 %;float: left;box-sizing: border-box;}";
            style += ".container:after,.row:after,.u-cf {content: '';display: table;clear: both;}";
            style += "@media (min-width: 550px) {";
            style += ".container {width: 80 %;}";
            style += ".column,.columns {margin-left: 4 %;}";
            style += ".column: first-child,.columns:first-child {margin-left: 0;}";
            style += ".one.column,.one.columns                    { width: 4.66666666667 %; }";
            style += ".two.columns                    { width: 13.3333333333 %; }";
            style += ".three.columns                  { width: 22 %; }";
            style += ".four.columns                   { width: 30.6666666667 %; }";
            style += ".five.columns                   { width: 39.3333333333 %; }";
            style += ".six.columns                    { width: 48 %; }";
            style += ".seven.columns                  { width: 56.6666666667 %; }";
            style += ".eight.columns                  { width: 65.3333333333 %; }";
            style += ".nine.columns                   { width: 74.0 %; }";
            style += ".ten.columns                    { width: 82.6666666667 %; }";
            style += ".eleven.columns                 { width: 91.3333333333 %; }";
            style += ".twelve.columns                 { width: 100 %; margin-left: 0; }";
            style += "}";
            style += ".match { margin:5px; padding: 1px; background: white; border: 1px solid red;  }";
            style += ".player { height: 72px; line-height: 72px; }";
            style += ".versus { padding: 10px; }";
            style += ".marker { text-align: center; padding-top:15px; background: pink; }";
            style += ".player { background:lightsalmon; }";
            string htmlbody = "";

            foreach (Round r in rounds)
            {
                htmlbody += "<div class='container round'>";
                htmlbody += "<div class='row'>";
                htmlbody += "<h1>Día " + r.day + "</h1>";
                htmlbody += "<div class='matches twelve columns'>";
                int matchId = 0;
                foreach (Match m in r.matches)
                {
                    htmlbody += "<div class='match six columns'>";
                    htmlbody += "<div class='row marker'>";
                    htmlbody += "<h1>" + m.points_p1 + ":" + m.points_p2 + "</h1>";
                    htmlbody += "</div>";
                    htmlbody += "<div class='row'>";
                    htmlbody += "<div class='player six columns'>" + m.player1.Name + "<small>( " + m.player1.Dni + " )</small>" + "</div>";
                    htmlbody += "<div class='versus one column'><h3>VS</h3></div>";
                    htmlbody += "<div class='player six columns'>" + m.player2.Name + "<small>( " + m.player2.Dni + " )</small>" + "</div>";
                    htmlbody += "</div>";
                    htmlbody += "</div>";
                    var list = new ListViewItem(r.key);
                    list.SubItems.Add("" + matchId);
                    list.SubItems.Add("" + r.day);
                    list.SubItems.Add(m.player1.Name);
                    list.SubItems.Add("" + m.points_p1);
                    list.SubItems.Add(m.player2.Name);
                    list.SubItems.Add("" + m.points_p2);
                    list.SubItems.Add((m.finished ? "true" : "false"));
                    listView2.Items.Add(list);
                    matchId++;
                }
                htmlbody += "</div>";
                htmlbody += "</div>";
                htmlbody += "</div>";
            }
            webBrowser1.Navigate("about:blank");
            webBrowser1.Document.OpenNew(false);
            webBrowser1.Document.Write("<html><head><meta http-equiv='X-UA-Compatible'  content='IT=edge,chrome=IE8'><meta charset='utf-8'><style>" + style + "</style></head><body>" + htmlbody + "</body></html>");
            webBrowser1.Refresh();
        }

        private void FormLigueDetails_Load(object sender, EventArgs e)
        {
           // m.ObservableLeaguePlayers(listView1, league);
            m.getLeaguePlayers(listView1, league);
            drawMatchesManager();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private async void button3_Click(object sender, EventArgs e)
        {
            var window = new FormSearchPlayer();
            window.StartPosition = FormStartPosition.CenterParent;
            window.ShowDialog(this);
            panePlayer.Visible = false;
            if (window.players.Count > 0)
            {
                foreach(Player p in window.players)
                {
                    await m.addLeaguePlayer(league, p);
                }
                m.getLeaguePlayers(listView1, league);
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            var player = listView1.SelectedItems[0];
            panePlayerTitle.Text = "Participation";
            panePlayerName.Text = player.SubItems[2].Text;
            panePlayerKey.Text = player.SubItems[0].Text;

            panePlayer.Visible = true;

        }

        private void panePlayer_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            var player = panePlayerKey.Text;
            m.deleteLeaguePlayer(league, player);
            panePlayer.Visible = false;
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            List<Player> players = await m.getLeaguePlayers(league);
            if (players.Count > 0)
            {
                LeagueManager lm = new LeagueManager();

                List<Round> rounds = lm.generateMatches(players);
                if(rounds != null)
                {
                    await m.saveLeagueRounds(league, rounds);
                    await drawMatchesManager();
                }
                else
                {
                    MessageBox.Show("The league doesn't have sufficient Players to do a Schedule");
                }
            }else
            {
                MessageBox.Show("The league doesn't have Players");
            }

        }

        private void listView2_Click(object sender, EventArgs e)
        {

        }

        private void listView2_MouseClick(object sender, MouseEventArgs e)
        {
            var match = listView2.SelectedItems[0];
            var window = new FormEditMatch(match);
            window.StartPosition = FormStartPosition.CenterParent;
            window.ShowDialog(this);
            if (window.match != null)
            {
                var key = window.match.SubItems[0].Text;
                var p1_points = window.match.SubItems[4].Text;
                var p2_points = window.match.SubItems[6].Text;
                var status = window.match.SubItems[7].Text;
                bool finished = status == "true" ? true : false;
                m.updateMatchPoints(league.key, key, 0, int.Parse(p1_points), int.Parse(p2_points), finished);
                drawMatchesManager();
            }

        }

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            league.Status = cbStatus.Text;
            m.client().Child("leagues").Child(league.key).Child("Status").PutAsync(league.Status);
        }

        private void cbStatus_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you want delete All Data of League?", "Delete League", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                m.deleteLeague(league);
                this.Close();
            }
        }
    }
}