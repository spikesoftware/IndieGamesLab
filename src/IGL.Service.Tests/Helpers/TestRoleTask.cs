using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using System.Runtime.Serialization;

namespace IGL.Service.Tests.Helpers
{
    class TestRoleTask : Service.IRoleTask
    {
        public event EventHandler<GamePacketArgs> OnGamePacketCompleted;
        public event EventHandler<GamePacketErrorArgs> OnGamePacketError;

        public async Task ProcessReceivedMessage(Task<IEnumerable<BrokeredMessage>> task)
        {
            foreach (var message in task.Result)
            {
                GamePacket packet = null;

                try
                {
                    packet = message.GetBody<GamePacket>(new DataContractSerializer(typeof(GamePacket)));

                    OnGamePacketCompleted(this, new GamePacketArgs() { GamePacket = packet });
                }
                catch (Exception ex)
                {
                    OnGamePacketError(this, new GamePacketErrorArgs() { Exception = ex });
                }
            }
        }
    }
}
