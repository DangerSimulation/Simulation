using UnityEngine;
using System;
using Newtonsoft.Json;
using System.Runtime.Remoting;
using Newtonsoft.Json.Linq;

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

    protected virtual void OnAdminUIMessage(string message)
    {
        AdminUIMessage?.Invoke(this, new AdminUIMessageArgs() { message = message });
    }

    public event EventHandler<ScenarioSelectedArgs> ScenarioSelected;

    protected virtual void OnScenarioSelected(string scenarioName)
    {
        Debug.LogFormat("OnScenarioSelected {0}", scenarioName);

        ScenarioSelected?.Invoke(this, new ScenarioSelectedArgs() { scenarioName = scenarioName });
    }

    public void ConvertScenarioSelectedMessageToEvent(string data)
    {
        string scenarioName = data;
        
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

    public void ConvertScenarioEventMessageToEvent(string message)
    {
        JObject data = JObject.Parse(message);

        string eventName = (string)data.SelectToken("eventName");
        
        //Return in cases to skip the second case statement and parsing of additional data
        switch (eventName)
        {
            case "DrowningMan":
                OnDrowningManInitiating();
                return;
            case "ShowDrowningMan":
                OnShowDrowningManInitiating();
                return;
        }

        //Events above dont have additional data. Create other case statments should new events have different types for additional data
        string additionalData = (string)data.SelectToken("additionalData");

        switch (eventName)
        {
            case "WeatherChange":
                HandleWeatherChange(additionalData);
                break;
            case "TimeChange":
                HandleTimeChange(additionalData);
                break;
            case "TemperatureChange":
                HandleTemperatureChange(additionalData);
                break;
            default:
                SendUnknownEventMessage(eventName);
                break;
        }

    }

    private void HandleTemperatureChange(string data)
    {
        string temperature = data;

        OnTemperatureChanging(temperature);
    }

    private void HandleTimeChange(string data)
    {
        string time = data;

        OnTimeChanging(time);
    }

    private void HandleWeatherChange(string data)
    {
        string weatherType = data;

        OnWeatherChanging(weatherType);
    }

    public void ConvertSystemUpdateMessageToEvent(string data)
    {
        AdminUISystemUpdateMessage<string> adminUIUpdate = JsonConvert.DeserializeObject<AdminUISystemUpdateMessage<string>>(data);

        switch(adminUIUpdate.action)
        {
            case "ScenarioCancel":
                HandleScenarioCanceledMessage(adminUIUpdate.additionalData);
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

        AdminUIMessage<AdminUISystemUpdateMessage<string>> message = new AdminUIMessage<AdminUISystemUpdateMessage<string>>()
        {
            eventType = "SystemUpdate",
            data = new AdminUISystemUpdateMessage<string>()
            {
                action = "UnknownEvent",
                additionalData = eventName
            }
        };
        OnAdminUIMessage(JsonConvert.SerializeObject(message));
    }

    public event EventHandler<ScenarioCanceledArgs> ScenarioCanceled;

    protected virtual void OnSceneCanceled(string reason)
    {
        ScenarioCanceled?.Invoke(this, new ScenarioCanceledArgs() { reason = reason });
    }

    public void HandleScenarioCanceledMessage(string data)
    {
        string additionalData = data;

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
    public string message { get; set; }
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
