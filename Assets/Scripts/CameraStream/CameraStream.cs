using System.Collections;
using UnityEngine;
using Unity.WebRTC;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Net;
using NativeWebSocket;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;

public class CameraStream : MonoBehaviour
{

    private RTCPeerConnection localConnection;
    private RTCDataChannel dataChannel;
    private WebSocket socket;


    [SerializeField] private Camera cam;

    private RTCConfiguration conf = new RTCConfiguration
    {
        iceServers = new[] { new RTCIceServer { urls = new[] { "stun:stun.l.google.com:19302" } } }
    };

    private RTCOfferOptions offerOption = new RTCOfferOptions
    {
        iceRestart = false,
        offerToReceiveAudio = false,
        offerToReceiveVideo = true
    };
    private RTCAnswerOptions answerOptions = new RTCAnswerOptions
    {
        iceRestart = false
    };

    private void Awake()
    {
        WebRTC.Initialize(EncoderType.Software);
    }

    private void Start()
    {
        WaitForBroadCastMessage();

        StartCoroutine(WebRTC.Update());

    }

    private void CreatePeerConnection()
    {
        Debug.Log("Create peer");
        localConnection = new RTCPeerConnection(ref conf);

        localConnection.OnIceCandidate = candidate =>
        {
            SendIce(candidate);
        };

        localConnection.OnTrack = track =>
        {
            Debug.LogWarning("Got track");
        };

        localConnection.OnDataChannel = channel =>
        {
            dataChannel = channel;
            dataChannel.OnOpen = () =>
            {
                Debug.Log("Data channel opened");
            };
            dataChannel.OnClose = () =>
            {
                Debug.Log("Data channel closed");
            };
            dataChannel.OnMessage = message =>
            {
                OnDataChannelMessage(Encoding.UTF8.GetString(message));
            };
        };


        var sendTrack = cam.CaptureStreamTrack(1920, 1080, 2000000);
        localConnection.AddTrack(sendTrack);

    }

    private void OnDataChannelMessage(string message)
    {
        Debug.LogFormat("Received message: {0}", message);
     
        AdminUIMessage<dynamic> adminUIMessage = JsonConvert.DeserializeObject<AdminUIMessage<dynamic>>(message);
        
        switch(adminUIMessage.eventType)
        {
            case "ScenarioSelection":
                EventManager.Instance.ConvertScenarioSelectedMessageToEvent(adminUIMessage.data);
                break;
            case "SystemUpdate":
                EventManager.Instance.ConvertSystemUpdateMessageToEvent(adminUIMessage.data);
                break;
            case "ScenarioEvent":
                EventManager.Instance.ConvertScenarioEventMessageToEvent(adminUIMessage.data);
                break;
            default:
                Debug.LogErrorFormat("{0} is not a valid eventType", adminUIMessage.eventType);
                break;
        }
    }

    public enum Side
    {
        Local,
        Remote
    }

    IEnumerator CreateDescription(RTCSdpType type)
    {
        Debug.Log($"[Create {type}]");

        var op = type == RTCSdpType.Offer ? localConnection.CreateOffer(ref offerOption) : localConnection.CreateAnswer(ref answerOptions);
        yield return op;
        if (op.IsError)
            Debug.LogError(op.Error.message);
        else
        {
            yield return SetDescription(op.Desc, Side.Local);
        }
    }

    IEnumerator SetDescription(RTCSessionDescription desc, Side side)
    {
        Debug.Log($"[Set {side} {desc.type}]");

        var op = side == Side.Local ? localConnection.SetLocalDescription(ref desc) : localConnection.SetRemoteDescription(ref desc);
        yield return op;
        if (op.IsError)
        {
            Debug.LogError(op.Error.message);
        }
        else if (desc.type == RTCSdpType.Offer)
        {
            StartCoroutine(CreateDescription(RTCSdpType.Answer));
        }
        else if (side == Side.Local && desc.type == RTCSdpType.Answer)
        {
            SendDescription(ref desc);
        }
    }

    public void SendDescription(ref RTCSessionDescription desc)
    {
        Debug.Log($"Send {desc.type}");
        var json = JsonUtility.ToJson(SignalingMessage.FromDesc(ref desc));
        socket.SendText(json);
    }

    public void SendIce(RTCIceCandidate cand)
    {
        Debug.Log($"Send Candidate");
        var json = JsonUtility.ToJson(SignalingMessage.FromCand(cand));
        socket.SendText(json);
    }

    private void HandleWebsocketMessage(byte[] data)
    {
        SignalingMessage message = JsonUtility.FromJson<SignalingMessage>(Encoding.UTF8.GetString(data));
        Debug.LogFormat("Receivied message with type {0}", message.type);

        if (localConnection == null)
        {
            CreatePeerConnection();
        }

        if (!string.IsNullOrEmpty(message.sdp))
        {
            StartCoroutine(SetDescription(message.ToDesc(), Side.Remote));
        }

        if (!string.IsNullOrEmpty(message.candidate))
        {
            localConnection.AddIceCandidate(message.ToCand());
        }

    }


    void Update()
    {
        if (socket != null)
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            socket.DispatchMessageQueue();
#endif
        }

    }

    private void SetUpWebSocket(string url)
    {
        Debug.Log("Try to connect to : " + url);

        socket = new WebSocket(url, headers: new Dictionary<string, string>()
        {
            { "From", "Simulation" }
        });

        socket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
        };

        socket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
        };

        socket.OnClose += (e) =>
        {
            Debug.Log("Connection closed!");
            StartReconnect();
        };


        socket.OnMessage += HandleWebsocketMessage;

        socket.Connect();
    
    }

    private void StartReconnect()
    {
        CloseEverything();

        WaitForBroadCastMessage();
    }

    private void CloseEverything()
    {
        if(localConnection.ConnectionState != RTCPeerConnectionState.Closed)
        {
            localConnection.Close();
        }
        if(dataChannel.ReadyState != RTCDataChannelState.Closed)
        {
            dataChannel.Close();
        }

        localConnection = null;
        dataChannel = null;

        socket = null;
    }

    private void WaitForBroadCastMessage()
    {
        int PORT = 12548;
        UdpClient udpListener = new UdpClient(PORT);
        Debug.Log("Listen to broadcast on port: " + PORT);

        var result = udpListener.ReceiveAsync();

        udpListener.BeginReceive(new AsyncCallback((IAsyncResult res) =>
        {
            IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Broadcast, PORT);
            byte[] received = udpListener.EndReceive(res, ref remoteIpEndPoint);

            Debug.Log(string.Format("Read message: {0} from {1}", received, remoteIpEndPoint.Address));
            string websocktUrl = string.Format("ws://{0}:{1}", remoteIpEndPoint.Address, 11653);

            SetUpWebSocket(websocktUrl);
            udpListener.Close();
        }), null);

    }

    void OnApplicationQuit()
    {
        CloseEverything();
    }

}

public class AdminUIMessage<T>
{
    public string eventType { get; set; }
    public T data { get; set; }
}