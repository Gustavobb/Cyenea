using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesSwitchController : MonoBehaviour
{
    public PlayerManegerScript ps;

    void Start()
    {
        ps.playerSwitch += FindPlayerEnemies;
    }

    public void FindPlayerEnemies()
    {
        for (int i = 0; i < transform.childCount; i++)
        if (transform.GetChild(i).GetComponent<LandbasedEnemie>() == null && transform.GetChild(i).GetComponent<RangedLandbasedEnemie>() == null)
            transform.GetChild(i).GetComponent<FlyingEnemie>().playerTransform = ps.newPlayer.transform;
        else if (transform.GetChild(i).GetComponent<LandbasedEnemie>() == null)
            transform.GetChild(i).GetComponent<RangedLandbasedEnemie>().playerTransform = ps.newPlayer.transform;
        else
            transform.GetChild(i).GetComponent<LandbasedEnemie>().playerTransform = ps.newPlayer.transform;
    }
}
