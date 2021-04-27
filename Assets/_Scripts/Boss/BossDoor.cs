using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDoor : MonoBehaviour
{
    public Animator animator;
    public void CanDie()
    {
        Destroy(gameObject);
    }
}
