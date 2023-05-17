using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{

    public int dmg;
    public float bulletSpeed;
    public float range;
    public int attackerId;
    public bool isMine;

    private Rigidbody rig;

    // Start is called before the first frame update
    private void Awake()
    {
        rig = GetComponent<Rigidbody>();
    }

    public void initialized(Vector3 dir, int id, bool ismine)
    {
        rig.velocity = dir * bulletSpeed;
        isMine = ismine;
        attackerId = id;
        Destroy(gameObject, range);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && isMine)
        {
            PlayerController otherPlayer = GameManager.instance.getPlayer(other.gameObject);
            if (otherPlayer.punId != attackerId)
            {
                otherPlayer.photonView.RPC("takeDamage", otherPlayer.photonPlayer, attackerId, this.dmg);
            }
        }
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void spawnBullet(Vector3 pos, Vector3 dir)
    {
       // GameObject bulletobj = Instantiate
    }

    public void reload()
    {

    }

    public void reload(int bullets)
    {

    }
}
