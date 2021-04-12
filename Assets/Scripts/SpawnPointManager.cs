using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointManager : MonoBehaviour
{
    [SerializeField]
    public GameObject player;
    [SerializeField]
    public GameObject cube;

    // Start is called before the first frame update
    void Start()
    {
        SceneLoader.Instance.OnLoadEnd.AddListener(OnSceneLoadEnd);
        SceneLoader.Instance.OnLoadBegin.AddListener(OnSceneLoadStart);
    }

    private void OnDestroy()
    {
        SceneLoader.Instance.OnLoadEnd.RemoveListener(OnSceneLoadEnd);
        SceneLoader.Instance.OnLoadBegin.RemoveListener(OnSceneLoadStart);
    }

    public void OnSceneLoadStart(string sceneName)
    {
        cube.transform.position = player.transform.position + player.transform.forward * 2;
    }

    public void OnSceneLoadEnd(string sceneName)
    {
        switch(sceneName)
        {
            case "Menu":
                player.transform.position = new Vector3(0, 2, 0);
                cube.transform.position = player.transform.position + player.transform.forward;
                break;
            case "Strand":
                player.transform.position = new Vector3(290, 102, 298);
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
