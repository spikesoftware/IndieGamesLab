using IGL.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;

namespace IGL.Client
{
    public class ServiceBusWriter : ServiceBusBase
    {
        internal static readonly string correlation = Guid.NewGuid().ToString().Replace("-", "");        
        static int _packet = 0;
        
        private static readonly object _syncRoot = new Object();

        public static event EventHandler<ErrorEventArgs> OnSubmitError;
        public static event EventHandler OnSubmitSuccess;

        public static bool SubmitGameEvent(string queueName, int eventId, GameEvent gameevent, KeyValuePair<string, string>[] properties = null, string sessionId = null)
        {
            if (Token == null)
                return false;

            GamePacket packet;

            lock (_syncRoot)
            {
                packet = new GamePacket
                {                    
                    GameId = CommonConfiguration.Instance.GameId,
                    PlayerId = CommonConfiguration.Instance.PlayerId,
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
                webClient.Headers[HttpRequestHeader.Authorization] = Token;

                // add the properties
                var collection = new NameValueCollection();

                if (properties != null)
                {
                    foreach (var property in properties)
                        collection.Add(property.Key, property.Value);
                }

                collection.Add(GamePacket.VERSION, GamePacket.Namespace);

                webClient.Headers.Add(collection);

                webClient.UploadDataCompleted += WebClient_UploadDataCompleted;
                webClient.UploadDataAsync(new Uri(CommonConfiguration.Instance.BackboneConfiguration.GetServiceMessagesAddress(queueName)), "POST", content);                
            }

            return true;
        }

        private static void WebClient_UploadDataCompleted(object sender, UploadDataCompletedEventArgs e)
        {
            try
            {
                string responseString = Encoding.UTF8.GetString(e.Result);

                OnSubmitSuccess?.Invoke(sender, new EventArgs());
            }
            catch(Exception ex)
            {
                OnSubmitError?.Invoke(sender, new System.IO.ErrorEventArgs(ex));
            }

        }
    }
}
