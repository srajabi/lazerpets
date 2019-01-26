using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    private static readonly Vector3 startPosition = new Vector3(-14.25f, 1.231f, 2.436f);
    private static readonly Quaternion startRotation = Quaternion.Euler(0, 90, 0);

    private Camera gameCamera;
    private Camera tvCamera;

    public Camera UICamera;
    public Button Play;
    public Button Options;
    public Button Quit;

    public void Initialize(Camera gameCamera, Camera tvCamera)
    {
        this.gameCamera = gameCamera ?? throw new ArgumentNullException(nameof(gameCamera));
        this.tvCamera = tvCamera ?? throw new ArgumentNullException(nameof(tvCamera));
    }

    private void Start()
    {
        Play.onClick.AddListener(() => OnPlay());
        Options.onClick.AddListener(() => OnOptions());
        Quit.onClick.AddListener(() => OnQuit());
    }

    private void OnPlay()
    {
        PlayExitMenuAnimation();
        MoveCameraBack();
    }

    private void OnOptions()
    {
    }

    private void OnQuit()
    {
        Application.Quit();
    }

    private void PlayExitMenuAnimation()
    {
        StartCoroutine(CameraAnimation());
    }

    private IEnumerator CameraAnimation()
    {
        while (Vector3.Distance(UICamera.transform.position, gameCamera.transform.position) > 0.05)
        {
            UICamera.transform.position = Vector3.Lerp(
                UICamera.transform.position,
                gameCamera.transform.position,
                Time.deltaTime * 0.7f);

            UICamera.transform.rotation = Quaternion.Lerp(
                UICamera.transform.rotation,
                gameCamera.transform.rotation,
                Time.deltaTime * 0.7f);

            yield return null;
        }

        HandoffCameraToGame();
    }

    private void HandoffCameraToGame()
    {
        UICamera.gameObject.SetActive(false);
        gameCamera.gameObject.SetActive(true);
        tvCamera.gameObject.SetActive(true);

        UICamera.transform.rotation = startRotation;
        UICamera.transform.position = startPosition;
    }

    private void MoveCameraBack()
    {
        UICamera.transform.position = startPosition;
    }
}
