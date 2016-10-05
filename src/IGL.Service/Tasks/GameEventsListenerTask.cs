using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using IGL.Data;
using IGL.Service.Common;
using IGL.Data.Repositories;

namespace IGL.Service.Tasks
{    
    public class GameEventsListenerTask : IRoleTask
    {
        /// <summary>
        /// Event called when a GameEvent has been successfully processed.
        /// </summary>
        public event EventHandler<GamePacketArgs> OnGamePacketCompleted;
        /// <summary>
        /// Event called when a failure has happened processing a GameEvent.
        /// </summary>
        public event EventHandler<GamePacketErrorArgs> OnGamePacketError;
                
        /// <summary>
        /// RoleTask to read a gamepacket, save to table storage and broadcast the event
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public async Task ProcessReceivedMessage(Task<IEnumerable<BrokeredMessage>> task)
        {
            var hubIGL = ServiceBusMessagingFactory.GetIGLEventHubClient();
            int errors = 0;

            foreach (var message in task.Result)
            {
                GamePacket packet = null;

                try
                {
                    Trace.TraceInformation("IGL.Service.GameEventsListenerTask.RunAsync() processing message {0}", message.SequenceNumber);

                    if (!message.Properties.ContainsKey(GamePacket.VERSION))
                        throw new ApplicationException(string.Format("IGL.Service.GameEventsListenerTask.RunAsync() message {0} does not have a valid {1} property.", message.SequenceNumber, GamePacket.VERSION));

                    // TODO: this is where the different versions of the message can be handled
                    var version = message.Properties[GamePacket.VERSION];

                    packet = message.GetBody<GamePacket>(new DataContractSerializer(typeof(GamePacket)));

                    // save - the packet is valid 
                    var tableGamePacket = new GamePacketRepository(packet.GameId);
                    tableGamePacket.AddGamePacket(packet);

                    await message.CompleteAsync();

                    // alert any listeners
                    OnGamePacketCompleted?.Invoke(null, new GamePacketArgs { GamePacket = packet });

                    await hubIGL.SendAsync(SuccessEvent());                    

                    Trace.TraceInformation("IGL.Service.GameEventsListenerTask.RunAsync() processed message {0}", message.SequenceNumber);
                }
                catch (Exception ex)
                {
                    errors++;
                    message.DeadLetter(ex.Message, ex.GetFullMessage());

                    OnGamePacketError?.Invoke(null, new GamePacketErrorArgs
                    {
                        Message = string.Format("Message {0} added to deadletter queue at UTC {1}.", message.SequenceNumber, DateTime.UtcNow.ToString()),
                        GameEvent = packet,
                        Exception = ex
                    });

                    hubIGL.Send(FailureEvent());

                    Trace.TraceError(string.Format("IGL.Service.GameEventsListenerTask.RunAsync() failed with {0}", ex.GetFullMessage()));
                }
            }

            Trace.TraceInformation(string.Format("IGL.Service.GameEventsListenerTask.ProcessReceivedMessage() processed {0} packets with {1} errors.", task.Result, errors));
        }

        private static EventData SuccessEvent()
        {
            var content = XmlSerializerHelper.Serialize<ProcessingEvent>(new ProcessingEvent { ProcessedDateTime = DateTime.Now, EventType = ProcessEventType.GameEvent, HasFailed = false });

            return new EventData(Encoding.UTF8.GetBytes(content));
        }

        private static EventData FailureEvent()
        {
            var content = XmlSerializerHelper.Serialize<ProcessingEvent>(new ProcessingEvent { ProcessedDateTime = DateTime.Now, EventType = ProcessEventType.GameEvent, HasFailed = true });

            return new EventData(Encoding.UTF8.GetBytes(content));
        }

        private static void LogErrors(object sender, ExceptionReceivedEventArgs e)
        {
            if (e.Exception != null)
            {
                Trace.WriteLine("Error: " + e.Exception.GetFullMessage());
            }
        }
       
    }
}
