using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Networking
{
    public class ClientConnection : BaseConnection
    {
        const int CONNECTION_TIMEOUT_SECONDS = 2;

        NetworkClient client;
        private readonly string serverAddress;

        public ClientConnection(string serverAddress)
        {
            this.serverAddress = serverAddress;
        }

        public override event Action OnActivePlayersUpdated;

        public override bool IsConnected => client.isConnected;

        List<NetworkPlayer> activePlayers = new List<NetworkPlayer>();

        public override NetworkPlayer[] ActivePlayers => activePlayers.ToArray();

        public override IEnumerator Initialize()
        {
            Debug.Log("Client Initializing...");

            client = new NetworkClient();

            client.RegisterHandler(GameMsgType.UpdateActivePlayers, UpdateActivePlayers);

            client.Connect(serverAddress, CONNECTION_PORT);

            var time = Time.time + CONNECTION_TIMEOUT_SECONDS;

            yield return new WaitUntil(() => client.isConnected || Time.time > time);

            if (client.isConnected)
            {
                Debug.Log("Client Connected!");
            }
            else
            {
                Debug.Log("Client Not Connected!");
            }
        }

        private void UpdateActivePlayers(NetworkMessage netMsg)
        {
            var playersUpdate = netMsg.ReadMessage<PlayersUpdateMessage>();

            Debug.Log("UPDATE ACTIVE PLAYERS:");
            foreach (var playerData in playersUpdate.Players)
            {
                var existingPlayer = activePlayers.Find(p => p.ID == playerData.id);
                if (existingPlayer == null)
                {
                    existingPlayer = new NetworkPlayer()
                    {
                        ID = playerData.id
                    };
                    activePlayers.Add(existingPlayer);
                }
                existingPlayer.Name = playerData.Name;

                Debug.Log("PLAYER #" + playerData.id + " name" + playerData.Name);
            }




            OnActivePlayersUpdated?.Invoke();
        }

        public override void Shutdown()
        {
            OnActivePlayersUpdated = null;

            if (client == null)
            {
                return;
            }
            client.Shutdown();
            client = null;
        }


    }

}