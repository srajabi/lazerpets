using System;
using UnityEngine.Networking;

public static class MyMessageTypes
{
    public const int HeartBeat = MsgType.Highest + 1;
    public const int Strings = MsgType.Highest + 2;
}
