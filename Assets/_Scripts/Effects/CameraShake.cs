using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public void ShakeCR(float duration, float magnitude)
    {
        StartCoroutine(Shake(duration, magnitude));
    }

    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPos = transform.position;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float xR = Random.Range(-1f, 1f);
            float xY = Random.Range(-1f, 1f);

            xR = xR <= 0f ? -1f : 1f;
            xY = xY <= 0f ? -1f : 1f;
            float x = xR * magnitude;
            float y = xY * magnitude;

            transform.position = new Vector3(transform.position.x + x, transform.position.y + y, originalPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        if(duration<.2f)
        transform.position = originalPos;
    }
}
