﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVampire : Player
{
    #region Variables
    int lastXDir;
    bool canGhostEffect = true;
    float originalMoveSpeed;
    float originalGravity;
    float originalAccelerationTimeAribone;
    #endregion

    public GameObject ghostEffect;
    GhostEffectScript gEffectScript;

    #region Unity functions
    void Awake()
    {
        stateMachine = new StateMachine();
        attackState = new AttackState(AttackStateCond, () => {atacking = true;}, () => {atacking = false; if (isOnFloor) { velocityXSmoothing = 0; targetVelocityX = 0; velocity.x = 0;}}, this, gameObject, stateMachine, "Attack");
        idleState = new IdleState(IdleStateCond, () => {}, () => {}, this, gameObject, stateMachine, "Idle");
        runState = new RunState(RunStateCond, () => {}, () => {}, this, gameObject, stateMachine, "Run");
        jumpState = new JumpState(JumpStateCond, () => {if (velocity.y > 0) jumping = true;}, () => jumping = false, this, gameObject, stateMachine, "Jump");
        specialState = new SpecialState(SpecialStateCond, OnEnterSpecialState, OnExitSpecialState, this, gameObject, stateMachine, "Special");
        deathState = new DeathState(DeathStateCond, () => {}, () => {}, this, gameObject, stateMachine, "Death");
    }
    #endregion

    #region Dash Functions
    protected override void Special()
    {
        xMovement = lastXDir;
        targetVelocityX = xMovement * moveSpeed;
        if (canGhostEffect)
        {
            StartCoroutine(WaitToGhostEffect(.005f));
            canGhostEffect = false;
        }
    }

    public override void SpecialStateCond()
    {
        Special();
        FacingDirection = lastXDir;
        spriteRenderer.flipX = FacingDirection == 1 ? false : true;
        
        bool cond = CastRay(new Vector2(FacingDirection, 0), "Enemies", 1.5f) || controller.collisions.left || controller.collisions.right;
        if (cond || specialCond) stateMachine.ChangeState(runState);
    }

    public void OnEnterSpecialState()
    {
        originalAccelerationTimeAribone = accelerationTimeAirbone;
        originalGravity = gravity;
        originalMoveSpeed = moveSpeed;
        velocity.y = 0f;
        moveSpeed *= 1.5f;
        lastXDir = FacingDirection;
        accelerationTimeAirbone = accelerationTimeGrounded;
        coolDownSpecialTimer = 0f;
        gravity = 0f;
    }

    public void OnExitSpecialState()
    {
        moveSpeed = originalMoveSpeed;
        accelerationTimeAirbone = originalAccelerationTimeAribone;
        gravity = originalGravity;
        coolDownSpecialTimer = 0f;
    }
    #endregion

    protected override void HandleJumpTrigger()
    {
        jumpTrigger = IJ.jump && (stateMachine.CurrentState != specialState);
        IJ.jump = false;

        if (IJ.jumpInputUp && velocity.y > 0f) velocity.y *= .5f;
        IJ.jumpInputUp = false;
    }

    #region Coroutine
    IEnumerator WaitToGhostEffect(float time)
    {
        yield return new WaitForSeconds(time);
        GameObject instance = (GameObject)Instantiate(ghostEffect, transform.position, Quaternion.identity);
        gEffectScript = instance.GetComponent<GhostEffectScript>();
        gEffectScript.spriteRenderer.sprite = spriteRenderer.sprite;
        gEffectScript.spriteRenderer.color = spriteRenderer.color;
        gEffectScript.spriteRenderer.flipX = spriteRenderer.flipX;
        canGhostEffect = true;
    }
    #endregion
}
