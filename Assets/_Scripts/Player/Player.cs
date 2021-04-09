using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : LandbasedEntity
{
    #region Jump
    public float hangTime = .1f;
    public float jumpBufferLenght = .15f;
    protected float hangCounter;
    protected float jumpBufferCount;
    #endregion

    #region States
    protected AttackState attackState { get; set; }
    protected IdleState idleState { get; set; }
    protected RunState runState { get; set; }
    protected JumpState jumpState { get; set; }
    protected SpecialState specialState { get; set; }
    protected DeathState deathState { get; set; }
    #endregion

    public float minAlpha;
    public Input_Joystick IJ;
    public LayerMask interactionMask;
    RaycastHit2D hit;
    float blinkTimer;
    float aplha;

    #region Override functions
    protected override void Initialize()
    {
        base.Initialize();
        stateMachine.Initialize(idleState);
    }

    public override void MovementLogic()
    {
        HandleInvencibility();
        HandleMovementDir();
        HandleAttackCond();
        HandleSpecialCond();
        HandleJumpTrigger();
        CoyoteTimer();
        JumpBuffer();
        CheckJump();
        Flip();

        // if (CastRay(new Vector2(FacingDirection, 0), "Enemies", .5f)) ApplyDamage(hit.collider.gameObject.GetComponent<Entity>().attackForce, hit.collider.gameObject.GetComponent<Entity>().FacingDirection);
    }

    protected override void CheckJump()
    {   
        if (jumpTrigger)
        {
            jumpTrigger = false;

            if (isOnFloor || hangCounter > 0f)
            {
                Jump();
                hangCounter = 0f;
            }

            else jumpBufferCount = jumpBufferLenght;
        }
        
        if (isOnFloor && jumpBufferCount > 0f) 
        {
            jumpBufferCount = 0f;
            Jump();
        }
    }

    protected override void HandleMovementDir()
    {
        int left = IJ.left ? 1 : 0;
        int right = IJ.right ? 1 : 0;
        xMovement = right - left;
        base.HandleMovementDir();
    }

    protected override void HandleAttackCond()
    {
        coolDownTimer += Time.deltaTime;
        attackCond = IJ.attack;
        IJ.attack = false;
    }

    protected override void HandleSpecialCond()
    {
        coolDownSpecialTimer += Time.deltaTime;
        specialCond = IJ.dash;
        IJ.dash = false;
    }

    protected override void HandleJumpTrigger()
    {
        jumpTrigger = IJ.jump;
        IJ.jump = false;

        if (IJ.jumpInputUp && velocity.y > 0f) velocity.y *= .5f;
        IJ.jumpInputUp = false;
    }

    protected override void HitDummyEnter()
    {
        StartCoroutine(Pause(.15f));
    }
    #endregion

    #region VirtualFunctions
    public virtual void IdleStateCond()
    {
        if (!isOnFloor) stateMachine.ChangeState(jumpState);
        else if (specialCond && coolDownSpecialTimer >= specialCoolDownMax) stateMachine.ChangeState(specialState);
        else if (attackCond && coolDownTimer >= attackCoolDownMax) stateMachine.ChangeState(attackState);
        else if (xMovement != 0) stateMachine.ChangeState(runState);
    }

    public virtual void RunStateCond()
    {
        if (!isOnFloor) stateMachine.ChangeState(jumpState);
        else if (specialCond && coolDownSpecialTimer >= specialCoolDownMax) stateMachine.ChangeState(specialState);
        else if (attackCond && coolDownTimer >= attackCoolDownMax) stateMachine.ChangeState(attackState);
        else if (xMovement == 0) stateMachine.ChangeState(idleState);
    }

    public virtual void AttackStateCond()
    {
        if (!atacking) stateMachine.ChangeState(runState);
    }

    public virtual void JumpStateCond()
    {
        if (attackCond && coolDownTimer >= attackCoolDownMax) stateMachine.ChangeState(attackState);
        else if (specialCond && coolDownSpecialTimer >= specialCoolDownMax) stateMachine.ChangeState(specialState);
        else if (isOnFloor) stateMachine.ChangeState(runState);
    }

    public virtual void SpecialStateCond() {}

    public virtual void DeathStateCond() {}

    protected virtual void HandleInvencibility()
    {
        if (beenHit)
        {
            if (blinkTimer >= .15f) aplha = .3f;
            else aplha = 1f;

            if (blinkTimer >= .3f) blinkTimer = 0f;

            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, aplha);
            blinkTimer += Time.deltaTime;
        }
    }
    #endregion

    #region Base functions
    public bool CastRay(Vector2 t, string tg, float distance)
    {
        hit = Physics2D.Raycast((Vector2) transform.position, t, distance, interactionMask);
        if (hit.collider != null && hit.collider.tag == tg) return true;
        return false;
    }

    void CoyoteTimer()
    {
        if (!isOnFloor && wasOnFloor && !jumping) hangCounter = hangTime;
        else hangCounter -= Time.deltaTime;
    }

    void JumpBuffer()
    {
        if (jumpBufferCount > 0f) jumpBufferCount -= Time.deltaTime;
    }

    void OnDrawGizmos() 
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    public void SwitchToIdle()
    {
        stateMachine.ChangeState(idleState);
    }
    #endregion
}
