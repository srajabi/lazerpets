using UnityEngine;

namespace Game
{
    public abstract class PlayerBehaviour : MonoBehaviour
    {
        public Player Player { get; private set; }

        public virtual void Awake()
        {
            Player = GetComponentInParent<Player>();
        }
    }
}