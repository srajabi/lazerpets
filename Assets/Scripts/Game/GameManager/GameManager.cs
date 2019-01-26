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

		private ConnectionManager connectionManager;
        private GameSpawner spawner;

        public void Awake()
        {
            connectionManager = new ConnectionManager();
            spawner = new GameSpawner(this);
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

            foreach (var player in players)
            {
                spawner.Spawn(player);
            }
        }

		public void OnActivePlayerChange()
		{
			Debug.Log("OnActivePlayerChange " + connectionManager.NumActivePlayers);
			foreach(var go in NetworkObjects)
			{
				GameObject.Destroy(go);
			}
			for (int i = 0; i < connectionManager.NumActivePlayers; i++)
			{
				var go = new GameObject();
				NetworkObjects.Add(go);
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