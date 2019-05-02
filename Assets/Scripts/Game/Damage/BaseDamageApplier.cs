using System;
using UnityEngine;
using Networking;

namespace Game
{
    public class BaseDamageApplier : MonoBehaviour
    {
        public event EventHandler OnDamage;
        public Player Creator { get; set; }

        [SerializeField]
        private float DamageAmount = 10;

        protected bool Apply(Collision collision)
        {
            if (collision.gameObject != null)
            {
                var health = collision.gameObject.GetComponentInParent<Health>();
                if (health != null)
                {
                    Debug.LogError("Point: " + collision.GetContact(0).point +
                        "Normal: " + collision.GetContact(0).normal);
                    GameManager.Instance.
                        ConnectionManager.
                        ActiveConnection.
                        SendMessage<PlayerEffectMessage>(
                            new PlayerEffectMessage() { 
                                Effect = Effects.Effect.Damage,
                                Point = collision.GetContact(0).point,
                                Normal = collision.GetContact(0).normal });


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