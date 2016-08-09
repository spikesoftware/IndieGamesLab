using System;

namespace IGL.Data.ServiceEntities
{
    public class GamePacketEntity : BaseTableEntity
    {
        public int GameID { get; set; }

        public Guid Correlation { get; set; }

        public long PacketNumber { get; set; }

        public DateTime PacketCreatedUTCDate { get; set; }

        public string Content { get; set; }

        public static implicit operator GamePacket(GamePacketEntity from)
        {
            return AutoMapper.Mapper.Map<GamePacketEntity, GamePacket>(from);
        }

        public static implicit operator GamePacketEntity(GamePacket from)
        {
            return AutoMapper.Mapper.Map<GamePacket, GamePacketEntity>(from);
        }
    }
}
