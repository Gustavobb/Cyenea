using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButDoorCheck : MonoBehaviour
{
    public LayerMask BulletLayers;
    public Animator anim;

    public int id = 0;
    public bool opened = false;

    void Update()
    {
        if (!opened)
        {
            Collider2D[] hitBullet = Physics2D.OverlapCircleAll(transform.position, 1f, BulletLayers);
            foreach (Collider2D player in hitBullet)
            {
                opened = true;
                GetComponent<AudioSource>().Play();
                anim.SetBool("Open", true);
                StartCoroutine(wait(1.5f));
            }
        }
    }

    IEnumerator wait(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(transform.parent.gameObject);
    }
}
