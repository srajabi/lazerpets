using UnityEngine;

public class SpinnyBoi : MonoBehaviour
{
    [SerializeField] private float rotationsPerMinute;
    [SerializeField] private Vector3 axis;

    private void FixedUpdate()
    {
        this.transform.Rotate(axis, RPMToAnglePer60Seconds());
    }

    private float RPMToAnglePer60Seconds()
    {
        return rotationsPerMinute / 10;
    }
}
