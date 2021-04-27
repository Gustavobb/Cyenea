using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGoblin : Player
{
    #region Variables
    bool doubleJump = false;
    #endregion

    #region Unity functions
    void Awake()
    {
        stateMachine = new StateMachine();
        attackState = new AttackState(AttackStateCond, () => {atacking = true;}, () => {atacking = false; if (isOnFloor) { velocityXSmoothing = 0; targetVelocityX = 0; velocity.x = 0;}}, this, gameObject, stateMachine, "Attack");
        idleState = new IdleState(IdleStateCond, () => {}, () => {}, this, gameObject, stateMachine, "Idle");
        runState = new RunState(RunStateCond, () => {footDust.Play();}, () => {footDust.Stop();}, this, gameObject, stateMachine, "Run");
        jumpState = new JumpState(JumpStateCond, () => {if (velocity.y > 0) jumping = true;}, () => {jumping = false; if (isOnFloor) {jumpDust.Play(); landSound.pitch = Random.Range(.8f, .9f); landSound.Play();}}, this, gameObject, stateMachine, "Jump");
        specialState = new SpecialState(SpecialStateCond, () => {specialCond = false; Jump();}, () => {jumpDust.Play(); landSound.pitch = Random.Range(.8f, .9f); landSound.Play();}, this, gameObject, stateMachine, "Jump");
        deathState = new DeathState(DeathStateCond, () => {GetComponent<BoxCollider2D>().enabled = false;StartCoroutine(Pause(.3f)); StartCoroutine(WaitToDie(2f));}, ()=>{}, this, gameObject, stateMachine, "Death");
    }
    #endregion
    
    #region Override functions
    protected override void Initialize()
    {
        base.Initialize();
        doubleJump = pm.GetComponent<PlayerManegerScript>().dJ;
    }
    protected override void HandleSpecialCond()
    {
        coolDownSpecialTimer = specialCoolDownMax;
        DoubleJump();
    }

    void DoubleJump()
    {
        if (!isOnFloor && hangCounter < 0f && IJ.jump)
        {
            if (!doubleJump)
            {
                specialCond = true;
                doubleJump = true;
                pm.GetComponent<PlayerManegerScript>().dJ = true;
            } 
        }

        if (isOnFloor) 
        {
            doubleJump = false;
            pm.GetComponent<PlayerManegerScript>().dJ = false;
        }
    }

    public override void SpecialStateCond()
    {
        if (attackCond && coolDownTimer >= attackCoolDownMax) stateMachine.ChangeState(attackState);
        else if (isOnFloor) stateMachine.ChangeState(runState);
    }
    #endregion
}
