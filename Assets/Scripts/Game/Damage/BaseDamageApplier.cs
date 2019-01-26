using System;
using UnityEngine;

namespace Game
{
    public class BaseDamageApplier : MonoBehaviour
    {
        public event EventHandler OnDamage;
        public Player Creator { get; set; }

        [SerializeField]
        private float DamageAmount = 10;

        protected bool Apply(GameObject gameObject)
        {
            if (gameObject != null)
            {
                var health = gameObject.GetComponentInParent<Health>();
                if (health != null)
                {
                    Damager damager = Creator == null ? new Damager(this.gameObject) : new Damager(Creator);
                    var args = health.Modify(-DamageAmount, damager);
                    OnDamage?.Invoke(this, EventArgs.Empty);
                    return args != null;
                }
            }

            return false;
        }
    }
}