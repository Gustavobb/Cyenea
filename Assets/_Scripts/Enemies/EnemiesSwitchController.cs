using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesSwitchController : MonoBehaviour
{
    public SwitchManegerScript sws;

    void Start()
    {
        sws.playerSwitch += FindPlayerEnemies;
    }

    public void FindPlayerEnemies()
    {
        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).GetComponent<LandbasedEnemie>().playerTransform = sws.newPlayer.transform;
    }
}
