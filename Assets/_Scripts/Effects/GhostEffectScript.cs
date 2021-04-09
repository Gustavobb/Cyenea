using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostEffectScript : MonoBehaviour
{
    #region Variables
    public float timeMax = .3f;
    public SpriteRenderer spriteRenderer;
    float alpha = 1f;
    float alphaSubtractor;
    float timeCounter;
    #endregion

    #region Unity Functions
    void Start()
    {
        alphaSubtractor = alpha / timeMax;
    }

    void Update()
    {
        Color tmp = spriteRenderer.color;
        tmp.a -= alphaSubtractor * Time.deltaTime;
        tmp.r -= alphaSubtractor * Time.deltaTime;
        tmp.g -= alphaSubtractor * Time.deltaTime;
        tmp.b -= alphaSubtractor * Time.deltaTime;
        spriteRenderer.color = tmp;

        timeCounter += Time.deltaTime;
        if (timeCounter >= timeMax) Destroy(gameObject);
    }
    #endregion
}
