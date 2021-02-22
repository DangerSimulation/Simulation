using UnityEngine;
using UnityEngine.SceneManagement;

public class BuildLoader : MonoBehaviour
{

    //Loads the Menu Scene on startup and adds it to the persistant scene
    //This is necessary to hook the menu scene up with the interaction manager from the persistant scene
    private void Awake()
    {
        if (!Application.isEditor)
            SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Additive);
    }
}
