using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelector : MonoBehaviour
{
    public void LoadBeach()
    {
        SceneLoader.Instance.LoadNewScene("Beach");
    }
}
