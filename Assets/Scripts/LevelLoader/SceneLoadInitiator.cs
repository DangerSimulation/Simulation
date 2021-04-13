using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoadInitiator : MonoBehaviour
{
    void Start()
    {
        EventManager.Instance.ScenarioSelected += OnScenarioSelected;
        EventManager.Instance.ScenarioCanceled += OnScenarioCancled;
    }

    public void OnScenarioSelected(object reference, ScenarioSelectedArgs args)
    {
        SceneLoader.Instance.LoadNewScene(args.scenarioName);
    }

    public void OnScenarioCancled(object reference, ScenarioCanceledArgs args)
    {
        SceneLoader.Instance.LoadNewScene("Menu");
    }
}