using System;
using UnityEngine;

namespace Game
{
    public class Effects : MonoBehaviour
    {
        public ParticleSystem damageEffect;

        public void InstantiateDamageEffect(float lifeTime)
        {
            Instantiate(damageEffect);
            Destroy(damageEffect, lifeTime);
        }
    }
}
