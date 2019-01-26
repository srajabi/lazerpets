using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TVCanvasController : MonoBehaviour
{
    private const string LocalHostIP = "127.0.0.1";

    [SerializeField] private Camera UICamera;
    [SerializeField] private Button CreateServer;
    [SerializeField] private Button Play;
    [SerializeField] private Button Back;
    [SerializeField] private InputField IPAddressField;
    [SerializeField] private Button Options;
    [SerializeField] private Button Quit;
    [SerializeField] private GameCanvasController GameCanvas;

    private Camera gameCamera;
    private Camera tvCamera;

    private CameraAnimator cameraAnimator;

    private string ipAddress;

    public void Initialize(Camera gameCamera, Camera tvCamera)
    {
        this.gameCamera = gameCamera ?? throw new ArgumentNullException(nameof(gameCamera));
        this.tvCamera = tvCamera ?? throw new ArgumentNullException(nameof(tvCamera));

        cameraAnimator = new CameraAnimator(UICamera, gameCamera);
        cameraAnimator.OnAnimationFinished += HandoffCameraToGame;

        ResetState();
    }

    private void OnPlay()
    {
        if (string.IsNullOrWhiteSpace(IPAddressField.text))
        {
            IPAddressField.text = LocalHostIP;
        }

        IPAddressField.gameObject.SetActive(true);
        Play.GetComponentInChildren<Text>().text = "Connect";
        Options.gameObject.SetActive(false);
        Quit.gameObject.SetActive(false);
        Back.gameObject.SetActive(true);

        Play.onClick.RemoveAllListeners();
        Play.onClick.AddListener(() => OnConnect());
    }

    private void OnConnect()
    {
        ipAddress = IPAddressField.text;
        PlayExitMenuAnimationAndConnect();
    }

    private void OnCreateServer()
    {
        ipAddress = "0.0.0.0"; //Get an ip address plz
        PlayExitMenuAnimationAndConnect();
    }

    private void OnOptions()
    {
    }

    private void OnQuit()
    {
        Application.Quit();
    }

    private void OnBack()
    {
        ResetState();
    }

    private void PlayExitMenuAnimationAndConnect()
    {
        StartCoroutine(cameraAnimator.Animate());
    }

    private void HandoffCameraToGame() // we can make this a IEnumerator if connecting is async
    {
        tvCamera.gameObject.SetActive(true);

        GameCanvas.Initialize(ipAddress, tvCamera, UICamera, gameCamera, this);
        GameCanvas.gameObject.SetActive(true);

        // TODO: Connect To Server, or we can do it while animating, and then whichever finishes last is the thing that triggers the connect thingy;
        // inject IP into ScreenSpace Canvas;
    }

    private void ResetState()
    {
        Play.GetComponentInChildren<Text>().text = "Play";

        CreateServer.gameObject.SetActive(true);
        Play.gameObject.SetActive(true);
        IPAddressField.gameObject.SetActive(false);
        Options.gameObject.SetActive(true);
        Quit.gameObject.SetActive(true);
        Back.gameObject.SetActive(false);

        CreateServer.onClick.RemoveAllListeners();
        Play.onClick.RemoveAllListeners();
        Options.onClick.RemoveAllListeners();
        Quit.onClick.RemoveAllListeners();
        Back.onClick.RemoveAllListeners();

        CreateServer.onClick.AddListener(() => OnCreateServer());
        Play.onClick.AddListener(() => OnPlay());
        Options.onClick.AddListener(() => OnOptions());
        Quit.onClick.AddListener(() => OnQuit());
        Back.onClick.AddListener(() => OnBack());
    }
}
