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
        string scenarioName = (string)data;

        switch (scenarioName)
        {
            case "Strand":
                OnScenarioSelected("Strand");
                break;
        }
    }

    public event EventHandler DrowningManInitiating;

    protected virtual void OnDrowningManInitiating()
    {
        DrowningManInitiating?.Invoke(this, EventArgs.Empty);
    }

    public event EventHandler ShowDrowningManInitiating;

    protected virtual void OnShowDrowningManInitiating()
    {
        ShowDrowningManInitiating?.Invoke(this, EventArgs.Empty);
    }

    public event EventHandler<WeatherChangingArgs> WeatherChanging;

    protected virtual void OnWeatherChanging(string weatherType)
    {
        Debug.Log("WeatherChanging");
        WeatherChanging?.Invoke(this, new WeatherChangingArgs() { type = weatherType});
    }

    public void ConvertInitiatorEventMessageToEvent(dynamic data)
    {
        Debug.Log("Converting");

        string initiatorEventName = (string)data;

        switch (initiatorEventName)
        {
            case "DrowningMan":
                OnDrowningManInitiating();
                break;
            case "ShowDrowningMan":
                OnShowDrowningManInitiating();
                break;
            case "Rain":
                OnWeatherChanging("Rain");
                break;
            case "BlueSky":
                OnWeatherChanging("BlueSky");
                break;
            case "Cloudy":
                OnWeatherChanging("Cloudy");
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

public class WeatherChangingArgs : EventArgs
{
    public string type { get; set; }
}
