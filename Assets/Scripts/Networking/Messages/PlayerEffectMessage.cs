using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Game;

namespace Networking
{
    public class PlayerEffectMessage : GameMessageBase
    {
        public Effects.Effect Effect;
        public Vector3 Point;
        public Vector3 Normal;
    }
}