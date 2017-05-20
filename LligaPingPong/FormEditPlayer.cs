using Firebase.Storage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LligaPingPong
{

    public partial class FormEditPlayer : Form
    {
        public String playerName = "";
        public String playerDNI = "";
        public String playerStatus = "";
        public String photoPlayer = "";


        public FormEditPlayer(Player p)
        {
            InitializeComponent();

            textBox1.Text = p.Name;
            textBox2.Text = p.Dni;
            comboBox1.Text = p.Status;
            if (File.Exists(p.AbsolutePhoto))
            {
                panePlayerAvatar.Image = new Bitmap(p.AbsolutePhoto);
            }
            else
            {
                Task.Run(async () =>
                {
                    Debug.Print("pKey: " + p.key);
                    String url = await DownloadPlayerAvatar(p.key, p.AbsolutePhoto);
                    WebRequest req = WebRequest.Create(url);
                    WebResponse response = req.GetResponse();
                    Stream stream = response.GetResponseStream();
                    panePlayerAvatar.Image = Image.FromStream(stream);
                    Debug.Print(url);
                    stream.Close();
                });
            }
            panePlayerAvatar.SizeMode = PictureBoxSizeMode.StretchImage;


            panePlayerUploadAvatar.Parent = panePlayerAvatar;
            panePlayerUploadAvatar.BackColor = Color.WhiteSmoke;
            panePlayerUploadAvatar.Top = 0;
            panePlayerUploadAvatar.Left = 0;
        }

        private async Task<string> DownloadPlayerAvatar(string key, string absolutePhoto)
        {
            return await Manager.DownloadPlayerAvatar(key, absolutePhoto);
        }

        private void panePlayerAvatar_MouseHover(object sender, EventArgs e)
        {
            panePlayerUploadAvatar.Visible = true;
        }



        private void panePlayerAvatar_MouseLeave(object sender, EventArgs e)
        {
            panePlayerUploadAvatar.Visible = false;
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void panePlayerAvatar_Click(object sender, EventArgs e)
        {

        }

        private void FormAddPlayer_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            playerName = textBox1.Text;
            playerDNI = textBox2.Text;
            playerStatus = comboBox1.Text;
            this.Close();
        }

        private void panePlayerUploadAvatar_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Open Image";
                dlg.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    // Create a new Bitmap object from the picture file on disk,
                    // and assign that to the PictureBox.Image property
                    photoPlayer = dlg.FileName;
                    panePlayerAvatar.Image = new Bitmap(dlg.FileName);
                    panePlayerAvatar.SizeMode = PictureBoxSizeMode.StretchImage;
                }
            }
        }
    }
}
