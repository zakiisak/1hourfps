using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Code.Net.Broadcasting
{
    public class Broadcaster
    {
        private NetManager client;
        private EventBasedNetListener listener;

        public Broadcaster()
        {
            listener = new EventBasedNetListener();
            client = new NetManager(listener);
            client.Start();
        }

        public void Update()
        {
            if (client.IsRunning)
                client.PollEvents();
        }

        public void SendServerInfo(int playerCount, string mapName)
        {
            PacketServerInfo packet = new PacketServerInfo(playerCount, mapName, BroadcastNetwork.LocalIP);
            SendBroadcastPacket(packet);
        }

        public void SendPlayerInfo(PlayerController player)
        {
            PacketPlayer packet = new PacketPlayer(
                player.Id,
                player.transform.position,
                player.transform.rotation,
                SceneManager.GetActiveScene().buildIndex,
                player.GetComponent<Rigidbody>().velocity,
                player.Name,
                player.Color,
                player.Dead
            );
            SendBroadcastPacket(packet);
        }

        public void SendBullet(BulletController bullet)
        {
            PacketBullet packet = new PacketBullet(bullet.Id, bullet.transform.position, bullet.transform.rotation, bullet.GetRigidbody().velocity);
            SendBroadcastPacket(packet);
        }
        public void SendBroadcastPacket(BroadcastPacket packet)
        {
            NetDataWriter writer = new NetDataWriter();
            packet.FillBuffer(writer);
            client.SendBroadcast(writer, BroadcastNetwork.PORT);
            client.SendBroadcast(writer, BroadcastNetwork.ALT_PORT);
        }
    }
}
