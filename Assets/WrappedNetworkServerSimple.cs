using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WrappedNetworkServerSimple : NetworkServerSimple {

	public override void OnConnected(NetworkConnection conn)
	{
		base.OnConnected(conn);
	}

	public override void OnData(NetworkConnection conn, int receivedSize, int channelId)
	{
		base.OnData(conn, receivedSize, channelId);
	}

	public override void OnDisconnected(NetworkConnection conn)
	{
		base.OnDisconnected(conn);
	}

}
