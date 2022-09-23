using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Net;
using UnityEngine;

namespace Assets.Code.Net.Broadcasting
{
    public delegate void OnReceiveBroadcastPacket(BroadcastPacket packet);

    public class BroadcastReceiver
    {
        private EventBasedNetListener listener;
        private NetManager receiver;
        private int portUsed;

        public bool Started
        {
            get
            {
                return receiver != null && receiver.IsRunning;
            }
        }

        private OnReceiveBroadcastPacket packetListener;

        public BroadcastReceiver(OnReceiveBroadcastPacket packetListener)
        {
            this.packetListener = packetListener;

            listener = new EventBasedNetListener();

            listener.NetworkReceiveUnconnectedEvent += OnReceive;

            receiver = new NetManager(listener);
            receiver.BroadcastReceiveEnabled = true;
        }

        public void OnReceive(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            BroadcastPacket packet = ParsePacket(reader);
            if (packet != null && packetListener != null)
            {
                packetListener(packet);
            }
            reader.Recycle();
        }

        public bool Start(int port)
        {
            if (receiver.Start(port))
            {
                Debug.Log("Broadcast receiver started");
                return true;
            }
            return false;
        }

        public bool Start()
        {
            if (Start(BroadcastNetwork.PORT) == false)
            {
                Debug.LogError("Failed to start broadcast receiver on normal port (" + BroadcastNetwork.PORT + ")");
                if (Start(BroadcastNetwork.ALT_PORT) == false)
                    Debug.LogError("Failed to start broadcast receiver on alternative port (" + BroadcastNetwork.ALT_PORT + ")");
                else return true;
            }
            else return true;
            return false;
        }

        public void Update()
        {
            if (receiver.IsRunning)
                receiver.PollEvents();
        }

        private BroadcastPacket ParsePacket(NetDataReader reader)
        {
            byte packetIdentifier = reader.GetByte();
            try
            {
                if (packetIdentifier == PacketPlayer.PacketType)
                    return new PacketPlayer(reader);

                else if (packetIdentifier == PacketBullet.PacketType)
                    return new PacketBullet(reader);
            }
            catch (Exception e)
            {
                Debug.LogError("Error occurred while parsing broadcast packet. Packet Identifier: " + packetIdentifier + ". Error: " + e);
            }

            return null;

        }

        public void Stop()
        {
            receiver.Stop();
            Debug.Log("Broadcast receiver stopped");
        }

    }
}
