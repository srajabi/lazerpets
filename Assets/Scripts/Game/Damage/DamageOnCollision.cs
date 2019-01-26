using System;
using UnityEngine;

namespace Game
{
    public class DamageOnCollision : MonoBehaviour
    {
        public event EventHandler OnDamage;
        public event EventHandler OnCollision;
        public Player Creator { get; set; }

        [SerializeField]
        private float DamageAmount = 10;

        public void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject != null)
            {
                var health = collision.gameObject.GetComponent<Health>();
                if (health != null)
                {
                    Damager damager = Creator == null ? new Damager(gameObject) : new Damager(Creator);
                    health.Modify(-DamageAmount, damager);
                    OnDamage?.Invoke(this, EventArgs.Empty);
                }
            }

            OnCollision?.Invoke(this, EventArgs.Empty);
        }
    }
}