using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WrappedNetworkServerSimple : NetworkServerSimple {

	public override void OnConnected(NetworkConnection conn)
    {
        conn.RegisterHandler(0, OnMsgReceived);
        conn.RegisterHandler(1, OnMsgReceived2);

        base.OnConnected(conn);
	}

    void OnMsgReceived(NetworkMessage msg)
    {
        Debug.Log(" " + msg + " " + msg.reader.ReadString());
    }

    void OnMsgReceived2(NetworkMessage msg)
    {
        Debug.Log("2 " + msg + " " + msg.reader.ReadMessage<ConnectionManager.ScoreMessage>());
    }

    public override void OnData(NetworkConnection conn, int receivedSize, int channelId)
	{
        Debug.Log(" " + conn + " " + receivedSize + " " + channelId);
		base.OnData(conn, receivedSize, channelId);
	}

	public override void OnDisconnected(NetworkConnection conn)
	{
		base.OnDisconnected(conn);
	}

}
