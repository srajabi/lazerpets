using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public enum ConnectionMode
{
    SERVER,
    CLIENT
}

public class GameMsgType
{
	public const short UpdateActivePlayers = MsgType.Highest + 1;
}

public class InitializeNewPlayerData : MessageBase
{
	public int NumActivePlayers;
}

public class PlayersUpdateMessage : MessageBase
{
    public class PlayerData
    {
        public int id;
        public string Name;

        internal static PlayerData Create(Player player)
        {
            var data = new PlayerData();
            data.id = player.ID;
            data.Name = player.Name;
            return data;
        }
    }

    public PlayerData[] Players = new PlayerData[] { };

    public static PlayersUpdateMessage Create(List<Player> activePlayers) {
        var message = new PlayersUpdateMessage();
        message.Players = new PlayerData[activePlayers.Count];

        for (int i = 0; i < activePlayers.Count; i++)
        {
            message.Players[i] = PlayerData.Create(activePlayers[i]);
        }
        return message;
    }
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

    protected Player CurrentPlayer = new Player();

    public abstract bool IsConnected
	{
		get;
	}

	public abstract IEnumerator Initialize();
	public abstract void Shutdown();

    public virtual void Update()
	{
		// no-op
	}
}

public class Player
{
    static string[] NamePool = new string[] { "Fluffy", "Jasper", "Spike", "Pet", "Scuffy", "Boots", "Doggie", "Birdie", "Kitty" }.OrderBy(n => Guid.NewGuid()).ToArray();

    internal bool isServer;
    internal NetworkConnection Connection;

    public int ID
    {
        get
        {
            return (isServer) ? 0 : Connection.connectionId;
        }
    }

    public string Name;

    public Player()
    {
        Name = NamePool[0];
    }
}

public class ServerConnection : BaseConnection
{
	WrappedNetworkServerSimple networkServerSimple;

    List<Player> activePlayers = new List<Player>();

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

        networkServerSimple.OnClientConnected += OnClientConnected;
        networkServerSimple.OnClientDisconnected += OnClientDisconnected;

        networkServerSimple.Initialize();
        networkServerSimple.Listen(CONNECTION_PORT);
		_isConnected = true;

        Debug.Log("Server Initialized");

        CurrentPlayer.isServer = true;

        activePlayers.Add(CurrentPlayer);

        yield break;
	}

    private void OnClientDisconnected(NetworkConnection obj)
    {
        Debug.Log("Server OnClientDisconnected" + obj.address + "connectionID " + obj.connectionId);

        var player = activePlayers.Where(p => p.Connection == obj).First();
        activePlayers.Remove(player);

        UpdateActivePlayers();
    }

    private void UpdateActivePlayers()
    {
        var message = PlayersUpdateMessage.Create(activePlayers);

        foreach (var remotePlayer in activePlayers)
        {
            if (remotePlayer.isServer)
            {
                continue;
            }

            remotePlayer.Connection.Send(GameMsgType.UpdateActivePlayers, message);
        }
    }

    private void OnClientConnected(NetworkConnection obj)
    {
        Debug.Log("Server OnClientConnected" + obj.address + "connectionID " + obj.connectionId);

        var player = new Player();

        player.Connection = obj;

        activePlayers.Add(player);

        UpdateActivePlayers();
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
        Debug.Log("Client Initializing...");

        client = new NetworkClient();      

        client.RegisterHandler(GameMsgType.UpdateActivePlayers, UpdateActivePlayers);      

		client.Connect(serverAddress, CONNECTION_PORT);      

		var time = Time.time + CONNECTION_TIMEOUT_SECONDS;

        yield return new WaitUntil(() => client.isConnected || Time.time > time);

        if (client.isConnected)
        {
            Debug.Log("Client Connected!");
        }
        else
        {
            Debug.Log("Client Not Connected!");
        }
	}

    private void UpdateActivePlayers(NetworkMessage netMsg)
    {
        var playersUpdate = netMsg.ReadMessage<PlayersUpdateMessage>();

        Debug.Log("UPDATE ACTIVE PLAYERS:");
        foreach(var player in playersUpdate.Players)
        {
            Debug.Log("PLAYER #" + player.id + " name" + player.Name);
        }
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
