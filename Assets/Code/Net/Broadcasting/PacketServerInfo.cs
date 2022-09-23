using System.IO;
using LiteNetLib.Utils;

namespace Assets.Code.Net.Broadcasting
{
    public class PacketServerInfo : BroadcastPacket
    {
        public static readonly byte PacketType = 0x02;

        public int PlayerCount { get; set; }
        public string MapName { get; set; }
        public string IP { get; set; }

        public PacketServerInfo(NetDataReader reader)
        {
            PlayerCount = reader.GetInt();
            MapName = reader.GetString();
            IP = reader.GetString();
        }

        
        public PacketServerInfo(int playerCount, string mapName, string ip)
        {
            PlayerCount = playerCount;
            MapName = mapName;
            IP = ip;
        }
        
        public override void FillBuffer(NetDataWriter writer)
        {
            writer.Put((byte)PacketType);
            writer.Put(PlayerCount);
            writer.Put(MapName);
            writer.Put(IP);
        }

    }
}
