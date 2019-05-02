using UnityEngine;

public class AxisBasedThrobEffect : MonoBehaviour
{
    [SerializeField] private Vector3 axis = new Vector3(0, 0, 1);
    [SerializeField] private float frequency = 3f;
    [SerializeField] private float amplitude = 0.18f;

    private Vector3 startPosition;

    private void Start()
    { 
        startPosition = transform.position;
    }

    private void Update()
    {
        var currentPosition = (amplitude * Mathf.Sin(Time.time * frequency)) * axis;
        transform.position = startPosition + currentPosition;
    }
}


