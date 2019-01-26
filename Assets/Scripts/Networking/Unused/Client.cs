using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Client
{
    NetworkClient client;

    public Client(string hostname, int port)
    {
        client = new NetworkClient();
        client.Connect(hostname, port);
        client.RegisterHandler(MsgType.Connect, OnClientConnected);
    }

    private void OnClientConnected(NetworkMessage netMsg)
    {
        Debug.Log("Connected: " + netMsg);

        TheGame.Instance.StartCoroutine(SendHeartBeat());
    }

    IEnumerator SendHeartBeat()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);

            client.Send(MyMessageTypes.HeartBeat, new HeartBeat());

            NetworkWriter writer = new NetworkWriter();
            writer.StartMessage(MyMessageTypes.Strings);
            writer.Write("Hello");
            writer.FinishMessage();
            client.SendWriter(writer, Channels.DefaultUnreliable);
        }
    }
}
