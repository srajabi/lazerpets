using System;
using System.Collections;
using System.Collections.Generic;

namespace Networking
{
    public enum ConnectionMode
    {
        SERVER,
        CLIENT
    }

    public class ConnectionManager
    {
        public ConnectionMode connectionMode { private set; get; }
        public int NumActivePlayers { private set; get; }

        public IEnumerable<NetworkPlayer> ActivePlayers
        {
            get { return activeConnection.ActivePlayers; }
        }

        private IConnection activeConnection;

        public event Action<NetworkPlayer> OnPlayerConnect;
        public event Action<NetworkPlayer> OnPlayerDisconnect;
        public event Action OnActivePlayersUpdated;

        public readonly string serverAddress = "localhost";

        public ConnectionManager(string serverAddress)
        {
            this.serverAddress = serverAddress;
        }

        public IEnumerator Initialize()
        {
            activeConnection = new ClientConnection(serverAddress);
            activeConnection.OnActivePlayersUpdated += ForwardOnActivePlayersUpdated;
            activeConnection.OnPlayerConnect += ForwardOnPlayerConnect;
            activeConnection.OnPlayerDisconnect += ForwardOnPlayerDisconnect;

            yield return activeConnection.Initialize();

            if (activeConnection.IsConnected)
            {
                connectionMode = ConnectionMode.CLIENT;
            }
            else
            {
                activeConnection.Shutdown();

                activeConnection = new ServerConnection();
                activeConnection.OnActivePlayersUpdated += ForwardOnActivePlayersUpdated;
                activeConnection.OnPlayerConnect += ForwardOnPlayerConnect;
                activeConnection.OnPlayerDisconnect += ForwardOnPlayerDisconnect;

                yield return activeConnection.Initialize();

                if (activeConnection.IsConnected)
                {
                    connectionMode = ConnectionMode.SERVER;
                }
            }
        }

        private void ForwardOnPlayerDisconnect(NetworkPlayer obj)
        {
            OnPlayerDisconnect?.Invoke(obj);
        }

        private void ForwardOnPlayerConnect(NetworkPlayer obj)
        {
            OnPlayerConnect?.Invoke(obj);
        }

        private void ForwardOnActivePlayersUpdated()
        {
            OnActivePlayersUpdated?.Invoke();
        }

        public void Update()
        {
            if (activeConnection != null)
            {
                activeConnection.Update();
            }
        }
    }
}