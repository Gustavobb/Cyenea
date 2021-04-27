using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudio : MonoBehaviour
{
    public void Play()
    {
        GameObject.Find("MusicPlayer").GetComponent<FadeMusic>().StartFadeIn(gameObject.GetComponent<AudioSource>(), 12f);
    }
}
