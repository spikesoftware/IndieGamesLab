using Newtonsoft.Json;
using System.Collections.Generic;

namespace IGL
{
    [JsonObject(MemberSerialization.OptIn)]
    public class GameEvent
    {
        /// <summary>
        /// The IndieGamesLab Game ID
        /// </summary>
        [JsonProperty("gameid")]
        public int GameId { get; set; }
        
        /// <summary>
        /// The IndieGamesLab Event ID
        /// </summary>
        [JsonProperty("eventid")]
        public int EventId { get; set; }

        [JsonProperty("properties")]
        public IDictionary<string, string> Properties { get; set; }

    }
}
