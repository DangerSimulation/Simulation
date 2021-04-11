using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public sealed class SceneLoader : Singleton<SceneLoader>
{
    public UnityEvent OnLoadBegin = new UnityEvent();
    public LoadEndEvent OnLoadEnd = new LoadEndEvent();
    public ScreenFader screenFader = null;

    private bool isLoading = false;

    private void Awake()
    {
        SceneManager.sceneLoaded += SetActiveScene;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= SetActiveScene;
    }

    public void LoadNewScene(string sceneName)
    {
        if (!isLoading)
            StartCoroutine(LoadScene(sceneName));
    }

    private IEnumerator LoadScene(string sceneName)
    {
        isLoading = true;

        OnLoadBegin?.Invoke();
        yield return screenFader.StartFadeIn();
        yield return StartCoroutine(UnloadCurrent());

        yield return StartCoroutine(LoadNew(sceneName));
        OnLoadEnd?.Invoke(sceneName);
        yield return screenFader.StartFadeOut();

        isLoading = false;
    }

    private IEnumerator UnloadCurrent()
    {
        AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());

        while(!unloadOperation.isDone)
            yield return null;
    }

    private IEnumerator LoadNew(string sceneName)
    {
        AsyncOperation unloadOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        while (!unloadOperation.isDone)
            yield return null;
    }

    private void SetActiveScene(Scene scene, LoadSceneMode mode)
    {
        SceneManager.SetActiveScene(scene);
    }
}

[System.Serializable]
public class LoadEndEvent: UnityEvent<string>
{

}