using Game;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Networking
{
    public class PlayersUpdateMessage : MessageBase
    {
        public class PlayerData
        {
            public int ID;
            public string Name;
            public CharacterTypes CharacterType;

            internal static PlayerData Create(NetworkPlayer player)
            {
                var data = new PlayerData();
                data.ID = player.ID;
                data.Name = player.Name;
                data.CharacterType = player.CharacterType;
                return data;
            }
        }

        public PlayerData[] Players = new PlayerData[] { };

        public static PlayersUpdateMessage Create(List<NetworkPlayer> activePlayers)
        {
            var message = new PlayersUpdateMessage();
            message.Players = new PlayerData[activePlayers.Count];

            for (int i = 0; i < activePlayers.Count; i++)
            {
                message.Players[i] = PlayerData.Create(activePlayers[i]);
            }
            return message;
        }
    }

}