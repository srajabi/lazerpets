using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Networking;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        public bool Initialized { get; private set; }
        public event EventHandler OnInitialized;
        
		private Player[] players;
        public IEnumerable<Player> Players { get { return players; } }

		private List<GameObject> NetworkObjects = new List<GameObject>();

		private ConnectionManager connectionManager;
        private GameSpawner spawner;

        public void Awake()
        {
            connectionManager = new ConnectionManager();
            spawner = new GameSpawner(this);
        }
        
        public IEnumerator Start()
        {
            connectionManager.OnPlayerConnect += OnNewConnect;
            connectionManager.OnActivePlayerChange += OnActivePlayerChange;
            connectionManager.OnPlayerDisconnect += OnNewDisconnect;

            yield return connectionManager.Initialize();

			//var client = new NetworkClient();
			//client.Connect("localhost", 64000);
            
			//if (!client.isConnected)
			//{
			//	var server = new NetworkServerSimple();
			//	server.Listen(64000);
			//}         

            InitializeGame();

            Initialized = true;
            OnInitialized?.Invoke(this, EventArgs.Empty);

            foreach (var player in players)
            {
                spawner.Spawn(player);
            }
        }

        private void OnNewDisconnect(Networking.NetworkPlayer obj)
        {
            Debug.Log("OnNewDisconnect");
        }

        private void OnNewConnect(Networking.NetworkPlayer obj)
        {
            Debug.Log("OnNewConnect");
        }

        public void OnActivePlayerChange()
		{
            Debug.Log("OnActivePlayerChange");
            foreach (Networking.NetworkPlayer player in connectionManager.ActivePlayers)
            {
                Debug.Log("OnActivePlayerChange Player #" + player.ID + "(" + player.Name + ")");
            }
        }

		public void Update()
		{
			connectionManager.Update();
		}

		private void InitializeGame()
        {
            FindPlayers();
        }

        private void FindPlayers()
        {
            players = FindObjectsOfType<Player>();
        }
    }
}