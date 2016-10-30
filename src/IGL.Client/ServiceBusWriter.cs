using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;

namespace IGL.Client
{
    public class ServiceBusWriter : ServiceBusBase
    {
        internal static readonly string correlation = Guid.NewGuid().ToString().Replace("-", "");        
        static int _packet = 0;
        
        private static readonly object _syncRoot = new Object();

        public static bool SubmitGameEvent(string queueName, int eventId, GameEvent gameevent, KeyValuePair<string, string>[] properties = null, string sessionId = null)
        {
            GamePacket packet;

            lock (_syncRoot)
            {
                packet = new GamePacket
                {                    
                    GameId = Configuration.GameId,
                    PlayerId = Configuration.PlayerId,
                    Correlation = correlation,
                    PacketNumber = _packet++,
                    PacketCreatedUTCDate = DateTime.UtcNow,                    
                    GameEvent = gameevent,
                    EventId = eventId,                    
                };
            }

            var content = Encoding.Default.GetBytes(DatacontractSerializerHelper.Serialize<GamePacket>(packet));

            using (WebClient webClient = new WebClient())
            {
                webClient.Headers[HttpRequestHeader.Authorization] = GetToken();

                // add the properties
                var collection = new NameValueCollection();

                if (properties != null)
                {
                    foreach (var property in properties)
                        collection.Add(property.Key, property.Value);
                }

                collection.Add(GamePacket.VERSION, GamePacket.Namespace);

                webClient.Headers.Add(collection);

                // Serialize the message
                var response = webClient.UploadData(Configuration.GetServiceMessagesAddress(queueName), "POST", content);
                string responseString = Encoding.UTF8.GetString(response);
            }

            return true;
        }        
    }
}
