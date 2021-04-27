using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTreatnt : Player
{
    float cooldown = 0f;
    float cooldownLeave = 0f;
    bool waited = false;
    int lastXDir;
    float posXAttack;
    float posYAttack;
    public GameObject attackGo;
    public AudioSource plantSound;
    int oldFacingDirection;

    #region Unity functions

    protected override void Initialize()
    {
        base.Initialize();
        coolDownSpecialTimer = pm.GetComponent<PlayerManegerScript>().coolDownSpecialTree;
        coolDownTimer = pm.GetComponent<PlayerManegerScript>().coolDownAttackTree;
    }
    void Awake()
    {
        stateMachine = new StateMachine();
        attackState = new AttackState(AttackStateCond, () => {atacking = true; oldFacingDirection = FacingDirection;}, () => {atacking = false; if (isOnFloor) { velocityXSmoothing = 0; targetVelocityX = 0; velocity.x = 0;}}, this, gameObject, stateMachine, "Attack");
        idleState = new IdleState(IdleStateCond, () => {}, () => {}, this, gameObject, stateMachine, "Idle");
        runState = new RunState(RunStateCond, () => {footDust.Play();}, () => {footDust.Stop();}, this, gameObject, stateMachine, "Run");
        jumpState = new JumpState(JumpStateCond, () => {if (velocity.y > 0) jumping = true;}, () => {jumping = false; if (isOnFloor) {jumpDust.Play(); landSound.pitch = Random.Range(.8f, .9f); landSound.Play();}}, this, gameObject, stateMachine, "Jump");
        specialState = new SpecialState(SpecialStateCond, OnEnterSpecialState, OnExitSpecialState, this, gameObject, stateMachine, "Special");
        deathState = new DeathState(DeathStateCond, () => {GetComponent<BoxCollider2D>().enabled = false;StartCoroutine(Pause(.3f)); StartCoroutine(WaitToDie(2f));}, ()=>{}, this, gameObject, stateMachine, "Death");
    }
    #endregion
    public override void SpecialStateCond()
    {
        if (cooldownLeave > 4f) waited = true;

        targetVelocityX = 0 * moveSpeed;
        spriteRenderer.flipX = lastXDir == 1 ? false : true;

        if (cooldown > 1.5f)
        {
            if (health < maxHealth) health += 1;
            cooldown = 0f;
        }

        cooldown += Time.deltaTime;
        cooldownLeave += Time.deltaTime;

        Debug.Log(waited);
        if (waited || beenHit || specialCond) stateMachine.ChangeState(runState);
    }

    protected override void HandleJumpTrigger()
    {
        jumpTrigger = IJ.jump && (stateMachine.CurrentState != specialState);
        IJ.jump = false;

        if (IJ.jumpInputUp && velocity.y > 0f) velocity.y *= .5f;
        IJ.jumpInputUp = false;
    }

    public override void JumpStateCond()
    {
        if (isOnFloor) stateMachine.ChangeState(runState);
    }

    public override void Attack()
    {
        coolDownTimer = 0f;
        posYAttack = transform.position.y;
        posXAttack = transform.position.x + (oldFacingDirection * 2f);
        StartCoroutine(Timer(.3f));
        pm.GetComponent<PlayerManegerScript>().coolDownAttackTree = 0f;
        pm.GetComponent<PlayerManegerScript>().cooldownBarTreeAttack.SetActive(true);
    }

    public void OnEnterSpecialState()
    {
        plantSound.pitch = Random.Range(0.8f, 1.1f);
        plantSound.Play();
        waited = false;
        cooldown = 0f;
        cooldownLeave = 0f;
        lastXDir = FacingDirection;
    }
    public void OnExitSpecialState(){
        plantSound.pitch = Random.Range(0.8f, 1.1f);
        plantSound.Play();
        coolDownSpecialTimer = 0f;
        waited = false;
        cooldown = 0f;
        cooldownLeave = 0f;
        pm.GetComponent<PlayerManegerScript>().coolDownSpecialTree = 0f;
        pm.GetComponent<PlayerManegerScript>().cooldownBarTree.SetActive(true);
    }

    IEnumerator Timer(float time)
    {
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(time);
            GameObject p = (GameObject)Instantiate(attackGo, new Vector2(posXAttack, posYAttack), Quaternion.identity);
            TreatntAttack pp = p.GetComponent<TreatntAttack>();
            pp.attackForce = attackForce;
            pp.mass = mass;
            posXAttack += (oldFacingDirection * 1.5f);
        }
    }
}
