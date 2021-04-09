using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchUI : MonoBehaviour
{
    public SwitchManegerScript sws;
    public Image[] cubos;
    void Start()
    {
        sws.playerSwitch += SwitchUIChar;
        SwitchUIChar();
    }

    public void SwitchUIChar()
    {
        for (int i = 0; i < cubos.Length; i++)
        {
            if (i==sws.activePlayer){
                cubos[i].enabled = true;
            }else{
                cubos[i].enabled = false;
            }
        }
    }
}
