using System.IO;
using LiteNetLib.Utils;

namespace Assets.Code.Net.Broadcasting
{
    public abstract class BroadcastPacket
    {
        public abstract void FillBuffer(NetDataWriter writer);
    }
}
