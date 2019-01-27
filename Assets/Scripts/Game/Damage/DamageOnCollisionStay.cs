using System;
using UnityEngine;

namespace Game
{
    public class DamageOnCollisionStay : BaseDamageApplier
    {
        [SerializeField]
        private float Interval;
        private float lastApplication;

        public event EventHandler OnCollision;

        public void OnCollisionStay(Collision collision)
        {
            var time = Time.time;
            if (time < lastApplication + Interval)
                return;

            if (Apply(collision))
            {
                lastApplication = time;
            }

            OnCollision?.Invoke(this, EventArgs.Empty);
        }
    }
}