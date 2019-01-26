using UnityEngine;

namespace Game
{
    public class SpawnPointTester : MonoBehaviour
    {
        public Transform A;
        public Transform B;
        public Transform C;

        [ContextMenu("TEST")]
        public void Test()
        {
            var furthest = SpawnPointManager.GetFurthestPoint(A, B);
            furthest.Occupy(C);
            C.position = furthest.transform.position;
        }
    }
}