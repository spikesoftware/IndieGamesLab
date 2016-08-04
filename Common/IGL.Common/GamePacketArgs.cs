using System;

namespace IGL.Game
{
    public class GamePacketArgs : EventArgs
    {
        public GamePacket GamePacket { get; set; }
    }

    public delegate void GamePacketEventHandler(Object sender, GamePacketArgs e);    
}
