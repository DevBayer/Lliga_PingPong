using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LligaPingPong
{
    public class Round
    {
        [JsonIgnore]
        public string key;
        public int day { get; set; }
        public List<Match> matches { get; set; }

        public Round()
        {
            matches = new List<Match>();
        }

    }
}
