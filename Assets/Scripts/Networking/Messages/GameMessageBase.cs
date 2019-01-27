using System.Collections.Generic;
using UnityEngine.Networking;

namespace Networking
{
    public class GameMessageBase : MessageBase
    {
        public int Id;
        public short Type;
    }
}