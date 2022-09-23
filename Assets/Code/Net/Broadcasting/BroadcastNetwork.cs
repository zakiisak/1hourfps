using System.Net;
using System.Net.Sockets;

namespace Assets.Code.Net.Broadcasting
{
    public class BroadcastNetwork
    {
        public static readonly int PORT = 7882;
        public static readonly int ALT_PORT = 7883;
        public static long UniqueOneTimeId = System.DateTime.Now.Ticks;
        public static string LocalIP { get; private set; }

        public static BroadcastReceiver BroadcastReceiver { get; private set; }
        public static Broadcaster Broadcaster { get; private set; }

        public static bool IsReceiverStarted {
            get {
                return BroadcastReceiver != null && BroadcastReceiver.Started;
            }
        }

        private static void DetermineLocalIp()
        {
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                //Just connect to some stable server
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                LocalIP = endPoint.Address.ToString();
            }
        }
        public static void Init()
        {
            DetermineLocalIp();
            Broadcaster = new Broadcaster();
        }

        public static void Update() {
            Broadcaster.Update();
            if(BroadcastReceiver != null)
                BroadcastReceiver.Update();
        }

        public static bool StartReceiver(OnReceiveBroadcastPacket packetListener)
        {
            if(BroadcastReceiver != null)
            {
                StopReceiver();
            }
            BroadcastReceiver = new BroadcastReceiver(packetListener);
            return BroadcastReceiver.Start();
        }

        public static void StopReceiver()
        {
            if(BroadcastReceiver != null)
            {
                BroadcastReceiver.Stop();
                BroadcastReceiver = null;

            }
        }

    }
}
