using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPun
{
    [Header("components")]
    public Rigidbody rig;
    public Camera cam;
    public AudioSource audio;
    public int punId;
    public Player photonPlayer;
    public Color skin;
    public MeshRenderer mr;

    [Header("movement stats")]
    public float moveSpeed;
    public float jumpForce;

    [Header("player stats")]
    public int health;
    public int maxHp;
    public int lives;
    public int ammo;
    public int score;
    public bool isDead;

    [Header("End game stats")]
    public int damageTaken;
    public int kills;
    public int damageGiven;
    public int shotsFired;
    public int shotsHit;
    public int itemsCollected;

    [Header("Weapons")]
    public bool has_pistol_1;
    public bool has_pistol_2;
    public bool has_assault_1;
    public bool has_shotgun_1;
    public bool has_smg_1;

    public int gun_index;
    public Transform[] gunList;
    public PlayerWeapon selectedWeapon;

    [Header("Others")]
    private int currentAttackerID;
    private bool isFlashing;


    private void Awake()
    {
        rig = GetComponent<Rigidbody>();
        audio = GetComponent<AudioSource>();
        cam = GetComponentInChildren<Camera>();
    }

    // Start is called before the first frame update
    void Start()
    {
        gun_index = 0;
        swapGun(gun_index);
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        move();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            tryJump();
        }

        

        if (Input.GetMouseButtonDown(0))
        {
            pullTrigger();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ammo -= selectedWeapon.maxClip - selectedWeapon.curClip;

            selectedWeapon.reload();
        }

        gunSwapKeyPress(gun_index);
    }

    [PunRPC]
    public void Initialize(Player player)
    {
        punId = player.ActorNumber;
        photonPlayer = player;
        GameManager.instance.players[punId - 1] = this;

        if (!photonView.IsMine)
        {
            GetComponentInChildren<Camera>().gameObject.SetActive(false);
            rig.isKinematic = true;
        }
        skin = GameManager.instance.playerColors[punId - 1];
        mr.material.color = skin;
    }

    private void move()
    {
        // get inputs
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 dir = (transform.forward * z + transform.right * x) * moveSpeed;
        dir.y = rig.velocity.y;

        rig.velocity = dir;
    }

    private void tryJump()
    {
        Ray ray = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(ray, 1.5f))
        {
            rig.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void pullTrigger()
    {
        selectedWeapon.tryShoot();
    }

    public void heal()
    {

    }

    public void addAmmo()
    {

    }

    [PunRPC]
    public void die()
    {

    }

    [PunRPC]
    public void takeDamage(int attacker, int damage)
    {
        if (isDead)
        {
            return;
        }
        health -= damage;
        currentAttackerID = attacker;

        // flash damage
        photonView.RPC("DamageFlash", RpcTarget.Others);

        // update ui

        // if it killed us
        if (health <= 0)
        {
            photonView.RPC("die", RpcTarget.All);
        }
    }

    [PunRPC]
    public void DamageFlash()
    {
        if (isFlashing)
        {
            return;
        }

        StartCoroutine(damageFlashCoroutine());

        IEnumerator damageFlashCoroutine()
        {
            isFlashing = true;
            for( int i = 0; i <= 3; i++)
            {
                mr.material.color = Color.red;
                yield return new WaitForSeconds(.05f);
                mr.material.color = skin;
            }
            isFlashing = false;
        }
    }

    public void gunSwapKeyPress(int index)
    {
        int new_index = index;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            new_index++;
            if (new_index < 0)
            {
                new_index = gunList.Length - 1;
            }
            if (new_index >= gunList.Length)
            {
                new_index = 0;
            }
        }else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            new_index = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            new_index = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            new_index = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            new_index = 3;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            new_index = 4;
        }
        else
        {
            return;
        }
        swapGun(new_index);
        gun_index = new_index;
    }

    public void swapGun(int index)
    {
        foreach (Transform gun in gunList)
        {
            gun.gameObject.SetActive(false);
        }

        gunList[index].gameObject.SetActive(true);
        selectedWeapon = gunList[index].GetComponent<PlayerWeapon>();
    }

}
