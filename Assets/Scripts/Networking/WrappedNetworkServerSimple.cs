using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WrappedNetworkServerSimple : NetworkServerSimple {

	public event Action<NetworkConnection> OnClientConnected;
	public event Action<NetworkConnection> OnClientData;
	public event Action<NetworkConnection> OnClientDisconnected;

	public override void OnConnected(NetworkConnection conn)
    {
		Debug.Log("ON CLIENT CONNECTED");
		//conn.RegisterHandler(MsgType.Highest + 1, OnMsgReceived);
		//conn.RegisterHandler(MsgType.Highest + 2, OnMsgReceived2);

        base.OnConnected(conn);
		OnClientConnected?.Invoke(conn);
	}

    public override void OnData(NetworkConnection conn, int receivedSize, int channelId)
	{
		Debug.Log("OnData " + conn + " " + receivedSize + " " + channelId);
		base.OnData(conn, receivedSize, channelId);
		OnClientData?.Invoke(conn);
	}

	public override void OnDisconnected(NetworkConnection conn)
	{
		Debug.Log("ON OnDisconnected");
		base.OnDisconnected(conn);
		OnClientDisconnected?.Invoke(conn);
	}

}
