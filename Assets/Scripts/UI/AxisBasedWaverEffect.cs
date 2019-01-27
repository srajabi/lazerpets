using UnityEngine;

public class AxisBasedWaverEffect : MonoBehaviour
{
    [SerializeField] private Vector3 axis = new Vector3(0, 0, 1);
    [SerializeField] private float frequency = 3f;
    [SerializeField] private float amplitude = 0.18f;

    private Quaternion startRotation;

    private void Start()
    {
        startRotation = transform.rotation;
    }

    private void Update()
    {
        var currentPosition = (amplitude * Mathf.Sin(Time.time * frequency)) * axis;
        transform.rotation = startRotation * Quaternion.Euler(currentPosition);
    }
}
