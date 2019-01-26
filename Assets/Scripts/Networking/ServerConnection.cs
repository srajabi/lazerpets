using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Networking
{
    public class ServerConnection : BaseConnection
    {
        WrappedNetworkServerSimple networkServerSimple;

        List<NetworkPlayer> activePlayers = new List<NetworkPlayer>();

        private bool _isConnected = false;
        public override bool IsConnected
        {
            get
            {
                return _isConnected;
            }
        }

        public override NetworkPlayer[] ActivePlayers => activePlayers.ToArray();

        public override event Action OnActivePlayersUpdated;
        public override event Action<NetworkPlayer> OnPlayerConnect;
        public override event Action<NetworkPlayer> OnPlayerDisconnect;

        public override IEnumerator Initialize()
        {
            networkServerSimple = new WrappedNetworkServerSimple();

            networkServerSimple.OnClientConnected += OnClientConnected;
            networkServerSimple.OnClientDisconnected += OnClientDisconnected;

            networkServerSimple.Initialize();
            networkServerSimple.Listen(CONNECTION_PORT);
            _isConnected = true;

            Debug.Log("Server Initialized");

            CurrentPlayer.isServer = true;
            CurrentPlayer.ID = 0;

            activePlayers.Add(CurrentPlayer);

            OnPlayerConnect?.Invoke(CurrentPlayer);

            UpdateActivePlayers();

            yield break;
        }

        private void OnClientDisconnected(NetworkConnection obj)
        {
            Debug.Log("Server OnClientDisconnected" + obj.address + "connectionID " + obj.connectionId);

            var player = activePlayers.Where(p => p.Connection == obj).First();

            OnPlayerDisconnect?.Invoke(player);

            //TODO Notify clients


            activePlayers.Remove(player);

            UpdateActivePlayers();
        }

        private void UpdateActivePlayers()
        {
            var message = PlayersUpdateMessage.Create(activePlayers);

            foreach (var remotePlayer in activePlayers)
            {
                if (remotePlayer.isServer)
                {
                    continue;
                }

                remotePlayer.Connection.Send(GameMsgType.UpdateActivePlayers, message);
            }

            OnActivePlayersUpdated?.Invoke();
        }

        private void OnClientConnected(NetworkConnection obj)
        {
            Debug.Log("Server OnClientConnected" + obj.address + "connectionID " + obj.connectionId);

            var player = new NetworkPlayer();

            player.Connection = obj;
            player.ID = obj.connectionId;

            activePlayers.Add(player);

            UpdateActivePlayers();
        }


        public override void Shutdown()
        {
            OnPlayerDisconnect = null;
            OnPlayerConnect = null;
            OnActivePlayersUpdated = null;

            if (networkServerSimple == null)
            {
                return;
            }
            networkServerSimple.ClearHandlers();
            networkServerSimple.DisconnectAllConnections();
            networkServerSimple.Stop();
            networkServerSimple = null;

            _isConnected = false;
        }

        public override void Update()
        {
            base.Update();
            networkServerSimple.Update();
        }

    }

}