using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchUI : MonoBehaviour
{
    public PlayerManegerScript ps;
    public Image[] cubos;
    public Image[] faces;
    void Start()
    {
        ps.playerSwitch += SwitchUIChar;
        ps.playerAddUI += PlayerAddUI;
        SwitchUIChar();
        PlayerAddUI();
    }

    public void SwitchUIChar()
    {
        for (int i = 0; i < cubos.Length; i++)
        {
            if (ps.obtainedPlayers[ps.activePlayer] != null && ps.obtainedPlayers[ps.activePlayer].name == ps.names[i]){
                cubos[i].enabled = true;
            } else {
                cubos[i].enabled = false;
            }
        }
    }
    public void PlayerAddUI()
    {
        for (int i = 0; i < faces.Length; i++)
        {
            if (ps.obtainedPlayers[i] != null && ps.obtainedPlayers[i].name == ps.names[i]){
                faces[i].color = Color.white;
            } 
        }
    }
}
