using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandbasedEntity : Entity
{
    #region Jump
    public float jumpHeight = 4f;
    public float timeToJumpApex = .32f;
    public float accelerationTimeAirbone = .2f;
    protected bool jumpTrigger = false;
    protected bool wasOnFloor = false;
    protected float jumpVelocity;
    protected float gravity;
    #endregion

    #region Grounded
    public float moveSpeed = 7f;
    public float accelerationTimeGrounded = .1f;
    [HideInInspector]
    public float targetVelocityX; 
    [HideInInspector]
    public float velocityXSmoothing;
    #endregion

    #region Overrides
    protected override void Initialize()
    {
        base.Initialize();
        gravity = - (2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
    }

    public override void MovementLogic()
    {
        HandleMovementDir();
        HandleAttackCond();
        HandleSpecialCond();
        HandleJumpTrigger();
        CheckJump();
        Flip();
    }
    
    public override void FixedMovementLogic()
    {
        ApplyGravity();
        Move();
    }

    protected override void Move()
    {
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirbone);
        controller.Move(velocity * Time.deltaTime);
    }

    protected override void HandleMovementDir() 
    {
        targetVelocityX = xMovement * moveSpeed;
    }

    public override void Attack()
    {
        coolDownTimer = 0f;
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.transform.position, attackRange, attackLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.gameObject.GetComponent<Entity>().ApplyDamage(attackForce, FacingDirection);
        }
    }
    #endregion

    #region Virtual functions
    protected virtual void CheckJump()
    {
        if (jumpTrigger && !jumping && isOnFloor)
        {
            jumpTrigger = false;
            Jump();
        }
    }

    protected virtual void ApplyGravity()
    {
        wasOnFloor = isOnFloor;
        isOnFloor = controller.collisions.below;
        if ((controller.collisions.above || isOnFloor) && !(velocity.y > 0f)) velocity.y = 0;
        else if (controller.collisions.above) velocity.y *= .5f;
        velocity.y += gravity * Time.deltaTime;
    }

    protected virtual void HandleJumpTrigger() {}
    #endregion

    #region Base functions
    protected void Jump()
    {
        velocity.y = jumpVelocity;
    }
    #endregion
}
