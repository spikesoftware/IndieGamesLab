using System;

namespace IGL
{
    public class GamePacketArgs : EventArgs
    {
        public GamePacket GamePacket { get; set; }
    }

    public delegate void GamePacketEventHandler(Object sender, GamePacketArgs e);    
}
