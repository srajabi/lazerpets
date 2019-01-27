using System.Collections.Generic;
using UnityEngine.Networking;

namespace Networking
{
    public class PlayerDamageMessage : MessageBase
    {
        public int id;
        public int damage;
    }

}