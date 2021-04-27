using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelScript : MonoBehaviour
{
    public Text tutorialText;
    public Text instructionText;

    public Text unlockedText;

    public Input_Joystick IJ;

    public bool canSkip = false;

    public float pauseEndTime;

    private void Start() {
        tutorialText.text = "";
        unlockedText.text = "";
        instructionText.color = new Color (instructionText.color.r,instructionText.color.g,instructionText.color.b,0f);
    }

    public void setText(string text, string text2){
        GetComponent<Animator>().SetTrigger("Open");
        StartCoroutine(WaitToSkip(text, text2));
    }

    private IEnumerator WaitToSkip(string text, string text2){
        yield return new WaitForSeconds(.3f);
        tutorialText.text = text;
        unlockedText.text = text2;
        yield return new WaitForSeconds(.5f);
        Time.timeScale = 0f;
        pauseEndTime = Time.realtimeSinceStartup + 5f;
        while (Time.realtimeSinceStartup < pauseEndTime) yield return 0;
        instructionText.color = new Color (instructionText.color.r,instructionText.color.g,instructionText.color.b,1f);
        canSkip = true;
    }

    private void Update() {
        if (canSkip && (IJ.left||IJ.jump||IJ.right)){
            Time.timeScale = 1f;
            tutorialText.text = "";
            unlockedText.text = "";
            GetComponent<Animator>().SetTrigger("Close");
            instructionText.color = new Color (instructionText.color.r,instructionText.color.g,instructionText.color.b,0f);
            Time.timeScale = 1f;
            canSkip = false;
        }
    }
}
