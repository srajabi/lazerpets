using System;
using UnityEngine;

namespace Game
{
    public enum SpawnTypes
    {
        Unassigned = 0,
        Cat        = 1,
        Dog        = 2,
        Bird       = 3,
    }

    public class SpawnPoint : MonoBehaviour
    {
        [SerializeField]
        private SpawnTypes _SpawnType;
        public SpawnTypes SpawnType { get { return _SpawnType; } }
        
        [SerializeField]
        private float Radius = 2;
        private float SqrRadius { get { return Radius * Radius; } }

        public bool IsVacant { get { return occupant == null; } }
        private Transform occupant;

        public void OnDrawGizmosSelected()
        {
            switch (SpawnType)
            {
                case SpawnTypes.Unassigned:
                    Gizmos.color = IsVacant ? Color.magenta : Color.red;
                    break;
                case SpawnTypes.Cat:
                    Gizmos.color = IsVacant ? Color.cyan : Color.blue;
                    break;
                case SpawnTypes.Dog:
                    Gizmos.color = IsVacant ? Color.green : Color.Lerp(Color.green, Color.blue, 0.3f);
                    break;
                case SpawnTypes.Bird:
                    Gizmos.color = IsVacant ? Color.Lerp(Color.yellow, Color.red, 0.5f) : Color.yellow;
                    break;
                default:
                    break;
            }

            Gizmos.DrawWireSphere(transform.position, Radius);
        }

        public void Update()
        {
            CheckOccupancy();
        }

        private void CheckOccupancy()
        {
            if (IsVacant)
                return;

            var sqrDistance = (transform.position - occupant.position).sqrMagnitude;
            if (sqrDistance > SqrRadius)
            {
                occupant = null;
            }
        }

        public void Occupy(Transform occupant)
        {
            if (!IsVacant)
                return;

            this.occupant = occupant;
        }
    }
}