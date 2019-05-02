using System;
using UnityEngine;


public class WrappedNetworkDiscovery: UnityEngine.Networking.NetworkDiscovery
{
	public event Action<string, string> OnServerFound;

	public override void OnReceivedBroadcast(string fromAddress, string data)
	{
		Debug.Log("fromAddress" + fromAddress);
		Debug.Log("data" + data);

		OnServerFound?.Invoke(fromAddress, data);



	}
}

