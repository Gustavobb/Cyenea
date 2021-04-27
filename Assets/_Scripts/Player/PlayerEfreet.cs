using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEfreet : Player
{
    public GameObject projectile;
    #region Unity functions
    void Awake()
    {
        coolDownTimer = attackCoolDownMax;
        stateMachine = new StateMachine();
        attackState = new AttackState(AttackStateCond, () => {atacking = true;}, () => {atacking = false; if (isOnFloor) { velocityXSmoothing = 0; targetVelocityX = 0; velocity.x = 0;}}, this, gameObject, stateMachine, "Attack");
        idleState = new IdleState(IdleStateCond, () => {}, () => {}, this, gameObject, stateMachine, "Idle");
        runState = new RunState(RunStateCond, () => {footDust.Play();}, () => {footDust.Stop();}, this, gameObject, stateMachine, "Run");
        jumpState = new JumpState(JumpStateCond, () => {if (velocity.y > 0) jumping = true;}, () => {jumping = false; jumpDust.Play();}, this, gameObject, stateMachine, "Jump");
        specialState = new SpecialState(SpecialStateCond, () => {specialCond = false;}, () => {}, this, gameObject, stateMachine, "Special");
        deathState = new DeathState(DeathStateCond, () => {GetComponent<BoxCollider2D>().enabled = false;StartCoroutine(Pause(.3f)); StartCoroutine(WaitToDie(2f));}, ()=>{}, this, gameObject, stateMachine, "Death");
    }
    #endregion


    #region Override functions
    protected override void HandleSpecialCond()
    {
        specialCond = false;
    }

    public override void Attack()
    {
        PlayAttackSound();
        coolDownTimer = 0f;
        GameObject p = (GameObject)Instantiate(projectile, attackPoint.position, Quaternion.identity);
        Projectile pp = p.GetComponent<Projectile>();
        p.GetComponent<SpriteRenderer>().flipX = spriteRenderer.flipX;
        pp.dir = FacingDirection;
        pp.attackForce = attackForce;
        pp.mass = mass;
    }
    #endregion
}
