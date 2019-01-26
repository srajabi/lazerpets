using System;
using System.Collections;

namespace Networking
{
    public abstract class BaseConnection : IConnection
    {
        protected const int CONNECTION_PORT = 64000;

        protected NetworkPlayer CurrentPlayer = new NetworkPlayer();

        public abstract event Action OnActivePlayersUpdated;
        public abstract event Action<NetworkPlayer> OnPlayerConnect;
        public abstract event Action<NetworkPlayer> OnPlayerDisconnect;

        public abstract bool IsConnected
        {
            get;
        }

        public abstract NetworkPlayer[] ActivePlayers
        {
            get;
        }

        public abstract IEnumerator Initialize();
        public abstract void Shutdown();

        public virtual void Update()
        {
            // no-op
        }
    }

}