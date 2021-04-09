using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchManegerScript : MonoBehaviour
{
    public Input_Joystick IJ;
    public GameObject[] players;
    public int activePlayer = 0;

    public delegate void PlayerSwitch();
    public PlayerSwitch playerSwitch;
    public GameObject newPlayer;

    void Update()
    {
        if (IJ.changeRight || IJ.changeLeft)
        {
            IJ.changeRight = false;
            IJ.changeLeft = false;
            GameObject player = GameObject.FindWithTag("Player");
            player.GetComponent<Player>().SwitchToIdle();
            Vector3 pos = player.transform.position;
            bool facingDir = player.GetComponent<Player>().spriteRenderer.flipX;
            float velocityXSmoothing = player.GetComponent<Player>().velocityXSmoothing;
            float targetVelocityX = player.GetComponent<Player>().targetVelocityX;
            int health = player.GetComponent<Player>().health;
            Vector2 velocity = player.GetComponent<Player>().velocity;
            Destroy(player);
        
            activePlayer += IJ.changeRight ? 1 : -1;
            if (activePlayer > players.Length) activePlayer = 0;
            else if (activePlayer < 0) activePlayer = players.Length - 1;
            
            newPlayer = (GameObject) Instantiate(players[activePlayer], pos, Quaternion.identity);
            newPlayer.GetComponent<Player>().IJ = IJ;
            newPlayer.GetComponent<Player>().targetVelocityX = targetVelocityX;
            newPlayer.GetComponent<Player>().velocityXSmoothing = velocityXSmoothing;
            newPlayer.GetComponent<Player>().velocity = new Vector2(velocity.x, velocity.y);
            newPlayer.GetComponent<Player>().spriteRenderer.flipX = facingDir;
            newPlayer.GetComponent<Player>().health = health;
            int dir = facingDir ? -1 : 1;
            newPlayer.GetComponent<Player>().attackPoint.localPosition = new Vector3(dir * newPlayer.GetComponent<Player>().attackPoint.localPosition.x, newPlayer.GetComponent<Player>().attackPoint.localPosition.y, newPlayer.GetComponent<Player>().attackPoint.localPosition.z);
            
            playerSwitch();
            if (!player.GetComponent<Player>().isOnFloor) StartCoroutine(Pause(.2f));
        }
    }

    #region Coroutine
    public IEnumerator Pause(float time)
    {
        Time.timeScale = 0.7f;
        float pauseEndTime = Time.realtimeSinceStartup + time;
        while (Time.realtimeSinceStartup < pauseEndTime) yield return 0;
        Time.timeScale = 1;
    }
    #endregion
}