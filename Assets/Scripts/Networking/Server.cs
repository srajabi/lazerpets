using UnityEngine;
using UnityEngine.Networking;

public class Server
{
    public Server(int port)
    {
        NetworkServer.RegisterHandler(MsgType.Connect, OnConnect);
        NetworkServer.RegisterHandler(MsgType.Disconnect, OnPlayerDisconnect);
        NetworkServer.RegisterHandler(MyMessageTypes.HeartBeat, OnHeartBeatReceived);
        NetworkServer.RegisterHandler(MyMessageTypes.Strings, OnStringReceived);
        NetworkServer.Listen(port);
    }

    public void OnHeartBeatReceived(NetworkMessage msg)
    {
        Debug.Log(
            string.Format("OnHeartBeatReceived: {0}",
            msg.ReadMessage<HeartBeat>())
            );
    }

    public void OnStringReceived(NetworkMessage msg)
    {
        Debug.Log(
            string.Format("OnStringReceived: {0}",
            msg.reader.ReadString())
        );
    }

    private void OnPlayerDisconnect(NetworkMessage netMsg)
    {
        Debug.Log("player disconnected!");
    }

    private void OnConnect(NetworkMessage netMsg)
    {
        Debug.Log("player connected!");
    }
}
