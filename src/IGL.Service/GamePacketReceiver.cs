using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.InteropExtensions;
using Microsoft.Extensions.Configuration;

namespace IGL.Service
{
    public abstract class GamePacketReceiver
    {
        protected readonly IConfiguration Configuration;
        protected readonly ServiceBusMessagingFactory Factory;

        private QueueClient _client;

        public GamePacketReceiver(IConfiguration configuration, ServiceBusMessagingFactory factory)
        {
            Configuration = configuration;
            Factory = factory;
        }

        public event GamePacketErrorHandler OnListenerError;
        public event GamePacketEventHandler OnGamePacketCompleted;

        public async Task StartListening(string queueName, string session = null)
        {
            if (_client != null && !_client.IsClosedOrClosing)
                throw new ApplicationException(
                    "GamePacketReceiver should be closed before listening is started again.");

            _client = await Factory.GetQueueClientByName(queueName,
                !string.IsNullOrEmpty(session));

            if (string.IsNullOrEmpty(session))
                _client.RegisterMessageHandler(ProcessMessages, new MessageHandlerOptions(ExceptionReceivedHandler)
                {
                    MaxConcurrentCalls = 1,
                    AutoComplete = false
                });
            else
                _client.RegisterSessionHandler(ProcessSessionMessages,
                    new SessionHandlerOptions(ExceptionReceivedHandler)
                    {
                        MaxConcurrentSessions = 1,
                        AutoComplete = false
                    });


            Trace.TraceInformation("IGL.Service.GameEventsListenerTask.RunAsync() ending.");
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            OnListenerError?.Invoke(this, new GamePacketErrorArgs
            {
                Exception = exceptionReceivedEventArgs.Exception,
                Message = "Exception Received Event"
            });
            return Task.CompletedTask;
        }

        private async Task ProcessMessages(Message message, CancellationToken token)
        {
            try
            {
                var packet = message.GetBody<GamePacket>(new DataContractSerializer(typeof(GamePacket)));

                if (packet == null)
                {
                    OnListenerError?.Invoke(this,
                        new GamePacketErrorArgs { Message = "Message body was not serialized as a GamePacket." });
                    await _client.DeadLetterAsync(message.SystemProperties.LockToken,
                        "Message body was not serialized as a GamePacket.");
                }
                else
                {
                    if (HandleGamePacket(packet))
                    {
                        OnGamePacketCompleted?.Invoke(this, new GamePacketArgs { GamePacket = packet });
                        await _client.CompleteAsync(message.SystemProperties.LockToken);
                    }
                    else
                    {
                        OnListenerError?.Invoke(this,
                            new GamePacketErrorArgs
                            {
                                GameEvent = packet,
                                Message = "Message was not handled completely."
                            });

                        await _client.DeadLetterAsync(message.SystemProperties.LockToken,
                            "Message was not handled completely.");
                    }
                }
            }
            catch (Exception e)
            {
                OnListenerError?.Invoke(this, new GamePacketErrorArgs
                {
                    Exception = e,
                    Message = "GamePacketReceiver failed to ProcessMessage"
                });
            }            
        }

        private async Task ProcessSessionMessages(IMessageSession session, Message message, CancellationToken token)
        {
            var packet = message.GetBody<GamePacket>(new DataContractSerializer(typeof(GamePacket)));

            if (HandleGamePacket(packet))
                OnGamePacketCompleted?.Invoke(this, new GamePacketArgs {GamePacket = packet});

            await session.CompleteAsync(message.SystemProperties.LockToken);
        }

        protected abstract bool HandleGamePacket(GamePacket packet);
    }
}