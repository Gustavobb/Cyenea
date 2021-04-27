using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedBoss : RangedLandbasedEnemie
{
    public int firstSpecialHealth, secondSpecialHealth, thirdSpecialHealth;
    public int maxHealth = 30;
    float cooldown = 0f;
    public int numEnemie = 4;

    public GameObject enemie, enemieSpawner, explosion, healthBoss, enemiesDelimiter;
    public AudioClip puff;
    public float yRand = 3f;

    int lastXDir;
    protected override void Awake()
    {
        stateMachine = new StateMachine();
        attackState = new AttackState(AttackStateCond, () => {atacking = true;}, () => {atacking = false;}, this, gameObject, stateMachine, "Attack");
        idleState = new IdleState(IdleStateCond, () => {}, () => {}, this, gameObject, stateMachine, "Idle");
        runState = new RunState(RunStateCond, () => {}, () => {}, this, gameObject, stateMachine, "Run");
        specialState = new SpecialState(SpecialStateCond, OnEnterSpecialState, () => {coolDownSpecialTimer = 0f; cooldown = 0f; specialCond = false; canBeHit = true; coolDownTimer = 0f;}, this, gameObject, stateMachine, "Special");
        deathState = new DeathState(DeathStateCond, () => {dead = true; GetComponent<BoxCollider2D>().enabled = false; StartCoroutine(Pause(.1f)); StartCoroutine(WaitToDie(6f));}, () => {}, this, gameObject, stateMachine, "Death");
        playerTransform = GameObject.FindWithTag("Player").transform;
        healthBoss = GameObject.Find("HealthBoss");
        health = maxHealth;
        healthBoss.GetComponent<HealthBar>().SetMaxHealth(maxHealth);
        healthBoss.GetComponent<HealthBar>().SetHealth(health);
    }

    public override void RunStateCond()
    {
        if (attackCond && coolDownTimer >= attackCoolDownMax && !beenHit) stateMachine.ChangeState(attackState);
        else if (specialCond) stateMachine.ChangeState(specialState);
        else if (reachedEndOfPath && !foundPlayer && !beenHit) stateMachine.ChangeState(idleState);
    }

    public override void IdleStateCond()
    {
        if (attackPlayerWhenIdling && (attackCond && coolDownTimer >= attackCoolDownMax)) stateMachine.ChangeState(attackState);
        else if (specialCond) stateMachine.ChangeState(specialState);
        else if (foundPlayer || beenHit) stateMachine.ChangeState(runState);
    }

    public void SpecialStateCond()
    {
        Special();
        bool cond = true;
        for (int i = 0; i < enemieSpawner.transform.childCount; i++)
        {
            if (enemieSpawner.transform.GetChild(i).gameObject.GetComponent<Entity>() != null && enemieSpawner.transform.GetChild(i).gameObject != gameObject && enemieSpawner.transform.GetChild(i).gameObject.GetComponent<Entity>().dead == false) cond = false;
        }

        if (cond) stateMachine.ChangeState(runState);
    }

    protected override void Special()
    {
        canBeHit = false;
        targetVelocityX = 0;

        spriteRenderer.flipX = lastXDir == 1 ? false : true;

        if (cooldown > 3f)
        {
            if (health <= maxHealth) health += 1;
            cooldown = 0f;
            healthBoss.GetComponent<HealthBar>().SetHealth(health);
        }

        cooldown += Time.deltaTime;
        Debug.Log(health);
    }

    protected override void HitDummyEnter()
    {
        base.HitDummyEnter();
        healthBoss.GetComponent<HealthBar>().SetHealth(health);
    }


    public void OnEnterSpecialState()
    {
        cooldown = 0f;
        lastXDir = FacingDirection;
        numEnemie += 1;
        
        for (int i = 0; i < numEnemie; i++)
        {
            AudioSource.PlayClipAtPoint(puff, this.gameObject.transform.position, 1f);
            Vector2 r = new Vector2(Random.Range(enemiesDelimiter.transform.GetChild(0).transform.position.x, enemiesDelimiter.transform.GetChild(1).transform.position.x), Random.Range(enemiesDelimiter.transform.position.y + yRand, enemiesDelimiter.transform.position.y - yRand));
            Instantiate(explosion, r, Quaternion.identity, enemieSpawner.transform);
            GameObject e = (GameObject)Instantiate(enemie, r, Quaternion.identity, enemieSpawner.transform);
            if (e.GetComponent<LandbasedEnemie>() == null) e.GetComponent<RangedLandbasedEnemie>().playerTransform = playerTransform;
            else e.GetComponent<LandbasedEnemie>().playerTransform = playerTransform;
        }
    }

    protected override void HandleAttackCond()
    {
        coolDownTimer += Time.deltaTime;
        attackCond = CastRay((Vector2) (playerTransform.position - transform.position).normalized, "Player", playerSeeDistance - 20f);
    }

    protected override void HandleSpecialCond() 
    {
        if (health <= firstSpecialHealth) 
        {
            specialCond = true;
            firstSpecialHealth = -10;
        }
        else if (health <= secondSpecialHealth)
        {
            specialCond = true;
            secondSpecialHealth = -10;
        }
        else if (health <= thirdSpecialHealth)
        {
            specialCond = true;
            thirdSpecialHealth = -10;
        }
    }

    protected override void HandleJumpTrigger() {}
    protected override void SeekPlayer()
    {
        if (playerTransform != null)
            foundPlayer = CastRay((Vector2) (playerTransform.position - transform.position).normalized, "Player", playerSeeDistance);
        else if (GameObject.FindWithTag("Player"))
            playerTransform = GameObject.FindWithTag("Player").transform;
    }
}
