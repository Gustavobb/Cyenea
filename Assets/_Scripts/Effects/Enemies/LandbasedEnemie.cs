using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class LandbasedEnemie : LandbasedEntity
{
    #region States
    protected AttackState attackState { get; set; }
    protected IdleState idleState { get; set; }
    protected RunState runState { get; set; }
    protected JumpState jumpState { get; set; }
    protected SpecialState specialState { get; set; }
    protected DeathState deathState { get; set; }
    #endregion

    #region Variables
    public bool playerSeeker = false;
    public bool attackPlayerWhenIdling = true;
    public float idleTime = 0f;
    public float attackDist;
    public float playerSeeDistance = 10f;
    public LayerMask interactableLayers;
    public GameObject followPoints;
    [HideInInspector]
    public Transform playerTransform;
    Vector2 targetTransform;
    Vector2[] followTransforms = new Vector2[10];
    bool foundPlayer = false;
    bool reachedEndOfPath = false;
    int followPointsCounter = 0;
    #endregion

    #region Unity functions
    void Awake()
    {
        stateMachine = new StateMachine();
        attackState = new AttackState(AttackStateCond, () => {atacking = true;}, () => {atacking = false;}, this, gameObject, stateMachine, "Attack");
        idleState = new IdleState(IdleStateCond, () => StartCoroutine(WaitToLeaveIdle(idleTime)), () => {}, this, gameObject, stateMachine, "Idle");
        runState = new RunState(RunStateCond, () => {}, () => {}, this, gameObject, stateMachine, "Run");
        jumpState = new JumpState(JumpStateCond, () => {if (velocity.y > 0) jumping = true;}, () => jumping = false, this, gameObject, stateMachine, "Jump");
        specialState = new SpecialState(SpecialStateCond, () => {}, () => {}, this, gameObject, stateMachine, "Special");
        deathState = new DeathState(DeathStateCond, () => {dead = true; GetComponent<BoxCollider2D>().enabled = false; StartCoroutine(Pause(.1f)); StartCoroutine(WaitToDie(6f));}, () => {}, this, gameObject, stateMachine, "Death");
    }
    #endregion

    #region Override functions
    protected override void Initialize()
    {
        base.Initialize();
        
        gravity = - (2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        stateMachine.Initialize(idleState);

        int children = followPoints.transform.childCount;
        for (int i = 0; i < children; i++)
            followTransforms[i] = followPoints.transform.GetChild(i).transform.position;
        
        targetTransform = followTransforms[0];
    }

    public override void MovementLogic()
    {
        base.MovementLogic();
        if (playerSeeker) SeekPlayer();

        reachedEndOfPath = Mathf.Abs(transform.position.x - targetTransform.x) < 1f ? true : false;

        if (foundPlayer) targetTransform = playerTransform.position;
        else if (reachedEndOfPath) SelectNextFollowPoint();
    }

    public override void FixedMovementLogic()
    {
        base.FixedMovementLogic();
    }

    protected override void HandleMovementDir()
    {
        float dif = transform.position.x - targetTransform.x;
        xMovement = dif < 0 ? 1 : -1;

        if (stateMachine.CurrentState == idleState) xMovement = 0;
        base.HandleMovementDir();
    }

    protected override void HandleAttackCond()
    {
        coolDownTimer += Time.deltaTime;
        attackCond = CastRay(new Vector2(FacingDirection, 0), "Player", attackDist);
    }

    protected override void HandleJumpTrigger()
    {
        if (!beenHit) jumpTrigger = CastRay(new Vector2(FacingDirection, 0), "Map", .9f);
    }

    protected override void HitDummyEnter()
    {
        spriteRenderer.color = Color.red;
        // cam.GetComponent<CameraShake>().ShakeCR(.05f, .04f);
    }

    protected override void AfterTimeScale()
    {
        PlayerData data = SaveSystem.LoadPlayer();
        SaveSystem.SavePlayer(data.activePlayerIdData, data.currentSpawnPointIdData, data.healthData, data.maxHealthData, data.manaData+soul, data.obtainedPlayersNameData, data.doorsClosed);
        pm.GetComponent<PlayerManegerScript>().checkMana();
        GameObject.Find("Main Camera").GetComponent<CameraShake>().ShakeCR(.06f, .06f);
    }
    #endregion

    #region VirtualFunctions
    public virtual void RunStateCond()
    {
        if (health <= 0 && !dead) stateMachine.ChangeState(deathState);
        else if (!isOnFloor) stateMachine.ChangeState(jumpState);
        else if (attackCond && coolDownTimer >= attackCoolDownMax && !beenHit) stateMachine.ChangeState(attackState);
        else if (reachedEndOfPath && !foundPlayer && !beenHit) stateMachine.ChangeState(idleState);
    }

    public virtual void AttackStateCond()
    {
        if (health <= 0 && !dead) stateMachine.ChangeState(deathState);
        else if (!atacking || beenHit) stateMachine.ChangeState(runState);
    }

    public virtual void JumpStateCond()
    {
        if (health <= 0 && !dead) stateMachine.ChangeState(deathState);
        else if (isOnFloor) stateMachine.ChangeState(runState);
        else if (attackCond && coolDownTimer >= attackCoolDownMax) stateMachine.ChangeState(attackState);
    }

    public virtual void IdleStateCond()
    {
        if (health <= 0 && !dead) stateMachine.ChangeState(deathState);
        else if (attackPlayerWhenIdling && (attackCond && coolDownTimer >= attackCoolDownMax)) stateMachine.ChangeState(attackState);
        else if (foundPlayer || beenHit) stateMachine.ChangeState(runState);
    }

    public virtual void SpecialStateCond()
    {
        
    }

    public virtual void DeathStateCond()
    {
        
    }
    #endregion

    #region Base functions
    void SelectNextFollowPoint()
    {
        if (foundPlayer) 
        {
            followPointsCounter = 0;
            return;
        }

        targetTransform = followTransforms[followPointsCounter];
        followPointsCounter ++;

        if (followPointsCounter == followPoints.transform.childCount) followPointsCounter = 0;
    }

    void SeekPlayer()
    {
        if (playerTransform != null)
            foundPlayer = CastRay((Vector2) (playerTransform.position - transform.position).normalized, "Player", playerSeeDistance);
        else if (GameObject.FindWithTag("Player"))
            playerTransform = GameObject.FindWithTag("Player").transform;
    }

    public bool CastRay(Vector2 t, string tg, float distance)
    {
        RaycastHit2D hit = Physics2D.Raycast((Vector2) transform.position, t, distance, interactableLayers);
        Debug.DrawRay((Vector2) transform.position, t * distance, Color.red);
        if (hit.collider != null && hit.collider.tag == tg) return true;
        return false;
    }

    void OnDrawGizmos() 
    {
        Vector3 fwd = transform.TransformDirection(Vector3.left);
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        Gizmos.color = Color.blue;
        if (playerTransform != null) Gizmos.DrawLine((Vector2)transform.position, (Vector2)transform.position + (Vector2) (playerTransform.position - transform.position).normalized * 10f);
        Gizmos.DrawLine((Vector2)transform.position, (Vector2)transform.position + new Vector2(FacingDirection, 0) * attackDist);
    }
    #endregion

    #region Coroutine
    IEnumerator WaitToLeaveIdle(float time)
    {
        reachedEndOfPath = false;
        yield return new WaitForSeconds(time);
        if (!dead) stateMachine.ChangeState(runState);
    }
    #endregion
}
