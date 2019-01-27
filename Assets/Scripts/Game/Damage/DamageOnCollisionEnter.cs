using System;
using UnityEngine;

namespace Game
{
    public class DamageOnCollisionEnter : BaseDamageApplier
    {
        public event EventHandler OnCollision;

        public void OnCollisionEnter(Collision collision)
        {
            Apply(collision);
            OnCollision?.Invoke(this, EventArgs.Empty);
        }
    }
}