using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerWeapon : MonoBehaviour
{

    [Header("Weapon stats")]
    public string name;
    public int curClip;
    public int maxClip;
    public int index;
    public bool isAuto;


    [Header("shooting stats")]
    public float fireRate;
    public float lastShotTime;

    [Header("prefabs")]
    public GameObject bulletPrefab;
    public GameObject flashPrefab;

    [Header("components")]
    public Animator animator;
    public PlayerController player;
    public Transform muzzlePos;
    public AudioSource audio;
    public AudioClip[] fxClips;

    // Start is called before the first frame update
    private void Awake()
    {
        animator = GetComponent<Animator>();
        audio = GetComponent<AudioSource>();
        player = GetComponentInParent<Transform>().GetComponentInParent<PlayerController>();
        muzzlePos = GameObject.FindWithTag("muzzle").GetComponent<Transform>();
    }

    public void tryShoot()
    {

        if (curClip <= 0 || Time.time - lastShotTime < fireRate)
        {
            return;
        }
        curClip--;
        lastShotTime = Time.time;
        // update ui

        //spawn a bullet
        player.photonView.RPC("spawnBullet", RpcTarget.All, muzzlePos.position, Camera.main.transform.forward, index);
        print("you shot shot the " + name);
        // play sound
        GameObject flash = Instantiate(flashPrefab, muzzlePos.position, Quaternion.identity);
        flash.transform.forward = Camera.main.transform.forward;
        Destroy(flash, .5f);
        // play animation
    }

    public void spawnBullet(Vector3 muzzlePos, Vector3 dir)
    {
        GameObject bulletobj = Instantiate(bulletPrefab, muzzlePos, Quaternion.identity);
        bulletobj.transform.forward = dir;
        BulletScript bullet = bulletobj.GetComponent<BulletScript>();
        bullet.initialized(dir, player.punId, player.photonView.IsMine);
        //audio.PlayoneShot(fxClips[0]);
        // initilize bullet
    }

    public void reload()
    {
        curClip = maxClip;
        //update ui
    }

    public void reload(int bullets)
    {
        curClip += bullets;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
