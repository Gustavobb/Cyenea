using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public LayerMask PlayerLayer;
    public string playerName;
    public PlayerManegerScript playerManegerScript;

    public string text;

    public AudioClip chestClip;
    public string text2;

    private void Start() {
        for (int i = 0; i < playerManegerScript.obtainedPlayers.Count; i++)
        {
            if (playerManegerScript.obtainedPlayers[i] != null && playerName == playerManegerScript.obtainedPlayers[i].name)
            {
                Debug.Log("pp");
                Destroy(gameObject);
            }
        }
    }

    void Update()
    {
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(transform.position, 1f, PlayerLayer);
        foreach (Collider2D player in hitPlayer)
        {
            AudioSource.PlayClipAtPoint(chestClip, transform.position);
            playerManegerScript.AddPlayer(playerName);
            GameObject.Find("SimplePanel").GetComponent<PanelScript>().setText(text, text2);
            Destroy(gameObject);
        }
    }
}
