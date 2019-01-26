using UnityEngine;

namespace Game
{
    public class Player : MonoBehaviour
    {
        public Health Health { get; private set; }

        public void Awake()
        {
            Health = GetComponentInChildren<Health>(true);
        }
    }
}