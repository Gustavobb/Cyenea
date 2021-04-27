using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISound : MonoBehaviour

{

    public AudioClip hoverClip, clickClip;
    public void HoverSound()
    {
        this.GetComponent<AudioSource>().PlayOneShot(hoverClip);
    }

    public void ClickSound()
    {
        this.GetComponent<AudioSource>().PlayOneShot(clickClip);
        
    }
}
