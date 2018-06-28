using System;

namespace IGL
{
    public class GamePacketArgs : EventArgs
    {
        public GamePacket GamePacket { get; set; }
    }

    public delegate void GamePacketEventHandler(object sender, GamePacketArgs e);
}