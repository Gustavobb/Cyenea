using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOgre : Player
{
    bool wallSliding;
    float wallSlideSpeedMax = .5f;
    Vector2 wallJumpClimb = new Vector2(6f, 20f);
    Vector2 wallJumpOff = new Vector2(7f, 7f);
    Vector2 wallLeap = new Vector2(18f, 17f);

    #region Unity functions

    protected override void Initialize()
    {
        base.Initialize();
        coolDownTimer = pm.GetComponent<PlayerManegerScript>().coolDownAttackOgre;
    }
    void Awake()
    {
        stateMachine = new StateMachine();
        attackState = new AttackState(AttackStateCond, () => {atacking = true;}, () => {atacking = false; if (isOnFloor) { velocityXSmoothing = 0; targetVelocityX = 0; velocity.x = 0;}}, this, gameObject, stateMachine, "Attack");
        idleState = new IdleState(IdleStateCond, () => {}, () => {}, this, gameObject, stateMachine, "Idle");
        runState = new RunState(RunStateCond, () => {footDust.Play();}, () => {footDust.Stop();}, this, gameObject, stateMachine, "Run");
        jumpState = new JumpState(JumpStateCond, () => {if (velocity.y > 0) jumping = true;}, () => {jumping = false; if (isOnFloor) {jumpDust.Play(); landSound.pitch = Random.Range(.8f, .9f); landSound.Play();}}, this, gameObject, stateMachine, "Jump");
        specialState = new SpecialState(SpecialStateCond, () => {specialCond = false;}, () => {}, this, gameObject, stateMachine, "Special");
        deathState = new DeathState(DeathStateCond, () => {GetComponent<BoxCollider2D>().enabled = false;StartCoroutine(Pause(.3f)); StartCoroutine(WaitToDie(2f));}, ()=>{}, this, gameObject, stateMachine, "Death");
    }
    #endregion

    void WallSliding()
    {
        wallSliding = false;
        int wallDirX = (controller.collisions.left) ? -1 : 1;

        if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0)
        {
            wallSliding = true;
            specialCond = true;
            if (velocity.y < -wallSlideSpeedMax)
            {
                velocity.y = -wallSlideSpeedMax;
            }
        }

        if (IJ.jump)
        {
            if (wallSliding)
            {
                if (wallDirX == xMovement)
                    velocity = new Vector2(-wallDirX * wallJumpClimb.x, wallJumpClimb.y);
                
                else if (xMovement == 0)
                    velocity = new Vector2(-wallDirX * wallJumpOff.x, wallJumpOff.y);
                
                else
                    velocity = new Vector2(-wallDirX * wallLeap.x, wallLeap.y);
            }
        }
    }

    #region Override functions
    protected override void HandleSpecialCond()
    {
        coolDownSpecialTimer = specialCoolDownMax;
        WallSliding();
    }

    public override void SpecialStateCond()
    {
        if (!wallSliding) stateMachine.ChangeState(jumpState);
    }

    public override void Attack()
    {
        base.Attack();
        pm.GetComponent<PlayerManegerScript>().coolDownAttackOgre = 0f;
        pm.GetComponent<PlayerManegerScript>().cooldownBarOgre.SetActive(true);
    }
    #endregion
}
