using IGL.Data;
using IGL.Service.Common;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace IGL.Service
{

    public class RoleTaskRunner
    {
        ManualResetEvent CompletedEvent = new ManualResetEvent(false);
        
        /// <summary>
        /// Event called when a failure has happened listening to the GameEvents queue.
        /// </summary>
        public static event EventHandler OnListenerError;

        public static event GamePacketEventHandler OnGamePacketCompleted;

        public static async Task RunAsync<TRoleTask>(CancellationToken cancellationToken, string queueName, int batchSize, TimeSpan serverWaitTime) where TRoleTask : IRoleTask, new()
        {
            QueueClient client = ServiceBusMessagingFactory.GetQueueClientByName(queueName);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var roleTask = new TRoleTask();
                    roleTask.OnGamePacketCompleted += RoleTask_OnGamePacketCompleted;
                    roleTask.OnGamePacketError += RoleTask_OnGamePacketError;                    

                    await client.ReceiveBatchAsync(batchSize, serverWaitTime).ContinueWith(roleTask.ProcessReceivedMessage);
                }
                catch (TimeoutException)
                {
                    Trace.TraceInformation("IGL.Service.GameEventsListenerTask.RunAsync() timeout.  No work to do.");
                }
                catch (Exception ex)
                {
                    OnListenerError?.Invoke(null, EventArgs.Empty);
                    Trace.TraceError(string.Format("IGL.Service.GameEventsListenerTask.RunAsync() failed with {0}", ex.GetFullMessage()));
                }
            }

            Trace.TraceInformation("IGL.Service.GameEventsListenerTask.RunAsync() ending.");
        }

        private static void RoleTask_OnGamePacketError(object sender, GamePacketErrorArgs e)
        {
            OnListenerError?.Invoke(sender, e);
        }

        private static void RoleTask_OnGamePacketCompleted(object sender, GamePacketArgs e)
        {
            OnGamePacketCompleted?.Invoke(sender, e);
        }
    }
}
