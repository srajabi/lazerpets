using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public enum ConnectionMode
{
    IDLE,
    SERVER,
    JOIN,
    CLIENT
}

public delegate void CurrentStateChange(ConnectionMode newState, ConnectionMode oldState);

public class ConnectionManager : MonoBehaviour {

	const int CONNECTION_PORT = 64000;

    //Unity stuff
	WrappedNetworkDiscovery networkDiscovery;
    
	WrappedNetworkServerSimple networkServer;

	NetworkClient networkClient;


	private ConnectionMode _currentState;

	public ConnectionMode CurrentState
	{
		private set
		{
			if (_currentState != value)
			{
				var oldState = _currentState;
				_currentState = value;
				OnCurrentStateChange?.Invoke(_currentState, oldState);
			}
		}
		get
		{
			return _currentState;
		}
	}
    
	public event CurrentStateChange OnCurrentStateChange;

    public enum NetworkMode { CLIENT, SERVER };
    public NetworkMode MyNetworkMode;

    // Use this for initialization
    void Start()
	{
		networkDiscovery = gameObject.AddComponent<WrappedNetworkDiscovery>();
		networkDiscovery.showGUI = false;



        /*
		networkDiscovery.Initialize();

        StartClientMode();
        networkDiscovery.OnServerFound += (a, b) =>
        {
            Debug.Log(a + " " + b);

            ConnectToServer(a);
        };
        */

        if (MyNetworkMode == NetworkMode.SERVER)
        {
            Server__();
        }
        else
        {
            client = new NetworkClient();
            client.Connect("localhost", CONNECTION_PORT);
            client.RegisterHandler(MsgType.Connect, OnClientConnected);
        }
    }

    NetworkClient client;

    public void Server__()
    {
        NetworkServer.RegisterHandler(MsgType.Connect, OnConnect);
        NetworkServer.RegisterHandler(MsgType.Disconnect, OnPlayerDisconnect);
        NetworkServer.Listen(CONNECTION_PORT);
    }


    private void OnPlayerDisconnect(NetworkMessage netMsg)
    {
        Debug.Log("player disconnected!");
    }

    private void OnConnect(NetworkMessage netMsg)
    {
        Debug.Log("player connected!");
    }


    private void OnClientConnected(NetworkMessage netMsg)
    {
        Debug.Log("Connected: " + netMsg);
        //ClientScene.Ready(netMsg.conn);
        //ClientScene.AddPlayer(0);



        StartCoroutine(sendOverWire());
    }

    public class ScoreMessage : MessageBase
    {
        public int score;
    }

    IEnumerator sendOverWire()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            NetworkWriter writer = new NetworkWriter();
            writer.Write("casdfasdfasdfa");
            client.SendWriter(writer, 0);


            client.Send(1, new ScoreMessage());
        }
    }

    public void StartServerMode()
	{
		if (CurrentState != ConnectionMode.IDLE)
		{
			throw new InvalidOperationException("You can only start server mode when idle.");
		}
		CurrentState = ConnectionMode.SERVER;


		networkServer = new WrappedNetworkServerSimple();

		networkServer.Initialize();
		networkServer.Listen(CONNECTION_PORT);



		networkDiscovery.broadcastData = GetServerName();      
		networkDiscovery.StartAsServer();


      
	}

	// TODO : Make this return cool pet based names?
	private string GetServerName()
	{
		return Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8);
	}

	public void StartClientMode()
	{
		if (CurrentState != ConnectionMode.IDLE)
        {
            throw new InvalidOperationException("You can only client  mode when idle.");
        }

		CurrentState = ConnectionMode.JOIN;

		networkDiscovery.StartAsClient();

	}

    public void ConnectToServer(string server)
	{
		if (CurrentState != ConnectionMode.JOIN)
		{
			throw new Exception("You can only connect to a server from join mode");
		}

		networkDiscovery.StopBroadcast();

		//networkManager.networkAddress = server;
		//networkManager.StartClient();

		CurrentState = ConnectionMode.CLIENT;


		networkClient = new NetworkClient();

		networkClient.Connect(server, CONNECTION_PORT);

		//networkClient.RegisterHandler()
                      
		//networkClient.
	}
    

}
