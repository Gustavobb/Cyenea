using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreatntAttack : MonoBehaviour
{
    public Transform attackPoint;
    public LayerMask attackLayers, interactionMask;
    public float mass = 1;
    public int attackForce = 1;
    public float attackRange = 0.5f;
    Animator animator;
    
    void Start()
    {
        RaycastHit2D hit = Physics2D.Raycast((Vector2) transform.position, Vector2.down, .5f, interactionMask);
        Debug.DrawRay((Vector2) transform.position, Vector2.down * .5f, Color.red);
        if (hit.collider == null) Destroy(gameObject);
        animator = GetComponent<Animator>();
        animator.SetBool("Start", true);
        StartCoroutine(Timer(5f));
    }

    void Update()
    {
        Debug.DrawRay((Vector2) transform.position, Vector2.down * 1f, Color.red);
        Attack();
    }
    
    void Attack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.transform.position, attackRange, attackLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            Entity e = enemy.gameObject.GetComponent<Entity>();
            e.ApplyDamage(attackForce, -e.FacingDirection, mass, 10f);
        }
    }

    public void Die() => Destroy(gameObject);

    IEnumerator Timer(float time)
    {
        yield return new WaitForSeconds(time);
        animator.SetBool("Die", true);
    }
}
