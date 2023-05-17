using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceFieldScript : MonoBehaviour
{

    public float shrinkWaitTime, shrinkAmount, shrinkDur, minShrinkAmount, lastShrinkTime, targetDiameter, lastDamageTime;
    public int playerDamage;
    public bool shrinking;

    // Start is called before the first frame update
    void Start()
    {
        lastShrinkTime = Time.time;
        targetDiameter = transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        checkPlayers();
        if (shrinking)
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.one * targetDiameter, (shrinkAmount / shrinkDur) * Time.deltaTime);

            if (transform.localScale.x == targetDiameter)
            {
                shrinking = false;
            }
        }
        else
        {
            if (Time.time - lastShrinkTime >= shrinkWaitTime && transform.localScale.x > minShrinkAmount)
            {
                shrink();
            }
        }
        
    }

    public void shrink()
    {
        shrinking = true;
        if (transform.localScale.x - shrinkAmount > minShrinkAmount)
        {
            targetDiameter -= shrinkAmount;
        }
        else
        {
            targetDiameter = minShrinkAmount;
        }
        lastShrinkTime = Time.time + shrinkDur;
    }

    void checkPlayers()
    {
        if (Time.time - lastDamageTime > 1.0f)
        {
            lastDamageTime = Time.time;
            foreach(PlayerController player in GameManager.instance.players)
            {
                if (player.isDead || !player)
                {
                    continue;
                }
                else
                {
                    if (Vector3.Distance(Vector3.zero, player.transform.position) >= transform.localScale.x)
                    {
                        player.photonView.RPC("takeDamage", player.photonPlayer, 0, playerDamage);
                    }
                }
            }
        }
    }
}
