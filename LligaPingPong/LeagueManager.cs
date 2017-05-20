using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LligaPingPong
{
    public class LeagueManager
    {
        public List<Round> generateMatches(List<Player> players)
        {
            List<Round> rounds = new List<Round>();
            if (players.Count % 2 != 0)
            {
                return null;
            }

            int numDays = (players.Count - 1);
            int halfSize = players.Count / 2;

            List<Player> teams = new List<Player>();

            teams.AddRange(players);
            teams.RemoveAt(0);

            int teamsSize = teams.Count;
            for (int day = 0; day < numDays; day++)
            {
                Round round = new Round();
                round.day = day + 1;

                int teamIdx = day % teamsSize;
                Match m1 = new Match(teams[teamIdx], players[0]);
                round.matches.Add(m1);

                for (int idx = 1; idx < halfSize; idx++)
                {
                    int firstTeam = (day + idx) % teamsSize;
                    int secondTeam = (day + teamsSize - idx) % teamsSize;
                    Match m2 = new Match(teams[firstTeam], teams[secondTeam]);
                    round.matches.Add(m2);
                }
                rounds.Add(round);
            }
            return rounds;
        }
    }
}
