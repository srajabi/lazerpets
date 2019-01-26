using System;

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

    public class Health : PlayerBehaviour
    {
        public float Default = 100;
        public float Current = 100;
        public bool IsDead = false;

        public event EventHandler<HealthEventArgs> OnModified;
        public event EventHandler<HealthEventArgs> OnDeath;

        public override void Awake()
        {
            base.Awake();

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
                OnDeath?.Invoke(this, new HealthEventArgs(Player, amount));
                return;
            }

            OnModified?.Invoke(this, new HealthEventArgs(Player, amount));
        }
    }
}