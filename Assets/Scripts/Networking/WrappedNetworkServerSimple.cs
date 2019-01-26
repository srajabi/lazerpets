using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WrappedNetworkServerSimple : NetworkServerSimple {

	public override void OnConnected(NetworkConnection conn)
    {
		Debug.Log("ON CONNECTED");
		//conn.RegisterHandler(MsgType.Highest + 1, OnMsgReceived);
		//conn.RegisterHandler(MsgType.Highest + 2, OnMsgReceived2);

        base.OnConnected(conn);
	}

    //void OnMsgReceived(NetworkMessage msg)
    //{
    //    Debug.Log(" " + msg + " " + msg.reader.ReadString());
    //}

    //void OnMsgReceived2(NetworkMessage msg)
    //{
    //    Debug.Log("2 " + msg + " " + msg.reader.ReadMessage<ConnectionManager.ScoreMessage>());
    //}

    public override void OnData(NetworkConnection conn, int receivedSize, int channelId)
	{
		Debug.Log("OnData " + conn + " " + receivedSize + " " + channelId);
		base.OnData(conn, receivedSize, channelId);
	}

	public override void OnDisconnected(NetworkConnection conn)
	{
		Debug.Log("ON OnDisconnected");
		base.OnDisconnected(conn);
	}

}
