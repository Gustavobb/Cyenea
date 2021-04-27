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
    public DeathState deathState { get; set; }
    #endregion

    public AudioSource walkSound, hitSound, landSound, attackSound;
    public float minAlpha;
    public Input_Joystick IJ;
    public LayerMask interactionMask;
    public ParticleSystem footDust, jumpDust;
    RaycastHit2D hit;
    float blinkTimer;
    float aplha;

    public int maxHealth = 4;

    public FadeMusic fadeMusic;


    #region Override functions
    protected override void Initialize()
    {
        base.Initialize();
        footDust.Stop();
        jumpDust.Stop();
        stateMachine.Initialize(idleState);
        fadeMusic = GameObject.Find("MusicPlayer").GetComponent<FadeMusic>();
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

        if (CastRay(Vector2.down, "Spikes", .6f)) ApplyDamage(1, FacingDirection, mass, 10f);
        if (CastRay(Vector2.up, "Spikes", .5f)) ApplyDamage(1, FacingDirection, mass, 10f);
        if (CastRay(new Vector2(FacingDirection, 0), "Spikes", .3f)) ApplyDamage(1, -FacingDirection, mass, 10f);
        if (CastRay(new Vector2(FacingDirection, 0), "Enemies", .3f)) ApplyDamage(1, -FacingDirection, mass, 10f);
    }

    protected override void CheckJump()
    {   
        if (jumpTrigger)
        {
            jumpTrigger = false;

            if (isOnFloor || hangCounter > 0f)
            {
                jumpDust.Play();
                Jump();
                hangCounter = 0f;
            }

            else jumpBufferCount = jumpBufferLenght;
        }
        
        if (isOnFloor && jumpBufferCount > 0f) 
        {
            jumpBufferCount = 0f;
            jumpDust.Play();
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
        PlayHitSound();
        
        GameObject.Find("Main Camera").GetComponent<CameraShake>().ShakeCR(.05f, .015f);
        int h = health;
        
        if (health <= 0) 
        {
            stateMachine.ChangeState(deathState);
            h = maxHealth;
        }
        else StartCoroutine(Pause(.3f));

        PlayerData data = SaveSystem.LoadPlayer();
        SaveSystem.SavePlayer(data.activePlayerIdData, data.currentSpawnPointIdData, h, data.maxHealthData, data.manaData, data.obtainedPlayersNameData, data.doorsClosed);
    }

    public override void Attack()
    {
        base.Attack();
        PlayAttackSound();
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

    public virtual void PlayRunSound() 
    {
        walkSound.pitch = Random.Range(2f, 3f);
        walkSound.Play();
    }

    public virtual void PlayAttackSound() 
    {
        attackSound.pitch = Random.Range(.7f, 1.5f);
        attackSound.Play();
    }
    public virtual void PlayHitSound() 
    {
        hitSound.pitch = Random.Range(.7f, 1.5f);
        hitSound.Play();
    }

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
        Debug.DrawRay((Vector2) transform.position, t * distance, Color.red);
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

    public override IEnumerator WaitToDie(float time)
    {
        fadeMusic.StartFadeOut(fadeMusic.GetComponent<AudioSource>(), 3f);
        yield return new WaitForSeconds(time/2);
        float pauseEndTime = Time.realtimeSinceStartup + time/2;
        while (Time.realtimeSinceStartup < pauseEndTime) 
        {
            float a = spriteRenderer.color.a - Time.deltaTime;
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, a);
            yield return 0;
        }

        PlayerData data = SaveSystem.LoadPlayer();
        SaveSystem.SavePlayer(data.activePlayerIdData, data.currentSpawnPointIdData, data.healthData, data.maxHealthData, 0, data.obtainedPlayersNameData, data.doorsClosed);
        pm.GetComponent<PlayerManegerScript>().checkMana();
        levelLoader.GetComponent<SceneManeger>().ReloadScene();
    }

}
