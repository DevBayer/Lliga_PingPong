using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LligaPingPong
{
    public class Match
    {
        public Player player1 { get; set; }
        public Player player2 { get; set; }
        public Boolean finished { get; set; }
        public int points_p1 { get; set; }
        public int points_p2 { get; set; }

        public Match(Player player1, Player player2)
        {
            this.player1 = player1;
            this.player2 = player2;
        }

        public Match()
        {
        }
    }
}
