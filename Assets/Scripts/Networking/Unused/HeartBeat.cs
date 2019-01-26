using UnityEngine;
using UnityEngine.Networking;

public class HeartBeat : MessageBase
{
    public float TimeStamp
    {
        get;
        set;
    }

    public override string ToString()
    {
        return "Heartbeat: " + TimeStamp;
    }

    public HeartBeat()
    {
        TimeStamp = Time.time;
    }
}