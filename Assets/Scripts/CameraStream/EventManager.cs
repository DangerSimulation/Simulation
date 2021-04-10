using UnityEngine;
using System;
using Newtonsoft.Json;

public class EventManager
{
    private static readonly EventManager instance = new EventManager();

    // Explicit static constructor to tell C# compiler
    // not to mark type as beforefieldinit
    static EventManager()
    {
    }

    private EventManager()
    {
    }

    public static EventManager Instance
    {
        get
        {
            return instance;
        }
    }

    public event EventHandler<ScenarioSelectedArgs> ScenarioSelected;

    protected virtual void OnScenarioSelected(string scenarioName)
    {
        ScenarioSelected?.Invoke(this, new ScenarioSelectedArgs() { scenarioName = scenarioName });
    }

    public void ConvertScenarioSelectedMessageToEvent(dynamic data)
    {
        string sceneName = (string)data;

        switch (sceneName)
        {
            case "Strand":
                OnScenarioSelected("Beach");
                break;
        }
    }

    public event EventHandler DrowningManInitiating;

    protected virtual void OnDrowningManInitiating()
    {
        DrowningManInitiating?.Invoke(this, EventArgs.Empty);
    }

    public void ConvertInitiatorEventMessageToEvent(dynamic data)
    {
        string initiatorEventName = (string)data;

        switch (initiatorEventName)
        {
            case "DrowningMan":
                OnDrowningManInitiating();
                break;
        }
    }

    public void ConvertSystemUpdateMessageToEvent(dynamic data)
    {
        AdminUISystemUpdateMessage<dynamic> adminUIUpdate = JsonConvert.DeserializeObject<AdminUISystemUpdateMessage<dynamic>>(JsonConvert.SerializeObject(data));

        switch(adminUIUpdate.action)
        {
            case "ScenarioCancel":
                HandleScenarioCanceleMessage(adminUIUpdate.additionalData);
                break;
            case "Ping":
                Debug.Log("Ping");
                break;
            default:
                Debug.LogErrorFormat("Unknown action {0}", adminUIUpdate.action);
                break;
        }
    }

    public event EventHandler<ScenarioCanceledArgs> ScenarioCanceled;

    protected virtual void OnSceneCanceled(string reason)
    {
        ScenarioCanceled?.Invoke(this, new ScenarioCanceledArgs() { reason = reason });
    }

    public void HandleScenarioCanceleMessage(dynamic data)
    {
        string additionalData = (string)data;

        OnSceneCanceled(additionalData);
    }

}

public class AdminUISystemUpdateMessage<T>
{
    public string action;

    public T additionalData;
}

public class ScenarioSelectedArgs: EventArgs
{
    public string scenarioName { get; set; }
}

public class ScenarioCanceledArgs : EventArgs
{
    public string reason { get; set; }
}
