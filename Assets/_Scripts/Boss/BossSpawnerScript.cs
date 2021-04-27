using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawnerScript : MonoBehaviour
{
    public GameObject bossDoor, boss, bossInstantiated, bossSpawnPoint, enemieSpawner, cam, enemiesDelimiter, bossHealth, bossMusic;
    public string chessContent;
    bool playerReached = false;
    public LayerMask PlayerLayer;
    public PlayerManegerScript playerManegerScript;
    
    void Start()
    {
        foreach (GameObject player in playerManegerScript.obtainedPlayers)
            if (player != null && player.name == chessContent) Destroy(gameObject);
    }
    void Update()
    {
        if (!playerReached)
        {
            Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(transform.position, 1f, PlayerLayer);
            foreach (Collider2D player in hitPlayer)
            {
                bossMusic.GetComponent<AudioSource>().Play();
                bossMusic.GetComponent<FadeMusic>().StartFadeIn(bossMusic.GetComponent<AudioSource>(), 1f);
                bossHealth.SetActive(true);
                GetComponent<AudioSource>().Play();
                StartCoroutine(Wait(4f));
                cam.GetComponent<CameraShake>().ShakeCR(2f, .02f);
                playerReached = true;
                bossDoor.GetComponent<BossDoor>().animator.SetBool("Enter", true);
                bossInstantiated = (GameObject)Instantiate(boss, bossSpawnPoint.transform.position, Quaternion.identity, enemieSpawner.transform);
                bossInstantiated.GetComponent<RangedBoss>().enemieSpawner = enemieSpawner;
                bossInstantiated.GetComponent<RangedBoss>().enemiesDelimiter = enemiesDelimiter;
            }
        }

        else
        {
            if (bossInstantiated == null)
            {
                bossDoor.GetComponent<BossDoor>().animator.SetBool("Exit", true);
                bossHealth.SetActive(false);
                bossMusic.GetComponent<FadeMusic>().StartFadeOut(bossMusic.GetComponent<AudioSource>(), 3f);
                Destroy(gameObject);
            }
        }
    }

    public virtual IEnumerator Wait(float time)
    {
        yield return new WaitForSeconds(time/1.5f);
        float pauseEndTime = Time.realtimeSinceStartup + time/1.5f;
        while (Time.realtimeSinceStartup < pauseEndTime) 
        {
            GetComponent<AudioSource>().volume -= Time.deltaTime * 3f;
            yield return 0;
        }

        GetComponent<AudioSource>().Stop();
    }

}