using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{

    [Header("Weapon stats")]
    public string name;
    public int dmg;
    public int curClip;
    public int maxClip;
    public float bulletSpeed;
    public float range;


    [Header("shooting stats")]
    public float fireRate;
    public float lastShotTime;

    [Header("prefabs")]
    public GameObject bulletPrefab;

    [Header("components")]
    public Animator animator;
    public PlayerController owner;
    public Transform muzzlePos;
    public AudioSource audio;
    public AudioClip[] fxClips;

    // Start is called before the first frame update
    private void Awake()
    {
        animator = GetComponent<Animator>();
        audio = GetComponent<AudioSource>();
        owner = GetComponentInParent<Transform>().GetComponentInParent<PlayerController>();
        muzzlePos = GameObject.FindWithTag("muzzle").GetComponent<Transform>();
    }

    public void tryShoot()
    {
        print("you shot the " + name);

        if (curClip <= 0 || Time.time - lastShotTime < fireRate)
        {
            return;
        }
        curClip--;
        lastShotTime = Time.time;
        // update ui

        //spawn a bullet
        //owner.photonView.RPC("spawnBullet", RpcTarget.All, muzzlePos, Camera.main.transform.forward, range);

        // play sound

        // play animation
    }

    public void spawnBullet(Transform muzzlePos, Vector3 dir, float range)
    {
        GameObject bullet = Instantiate(bulletPrefab, muzzlePos.position, Quaternion.identity);
        bullet.transform.forward = dir;
        // initilize bullet
    }

    public void reload()
    {
        curClip = maxClip;
        //update ui
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
