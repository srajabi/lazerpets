using System;
using UnityEngine;

namespace Game
{
    public class HealthEventArgs : EventArgs
    {
        public float Modification;
        public Player Player;

        public HealthEventArgs(Player player, float modification)
        {
            Modification = modification;
            Player = player;
        }
    }

    public class Health : MonoBehaviour
    {
        public float Default = 100;
        public float Current = 100;
        public bool IsDead = false;

        public event EventHandler<HealthEventArgs> OnModified;
        public event EventHandler<HealthEventArgs> OnDeath;

        public void Awake()
        {
            Current = Default;
            IsDead = false;
        }

        public void Modify(float amount)
        {
            if (IsDead)
            {
                return;
            }

            Current += amount;

            if (Current <= 0)
            {
                OnDeath?.Invoke(this, GetArgs(amount));
                return;
            }

            OnModified?.Invoke(this, GetArgs(amount));
        }

        private HealthEventArgs GetArgs(float modification)
        {
            return new HealthEventArgs(GetComponentInParent<Player>(), modification);
        }
    }
}