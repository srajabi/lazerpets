using System;
using System.Collections;

namespace Networking
{
    public interface IConnection
    {
        IEnumerator Initialize();
        event Action OnActivePlayersUpdated;
        event Action<NetworkPlayer> OnPlayerConnect;
        event Action<NetworkPlayer> OnPlayerDisconnect;

        NetworkPlayer[] ActivePlayers
        {
            get;
        }

        bool IsConnected
        {
            get;
        }

        void Shutdown();

        void Update();
    }

}