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

        internal static PlayerData Create(NetworkPlayer player)
        {
            var data = new PlayerData();
            data.id = player.ID;
            data.Name = player.Name;
            return data;
        }
    }

    public PlayerData[] Players = new PlayerData[] { };

    public static PlayersUpdateMessage Create(List<NetworkPlayer> activePlayers) {
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
    event Action OnActivePlayersUpdated;
    NetworkPlayer[] ActivePlayers
    {
        get;
    }

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

    protected NetworkPlayer CurrentPlayer = new NetworkPlayer();

    public abstract event Action OnActivePlayersUpdated;

    public abstract bool IsConnected
	{
		get;
	}

    public abstract NetworkPlayer[] ActivePlayers
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

public class NetworkPlayer
{
    static string[] NamePool = new string[] { "Fluffy", "Jasper", "Spike", "Pet", "Scuffy", "Boots", "Doggie", "Birdie", "Kitty" };

    internal bool isServer;
    internal NetworkConnection Connection;

    public int ID;
    public string Name;

    public NetworkPlayer()
    {
        Name = NamePool.OrderBy(n => Guid.NewGuid()).First();
    }
}

public class ServerConnection : BaseConnection
{
	WrappedNetworkServerSimple networkServerSimple;

    List<NetworkPlayer> activePlayers = new List<NetworkPlayer>();

    private bool _isConnected = false;
	public override bool IsConnected
	{
		get
		{
			return _isConnected;
		}
	}

    public override NetworkPlayer[] ActivePlayers => activePlayers.ToArray();

    public override event Action OnActivePlayersUpdated;

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
        CurrentPlayer.ID = 0;

        activePlayers.Add(CurrentPlayer);

        UpdateActivePlayers();

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

        OnActivePlayersUpdated?.Invoke();
    }

    private void OnClientConnected(NetworkConnection obj)
    {
        Debug.Log("Server OnClientConnected" + obj.address + "connectionID " + obj.connectionId);

        var player = new NetworkPlayer();

        player.Connection = obj;
        player.ID = obj.connectionId;

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

    public override event Action OnActivePlayersUpdated;

    public override bool IsConnected => client.isConnected;

    List<NetworkPlayer> activePlayers = new List<NetworkPlayer>();

    public override NetworkPlayer[] ActivePlayers => activePlayers.ToArray();

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
        foreach(var playerData in playersUpdate.Players)
        {
            var existingPlayer = activePlayers.Find(p => p.ID == playerData.id);
            if (existingPlayer == null)
            {
                existingPlayer = new NetworkPlayer()
                {
                    ID = playerData.id
                };
                activePlayers.Add(existingPlayer);
            }
            existingPlayer.Name = playerData.Name;

            Debug.Log("PLAYER #" + playerData.id + " name" + playerData.Name);
        }




        OnActivePlayersUpdated?.Invoke();
    }

    public override void Shutdown()
	{
        OnActivePlayersUpdated = null;

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

        activeConnection = new ClientConnection("localhost");
        activeConnection.OnActivePlayersUpdated += ForwardOnActivePlayersUpdated;

        yield return activeConnection.Initialize();

		if (activeConnection.IsConnected)
		{
			connectionMode = ConnectionMode.CLIENT;
		}
		else
		{
            activeConnection.Shutdown();

            activeConnection = new ServerConnection();
            activeConnection.OnActivePlayersUpdated += ForwardOnActivePlayersUpdated;

            yield return activeConnection.Initialize();

			if (activeConnection.IsConnected)
			{
				connectionMode = ConnectionMode.SERVER;
			}         
		}


          
	}

    private void ForwardOnActivePlayersUpdated()
    {
        OnActivePlayerChange?.Invoke();
    }

    public int NumActivePlayers
	{
		get;
		private set;
	}
    public IEnumerable<NetworkPlayer> ActivePlayers
    {
        get
        {
            return activeConnection.ActivePlayers;
        }
    }

	public void Update()
	{
		if (activeConnection != null)
		{
			activeConnection.Update();
		}
	}
    

}
