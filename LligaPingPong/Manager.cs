using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LligaPingPong
{
    class Manager
    {
        FirebaseClient firebase = new FirebaseClient("https://lligapingpong.firebaseio.com/");
        static String bucketStorage = "lligapingpong.appspot.com";

        public FirebaseClient client()
        {
            return firebase;
        }

        public async Task AddLeague(League league)
        {
            await ((Firebase.Database.Query.ChildQuery)firebase.Child((string)"leagues")).PostAsync(league);
        }

        public async Task AddPlayer(Player player)
        {
            var saved = await ((Firebase.Database.Query.ChildQuery)firebase.Child((string)"players")).PostAsync(player);
            uploadPlayerAvatar(saved.Key, player.AbsolutePhoto);
        }

        public async Task UpdatePlayer(Player player)
        {
            var saved = await ((Firebase.Database.Query.ChildQuery)firebase.Child((string)"players/"+ player.key)).PostAsync(player);
            uploadPlayerAvatar(saved.Key, player.AbsolutePhoto);
        }

        public Task<Player> GetPlayer(String id)
        {
            var dinos = firebase.Child("players/" + id).OnceSingleAsync<Player>();
            dinos.Result.key = id;
            return dinos;
        }

        public Task<League> GetLeague(String id)
        {
            var dinos = firebase.Child("leagues/" + id).OnceSingleAsync<League>();
            dinos.Result.key = id;
            return dinos;
        }

        public void ObservablePlayers(DataGridView grid)
        {
            try
            {
                DataTable myTab = new DataTable();
                myTab.Columns.Add("ID");
                myTab.Columns.Add("Photo", typeof(byte[]));
                myTab.Columns.Add("DNI");
                myTab.Columns.Add("Player Name");
                myTab.Columns.Add("Status");
                var child = ((Firebase.Database.Query.ChildQuery)firebase.Child((string)"players"));
                var observable = child.AsObservable<InboundPlayer>();
                var subscription = observable
                    .Where(f => !string.IsNullOrEmpty(f.Key))
                    .Subscribe(async f =>
                    {
                        f.Object.key = f.Key;
                        if (f.EventType == Firebase.Database.Streaming.FirebaseEventType.InsertOrUpdate)
                        {
                            Image img = null;
                            try
                            {
                                img = Image.FromFile(f.Object.AbsolutePhoto);
                                img = resizeImage(img, new Size(100, 100));
                            }
                            catch (Exception e)
                            {
                                Debug.Print(e.Message);
                                try
                                {
                                    String url = await DownloadPlayerAvatar(f.Object.key, f.Object.AbsolutePhoto);
                                    WebRequest req = WebRequest.Create(url);
                                    WebResponse response = req.GetResponse();
                                    Stream stream = response.GetResponseStream();
                                    img = Image.FromStream(stream);
                                    stream.Close();
                                    img = resizeImage(img, new Size(100, 100));
                                    //...
                                }
                                catch (Exception)
                                {
                                    MessageBox.Show("There was a problem downloading the file");
                                }
                            }
                            myTab.Rows.Add(f.Key, imageToByteArray(img), f.Object.Dni, f.Object.Name, f.Object.Status);
                        }
                        else if (f.EventType == Firebase.Database.Streaming.FirebaseEventType.Delete)
                        {
                            for (int v = 0; v < grid.Rows.Count; v++)
                            {
                                if (string.Equals(grid[0, v].Value as string, f.Key))
                                {
                                    grid.Rows.RemoveAt(v);
                                    v--; // this just got messy. But you see my point.
                                }
                            }
                        }

                        grid.Invoke((MethodInvoker)delegate
                        {
                            grid.Update();
                            grid.Refresh();
                            grid.Focus();
                        });

                    }
                    );

                grid.DataSource = myTab;
                grid.Focus();
                grid.Update();
                grid.Refresh();
            }
            catch (System.IO.FileLoadException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public byte[] imageToByteArray(System.Drawing.Image imageIn)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            return ms.ToArray();
        }

        public static Image resizeImage(Image imgToResize, Size size)
        {
            return (Image)(new Bitmap(imgToResize, size));
        }

        public void ObservableLeagues(DataGridView grid)
        {
            try
            {
                DataTable myTab = new DataTable();
                myTab.Columns.Add("ID");
                myTab.Columns.Add("League");
                myTab.Columns.Add("Status");
                var child = ((Firebase.Database.Query.ChildQuery)firebase.Child((string)"leagues"));
                var observable = child.AsObservable<InboundLeague>();
                var subscription = observable
                    .Where(f => !string.IsNullOrEmpty(f.Key))
                    .Subscribe(f =>
                    {
                        f.Object.key = f.Key;
                         if (f.EventType == Firebase.Database.Streaming.FirebaseEventType.InsertOrUpdate)
                        {
                            bool flag = true;
                            foreach (DataRow row in myTab.Rows)
                            {
                                if (row[0].Equals(f.Key))
                                {
                                    row[0] = f.Key;
                                    row[1] = f.Object.Name;
                                    row[2] = f.Object.Status;
                                    flag = false;
                                    break;
                                }
                                else
                                {
                                    flag = true;
                                }
                            }

                            if (flag)
                            {
                                myTab.Rows.Add(f.Key, f.Object.Name, f.Object.Status);
                            }
                        }else if (f.EventType == Firebase.Database.Streaming.FirebaseEventType.Delete)
                        {
                            for (int v = 0; v < grid.Rows.Count; v++)
                            {
                                if (string.Equals(grid[0, v].Value as string, f.Key))
                                {
                                    grid.Rows.RemoveAt(v);
                                    v--; // this just got messy. But you see my point.
                                }
                            }
                        }
                        grid.Invoke((MethodInvoker)delegate {
                            grid.Refresh();
                        });
                    }                    
                    );

                grid.DataSource = myTab;
                grid.Refresh();
            }
            catch (System.IO.FileLoadException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        internal void uploadPlayerAvatar(string player, string photoPlayer)
        {
            try
            {
                var stream = System.IO.File.OpenRead(photoPlayer);
                // Constructr FirebaseStorage, path to where you want to upload the file and Put it there
                var task = new FirebaseStorage(bucketStorage)
                    .Child("players")
                    .Child(player)
                    .Child(System.IO.Path.GetFileName(photoPlayer))
                    .PutAsync(stream);
            }
            catch (Exception e)
            {
                MessageBox.Show("There was a problem uploading the file: " + e.Message);
            }
        }

        public static async Task<string> DownloadPlayerAvatar(string player, string filePhoto)
        {
            var task = new FirebaseStorage(bucketStorage)
                .Child("players")
                .Child(player)
                .Child(System.IO.Path.GetFileName(filePhoto));
            return await task.GetDownloadUrlAsync();
        }

        public void ObservableLeague(String key, DataGridView grid)
        {
            try
            {
                DataTable myTab = new DataTable();
                myTab.Columns.Add("ID");
                myTab.Columns.Add("Name");
                myTab.Columns.Add("Points");
                var child = ((Firebase.Database.Query.ChildQuery)firebase.Child((string)"players")).Child(key);
                var observable = child.AsObservable<InboundLeague>();
                var subscription = observable
                    .Where(f => !string.IsNullOrEmpty(f.Key))
                    .Subscribe(f =>
                    {
                        f.Object.key = f.Key;
                        if (f.EventType == Firebase.Database.Streaming.FirebaseEventType.InsertOrUpdate)
                        {
                            myTab.Rows.Add(f.Key, f.Object.Name, f.Object.Status);
                        }
                        else if (f.EventType == Firebase.Database.Streaming.FirebaseEventType.Delete)
                        {
                            for (int v = 0; v < grid.Rows.Count; v++)
                            {
                                if (string.Equals(grid[0, v].Value as string, f.Key))
                                {
                                    grid.Rows.RemoveAt(v);
                                    v--; // this just got messy. But you see my point.
                                }
                            }
                        }
                        grid.Invoke((MethodInvoker)delegate {
                            grid.Refresh();
                        });
                    }
                    );

                grid.DataSource = myTab;
                grid.Refresh();
            }
            catch (System.IO.FileLoadException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public async Task<List<Player>> getLeaguePlayers(League league)
        {
            List<Player> listPlayers = new List<Player>();
            var players = await firebase.Child("rel_league_player").Child(league.key).OrderByKey().OnceAsync<Player>();
            foreach (var data in players)
            {
                data.Object.key = data.Key;
                listPlayers.Add(data.Object);
            }
            return listPlayers;
        }

        public async Task<List<Round>> getLeagueMatches(League league)
        {
            List<Round> matches = new List<Round>();
            var rounds = await firebase.Child("league_rounds").Child(league.key).OnceAsync<Round>();

            foreach(var round in rounds)
            {
                round.Object.key = round.Key;
                matches.Add(round.Object);
            }

            return matches;
        }

        public async Task<Match> getLeagueMatch(String league, String key, int match)
        {
            var matchObj = await firebase.Child("league_rounds").Child(league).Child(key).Child("matches").Child(""+match).OnceAsync<Object>();
            Match rMatch = new Match();
            foreach (var m in matchObj)
            {

                if(m.Key == "finished")
                {
                    rMatch.finished = (bool)m.Object;
                }else if(m.Key == "points_p1")
                {
                    rMatch.points_p1 = int.Parse(""+ m.Object );
                }
                else if (m.Key == "points_p2")
                {
                    rMatch.points_p2 = int.Parse("" + m.Object);
                }
            }
            return rMatch;
        }

        public void updateMatchPoints(String league, String key, int match, int p1_Points, int p2_Points, bool finished)
        {
            firebase.Child("league_rounds/" + league).Child(key).Child("matches").Child(match + "").Child("points_p1").PutAsync(p1_Points);
            firebase.Child("league_rounds/" + league).Child(key).Child("matches").Child(match + "").Child("points_p2").PutAsync(p2_Points);
            firebase.Child("league_rounds/" + league).Child(key).Child("matches").Child(match + "").Child("finished").PutAsync(finished);
        }

        public async 
        Task
saveLeagueRounds(League league, List<Round> rounds)
        {
            foreach(Round r in rounds)
            {
                await firebase.Child("league_rounds").Child(league.key).PostAsync(r);
                
            }
        }

        public async 
        Task
addLeaguePlayer(League league, Player p)
        {
            await firebase.Child("rel_league_player").Child(league.key).Child(p.key).PutAsync(p);
        }

        public void deleteLeaguePlayer(League league, String pkey)
        {
            firebase.Child("rel_league_player").Child(league.key).Child(pkey).DeleteAsync();
        }

        public void ObservableLeaguePlayers(ListView lv, League league)
        {
            try
            {
                var child = ((Firebase.Database.Query.ChildQuery)firebase.Child("rel_league_player").Child(league.key));
                var observable = child.AsObservable<InboundPlayer>();
                var subscription = observable
                    .Where(f => !string.IsNullOrEmpty(f.Key))
                    .Subscribe(f =>
                    {

                        f.Object.key = f.Key;
                        if (f.EventType == Firebase.Database.Streaming.FirebaseEventType.InsertOrUpdate)
                        {
                            var list = new ListViewItem(f.Object.key);
                            list.SubItems.Add(f.Object.Dni);
                            list.SubItems.Add(f.Object.Name);
                            lv.Invoke((MethodInvoker)delegate {
                                lv.Items.Add(list);
                            });
                        }
                        else if (f.EventType == Firebase.Database.Streaming.FirebaseEventType.Delete)
                        {
                            for (int v = 0; v < lv.Items.Count; v++)
                            {
                                if (string.Equals(lv.Items[v].SubItems[0], f.Key))
                                {
                                    lv.Items[v].Remove();
                                    v--; // this just got messy. But you see my point.
                                }
                            }
                        }
                        lv.Invoke((MethodInvoker)delegate {
                            lv.Refresh();
                        });
                    }
                    );
                lv.Refresh();
            }
            catch (System.IO.FileLoadException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public async void getLeaguePlayers(ListView lv, League league)
        {
            lv.Items.Clear();
            var child = await firebase.Child("rel_league_player").Child(league.key).OnceAsync<Player>();
            foreach(var f in child) {
                f.Object.key = f.Key;
                var list = new ListViewItem(f.Object.key);
                list.SubItems.Add(f.Object.Dni);
                list.SubItems.Add(f.Object.Name);
                lv.Items.Add(list);
             }
            lv.Refresh();
        }

        public async void deleteLeague(League l)
        {
            await firebase.Child("rel_league_players").Child(l.key).DeleteAsync();
            await firebase.Child("league_rounds").Child(l.key).DeleteAsync();
            await firebase.Child("leagues").Child(l.key).DeleteAsync();
        }

    }
}
