using System;
using UnityEngine;

namespace Game
{
    public class DamageOnCollisionEnter : BaseDamageApplier
    {
        public event Action<bool> OnCollision;

        public void OnCollisionEnter(Collision collision)
        {
            bool hitPlayer = Apply(collision);
            OnCollision?.Invoke(hitPlayer);
        }
    }
}