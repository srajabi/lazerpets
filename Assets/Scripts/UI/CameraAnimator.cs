using System;
using System.Collections;
using UnityEngine;

public class CameraAnimator
{
    public event Action OnAnimationFinished;

    private readonly Camera sourceCamera;
    private readonly Camera destinationCamera;

    private readonly Vector3 startPosition;
    private readonly Quaternion startRotation;

    public CameraAnimator(Camera sourceCamera, Camera destinationCamera)
    {
        this.sourceCamera = sourceCamera;
        this.destinationCamera = destinationCamera;

        this.startPosition = sourceCamera.transform.position;
        this.startRotation = sourceCamera.transform.rotation;
    }

    public IEnumerator Animate()
    {
        var startTime = Time.time;
        while (Vector3.Distance(sourceCamera.transform.position, destinationCamera.transform.position) > 0.05)
        {
            var timeSinceStarted = Time.time - startTime;

            sourceCamera.transform.position = Vector3.Lerp(
                sourceCamera.transform.position,
                destinationCamera.transform.position,
                Time.deltaTime * timeSinceStarted);

            sourceCamera.transform.rotation = Quaternion.Lerp(
                sourceCamera.transform.rotation,
                destinationCamera.transform.rotation,
                Time.deltaTime * timeSinceStarted);

            yield return null;
        }

        OnAnimationFinished?.Invoke();

        sourceCamera.gameObject.SetActive(false);
        destinationCamera.gameObject.SetActive(true);

        sourceCamera.transform.rotation = startRotation;
        sourceCamera.transform.position = startPosition;
    }
}
