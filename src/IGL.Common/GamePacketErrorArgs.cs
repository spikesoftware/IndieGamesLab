using System;

namespace IGL
{
    public class GamePacketErrorArgs : EventArgs
    {
        public GamePacket GameEvent { get; set; }
        public Exception Exception { get; set; }
        public string Message { get; set; }
    }

    public delegate void GamePacketErrorHandler(object sender, GamePacketErrorArgs e);
}