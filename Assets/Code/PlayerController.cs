using Assets.Code.Net.Broadcasting;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static Dictionary<long, PlayerController> Players = new Dictionary<long, PlayerController>();
    public static PlayerController Instance;
    private static GameObject canvasObject;


    private Camera camera;
    private float MovementSpeed = 10.0f;
    private float RotationSpeed = 2.0f;
    private float JumpSpeed = 2.0f;

    private int collisions = 0;

    public long Id { get; private set; }
    public string Name { get; private set; }
    public Color Color { get; private set; }
    public bool Remote { get; private set; }

    private Rigidbody body;
    private MeshRenderer renderer;

    public bool Dead { get; private set; } = false;

    private float lastUpdated;

    private float timeToRespawn;

    private GameObject playerNameObj;

    public void InitFromPacket(PacketPlayer packet)
    {
        lastUpdated = Time.time;
        this.transform.position = packet.Position;
        this.Dead = packet.Dead;
        this.Color = packet.Color;
        this.Name = packet.Name;
        this.Id = packet.ID;
        this.Remote = true;
    }


    public void UpdateFromPacket(PacketPlayer packet)
    {
        lastUpdated = Time.time;
        this.transform.position = packet.Position;
        this.transform.rotation = packet.Rotation;
        if(this.body != null)
        {
            this.body.position = packet.Position;
            this.body.velocity = packet.Velocity;
        }
        if(renderer != null)
        {
            renderer.material.color = packet.Color;
        }
        if(this.Dead == false && packet.Dead)
        {
            Die();
        }
        else if(this.Dead && !packet.Dead)
        {
            Respawn();
        }

        this.Dead = packet.Dead;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name.Contains("Bullet"))
        {
            body.velocity += collision.relativeVelocity * 2.0f;
        }
        collisions++;
    }

    public void OnCollisionExit(Collision collision)
    {
        collisions--;
    }

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
        renderer = GetComponent<MeshRenderer>();
        renderer.material.color = Color;
        if (Remote == false)
        {
            Id = DateTime.Now.Ticks;
            camera = Camera.main;
            Cursor.lockState = CursorLockMode.Locked;
            Name = Environment.UserName;

            System.Random rng = new System.Random();
            Color = Color.HSVToRGB((float) rng.NextDouble(), 1.0f, 1.0f);

            Instance = this;
            canvasObject = GameObject.Find("Canvas");
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
        else
        {
            playerNameObj = Instantiate(MainController.Instance.PlayerNamePrefab, canvasObject.transform);
            playerNameObj.GetComponent<TextMeshProUGUI>().text = Name;
        }
    }

    private void Update()
    {
        if(Remote == false)
        {
            if (Dead == false)
            {
                UpdateMovement();
                UpdateRotation();
                UpdateControls();
                UpdateOutOfBounds();

            }
            else UpdateDead();
            BroadcastNetwork.Broadcaster.SendPlayerInfo(this);
        }
        else
        {
            UpdateRemotePlayer();
        }
    }

    private void UpdateDead()
    {
        timeToRespawn -= Time.deltaTime;
        if(timeToRespawn <= 0)
        {
            Respawn();
        }
    }

    private void UpdateRemotePlayer()
    {
        if (Time.time - lastUpdated > 5.0f)
        {
            Destroy(gameObject);
        }

        Vector3 pos = Instance.camera.WorldToScreenPoint(transform.position);
        playerNameObj.transform.position = new Vector3(pos.x, pos.y, 0);
    }

    public void OnDestroy()
    {
        if(Remote)
        {
            if(Players.ContainsKey(Id))
            {
                Players.Remove(Id);
            }

            Destroy(playerNameObj);
        }
    }

    private void UpdateOutOfBounds()
    {
        if(transform.position.y < -50.0f || transform.position.y > 1000)
        {
            Die();
        }
    }

    private void Die()
    {
        Dead = true;
        timeToRespawn = 3.5f;
        Instantiate(MainController.Instance.ExplosionPrefab, transform.position, Quaternion.identity);
    }

    System.Random rng = new System.Random();

    private void Respawn()
    {
        if(Remote == false)
            transform.position = new Vector3(-24f + (float)rng.NextDouble() * 50f, 10, -24f + (float)rng.NextDouble() * 50f);
        Dead = false;
    }

    private void UpdateMovement()
    {
        float y = Input.GetAxis("Vertical");
        float x = Input.GetAxis("Horizontal");


        if (Mathf.Abs(x) < 0.25f) x = 0;
        else x *= MovementSpeed;
        if (Mathf.Abs(y) < 0.25f) y = 0;
        else y *= MovementSpeed;

        Vector3 movement = camera.transform.TransformDirection(new Vector3(x, 0, y));
        body.velocity = new Vector3(movement.x, body.velocity.y, movement.z);
        camera.transform.position = transform.position;

        if(Input.GetKey(KeyCode.Space))
        {
            if(collisions > 0)//if (Physics.BoxCast(transform.position - new Vector3(0, 1.0f, 0), new Vector3(0.5f, 0.5f, 0.5f), Vector3.down))
            {
                body.velocity = new Vector3(body.velocity.x, Mathf.Abs(body.velocity.y) + JumpSpeed * Time.deltaTime * 70f, body.velocity.z);
            }
        }
    }

    public Rigidbody GetRigidbody()
    {
        return body;
    }

    private void UpdateControls()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            Debug.Log("Fire!");
            FireBullet();
        }
    }

    private void FireBullet()
    {
        MainController.SpawnBullet(transform.position + camera.transform.forward, camera.transform.rotation, camera.transform.forward * 60.0f, false);
    }

    Vector3 rotation = new Vector3();

    private void UpdateRotation()
    {
        float h = RotationSpeed * Input.GetAxis("Mouse X");
        float v = RotationSpeed * Input.GetAxis("Mouse Y");
        rotation.x -= v;
        rotation.y += h;

        camera.transform.rotation = Quaternion.Euler(rotation);
        transform.rotation = camera.transform.rotation;
    }
}
