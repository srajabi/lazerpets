using UnityEngine.Networking;

namespace Networking
{
    public class GameMsgType
    {
        public const short UpdateActivePlayers = MsgType.Highest + 1;
        public const short PlayerDisconnect = MsgType.Highest + 2;
    }

}