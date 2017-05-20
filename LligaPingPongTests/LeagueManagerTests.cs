using Microsoft.VisualStudio.TestTools.UnitTesting;
using LligaPingPong;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LligaPingPong.Tests
{
    [TestClass()]
    public class LeagueManagerTests
    {
        [TestMethod()]
        public void generateRoundsTest()
        {
            LeagueManager manager = new LeagueManager();
            League league = new League();

            Player player1 = new Player();
            player1.Name = "PlayerUnitTest";
            league.Players.Add(player1);

            Player player2 = new Player();
            player2.Name = "PlayerUnitTest";
            league.Players.Add(player2);

            Player player3 = new Player();
            player3.Name = "PlayerUnitTest";
            league.Players.Add(player3);

            Player player4 = new Player();
            player4.Name = "PlayerUnitTest";
            league.Players.Add(player4);

            List<Round> rounds = manager.generateMatches(league.Players);

            Assert.IsTrue(rounds.Count == 3);
        }

        [TestMethod()]
        public void ScheduleMatchesTest()
        {
            LeagueManager manager = new LeagueManager();
            League league = new League();

            Player player1 = new Player();
            player1.Name = "PlayerUnitTest";
            league.Players.Add(player1);

            Player player2 = new Player();
            player2.Name = "PlayerUnitTest";
            league.Players.Add(player2);

            Player player3 = new Player();
            player3.Name = "PlayerUnitTest";
            league.Players.Add(player3);

            Player player4 = new Player();
            player4.Name = "PlayerUnitTest";
            league.Players.Add(player4);

            int matches_expected = 6;

            List<Round> rounds = manager.generateMatches(league.Players);

            int matches = 0;
            foreach(Round r in rounds)
            {
                matches += r.matches.Count;
            }

            Assert.IsTrue(matches_expected == 6);
        }

        [TestMethod()]
        public void generateMatchesOnePlayerTest()
        {
            LeagueManager manager = new LeagueManager();
            League league = new League();

            Player player1 = new Player();
            player1.Name = "PlayerUnitTest";
            league.Players.Add(player1);
            List<Round> rounds = manager.generateMatches(league.Players);

            Assert.IsTrue(rounds == null);
        }

        [TestMethod()]
        public void generateMatchesEvenTest()
        {
            LeagueManager manager = new LeagueManager();
            League league = new League();

            Player player1 = new Player();
            player1.Name = "PlayerUnitTest";
            league.Players.Add(player1);
            List<Round> rounds = manager.generateMatches(league.Players);

            Player player2 = new Player();
            player2.Name = "PlayerUnitTest";
            league.Players.Add(player2);

            Player player3 = new Player();
            player3.Name = "PlayerUnitTest";
            league.Players.Add(player3);

            Assert.IsTrue(rounds == null);
        }

    }
}