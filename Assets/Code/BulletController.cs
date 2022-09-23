using Assets.Code.Net.Broadcasting;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    private static bool fireworkFeature = false;
    public static Dictionary<long, BulletController> Bullets = new Dictionary<long, BulletController>();
    public long Id { get; private set; }
    public bool Remote { get; set; }
    private Rigidbody body;


    private float life = 8.0f;

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Bullet")
        {
            //Maybe add explosion
        }
    }

    public void UpdateFromPacket(PacketBullet bullet)
    {
        this.body.velocity = bullet.Velocity;
        this.body.position = bullet.Position;
        this.transform.position = bullet.Position;
    }

    void Start()
    {
        Id = DateTime.Now.Ticks;
        body = GetComponent<Rigidbody>();

        if (fireworkFeature == false)
        {
            if (Bullets.ContainsKey(Id) == false)
                BulletController.Bullets.Add(Id, this);
            else
            {
                GameObject.Destroy(gameObject);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Remote == false)
            BroadcastNetwork.Broadcaster.SendBullet(this);

        life -= Time.deltaTime;
        if(life <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if(Bullets.ContainsKey(Id))
        {
            Bullets.Remove(Id);
        }
    }

    public Rigidbody GetRigidbody()
    {
        return body;
    }
}
