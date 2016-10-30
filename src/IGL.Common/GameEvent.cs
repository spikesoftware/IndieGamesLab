using Newtonsoft.Json;
using System.Collections.Generic;

namespace IGL
{
    [JsonObject(MemberSerialization.OptIn)]
    public class GameEvent
    {
        [JsonProperty("properties")]
        public IDictionary<string, string> Properties { get; set; }
    }
}
