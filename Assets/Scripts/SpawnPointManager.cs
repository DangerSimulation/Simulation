using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointManager : MonoBehaviour
{
    [SerializeField]
    public GameObject player;
    
    // Start is called before the first frame update
    void Start()
    {
        SceneLoader.Instance.OnLoadEnd.AddListener(OnSceneLoadEnd);
    }

    public void OnSceneLoadEnd(string sceneName)
    {
        switch(sceneName)
        {
            case "Menu":
                player.transform.position = new Vector3(0, 2, 0);
                break;
            case "Beach":
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
