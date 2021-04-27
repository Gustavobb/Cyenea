using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (BoxCollider2D))]

public class Projectile : MonoBehaviour
{
    public int moveSpeed;
    [HideInInspector]
    public int attackForce, dir;
    [HideInInspector]
    public float mass;
    public LayerMask interactableLayers;
    bool stopped = false;
    public bool destroyWhenHitMap = false;
    public GameObject explosion;
    public string target = "Player";

    void Update() {
        if (!stopped)
        {
            RaycastHit2D hit = Physics2D.Raycast((Vector2) transform.position, new Vector2(dir, 0), .2f, interactableLayers);

            if (hit.collider != null)
            {
                if (hit.collider.tag == target) 
                {
                    Entity e = hit.collider.gameObject.GetComponent<Entity>();
                    if (e.canBeHit)
                    {
                        if (explosion != null && destroyWhenHitMap) Instantiate(explosion, transform.position, Quaternion.identity);
                        stopped = true;
                        e.ApplyDamage(attackForce, dir, mass, 10f);
                        Destroy(gameObject);
                    }
                }

                else if (hit.collider.tag == "Map")
                {
                    if (destroyWhenHitMap)
                    {
                        if (explosion != null)
                            Instantiate(explosion, transform.position, Quaternion.identity);
                        Destroy(gameObject);
                    }

                    stopped = true;
                    StartCoroutine(WaitToDie(6f));
                }
            }
        }

        if (!stopped)
        {
            transform.Translate(new Vector3(dir * moveSpeed * Time.deltaTime, 0, 0));
        }
    }
    IEnumerator WaitToDie(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
