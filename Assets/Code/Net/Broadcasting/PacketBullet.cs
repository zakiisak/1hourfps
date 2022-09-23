using LiteNetLib.Utils;
using UnityEngine;

namespace Assets.Code.Net.Broadcasting
{
    public class PacketBullet : BroadcastPacket
    {
        public static readonly byte PacketType = 0x02;
        public long ID { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public Vector3 Velocity { get; set; }

        public PacketBullet(NetDataReader reader)
        {
            ID = reader.GetLong();
            Position = new Vector3(reader.GetFloat(), reader.GetFloat(), reader.GetFloat());
            Rotation = new Quaternion(reader.GetFloat(), reader.GetFloat(), reader.GetFloat(), reader.GetFloat());
            Velocity = new Vector3(reader.GetFloat(), reader.GetFloat(), reader.GetFloat());
        }

        public PacketBullet(long id, Vector3 position, Quaternion rotation, Vector3 velocity)
        {
            ID = id;
            Position = position;
            Rotation = rotation;
            Velocity = velocity;
        }

        public override void FillBuffer(NetDataWriter writer)
        {
            writer.Put(PacketType);
            writer.Put(ID);
            writer.Put(Position.x);
            writer.Put(Position.y);
            writer.Put(Position.z);

            writer.Put(Rotation.x);
            writer.Put(Rotation.y);
            writer.Put(Rotation.z);
            writer.Put(Rotation.w);

            writer.Put(Velocity.x);
            writer.Put(Velocity.y);
            writer.Put(Velocity.z);
        }

    }
}
