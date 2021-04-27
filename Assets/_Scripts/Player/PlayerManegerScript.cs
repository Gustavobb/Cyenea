using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManegerScript : MonoBehaviour
{
    public GameObject endMaps;
    public Input_Joystick IJ;
    public GameObject[] players;
    public int activePlayer = 0;

    public delegate void PlayerSwitch();
    public PlayerSwitch playerSwitch;
    public delegate void PlayerAddUI();
    public PlayerSwitch playerAddUI;
    public GameObject healthBar, explosion, manaBar, cooldownBarOgre, cooldownBarTree, cooldownBarTreeAttack, hpUp;
    [HideInInspector]
    public GameObject newPlayer;
    Player p;
    [HideInInspector]
    public bool dJ = false;
    PlayerData data;
    public string[] names = new string[6] { "Paladin", "Goblin2", "Efreet2", "Treatnt2", "Vampire1", "Ogre1" };

    public List<GameObject> obtainedPlayers = new List<GameObject>();
    [HideInInspector]
    public float coolDownSpecialTree, coolDownAttackOgre, coolDownMaxSpecialTree, coolDownMaxAttackOgre, coolDownAttackTree, coolDownMaxAttackTree;
    void Awake()
    {
        coolDownMaxAttackTree = 5f;
        coolDownMaxSpecialTree = coolDownMaxAttackOgre = 10f;
        coolDownSpecialTree = coolDownAttackOgre = coolDownAttackTree = 0f;
        if (!SaveSystem.FileExists()) SaveSystem.CreateFile();

        PlayerData data = SaveSystem.LoadPlayer();
        activePlayer = data.activePlayerIdData;

        for (int i = 0; i < players.Length; i++)
            obtainedPlayers.Add(null);

        for (int i = 0; i < data.obtainedPlayersNameData.Length; i++)
            for (int j = 0; j < players.Length; j++)
                if (names[j] == data.obtainedPlayersNameData[i])
                    obtainedPlayers[j] = players[j];

        Vector2 playerSpawnPoint = new Vector2(-8.42f, -7.04f);
        for (int i = 0; i < endMaps.transform.childCount; i++)
            if (endMaps.transform.GetChild(i).GetComponent<EndMapScript>().id == data.currentSpawnPointIdData) playerSpawnPoint = endMaps.transform.GetChild(i).transform.GetChild(0).transform.position;

        SpawnPlayer(players[activePlayer], playerSpawnPoint, data.healthData, data.maxHealthData, data.manaData);
        foreach (GameObject player in obtainedPlayers){
            if (player != null && player.name == "Ogre1") {
                cooldownBarOgre.SetActive(true);
            }
            else if (player != null && player.name == "Treatnt2"){
                cooldownBarTree.SetActive(true);
                cooldownBarTreeAttack.SetActive(true);
            }
        }
    }

    void Update()
    {
        dJ = p.isOnFloor;
        bool cR = IJ.changeRight;
        bool cL = IJ.changeLeft;

        IJ.changeRight = false;
        IJ.changeLeft = false;

        coolDownAttackOgre += Time.deltaTime;
        coolDownSpecialTree += Time.deltaTime;
        coolDownAttackTree += Time.deltaTime;

        if(coolDownAttackOgre > coolDownMaxAttackOgre) cooldownBarOgre.SetActive(false);
        else cooldownBarOgre.GetComponent<HealthBar>().SetSlider(coolDownAttackOgre);

        if(coolDownSpecialTree > coolDownMaxSpecialTree) cooldownBarTree.SetActive(false);
        else cooldownBarTree.GetComponent<HealthBar>().SetSlider(coolDownSpecialTree);
        
        if(coolDownAttackTree > coolDownMaxAttackTree) cooldownBarTreeAttack.SetActive(false);
        else cooldownBarTreeAttack.GetComponent<HealthBar>().SetSlider(coolDownAttackTree);

        if ((cR || cL) && p.stateMachine.CurrentState != p.deathState && p.canBeHit)
        {
            GetComponent<AudioSource>().pitch = Random.Range(.8f, 1.4f);
            GetComponent<AudioSource>().Play();
            IJ.dash = false;
            GameObject player = GameObject.FindWithTag("Player");
            Player p1 = player.GetComponent<Player>();
            p1.SwitchToIdle();
            Vector3 pos = player.transform.position;
            bool facingDir = p1.spriteRenderer.flipX;
            float velocityXSmoothing = p1.velocityXSmoothing;
            float targetVelocityX = p1.targetVelocityX;
            int health = p1.health;
            int maxHealth = p1.maxHealth;
            Vector2 velocity = p1.velocity;
            Destroy(player);
            
            activePlayer += cR ? 1 : -1;
            if (activePlayer < 0) activePlayer = obtainedPlayers.Count - 1;
            while (activePlayer < obtainedPlayers.Count && activePlayer >= 0 && obtainedPlayers[activePlayer] == null) activePlayer += cR ? 1 : -1;
            
            if (activePlayer >= obtainedPlayers.Count) activePlayer = 0;
            else if (activePlayer < 0) activePlayer = obtainedPlayers.Count - 1;
            
            GameObject explos = (GameObject) Instantiate(explosion, player.transform.position + new Vector3(0, -.2f, 0), Quaternion.identity);
            explos.GetComponent<Animator>().SetTrigger(obtainedPlayers[activePlayer].name);
            newPlayer = (GameObject) Instantiate(obtainedPlayers[activePlayer], pos, Quaternion.identity);
            p = newPlayer.GetComponent<Player>();
            p.IJ = IJ;
            p.targetVelocityX = targetVelocityX;
            p.velocityXSmoothing = velocityXSmoothing;
            p.velocity = new Vector2(velocity.x, 0);
            p.spriteRenderer.flipX = facingDir;
            p.health = health;
            p.maxHealth = maxHealth;

            int dir = facingDir ? -1 : 1;
            p.attackPoint.localPosition = new Vector3(dir * p.attackPoint.localPosition.x, p.attackPoint.localPosition.y, p.attackPoint.localPosition.z);
            
            PlayerData data = SaveSystem.LoadPlayer();
            SaveSystem.SavePlayer(activePlayer, data.currentSpawnPointIdData, data.healthData, data.maxHealthData, data.manaData, data.obtainedPlayersNameData, data.doorsClosed);

            playerSwitch();

            if (!player.GetComponent<Player>().isOnFloor) 
            {
                // p.velocity = new Vector2(0, 0);
                StartCoroutine(Pause(.25f));
            }
        }
        healthBar.GetComponent<HealthBar>().SetHealth(GameObject.FindWithTag("Player").GetComponent<Player>().health);
    }

    public void AddPlayer(string playerName) 
    {
        foreach (GameObject player in obtainedPlayers)
            if (player != null && player.name == playerName) return;

        for (int j = 0; j < players.Length; j++)
        {
            if (names[j] == playerName)
            {
                obtainedPlayers[j] = players[j];
                
                PlayerData data = SaveSystem.LoadPlayer();
                data.obtainedPlayersNameData = new string[obtainedPlayers.Count];
                
                for (int i = 0; i < obtainedPlayers.Count; i++)
                    if (obtainedPlayers[i] != null) data.obtainedPlayersNameData[i] = obtainedPlayers[i].name;

                SaveSystem.SavePlayer(data.activePlayerIdData, data.currentSpawnPointIdData, data.healthData, data.maxHealthData, data.manaData, data.obtainedPlayersNameData, data.doorsClosed);
            }
        }
        coolDownAttackOgre = coolDownSpecialTree = 10f;
        playerAddUI();
    }

    void SpawnPlayer(GameObject activePlayerGameObject, Vector2 spawnPoint, int health, int maxHealth, int mana)
    {
        newPlayer = (GameObject) Instantiate(activePlayerGameObject, spawnPoint, Quaternion.identity);
        p = newPlayer.GetComponent<Player>();
        p.IJ = IJ;
        p.health = health;
        p.maxHealth = maxHealth;
        healthBar.GetComponent<HealthBar>().SetHealth(health);
        healthBar.GetComponent<HealthBar>().SetMaxHealth(maxHealth);
        manaBar.GetComponent<ManaBar>().SetMana(mana);
    }

    public void checkMana(){
        PlayerData data = SaveSystem.LoadPlayer();
        if(data.manaData >= 20){
            p.maxHealth += 1;
            p.health += 1;
            healthBar.GetComponent<HealthBar>().SetMaxHealth(p.maxHealth);
            SaveSystem.SavePlayer(data.activePlayerIdData, data.currentSpawnPointIdData, newPlayer.GetComponent<Player>().health, newPlayer.GetComponent<Player>().maxHealth, 0, data.obtainedPlayersNameData, data.doorsClosed);
            hpUp.GetComponent<Animator>().SetTrigger("Anim");
        }
        data = SaveSystem.LoadPlayer();
        manaBar.GetComponent<ManaBar>().SetMana(data.manaData);
    }

    #region Coroutine
    public IEnumerator Pause(float time)
    {
        Time.timeScale = 0.3f;
        Time.fixedDeltaTime = Time.timeScale * .02f;
        float pauseEndTime = Time.realtimeSinceStartup + time;
        while (Time.realtimeSinceStartup < pauseEndTime) yield return 0;
        Time.timeScale = 1;
        Time.fixedDeltaTime = Time.timeScale * .02f;
    }
    #endregion
}