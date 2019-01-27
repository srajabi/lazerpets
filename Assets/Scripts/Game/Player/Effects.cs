using System;
using UnityEngine;

namespace Game
{
    public class Effects : MonoBehaviour
    {
        public Player Player;
        public enum Effect { Damage, Win, Loss };

        public void ApplyEffect(Effect effect, Vector3 point, Vector3 normal)
        {
            switch(effect)
            {
                case Effect.Damage:
                    InstantiateDamageEffect(point, normal);
                    break;
                case Effect.Win:
                    break;
                case Effect.Loss:
                    break;
            }
        }

        public float DamageLifetime;
        public GameObject DamageEffect;

        public void InstantiateDamageEffect(Vector3 point, Vector3 normal)
        {
            Debug.LogError("Point: " + point + " Normal: " + normal);
            GameObject effect = GameObject.Instantiate(
                DamageEffect, 
                point, 
                Quaternion.LookRotation(normal) * Quaternion.Euler(0, 270, 0));
            effect.SetActive(true);
            Destroy(effect, DamageLifetime);
        }
    }
}
