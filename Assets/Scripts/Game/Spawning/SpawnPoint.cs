using System;
using UnityEngine;

namespace Game
{
    public class SpawnPoint : MonoBehaviour
    {
        [SerializeField]
        private CharacterTypes _SpawnType;
        public CharacterTypes SpawnType { get { return _SpawnType; } }
        
        [SerializeField]
        private float Radius = 2;
        private float SqrRadius { get { return Radius * Radius; } }

        public bool IsVacant { get { return occupant == null; } }
        private Transform occupant;

        public void OnDrawGizmosSelected()
        {
            switch (SpawnType)
            {
                case CharacterTypes.Unassigned:
                    Gizmos.color = IsVacant ? Color.magenta : Color.red;
                    break;
                case CharacterTypes.Cat:
                    Gizmos.color = IsVacant ? Color.cyan : Color.blue;
                    break;
                case CharacterTypes.Dog:
                    Gizmos.color = IsVacant ? Color.green : Color.Lerp(Color.green, Color.blue, 0.3f);
                    break;
                case CharacterTypes.Bird:
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