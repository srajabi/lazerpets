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

        [SerializeField]
        private Player PlayerPrefab;

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

            InitializeGame();

            Initialized = true;
            OnInitialized?.Invoke(this, EventArgs.Empty);

        }

        private void OnPlayerDisconnect(Networking.NetworkPlayer player)
        {
            var go = NetworkToGameMap[player];

            NetworkToGameMap.Remove(player);

            GameObject.Destroy(go);

            


            Debug.Log("OnPlayerDisconnect Player #" + player.ID + "(" + player.Name + ")");
        }

        Dictionary<Networking.NetworkPlayer, Player> NetworkToGameMap = new Dictionary<Networking.NetworkPlayer, Player>();

        private void OnPlayerConnect(Networking.NetworkPlayer netPlayer)
        {
            //var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            //go.name = "Player_" + player.Name;

            Player player = CreatePlayerObject(netPlayer);

            

            NetworkToGameMap.Add(netPlayer, player);

            Debug.Log("OnPlayerConnect Player #" + netPlayer.ID + "(" + netPlayer.Name + ")");
        }

        private Player CreatePlayerObject(Networking.NetworkPlayer netPlayer)
        {
            var player = GameObject.Instantiate<Player>(PlayerPrefab);
            player.NetworkPlayer = netPlayer;
            player.name = string.Format("[Player] {0}", netPlayer.Name);
            netPlayer.Player = player;

            spawner.Spawn(player);

            return player;
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