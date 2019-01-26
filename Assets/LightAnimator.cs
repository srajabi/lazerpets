using UnityEngine;

public class LightAnimator : MonoBehaviour
{
    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    private void Update()
    {
        var currentLightPosition = (0.18f * Mathf.Sin(Time.time * 3f)) * Vector3.right;
        transform.position = startPosition + currentLightPosition;
    }
}
