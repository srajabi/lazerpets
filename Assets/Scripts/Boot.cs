using UnityEngine;
using UnityEngine.SceneManagement;

public class Boot : MonoBehaviour
{
    private void Start()
    {
        SceneManager.LoadScene("Game", LoadSceneMode.Additive);
        var uiPrefab = Resources.Load<GameObject>("Prefabs/UI");
        var gameCameraPrefab = Resources.Load<Camera>("Prefabs/GameCamera");
        var tvCameraPrefab = Resources.Load<Camera>("Prefabs/TVCamera");

        var ui = Instantiate(uiPrefab);
        var gameCamera = Instantiate(gameCameraPrefab);
        var tvCamera = Instantiate(tvCameraPrefab);

        var canvasController = ui.GetComponentInChildren<TVCanvasController>();
        canvasController.Initialize(gameCamera, tvCamera);
    }
}
