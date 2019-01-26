using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public enum ConnectionMode
{
    SERVER,
    CLIENT
}

public class GameMsgType
{
	public const short InitializeNewPlayer = MsgType.Highest + 1;
}

public class InitializeNewPlayerData : MessageBase
{
	public int NumActivePlayers;
}


public interface IConnection
{
	IEnumerator Initialize();

    bool IsConnected
	{
		get;
	}

	void Shutdown();

	void Update();
}

public abstract class BaseConnection : IConnection
{
	protected const int CONNECTION_PORT = 64000;

	public abstract bool IsConnected
	{
		get;
	}

	public abstract IEnumerator Initialize();
	public abstract void Shutdown();

    public virtual void Update()
	{
		
	}
}

public class ServerConnection : BaseConnection
{
	WrappedNetworkServerSimple networkServerSimple;

	private bool _isConnected = false;
	public override bool IsConnected
	{
		get
		{
			return _isConnected;
		}
	}

	public override IEnumerator Initialize()
	{
		networkServerSimple = new WrappedNetworkServerSimple();

        //networkServerSimple.OnClientConnected += OnClientConnected;
        //networkServerSimple.OnClientDisconnected += OnClientDisconnected;

        networkServerSimple.Initialize();
        networkServerSimple.Listen(CONNECTION_PORT);
		_isConnected = true;

		yield break;
	}

	public override void Shutdown()
	{
		if (networkServerSimple == null)
		{
			return;
		}
		networkServerSimple.ClearHandlers();
		networkServerSimple.DisconnectAllConnections();
		networkServerSimple.Stop();
		networkServerSimple = null;
		_isConnected = false;
	}

	public override void Update()
	{
		base.Update();
		networkServerSimple.Update();
	}

}

public class ClientConnection : BaseConnection
{
	const int CONNECTION_TIMEOUT_SECONDS = 2;

	NetworkClient client;
	private readonly string serverAddress;

	public ClientConnection(string serverAddress)
	{
		this.serverAddress = serverAddress;
	}

	public override bool IsConnected => client.isConnected;

	public override IEnumerator Initialize()
	{
		client = new NetworkClient();      

        //client.RegisterHandler(GameMsgType.InitializeNewPlayer, HandleInitializeNewPlayer);      

		client.Connect(serverAddress, CONNECTION_PORT);      

		var time = Time.time + CONNECTION_TIMEOUT_SECONDS;

        yield return new WaitUntil(() => client.isConnected || Time.time > time);
	}

	public override void Shutdown()
	{
		if (client == null)
		{
			return;
		}
		client.Shutdown();
		client = null;
	}


}

public class ConnectionManager {

	private IConnection activeConnection;   

	public event Action OnActivePlayerChange;

	public ConnectionMode connectionMode
	{
		private set;
		get;
	}

	public IEnumerator Initialize()
	{

		IConnection pendingConnection = new ClientConnection("localhost");

		yield return pendingConnection.Initialize();

		if (pendingConnection.IsConnected)
		{
			activeConnection = pendingConnection;
			connectionMode = ConnectionMode.CLIENT;
		}
		else
		{
			pendingConnection.Shutdown();

			pendingConnection = new ServerConnection();

			yield return pendingConnection.Initialize();

			if (pendingConnection.IsConnected)
			{
				activeConnection = pendingConnection;
				connectionMode = ConnectionMode.SERVER;
			}         
		}    
		//OnActivePlayerChange.Invoke();      
	}

    public int NumActivePlayers
	{
		get;
		private set;
	}


	private void HandleInitializeNewPlayer(NetworkMessage message)
	{
		var data = message.ReadMessage<InitializeNewPlayerData>();
		NumActivePlayers = data.NumActivePlayers;
		Debug.Log("HandleInitializeNewPlayer NumActivePlayers:" + data.NumActivePlayers);
		OnActivePlayerChange.Invoke();

	}

	//public void OnClientConnected(NetworkConnection conn)
	//{
	//	NumActivePlayers = networkServerSimple.connections.Count + 1;
	//	conn.Send(GameMsgType.InitializeNewPlayer, new InitializeNewPlayerData(){
	//		NumActivePlayers = NumActivePlayers
	//	});      
	//	OnActivePlayerChange.Invoke();


	//}
    
	//private void OnClientDisconnected(NetworkConnection conn)
	//{
	//	OnActivePlayerChange.Invoke();
	//}

	public void Update()
	{
		if (activeConnection != null)
		{
			activeConnection.Update();
		}
	}
    

}
