using UnityEngine;
using System;
using Newtonsoft.Json;
using System.Runtime.Remoting;

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

    public event EventHandler<AdminUIMessageArgs> AdminUIMessage;

    protected virtual void OnAdminUIMessage(AdminUIMessage<dynamic> message)
    {
        AdminUIMessage?.Invoke(this, new AdminUIMessageArgs() { message = message });
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
            default:
                SendUnknownEventMessage(scenarioName);
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

    //This emits Rain, BlueSky or Cloudy
    protected virtual void OnWeatherChanging(string weatherType)
    {
        WeatherChanging?.Invoke(this, new WeatherChangingArgs() { type = weatherType});
    }

    public event EventHandler<TimeChangingArgs> TimeChanging;

    //This emits a time with the pattern HH:MM. 
    protected virtual void OnTimeChanging(string time)
    {
        TimeChanging?.Invoke(this, new TimeChangingArgs() { time = time });
    }

    public event EventHandler<TemperatureChangingArgs> TemperatureChanging;

    //This emits Chilly, Normal, Hot, Very Hot. This is equal to 9 °C, 19 °C, 29 °C, 39 °C
    protected virtual void OnTemperatureChanging(string temperature)
    {
        TemperatureChanging?.Invoke(this, new TemperatureChangingArgs() { temperature = temperature });
    }

    public void ConvertScenarioEventMessageToEvent(dynamic data)
    {
        switch (data.GetType().Name)
        {
            case "String":
                HandleStringEvents(data);
                break;
            case "JObject":
                HandleObjectEvents(data);
                break;
        }

    }

    private void HandleObjectEvents(dynamic data)
    {
        AdminUIScenarioEventMessage<dynamic> eventData = JsonConvert.DeserializeObject<AdminUIScenarioEventMessage<dynamic>>(JsonConvert.SerializeObject(data));

        switch(eventData.eventName)
        {
            case "WeatherChange":
                HandleWeatherChange(eventData.additionalData);
                break;
            case "TimeChange":
                HandleTimeChange(eventData.additionalData);
                break;
            case "TemperatureChange":
                HandleTemperatureChange(eventData.additionalData);
                break;
            default:
                SendUnknownEventMessage(eventData.eventName);
                break;
        }
    }

    private void HandleStringEvents(dynamic data)
    {
        string eventName = (string)data;


        switch (eventName)
        {
            case "DrowningMan":
                OnDrowningManInitiating();
                break;
            case "ShowDrowningMan":
                OnShowDrowningManInitiating();
                break;
            default:
                SendUnknownEventMessage(eventName);
                break;
        }
    }

    private void HandleTemperatureChange(dynamic data)
    {
        string temperature = (string)data;

        OnTemperatureChanging(temperature);
    }

    private void HandleTimeChange(dynamic data)
    {
        string time = (string)data;

        OnTimeChanging(time);
    }

    private void HandleWeatherChange(dynamic data)
    {
        string weatherType = (string)data;

        OnWeatherChanging(weatherType);
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
                SendUnknownEventMessage(adminUIUpdate.action);
                break;
        }
    }

    private void SendUnknownEventMessage(string eventName)
    {
        Debug.LogErrorFormat("Unknown event {0}", eventName);

        AdminUIMessage<dynamic> message = new AdminUIMessage<dynamic>()
        {
            eventType = "SystemUpdate",
            data = new AdminUISystemUpdateMessage<dynamic>()
            {
                action = "UnknownEvent",
                additionalData = eventName
            }
        };
        OnAdminUIMessage(message);
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

public class AdminUIMessage<T>
{
    public string eventType { get; set; }
    public T data { get; set; }
}

public class AdminUISystemUpdateMessage<T>
{
    public string action;

    public T additionalData;
}

public class AdminUIScenarioEventMessage<T>
{
    public string eventName;

    public T additionalData;
}

public class AdminUIMessageArgs : EventArgs
{
    public AdminUIMessage<dynamic> message { get; set; }
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

public class TimeChangingArgs : EventArgs
{
    public string time { get; set; }
}

public class TemperatureChangingArgs : EventArgs
{
    public string temperature { get; set; }
}
