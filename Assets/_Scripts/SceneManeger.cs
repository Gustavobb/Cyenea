using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManeger : MonoBehaviour
{
    Animator animator;
    public TextAsset jsonFile;
    int nextSpawnPointId;
    public int endMapid;
    int sceneId;
    public bool loadNextScene;

    public FadeMusic fadeMusic;

    private void Start() 
    {
        animator = this.gameObject.GetComponent<Animator>();
        loadNextScene = false;
    }
    
    void Update()
    {
        if (loadNextScene)
        {
            Debug.Log("value.TargetSceneName");
            loadNextScene = false;
            HandleScene();
        }
    }
    public void PlayButton()
    {
        fadeMusic.StartFadeOut(fadeMusic.GetComponent<AudioSource>(), 3f);
        StartCoroutine(PlayButtonCoroutine());
    }

    public void CreditsButton()
    {
        fadeMusic.StartFadeOut(fadeMusic.GetComponent<AudioSource>(), 3f);
        StartCoroutine(CreditsButtonCoroutine());
    }

    public void LoadFinal()
    {
        fadeMusic.StartFadeOut(fadeMusic.GetComponent<AudioSource>(), 3f);
        StartCoroutine(LoadFinalIE());
    }

    public void MenuButton()
    {
        fadeMusic.StartFadeOut(fadeMusic.GetComponent<AudioSource>(), 3f);
        StartCoroutine(MenuButtonCoroutine());
    }

    public void ReloadScene()
    {
        StartCoroutine(ReloadSceneCoroutine());
    }

    #region Functions

    public void HandleJson()
    {
        SceneData sceneDatajson = JsonUtility.FromJson<SceneData>(jsonFile.text);

        foreach (Scenes value in sceneDatajson.scenes)
        {
            // Debug.Log(value.TargetSceneName);

            if (value.SceneName == SceneManager.GetActiveScene().name)
            {
                if (value.EndMapId == endMapid)
                {
                    nextSpawnPointId = value.TargetEndMapId;
                    sceneId = value.TargetSceneId;
                    return;
                }
            }
        }
    }
    public void HandleScene()
    {
        fadeMusic.StartFadeOut(fadeMusic.GetComponent<AudioSource>(), 3f);
        HandleJson();
        PlayerData data = SaveSystem.LoadPlayer();
        data.currentSpawnPointIdData = nextSpawnPointId;

        SaveSystem.SavePlayer(data.activePlayerIdData, data.currentSpawnPointIdData, data.healthData, data.maxHealthData, data.manaData, data.obtainedPlayersNameData, data.doorsClosed);
        StartCoroutine(LoadNextScene());
    }
    #endregion

    private IEnumerator PlayButtonCoroutine(){
        animator.SetTrigger("End");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.sceneCountInBuildSettings - 2);
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
    private IEnumerator LoadNextScene(){
        animator.SetTrigger("End");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(sceneId);
    }

    private IEnumerator LoadFinalIE(){
        animator.SetTrigger("End");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(15);
    }
}