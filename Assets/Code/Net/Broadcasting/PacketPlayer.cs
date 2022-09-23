using LiteNetLib.Utils;
using UnityEngine;

namespace Assets.Code.Net.Broadcasting
{
    public class PacketPlayer : BroadcastPacket
    {
        public static readonly byte PacketType = 0x01;
        public long ID { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public Vector3 Velocity { get; set; }
        public string Name { get; set; }
        public Color Color { get; set; }
        public bool Dead { get; set; }

        public PacketPlayer(NetDataReader reader)
        {
            ID = reader.GetLong();
            Position = new Vector3(reader.GetFloat(), reader.GetFloat(), reader.GetFloat());
            Rotation = new Quaternion(reader.GetFloat(), reader.GetFloat(), reader.GetFloat(), reader.GetFloat());
            Velocity = new Vector3(reader.GetFloat(), reader.GetFloat(), reader.GetFloat());
            Name = reader.GetString();
            Color = new Color32(reader.GetByte(), reader.GetByte(), reader.GetByte(), 1);
            Dead = reader.GetBool();
        }

        public PacketPlayer(long id, Vector3 position, Quaternion rotation, int sceneIndex, Vector3 velocity, string name, Color color, bool dead)
        {
            ID = id;
            Position = position;
            Rotation = rotation;
            Velocity = velocity;
            Name = name;
            Color = color;
            Dead = dead;
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

            writer.Put(Name);

            Color32 c = Color;
            writer.Put(c.r);
            writer.Put(c.g);
            writer.Put(c.b);

            writer.Put(Dead);
        }

    }
}
