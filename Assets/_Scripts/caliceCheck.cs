using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class caliceCheck : MonoBehaviour
{
    public LayerMask PlayerLayers;
    void Update()
    {
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(transform.position, 1f, PlayerLayers);
        foreach (Collider2D player in hitPlayer)
        {
            SceneManager.LoadScene(SceneManager.sceneCountInBuildSettings -1);
        }
    }
}
