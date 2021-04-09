using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Controller2D))]

public class Entity : MonoBehaviour
{
    #region Components
    [HideInInspector]
    private GameObject levelLoader;
    public Vector2 velocity;
    public Animator animator;
    public StateMachine stateMachine;
    public SpriteRenderer spriteRenderer;
    public Controller2D controller;
    public Transform attackPoint;
    public LayerMask attackLayers;
    #endregion

    #region Variables
    public int health = 1;
    public int attackForce = 1;
    public float invencibilityTimer = .2f;
    [HideInInspector]
    public bool jumping = false;
    [HideInInspector]
    public bool isOnFloor = false;
    protected bool attackCond = false;
    protected bool specialCond = false;
    [HideInInspector]
    public int FacingDirection;
    protected int xMovement;
    protected Color originalColor;
    #endregion

    #region Attack and Special
    public float attackRange = 0.5f;
    protected float coolDownTimer = 0f;
    protected float coolDownSpecialTimer = .45f;
    public float attackCoolDownMax = 1f;
    public float specialCoolDownMax = .5f;
    [HideInInspector]
    public bool atacking = false;
    [HideInInspector]
    public bool beenHit = false;
    bool canBeHit = true;
    #endregion

    #region Unity func
    void Start() => Initialize();
    void Update() => stateMachine.CurrentState.LogicUpdate();
    void FixedUpdate() => stateMachine.CurrentState.PhysicsUpdate();
    #endregion

    #region Virtual func
    public virtual void Attack() {}
    protected virtual void Move() {}
    protected virtual void Special() {}
    public virtual void MovementLogic() {}
    protected virtual void HitDummyExit() {}
    protected virtual void HitDummyEnter() {}
    protected virtual void AfterTimeScale() {}
    public virtual void FixedMovementLogic() {}
    protected virtual void HandleAttackCond() {}
    protected virtual void HandleSpecialCond() {}
    protected virtual void HandleMovementDir() {}
    
    protected virtual void Initialize() 
    {
        originalColor = spriteRenderer.color;
        FacingDirection = spriteRenderer.flipX ? -1 : 1;
        levelLoader = GameObject.Find("LevelLoader");
    } 

    protected virtual void Die()
    {
        StartCoroutine(Pause(.15f));
        
        if (gameObject.tag == "Player")
        {
            levelLoader.GetComponent<ButtonManager>().ReloadScene();
        }
    }

    public virtual void OnAttackAnimationExit()
    {
        atacking = false;
    }
    #endregion

    #region Base func
    public void ApplyDamage(int amount, int dir) 
    {
        beenHit = true;

        if (canBeHit)
        {
            HitDummyEnter();
            health -= 1;
            KnockBack(dir * amount);
            StartCoroutine(HitTimer(invencibilityTimer));
            if (health <= 0) Die();
        }
    }

    protected void KnockBack(int amount) 
    {
        velocity.x = amount * 10f * Random.Range(.7f, 1f);
        velocity.y = 10f * Random.Range(.6f, 1f);
    }

    public void Flip() 
    {
        if (atacking) return;
        
        if (xMovement != 0)
            FacingDirection = xMovement == 1 ? 1 : -1;
        
        bool fd = FacingDirection == 1 ? false : true;

        if (spriteRenderer.flipX != fd) 
        {
            spriteRenderer.flipX = fd;
            attackPoint.localPosition = new Vector3(-1 * attackPoint.localPosition.x, attackPoint.localPosition.y, attackPoint.localPosition.z);
        }
            
    }
    #endregion

    #region Coroutine
    IEnumerator WaitToAttack(float time)
    {
        coolDownTimer = 0;
        yield return new WaitForSeconds(time);
        Attack();
    }

    public IEnumerator Pause(float time)
    {
        Time.timeScale = 0f;
        float pauseEndTime = Time.realtimeSinceStartup + time;
        while (Time.realtimeSinceStartup < pauseEndTime) yield return 0;
        Time.timeScale = 1;
        AfterTimeScale();
    }

    IEnumerator HitTimer(float time)
    {
        canBeHit = false;
        yield return new WaitForSeconds(time);
        HitDummyExit();
        canBeHit = true;
        beenHit = false;
        spriteRenderer.color = originalColor;
    }
    #endregion
}
