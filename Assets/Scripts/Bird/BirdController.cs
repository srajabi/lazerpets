using UnityEngine;


public class BirdController : MonoBehaviour
{
    [SerializeField] float MouseYawSensitivity = 1f;
    [SerializeField] float MousePitchSensitivity = 1f;
    [SerializeField] float KeyRollSensitivity = 1f;
    [SerializeField] float RollYawTrim = 0.5f;
    [SerializeField] float PitchClampDegrees = 45f;
    [SerializeField] float RollClampDegrees = 45f;

    [SerializeField] float FlapFrequency = 3f;
    [SerializeField] float Mass = 0.1f;
    [SerializeField] float DragMagic = 0.1f;
    [SerializeField] float LiftMagic = 0.01f;
    [SerializeField] float GlideMagic = 10f;
    [SerializeField] float FlapThrustToLiftRatioPressingW = .7f;
    [SerializeField] float FlapThrustToLiftRatioPressingNotPressingW = .3f;
    [SerializeField] float FlapPower = 5f;

    [SerializeField] float SizeOfShitRelativeToBird = .5f;
    [SerializeField] float ShitsPerMinute = 120f;

    Rigidbody rb;
    float _yawDegress;
    float _pitchDegrees;
    float _rollDegrees;

    float timeOfLastBowelMovement = 0f;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        rb = gameObject.GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.mass = Mass;
    }

    void FixedUpdate()
    {
        var orientation = UpdateOrientation();
        transform.rotation = orientation;

        var angleOfAttack = Mathf.Acos(Vector3.Dot(rb.velocity.normalized, transform.forward.normalized)) * Mathf.Rad2Deg;

        float locForwardVel = transform.InverseTransformDirection(rb.velocity).z;
        float drag = DragMagic * (Mathf.Pow(locForwardVel, 2) / 2) * angleOfAttack;

        var sign = (Quaternion.LookRotation(Vector3.forward, Vector3.up) * transform.forward.normalized).y;
        var signedAngleOfAttack = angleOfAttack;
        if (sign < 0) signedAngleOfAttack *= -1;

        if (Input.GetKey(KeyCode.Space))
        {
            float flapForce = Mathf.Cos(Time.time * FlapFrequency) * FlapPower;
            flapForce = Mathf.Abs(flapForce);
            var flapThrustToLiftRatio = Input.GetKey(KeyCode.W) ? FlapThrustToLiftRatioPressingW : FlapThrustToLiftRatioPressingNotPressingW;
            rb.AddForce(transform.forward * flapForce * flapThrustToLiftRatio);
            rb.AddForce(transform.up * flapForce * (1- flapThrustToLiftRatio));
        }

        var dragVector = rb.velocity * -drag;
        rb.AddForce(dragVector);

        float lift = (Mathf.Pow(locForwardVel, 2) * LiftMagic * signedAngleOfAttack) / 2;
        var liftVector = transform.up * lift;
        rb.AddForce(liftVector);
        rb.AddForce(Physics.gravity, ForceMode.Acceleration);

        // janky, no science glide bs
        var fallVelocity = rb.velocity.y;
        var glideRatio = lift / drag;
        if (fallVelocity < 0 && glideRatio < 0)
        {
            var forwardGlide = fallVelocity * glideRatio * GlideMagic;
            rb.AddForce(transform.forward * forwardGlide, ForceMode.VelocityChange);
        }

        bool readyForBM = Time.time - timeOfLastBowelMovement > ShitsPerMinute / 60;
        if (Input.GetKey(KeyCode.Mouse0) && readyForBM)
        {
            timeOfLastBowelMovement = Time.time;
            var shit = Instantiate(Resources.Load("Prefabs/BirdShit") as GameObject);
            shit.transform.localScale = transform.localScale * SizeOfShitRelativeToBird;
            var birdShit = shit.GetComponent<BirdShit>();
            birdShit.SetInitialVelocity(rb.velocity);
        }
    }

    Quaternion UpdateOrientation()
    {
        var mouseX = Input.GetAxis("Mouse X") * KeyRollSensitivity;
        _rollDegrees -= mouseX;
        _rollDegrees = Mathf.Clamp(_rollDegrees, -RollClampDegrees, RollClampDegrees);

        float yaw = 0f;
        if (Input.GetKey(KeyCode.A)) yaw -= MouseYawSensitivity;
        if (Input.GetKey(KeyCode.D)) yaw += MouseYawSensitivity;
        _yawDegress += yaw * MouseYawSensitivity;
        _yawDegress += mouseX * RollYawTrim;

        _pitchDegrees -= Input.GetAxis("Mouse Y") * MousePitchSensitivity;
        _pitchDegrees = Mathf.Clamp(_pitchDegrees, -PitchClampDegrees, PitchClampDegrees);

        return Quaternion.Euler(_pitchDegrees, _yawDegress, _rollDegrees);
    }
}