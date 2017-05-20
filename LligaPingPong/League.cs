using Newtonsoft.Json;
using System.Collections.Generic;

namespace LligaPingPong
{
    public class League
    {
        [JsonIgnore]
        public string key;

        string name;
        string status;
        List<Player> players = new List<Player>();

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public string Status
        {
            get
            {
                return status;
            }

            set
            {
                status = value;
            }
        }

        public List<Player> Players
        {
            get
            {
                return players;
            }

            set
            {
                players = value;
            }
        }

        public override string ToString()
        {
            return name;
        }

    }

    public class InboundLeague : League
    {
        public long Timestamp { get; set; }
    }

    public class OutboundLeague : League
    {
        [JsonProperty("Timestamp")]
        public ServerTimeStamp TimestampPlaceholder { get; } = new ServerTimeStamp();
    }
}