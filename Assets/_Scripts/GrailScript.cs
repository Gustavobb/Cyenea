using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrailScript : MonoBehaviour
{
    public LayerMask PlayerLayers;
    public SceneManeger levelLoader;
    void Update()
    {
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(transform.position, 1f, PlayerLayers);
        foreach (Collider2D player in hitPlayer)
        {
            levelLoader.LoadFinal();
        }
    }
}
