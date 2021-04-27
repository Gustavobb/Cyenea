using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Controller2D))]

public class Entity : MonoBehaviour
{
    #region Components
    [HideInInspector]
    protected GameObject levelLoader;
    
    public Vector2 velocity;
    public Animator animator;
    public StateMachine stateMachine;
    public SpriteRenderer spriteRenderer;
    public Controller2D controller;
    public Transform attackPoint;
    public LayerMask attackLayers;
    [HideInInspector]
    public GameObject pm;

    #endregion

    #region Variables
    public int health = 1;

    public int soul = 1;

    public float mass = 1;
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
    public float coolDownTimer = 0f;
    public float coolDownSpecialTimer = .5f;
    public float attackCoolDownMax = 1f;
    public float specialCoolDownMax = .5f;
    [HideInInspector]
    public bool atacking = false;
    [HideInInspector]
    public bool beenHit = false;
    [HideInInspector]
    public bool canBeHit = true;
    [HideInInspector]
    public bool dead = false;
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
        pm = GameObject.Find("PlayerManeger");
    } 

    public virtual void OnAttackAnimationExit()
    {
        atacking = false;
    }
    #endregion

    #region Base func
    public void ApplyDamage(int amount, int dir, float hitterMass, float yMult) 
    {
        if (canBeHit)
        {
            beenHit = true;
            health -= amount;
            KnockBack(dir * hitterMass/mass, yMult);
            StartCoroutine(HitTimer(invencibilityTimer));
            HitDummyEnter();
        }
    }

    protected void KnockBack(float amount, float yMult) 
    {
        if (health >= 0)
        {
            velocity.x = amount * 25f * Random.Range(.7f, 1f);
            velocity.y = yMult * Random.Range(.6f, 1f);
        }
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

    public virtual IEnumerator WaitToDie(float time)
    {
        yield return new WaitForSeconds(time/2);
        float pauseEndTime = Time.realtimeSinceStartup + time/2;
        while (Time.realtimeSinceStartup < pauseEndTime) 
        {
            float a = spriteRenderer.color.a - Time.deltaTime;
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, a);
            yield return 0;
        }

        Destroy(gameObject);
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
