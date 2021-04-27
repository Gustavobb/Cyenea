using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyEfreet : MonoBehaviour
{
    public PlayerEfreet player;

    public void Attack()
    {
        player.Attack();
    }

    public void OnAttackAnimationExit()
    {
        player.OnAttackAnimationExit();
    }
}
