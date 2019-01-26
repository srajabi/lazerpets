using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This component controls the behaviour of the player character.
/// </summary>
public class BrokenCatController : MonoBehaviour 
{
    #region Component Configuration

        [Tooltip("Multiplier on the mouse's horizontal change.")]
        [SerializeField] float _mouseYawSensitivity = 1f;

        [Tooltip("Multiplier on the mouse's vertical change.")]
        [SerializeField] float _mousePitchSensitivity = 1f;

        [Tooltip("Degrees away from parallel with the ground which the player can look.")]
        [SerializeField] float _pitchClampDegrees = 45f;

        [Tooltip("Multiplier on the amount to sway the camera when walking.")]
        [SerializeField] float _cameraBobMagnitude = 0.1f;

        [Tooltip("Frequency of the camera sway animation.")]
        [SerializeField] float _cameraBobSpeed = 10f;

        [Tooltip("Maximum distance the player can be affected by rocket jumps.")]
        [SerializeField] float _maxRocketJumpDistance = 10f;

        [Tooltip("Rocket jumps that occur this far above the player will be ignored.")]
        [SerializeField] float _minRocketJumpDeltaY = 1f;

        [Tooltip("Maximum power a rocket jump can apply to the player.")]
        [SerializeField] float _maxRocketJumpPower = 100f;

        [Tooltip("If the player falls below this y-level in world space, they are respawned.")]
        [SerializeField] float _deathElevation = -10f;

        [Tooltip("Time the player must wait between gun shots.")]
        [SerializeField] float _minTimeBetweenShots = 0.1f;

        [Tooltip("If the player stands on a collider on these layers, they are not allowed to shoot.")]
        [SerializeField] LayerMask _layersPreventingShooting;

        [Tooltip("Configuration for PlayerMover.")]
        [SerializeField] PlayerMover.Configuration _movementConfig;

        [Tooltip("Prefab of the TNT platform to fire from the gun.")]
        [SerializeField] GameObject _platformPrefab;

    #endregion 

    //Transform _headChild;
    Camera _cameraChild;
    // HUDController _hud;
    // SoundPlayer _sounds;

    float _yawDegress;
    float _pitchDegrees;
    Vector3 _movementDirection;

    float _playerHeight;
    PlayerMover _playerMover;
    // PlatformController _currentPlatform;

    float _timeLastShot;
    bool _ableToShoot;
    bool _respawning;

    void Start()
    {
        // transform.position = CheckpointController.LatestPosition;

        //_headChild = transform.Find("Head");
        _cameraChild = GetComponentInChildren<Camera>();
        // _hud = FindObjectOfType<HUDController>();
        // _sounds = FindObjectOfType<SoundPlayer>();

        _playerHeight = 0.1f; //_headChild.transform.localPosition.y;
        _playerMover = new PlayerMover(_playerHeight, transform.position, _movementConfig);

        Cursor.lockState = CursorLockMode.Locked;
        // ScoreTracker.StartTime = Time.time;
    }

    void FixedUpdate()
    {
        _playerMover.Step(_movementDirection, Input.GetKey(KeyCode.Space));

        _ableToShoot = (_layersPreventingShooting.value & (1 << _playerMover.LatestStandingLayer)) == 0;

        // Handle falling below the death elevation and respawning.
        if (!_respawning && _playerMover.Position.y < _deathElevation) {
            _respawning = true;
            // ScoreTracker.DeathCount++;
            // var fader = FindObjectOfType<CameraFader>();
            // fader.FadeOut(() => {
            //     _playerMover.SetPosition(CheckpointController.LatestPosition);
            //     GameObjectUtils.DestroyAll<PlatformController>();
            //     fader.FadeIn();
            //     _respawning = false;
            // });
        }
    }
    
    void Update()
    {
        transform.position = _playerMover.GetRenderPosition();

        // Read camera look orientation from inputs and update local state.
        var orientation = updateOrientation();
        //_headChild.transform.rotation = orientation;
        _movementDirection = getMovementDirection(orientation);

        // Offset the camera's local position for the camera bobbing effect, and tell HUD to bob gun.
        var bob = _playerMover.GetSpeedRatio() * _cameraBobMagnitude * cameraBob(_cameraBobSpeed * Time.time);
        _cameraChild.transform.localPosition = bob.AsVector3WithZ(0f);
        // _hud.SetGunBob(bob);

        // Handle firing new platforms when the primary mouse button is clicked, and we're not pausing time.
        if (_ableToShoot && Time.time > _timeLastShot + _minTimeBetweenShots && Input.GetMouseButtonDown(0) && !Input.GetMouseButton(1)) {
        //    var platformObj = Instantiate(_platformPrefab, transform.position + Vector3.up * _playerHeight, Quaternion.identity);
        //    _currentPlatform = platformObj.GetComponent<PlatformController>();
        //    _currentPlatform.Init(this, _cameraChild.transform.rotation * Vector3.forward);
        //    _hud.AnimateGunShot();
        //    _sounds.PlayGunshotSound();
        //    _timeLastShot = Time.time;
        //    ScoreTracker.ShotCount++;
        }

        // Handle pausing time on platforms when the secondary mouse button is clicked.
    //    if (Input.GetMouseButtonDown(1)) {
    //        foreach (var p in FindObjectsOfType<PlatformController>()) {
    //            p.SetPaused(true);
    //        }
    //    } else if (Input.GetMouseButtonUp(1)) {
    //        foreach (var p in FindObjectsOfType<PlatformController>()) {
    //            p.SetPaused(false);
    //        }
    //    }

        //_sounds.SetFootstepsPlaying(_playerMover.Standing && _movementDirection.sqrMagnitude > 0.5f);
    }

    /// <summary>
    /// Applies an impulse to the player from a point in world space. Fall-off of the impulse
    /// is linear relative to distance from the source.
    /// <summary>
    /// <param name="sourcePosition"> World space position of the explosion. </param>
    /// <param name="power"> Power of the explosion in units of velocity * distance. </param>
    public void RocketJump(Vector3 sourcePosition, float power)
    {
        var ds = _playerMover.Position - sourcePosition;
        if (ds.y > _minRocketJumpDeltaY && ds.sqrMagnitude < _maxRocketJumpDistance * _maxRocketJumpDistance) {
            var scaledPower = power / ds.magnitude;
            if (scaledPower > _maxRocketJumpPower) {
                scaledPower = _maxRocketJumpPower;
            }
            _playerMover.ApplyImpulse(ds.normalized * scaledPower);
        }
    }

    static Vector2 cameraBob(float t)
    {
        var x = Mathf.Sin(t);
        var y = -x * x;
        return new Vector2(x, y);
    }

    Quaternion updateOrientation()
    {
        _yawDegress += Input.GetAxis("Mouse X") * _mouseYawSensitivity;
        _pitchDegrees -= Input.GetAxis("Mouse Y") * _mousePitchSensitivity;
        _pitchDegrees = Mathf.Clamp(_pitchDegrees, -_pitchClampDegrees, _pitchClampDegrees);
        return Quaternion.Euler(_pitchDegrees, _yawDegress, 0f);
    }

    static Vector3 getMovementDirection(Quaternion orientation)
    {
        var moveForward = (orientation * Vector3.forward).WithY(0).normalized;
        var moveRight   = (orientation * Vector3.right)  .WithY(0).normalized;

        var movement = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) movement += moveForward;
        if (Input.GetKey(KeyCode.S)) movement -= moveForward;
        if (Input.GetKey(KeyCode.D)) movement += moveRight;
        if (Input.GetKey(KeyCode.A)) movement -= moveRight;
        return movement.normalized;
    }
}