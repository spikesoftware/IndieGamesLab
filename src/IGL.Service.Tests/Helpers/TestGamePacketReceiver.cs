using Microsoft.Extensions.Configuration;

namespace IGL.Service.Tests.Helpers
{
    internal class TestGamePacketReceiver : GamePacketReceiver
    {
        public TestGamePacketReceiver(IConfiguration configuration, ServiceBusMessagingFactory factory) : base(
            configuration, factory)
        {
        }

        protected override bool HandleGamePacket(GamePacket packet)
        {
            return true;
        }
    }
}