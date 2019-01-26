using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        public bool Initialized { get; private set; }
        public event EventHandler OnInitialized;

		private Player[] players;
        public IEnumerable<Player> Players { get { return players; } }
        
		ConnectionManager connectionManager;

        public void Awake()
        {
            connectionManager = new ConnectionManager();
        }

        public IEnumerator Start()
        {
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