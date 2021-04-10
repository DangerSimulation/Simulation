using Unity.WebRTC;

public class SignalingMessage
{
    public string type;
    public string sdp;
    public string candidate;
    public string sdpMid;
    public int sdpMLineIndex;

    public SignalingMessage(string type, string sdp)
    {
        this.type = type;
        this.sdp = sdp;
    }

    public SignalingMessage(string candidate, string sdpMid, int sdpMLineIndex)
    {
        this.type = "candidate";
        this.candidate = candidate;
        this.sdpMid = sdpMid;
        this.sdpMLineIndex = sdpMLineIndex;
    }

    public static SignalingMessage FromDesc(ref RTCSessionDescription desc)
    {
        return new SignalingMessage(desc.type.ToString().ToLower(), desc.sdp);
    }

    public static SignalingMessage FromCand(RTCIceCandidate cand)
    {
        return new SignalingMessage(cand.Candidate, cand.SdpMid, cand.SdpMLineIndex.Value);
    }

    public RTCSessionDescription ToDesc()
    {
        return new RTCSessionDescription
        {
            type = type == "offer" ? RTCSdpType.Offer :
                    type == "answer" ? RTCSdpType.Answer :
                    type == "pranswer" ? RTCSdpType.Pranswer :
                    RTCSdpType.Rollback,
            sdp = sdp
        };
    }

    public RTCIceCandidate ToCand()
    {
        var candidateInfo = new RTCIceCandidateInit
        {
            candidate = candidate,
            sdpMid = sdpMid,
            sdpMLineIndex = sdpMLineIndex
        };
        var cand = new RTCIceCandidate(candidateInfo);
        return cand;
    }

    public string ToJson()
    {
        if (!string.IsNullOrEmpty(sdp))
            return $"{{\"type\":\"{type.ToString().ToLower()}\", \"sdp\":\"{sdp}\"}}";
        else
            return $"\"candidate\":\"{candidate}\", \"sdpMid\":\"{sdpMid}\", \"sdpMLineIndex\":{sdpMLineIndex}}}";
    }
}
