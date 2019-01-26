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
            connectionManager.OnPlayerConnect += OnPlayerConnect;
            connectionManager.OnActivePlayersUpdated += OnActivePlayersUpdated;
            connectionManager.OnPlayerDisconnect += OnPlayerDisconnect;

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

        private void OnPlayerDisconnect(Networking.NetworkPlayer player)
        {
            Debug.Log("OnPlayerDisconnect Player #" + player.ID + "(" + player.Name + ")");
        }

        private void OnPlayerConnect(Networking.NetworkPlayer player)
        {
            Debug.Log("OnPlayerConnect Player #" + player.ID + "(" + player.Name + ")");
        }

        public void OnActivePlayersUpdated()
		{
            Debug.Log("OnActivePlayersUpdated");
            foreach (Networking.NetworkPlayer player in connectionManager.ActivePlayers)
            {
                Debug.Log("OnActivePlayersUpdated Player #" + player.ID + "(" + player.Name + ")");
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