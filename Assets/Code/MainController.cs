using Assets.Code.Net.Broadcasting;
using TMPro;
using UnityEngine;

public class MainController : MonoBehaviour
{
    public static MainController Instance;

    public GameObject PlayerPrefab;
    public GameObject BulletPrefab;
    public GameObject ExplosionPrefab;
    public GameObject PlayerNamePrefab;

    public GameObject PlayerCountText;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;

        BroadcastNetwork.Init();
        BroadcastNetwork.StartReceiver(ReceivePacket);
    }

    private int lastAmountOfPlayers = -1;

    // Update is called once per frame
    void Update()
    {
        BroadcastNetwork.Update();

        UpdatePlayerText();
    }

    private void UpdatePlayerText()
    {
        if(lastAmountOfPlayers != PlayerController.Players.Count)
        {
            string text = "Players: " + (PlayerController.Players.Count + 1 /*plus us the local player*/) + " ( " + PlayerController.Instance.Name + " ";

            foreach(PlayerController player in PlayerController.Players.Values)
            {
                text += player.Name + " ";
            }
            text += ")";
            PlayerCountText.GetComponent<TextMeshProUGUI>().text = text;
            lastAmountOfPlayers = PlayerController.Players.Count;
        }    
    }

    private void ReceivePacket(BroadcastPacket packet)
    {
        if(packet is PacketPlayer)
        {
            PacketPlayer player = (PacketPlayer)packet;
            if(player.ID != PlayerController.Instance.Id)
            {
                if (PlayerController.Players.ContainsKey(player.ID))
                {
                    PlayerController.Players[player.ID].UpdateFromPacket(player);
                }
                else
                {
                    SpawnPlayer(player);
                }
            }
        }
        else if(packet is PacketBullet)
        {
            PacketBullet bullet = (PacketBullet)packet;
            if(BulletController.Bullets.ContainsKey(bullet.ID))
            {
                BulletController controller = BulletController.Bullets[bullet.ID];
                if(controller.Remote)
                    BulletController.Bullets[bullet.ID].UpdateFromPacket(bullet);
            }
            else
            {
                SpawnBullet(bullet.Position, bullet.Rotation, bullet.Velocity, true);
            }
        }
    }

    private static void SpawnPlayer(PacketPlayer player)
    {
        GameObject obj = GameObject.Instantiate(Instance.PlayerPrefab, player.Position, player.Rotation);
        PlayerController controller = obj.GetComponent<PlayerController>();
        PlayerController.Players.Add(player.ID, controller);
        obj.GetComponent<MeshRenderer>().material.color = player.Color;
        controller.InitFromPacket(player);


    }

    public static void SpawnBullet(Vector3 position, Quaternion rotation, Vector3 velocity, bool remote)
    {
        GameObject bullet = GameObject.Instantiate(Instance.BulletPrefab, position, rotation);
        bullet.GetComponent<Rigidbody>().velocity = velocity;
        bullet.GetComponent<BulletController>().Remote = remote;
    }
}
