using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    Animator animator;

    private void Start() 
    {
        animator = this.gameObject.GetComponent<Animator>();
    }
    
    public void PlayButton()
    {
        StartCoroutine(PlayButtonCoroutine());
    }

    public void CreditsButton()
    {
        StartCoroutine(CreditsButtonCoroutine());
    }

    public void MenuButton()
    {
        StartCoroutine(MenuButtonCoroutine());
    }

    public void ReloadScene()
    {
        StartCoroutine(ReloadSceneCoroutine());
    }

    #region papaapap
    public void HandleScene(int id)
    {
        
    }
    #endregion

    private IEnumerator PlayButtonCoroutine(){
        animator.SetTrigger("End");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    private IEnumerator CreditsButtonCoroutine(){
        animator.SetTrigger("End");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.sceneCountInBuildSettings - 1);
    }
    private IEnumerator MenuButtonCoroutine(){
        animator.SetTrigger("End");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(0);
    }
    private IEnumerator ReloadSceneCoroutine(){
        animator.SetTrigger("End");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}