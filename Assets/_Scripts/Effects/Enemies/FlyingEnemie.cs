using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class FlyingEnemie : Entity
{
    #region States
    protected AttackState attackState { get; set; }
    protected IdleState idleState { get; set; }
    protected RunState runState { get; set; }
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
    Vector2 direction;
    float yMovement;
    float XMovement;
    bool foundPlayer = false;
    bool reachedEndOfPath = false;
    int followPointsCounter = 0;
    #endregion

    #region Movement
    public float moveSpeed = 7f;
    public float accelerationTimeAirbone = .2f;
    [HideInInspector]
    public float targetVelocityX; 
    [HideInInspector]
    public float velocityXSmoothing;
    [HideInInspector]
    public float velocityYSmoothing;
    [HideInInspector]
    public float targetVelocityY;
    #endregion

    #region A*
    Path path;
    int currentWaypoint = 0;
    Seeker seeker;
    float nextWaypointDistance = 1f;
    #endregion

    #region Unity functions
    void Awake()
    {
        stateMachine = new StateMachine();
        attackState = new AttackState(AttackStateCond, () => {atacking = true;}, () => {atacking = false;}, this, gameObject, stateMachine, "Attack");
        idleState = new IdleState(IdleStateCond, () => StartCoroutine(WaitToLeaveIdle(idleTime)), () => {}, this, gameObject, stateMachine, "Idle");
        runState = new RunState(FlyStateCond, () => {}, () => {}, this, gameObject, stateMachine, "Fly");
        deathState = new DeathState(DeathStateCond, () => {dead = true; GetComponent<BoxCollider2D>().enabled = false; StartCoroutine(Pause(.1f)); StartCoroutine(WaitToDie(6f));}, () => {}, this, gameObject, stateMachine, "Death");
    }
    #endregion

    #region Override functions
    protected override void Initialize()
    {
        base.Initialize();
        isOnFloor = true;
        stateMachine.Initialize(idleState);

        int children = followPoints.transform.childCount;
        for (int i = 0; i < children; i++)
            followTransforms[i] = followPoints.transform.GetChild(i).transform.position;
        
        targetTransform = followTransforms[0];

        seeker = GetComponent<Seeker>();
        InvokeRepeating("UpdatePath", 0, .5f);
    }

    public override void MovementLogic()
    {
        if (path == null) return;

        HandleMovementDir();
        HandleAttackCond();
        Flip();

        if (playerSeeker) SeekPlayer();

        reachedEndOfPath = Mathf.Abs(((Vector2) transform.position - targetTransform).magnitude) < 1f ? true : false;
        if (foundPlayer) targetTransform = playerTransform.position;
        else if (reachedEndOfPath) SelectNextFollowPoint();

        if (currentWaypoint >= path.vectorPath.Count) return;
        direction = ((Vector2) path.vectorPath[currentWaypoint] - (Vector2) transform.position).normalized;
        if (Vector2.Distance(transform.position, path.vectorPath[currentWaypoint]) < nextWaypointDistance) currentWaypoint ++;
    }

    public override void FixedMovementLogic()
    {
        Move();
    }

    protected override void HandleMovementDir()
    {        
        XMovement = direction.x;
        yMovement = direction.y;
        xMovement = XMovement > 0 ? 1 : -1;

        if (stateMachine.CurrentState == idleState) 
        {
            XMovement = 0;
            xMovement = 0;
            yMovement = 0;
        }

        targetVelocityX = XMovement * moveSpeed;
        targetVelocityY = yMovement * moveSpeed;
    }

    protected override void Move()
    {
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, accelerationTimeAirbone);
        velocity.y = Mathf.SmoothDamp(velocity.y, targetVelocityY, ref velocityYSmoothing, accelerationTimeAirbone);
        controller.Move(velocity * Time.deltaTime);
    }

    protected override void HandleAttackCond()
    {
        coolDownTimer += Time.deltaTime;
        attackCond = CastRay(new Vector2(FacingDirection, 0), "Player", attackDist);
    }

    protected override void HitDummyEnter()
    {
        spriteRenderer.color = Color.red;
        if (health <= 0) stateMachine.ChangeState(deathState);
        // cam.GetComponent<CameraShake>().ShakeCR(.05f, .04f);
    }
    protected override void AfterTimeScale()
    {
        PlayerData data = SaveSystem.LoadPlayer();
        SaveSystem.SavePlayer(data.activePlayerIdData, data.currentSpawnPointIdData, data.healthData, data.maxHealthData, data.manaData+soul, data.obtainedPlayersNameData, data.doorsClosed);
        pm.GetComponent<PlayerManegerScript>().checkMana();
        GameObject.Find("Main Camera").GetComponent<CameraShake>().ShakeCR(.06f, .06f);
    }

    public override void Attack()
    {
        coolDownTimer = 0f;
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.transform.position, attackRange, attackLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            Entity e = enemy.gameObject.GetComponent<Entity>();
            e.ApplyDamage(attackForce, FacingDirection, mass, 10f);
        }
    }
    #endregion

    #region VirtualFunctions
    public virtual void FlyStateCond()
    {
        if (health <= 0 && !dead) stateMachine.ChangeState(deathState);
        else if (attackCond && coolDownTimer >= attackCoolDownMax && !beenHit) stateMachine.ChangeState(attackState);
        else if (reachedEndOfPath || foundPlayer) stateMachine.ChangeState(idleState);
    }

    public virtual void AttackStateCond()
    {
        if (health <= 0 && !dead) stateMachine.ChangeState(deathState);
        else if (!atacking || beenHit) stateMachine.ChangeState(runState);
    }

    public virtual void IdleStateCond()
    {
        if (health <= 0 && !dead) stateMachine.ChangeState(deathState);
        else if (attackPlayerWhenIdling && (attackCond && coolDownTimer >= attackCoolDownMax)) stateMachine.ChangeState(attackState);
        else if (foundPlayer || beenHit) stateMachine.ChangeState(runState);
    }

    public virtual void DeathStateCond()
    {
        
    }
    #endregion

    #region Base functions
    void UpdatePath() 
    {
        if (seeker.IsDone() && stateMachine.CurrentState != idleState) seeker.StartPath(transform.position, targetTransform, OnPathDone);
    }

    void OnPathDone(Path p) 
    {
        if (!p.error) 
        {
            path = p;
            currentWaypoint = 0;
        }
    }

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
        Gizmos.DrawLine((Vector2)transform.position, (Vector2)transform.position + new Vector2(FacingDirection, 0) * .8f);
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
