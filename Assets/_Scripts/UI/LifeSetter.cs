using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeSetter : MonoBehaviour
{
    public Image[] hearts;

    public PlayerManegerScript ps;
    void Start()
    {
        hearts= new Image[transform.childCount];
        ps.playerSwitch += UpdateLifeUI;
        for (int i = 0; i < transform.childCount; i++)
            hearts[i] = transform.GetChild(i).GetComponent<Image>();
    }

    public void UpdateLifeUI()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i<ps.newPlayer.GetComponent<Player>().health){
                hearts[i].enabled = true;
            }else{
                hearts[i].enabled = false;
            }
        }
    }
}
