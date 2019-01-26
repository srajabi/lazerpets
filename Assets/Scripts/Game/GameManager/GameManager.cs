using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
		private Player[] players;

		public event EventHandler<GameOutcomeEventArgs> OnGameEnded;

		ConnectionManager connectionManager;

		public IEnumerator Start()
        {
			connectionManager = new ConnectionManager();

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
            Debug.LogErrorFormat("Player {0} ate shit and died thanks to {1}.", e.Causee.GetName(), e.Causer.GetName());
        }

        private void OnHealthModified(object sender, HealthEventArgs e)
        {
            Debug.LogErrorFormat("Player {0} ate shit and took some fucking damage from {1}.", e.Causee.GetName(), e.Causer.GetName());
        }
    }
}