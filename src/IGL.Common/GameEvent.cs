using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace IGL
{
    [JsonObject(MemberSerialization.OptIn)]
    [Serializable]
    public class GameEvent
    {
        public GameEvent()
        {
            Properties = new ConcurrentDictionary<string, string>();
        }

        [JsonProperty("properties")]
        public IDictionary<string, string> Properties { get; set; }
    }
}