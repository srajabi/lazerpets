using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        public bool Initialized { get; private set; }
        public event EventHandler OnInitialized;
        
		private Player[] players;
        public IEnumerable<Player> Players { get { return players; } }

		private List<GameObject> NetworkObjects = new List<GameObject>();

		ConnectionManager connectionManager;

        public void Awake()
        {
            connectionManager = new ConnectionManager();
        }
        
        public IEnumerator Start()
        {
			connectionManager.OnActivePlayerChange += OnActivePlayerChange;

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

		public void OnActivePlayerChange()
		{
            Debug.Log("OnActivePlayerChange");
            foreach (NetworkPlayer player in connectionManager.ActivePlayers)
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