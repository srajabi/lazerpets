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

        private IConnection activeConnection;

        public event Action<NetworkPlayer> OnPlayerConnect;
        public event Action<NetworkPlayer> OnPlayerDisconnect;
        public event Action OnActivePlayersUpdated;

        public string ServerAddress = "localhost";

        public ConnectionMode connectionMode
        {
            private set;
            get;
        }

        public IEnumerator Initialize()
        {

            activeConnection = new ClientConnection(ServerAddress);
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

        public int NumActivePlayers
        {
            get;
            private set;
        }
        public IEnumerable<NetworkPlayer> ActivePlayers
        {
            get
            {
                return activeConnection.ActivePlayers;
            }
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