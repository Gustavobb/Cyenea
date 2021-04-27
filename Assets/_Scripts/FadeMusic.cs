using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeMusic : MonoBehaviour
{   
    private void Start() {
        StartCoroutine(FadeIn(GetComponent<AudioSource>(), 10f));
    } 
    public void StartFadeOut(AudioSource audioSource, float FadeTime){
        StartCoroutine(FadeOut(audioSource, FadeTime));
    }
    public void StartFadeIn(AudioSource audioSource, float FadeTime){
        StartCoroutine(FadeIn(audioSource, FadeTime));
    }
    public IEnumerator FadeIn (AudioSource audioSource, float FadeTime) { 
        while (audioSource.volume < 0.07) {
            audioSource.volume += 0.1f * Time.deltaTime / FadeTime;
 
            yield return null;
        }
    }
    public IEnumerator FadeOut (AudioSource audioSource, float FadeTime) {
        float startVolume = audioSource.volume;
 
        while (audioSource.volume > 0) {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;
 
            yield return null;
        }
 
        audioSource.Stop ();
        audioSource.volume = startVolume;
    }
}
