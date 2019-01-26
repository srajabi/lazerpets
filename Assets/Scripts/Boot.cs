using UnityEngine;
using UnityEngine.SceneManagement;

public class Boot : MonoBehaviour
{
    private void Start()
    {
        SceneManager.LoadScene("MainLevel");
        var uiPrefab = Resources.Load<CanvasController>("Prefabs/UI");
        var gameCameraPrefab = Resources.Load<Camera>("Prefabs/GameCamera");
        var tvCameraPrefab = Resources.Load<Camera>("Prefabs/TVCamera");

        var ui = Instantiate(uiPrefab);
        var gameCamera = Instantiate(gameCameraPrefab);
        var tvCamera = Instantiate(tvCameraPrefab);

        ui.Initialize(gameCamera, tvCamera);
    }
}
