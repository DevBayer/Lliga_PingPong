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
    public class LeagueTests
    {
        [TestMethod()]
        public void addPlayerTest()
        {
            League league = new League();
            Player player1 = new Player();
            player1.Name = "PlayerUnitTest";

            Player player2 = new Player();
            player2.Name = "PlayerUnitTests";
            league.Players.Add(player1);
            league.Players.Add(player2);
            Assert.IsTrue(league.Players.Count == 2);
        }
    }
}