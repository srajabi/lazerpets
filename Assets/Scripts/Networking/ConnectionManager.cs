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
        public event Action OnActivePlayerChange;

        public ConnectionMode connectionMode
        {
            private set;
            get;
        }

        public IEnumerator Initialize()
        {

            activeConnection = new ClientConnection("localhost");
            activeConnection.OnActivePlayersUpdated += ForwardOnActivePlayersUpdated;

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

                yield return activeConnection.Initialize();

                if (activeConnection.IsConnected)
                {
                    connectionMode = ConnectionMode.SERVER;
                }
            }



        }

        private void ForwardOnActivePlayersUpdated()
        {
            OnActivePlayerChange?.Invoke();
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