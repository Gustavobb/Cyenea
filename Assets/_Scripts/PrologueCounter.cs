using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PrologueCounter : MonoBehaviour
{
    public FadeMusic fadeMusic;
    public GameObject scroll;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("NextScene");
    }

    private IEnumerator NextScene(){
        yield return new WaitForSeconds(60f);
        GameObject.Find("LevelLoader").GetComponent<Animator>().SetTrigger("End");
        fadeMusic.StartFadeOut(scroll.GetComponent<AudioSource>(), 5f);
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene(1);
    }

}
