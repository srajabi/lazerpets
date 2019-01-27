using UnityEngine.Networking;

namespace Networking
{
    public class GameMsgType
    {
        public const short UpdateActivePlayers = MsgType.Highest + 1;
        public const short PlayerDisconnect = MsgType.Highest + 2;
        public const short DamageReceived = MsgType.Highest + 3;
    }

}