using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class SpawnPointManager : MonoBehaviour
    {
        private static SpawnPointManager instance;

        [SerializeField]
        private SpawnPoint[] SpawnPoints;

        public void Awake()
        {
            instance = this;
        }

        public void OnValidate()
        {
            SpawnPoints = GetComponentsInChildren<SpawnPoint>();
        }

        public void OnDrawGizmosSelected()
        {
            foreach (var point in SpawnPoints)
            {
                point.OnDrawGizmosSelected();
            }
        }

        public static bool AnyVacantPoints()
        {
            return instance.SpawnPoints.Any(p => p.IsVacant);
        }

        public static SpawnPoint GetFirstVacantPoint()
        {
            return instance.SpawnPoints.First(p => p.IsVacant);
        }

        public static SpawnPoint GetRandomVacantPoint()
        {
            var points = instance.SpawnPoints.Where(p => p.IsVacant);
            int count = points.Count();
            return points.ElementAt(Random.Range(0, count));
        }

        public static SpawnPoint GetFirstVacantPoint(CharacterTypes type)
        {
            return instance.SpawnPoints.First(p => p.IsVacant && p.SpawnType == type);
        }

        public static SpawnPoint GetRandomVacantPoint(CharacterTypes type)
        {
            var points = instance.SpawnPoints.Where(p => p.IsVacant && p.SpawnType == type);
            int count = points.Count();
            return points.ElementAt(Random.Range(0, count));
        }

        public static SpawnPoint GetFurthestPoint(params Transform[] objects)
        {
            var points = instance.SpawnPoints.Where(p => p.IsVacant);
            float maxDistance = float.MinValue;
            SpawnPoint candidate = null;

            foreach (var point in points)
            {
                float distance = float.MaxValue;

                foreach (var obj in objects)
                {
                    float dist = (obj.position - point.transform.position).sqrMagnitude;
                    distance = Mathf.Min(dist, distance);
                }

                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    candidate = point;
                }
            }

            return candidate;
        }

        public static SpawnPoint GetFurthestPoint(CharacterTypes types, params Transform[] objects)
        {
            var points = instance.SpawnPoints.Where(p => p.IsVacant && p.SpawnType == types);
            float maxDistance = float.MinValue;
            SpawnPoint candidate = null;

            foreach (var point in points)
            {
                float distance = float.MaxValue;

                foreach (var obj in objects)
                {
                    float dist = (obj.position - point.transform.position).sqrMagnitude;
                    distance = Mathf.Min(dist, distance);
                }

                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    candidate = point;
                }
            }

            return candidate;
        }
    }
}