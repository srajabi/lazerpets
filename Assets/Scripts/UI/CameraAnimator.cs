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
        while (Vector3.Distance(sourceCamera.transform.position, destinationCamera.transform.position) > 0.05)
        {
            sourceCamera.transform.position = Vector3.Lerp(
                sourceCamera.transform.position,
                destinationCamera.transform.position,
                Time.deltaTime * 0.7f);

            sourceCamera.transform.rotation = Quaternion.Lerp(
                sourceCamera.transform.rotation,
                destinationCamera.transform.rotation,
                Time.deltaTime * 0.7f);

            yield return null;
        }

        OnAnimationFinished?.Invoke();

        sourceCamera.gameObject.SetActive(false);
        destinationCamera.gameObject.SetActive(true);

        sourceCamera.transform.rotation = startRotation;
        sourceCamera.transform.position = startPosition;
    }
}
