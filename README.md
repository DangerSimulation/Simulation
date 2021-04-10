# Simulation

Test the ability of lifeguards to assess a situation accordingly. Trainers can oversee trainees with [AdminUI](https://www.github.com/DangerSimulation/AdminUI).
Trainers can also influence the simulation with AdminUI. They control scene selection and scenario progression.

### Connectivity 

*Simulation* talks with *AdminUI* mainly with WebRTC. Simulation sends video and AdminUI sends instructions.

![The connection flow displayed in one picture](https://github.com/DangerSimulation/Documentation/blob/main/Files/ConnectionFlow.png?raw=true)

AdminUI first starts to send UDP broadcast messages and creates a websocket server. Simulation listens to broadcast
messages and connects to the websocket of the admin ui. This is possible with the sender ip address from the broadcast
message. The webrtc handshake gets initiated as soon as the websocket connection is established. The webrtc handshake
can be viewed in [this](https://developer.mozilla.org/en-US/docs/Web/API/WebRTC_API/Signaling_and_video_calling)
tutorial. The "Signalling Server" is the websocket.

We restart the connection flow once a connection is lost.

## Events

AdminUI sends instructions with events over the WebRTC data channel. These Events are processed in a single Script.
[EventManager](./Assets/Scripts/CameraStream/EventManager.cs) has multiple events/delegates to subscribe to. 
These events are:

- ScenarioSelected
  - Arguments: scenarioName
- DrowningManInitiating
  - Arguments: None
- ShowDrowningManInitiating
  - Arguments: None
- WeatherChanging
  - Arguments: weatherType
- ScenarioCanceled
  - Arguments: reason

### Listening to events 

To subscribe to an event, add a function with the right signature to the delegate/event you want to subscribe to. E.g:

```csharp
    EventManager.Instance.DrowningManInitiating += OnDrowningManInitiated;

    public void OnDrowningManInitiated(object reference, EventArgs args)
    {
        Debug.Log("DrowningMan initiated");
    }
```

### Adding new events

AdminUI sends three types of events:

- InitiatorEvents
  - All events impacting circumstances inside of a scenario
- SystemUpdates
  - Events regarding the whole application
- ScenarioSelection
  - Selection of scenarios 
  
These events are handled and converted to delegates in their respective methods: 
- `ConvertInitiatorEventMessageToEvent`
- `ConvertSystemUpdateMessageToEvent`
- `ConvertScenarioSelectedMessageToEvent`

To add a new event, you have to extend one of these methods. 
`ConvertInitiatorEventMessageToEvent` and `ConvertScenarioSelectedMessageToEvent` receive strings based on which a new 
event has to be created. `ConvertSystemUpdateMessageToEvent` gets an object of type :
```csharp
public class AdminUISystemUpdateMessage<T>
{
    public string action;

    public T additionalData;
}
```

`additionalData` has to be handled based on the action. You should be able to infer the type of `additionalData` from 
`action`. 

For each event or a group of events, add an event/delegate for the event. This uses the `EventHandler` type. Adding a 
new event has the signature `public event EventHandler <event name>`. Each event/delegate needs a method invoking it. 
This method needs the signature `protected virtual void On<event name>()`. Example: 

```csharp
public event EventHandler CarCrashed;

protected virtual void OnCarCrashed()
{
    CarCrashed?.Invoke(this, EventArgs.Empty);
}
```

If you want to have arguments with your event, create a new class extending `EventHandler`. Example:

```csharp
public class CasualtiesReportedArgs : EventArgs
{
    public string amount { get; set; }
}
```

Used like this: 

```csharp
public event EventHandler<CasualtiesReportedArgs> CasualtiesReported;

protected virtual void OnCasualtiesReported(int amount)
{
    CasualtiesReported?.Invoke(this, new CasualtiesReportedArgs() { amount = amount });
}
```

Adding a `InitiatorEvent` *CarCrashed* to `ConvertInitiatorEventMessageToEvent` would look like this: 

```csharp
switch (initiatorEventName)
{
    case "DrowningMan":
        OnDrowningManInitiating();
        break;
    case "CarCrashed":
        OnCarCrashed();
        break;
    case "CasualtiesReport":
        OnCasualtiesReported(500);
        break;
}
```
