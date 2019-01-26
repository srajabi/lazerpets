using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
		private Player[] players;

		public event EventHandler<GameOutcomeEventArgs> OnGameEnded;

        //This is temp and dumb
		private List<GameObject> NetworkObjects = new List<GameObject>();

		ConnectionManager connectionManager;

		public IEnumerator Start()
        {
			connectionManager = new ConnectionManager();

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
            for (int i = 0; i < players.Length; i++)
            {
                InitializePlayer(players[i]);
            }
        }

        private void FindPlayers()
        {
            players = FindObjectsOfType<Player>();
        }

        private void InitializePlayer(Player player)
        {
            player.Health.OnModified += OnHealthModified;
            player.Health.OnDeath += OnDeath;
        }

        private void OnDeath(object sender, HealthEventArgs e)
        {
            Debug.LogErrorFormat("Player {0} ate shit and died thanks to {1}.", e.Causee, e.Causer);
        }

        private void OnHealthModified(object sender, HealthEventArgs e)
        {
            Debug.LogErrorFormat("Player {0} ate shit and took some fucking damage from {1}.", e.Causee, e.Causer);
        }
    }
}