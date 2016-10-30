using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IGL.Service
{
    public interface IRoleTask
    {
        /// <summary>
        /// Event called when a GameEvent has been successfully processed.
        /// </summary>
        event EventHandler<GamePacketArgs> OnGamePacketCompleted;
        /// <summary>
        /// Event called when a failure has happened processing a GameEvent.
        /// </summary>
        event EventHandler<GamePacketErrorArgs> OnGamePacketError;

        Task ProcessReceivedMessage(Task<IEnumerable<BrokeredMessage>> task);
    }
}
