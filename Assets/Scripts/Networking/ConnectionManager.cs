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
	public const short Beef = MsgType.Highest + 1;
}

public class BeefMessage : MessageBase
{
	public int data;
}

public class ConnectionManager {

	const int CONNECTION_PORT = 64000;

	NetworkClient client;

	WrappedNetworkServerSimple networkServerSimple;

	public ConnectionMode connectionMode
	{
		private set;
		get;
	}

	public IEnumerator Initialize()
	{

		connectionMode = ConnectionMode.CLIENT;
		client = new NetworkClient();

		client.Connect("localhost", CONNECTION_PORT);


		var time = Time.time + 2;

		yield return new WaitUntil(() => client.isConnected || Time.time > time);
  
		if (client.isConnected)
		{
			Debug.Log("Client MODE");         

			client.RegisterHandler(MsgType.Connect, OnClientConnect);


			client.RegisterHandler(GameMsgType.Beef, HandleBeef);


			client.Send(GameMsgType.Beef, new BeefMessage());

		}
		else 
		{

			client = null;

			Debug.Log("SERVER MODE");

			connectionMode = ConnectionMode.SERVER;
			networkServerSimple = new WrappedNetworkServerSimple();
			networkServerSimple.Initialize();
			networkServerSimple.Listen(CONNECTION_PORT);
			networkServerSimple.RegisterHandler(GameMsgType.Beef, HandleBeef);


		}

	}

    public void Update()
	{
		if (connectionMode == ConnectionMode.SERVER && networkServerSimple != null)
		{
			networkServerSimple.Update();
		}
	}

	public void OnClientConnect(NetworkMessage msg)
    {
		Debug.Log("OnClientConnect" + connectionMode);

    }

	public void HandleBeef(NetworkMessage msg)
	{
		Debug.Log("HANDLING BEEF" + connectionMode);
        
	}

	//const int CONNECTION_PORT = 64000;

 //   //Unity stuff
	//WrappedNetworkDiscovery networkDiscovery;
    
	//WrappedNetworkServerSimple networkServer;

	//NetworkClient networkClient;


	//private ConnectionMode _currentState;

	//public ConnectionMode CurrentState
	//{
	//	private set
	//	{
	//		if (_currentState != value)
	//		{
	//			var oldState = _currentState;
	//			_currentState = value;
	//			OnCurrentStateChange?.Invoke(_currentState, oldState);
	//		}
	//	}
	//	get
	//	{
	//		return _currentState;
	//	}
	//}

	//public event CurrentStateChange OnCurrentStateChange;

 //   public enum NetworkMode { CLIENT, SERVER };
 //   public NetworkMode MyNetworkMode;

 //   // Use this for initialization
 //   void Start()
	//{
	//	networkDiscovery = gameObject.AddComponent<WrappedNetworkDiscovery>();
	//	networkDiscovery.showGUI = false;



	//	/*
	//	networkDiscovery.Initialize();

 //       StartClientMode();
 //       networkDiscovery.OnServerFound += (a, b) =>
 //       {
 //           Debug.Log(a + " " + b);

 //           ConnectToServer(a);
 //       };
 //       */

	//	Debug.Log("ServerMode" + MyNetworkMode);

 //       if (MyNetworkMode == NetworkMode.SERVER)
 //       {
 //           Server__();
 //       }
 //       else
 //       {
 //           client = new NetworkClient();
 //           client.Connect("localhost", CONNECTION_PORT);
 //           client.RegisterHandler(MsgType.Connect, OnClientConnected);
 //       }
 //   }

 //   NetworkClient client;

 //   public void Server__()
 //   {
 //       NetworkServer.RegisterHandler(MsgType.Connect, OnConnect);
 //       NetworkServer.RegisterHandler(MsgType.Disconnect, OnPlayerDisconnect);
	//	NetworkServer.RegisterHandler(MsgType.Highest + 1, OnMsgReceived);
	//	NetworkServer.RegisterHandler(MsgType.Highest + 2, OnMsgReceived2);
 //       NetworkServer.Listen(CONNECTION_PORT);
 //   }

 //   void OnMsgReceived(NetworkMessage msg)
 //   {
 //       Debug.Log(" " + msg + " " + msg.reader.ReadString());
 //   }

 //   void OnMsgReceived2(NetworkMessage msg)
 //   {
 //       Debug.Log("2 " + msg + " " + msg.reader.ReadMessage<ConnectionManager.ScoreMessage>());
 //   }


 //   private void OnPlayerDisconnect(NetworkMessage netMsg)
 //   {
 //       Debug.Log("player disconnected!");
 //   }

 //   private void OnConnect(NetworkMessage netMsg)
 //   {
 //       Debug.Log("player connected!");
 //   }


 //   private void OnClientConnected(NetworkMessage netMsg)
 //   {
 //       Debug.Log("Connected: " + netMsg);
 //       //ClientScene.Ready(netMsg.conn);
 //       //ClientScene.AddPlayer(0);



 //       StartCoroutine(sendOverWire());
 //   }

 //   public class ScoreMessage : MessageBase
 //   {
 //       public int score;
 //   }

 //   IEnumerator sendOverWire()
 //   {
 //       while (true)
 //       {
 //           yield return new WaitForSeconds(1);
 //           NetworkWriter writer = new NetworkWriter();
 //           writer.Write("casdfasdfasdfa");


	//		client.Send(MsgType.Highest + 2, new ScoreMessage());
 //       }
 //   }

 //   public void StartServerMode()
	//{
	//	if (CurrentState != ConnectionMode.IDLE)
	//	{
	//		throw new InvalidOperationException("You can only start server mode when idle.");
	//	}
	//	CurrentState = ConnectionMode.SERVER;


	//	networkServer = new WrappedNetworkServerSimple();

	//	networkServer.Initialize();
	//	networkServer.Listen(CONNECTION_PORT);



	//	networkDiscovery.broadcastData = GetServerName();      
	//	networkDiscovery.StartAsServer();


      
	//}

	//// TODO : Make this return cool pet based names?
	//private string GetServerName()
	//{
	//	return Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8);
	//}

	//public void StartClientMode()
	//{
	//	if (CurrentState != ConnectionMode.IDLE)
 //       {
 //           throw new InvalidOperationException("You can only client  mode when idle.");
 //       }

	//	CurrentState = ConnectionMode.JOIN;

	//	networkDiscovery.StartAsClient();

	//}

 //   public void ConnectToServer(string server)
	//{
	//	if (CurrentState != ConnectionMode.JOIN)
	//	{
	//		throw new Exception("You can only connect to a server from join mode");
	//	}

	//	networkDiscovery.StopBroadcast();

	//	//networkManager.networkAddress = server;
	//	//networkManager.StartClient();

	//	CurrentState = ConnectionMode.CLIENT;


	//	networkClient = new NetworkClient();

	//	networkClient.Connect(server, CONNECTION_PORT);

	//	//networkClient.RegisterHandler()
                      
	//	//networkClient.
	//}
    

}
