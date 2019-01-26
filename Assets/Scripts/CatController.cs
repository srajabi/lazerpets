using UnityEngine;

public class CatController : MonoBehaviour
{
    [SerializeField] float extraHeight;
    [SerializeField] float mouseYawSensitivity;
    [SerializeField] float mousePitchSensitivity;
    [SerializeField] float pitchClampDegrees;

    [SerializeField] GameObject childHead;
    GameObject childCamera;

    Rigidbody rb;
    float radius;

    float _yawDegrees;
    float _pitchDegrees;

    int grounded;
    float cameraBobT;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        radius = GetComponent<SphereCollider>().radius;
        childCamera = childHead.GetComponentInChildren<Camera>().gameObject;
        cameraBobT = 0;

        Cursor.lockState = CursorLockMode.Locked;
    }

    Quaternion updateOrientation()
    {
        _yawDegrees += Input.GetAxis("Mouse X") * mouseYawSensitivity;
        _pitchDegrees -= Input.GetAxis("Mouse Y") * mousePitchSensitivity;
        _pitchDegrees = Mathf.Clamp(_pitchDegrees, -pitchClampDegrees, pitchClampDegrees);
        return Quaternion.Euler(_pitchDegrees, _yawDegrees, 0f);
    }

    private void Update()
    {
        cameraBobT += rb.velocity.WithY(0).magnitude * 0.05f;
        childHead.transform.rotation = updateOrientation();
        childCamera.transform.localPosition = 0.02f * cameraBob(cameraBobT);
    }

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.W)) {
            rb.velocity += childHead.transform.forward * 0.1f;
        }
        else if (Input.GetKey(KeyCode.S)) {
            rb.velocity += childHead.transform.forward * -0.1f;
        }

        if (Input.GetKey(KeyCode.D)) {
            rb.velocity += childHead.transform.right * 0.1f;
        }
        else if (Input.GetKey(KeyCode.A)) {
            rb.velocity += childHead.transform.right * -0.1f;
        }

        if (Input.GetKey(KeyCode.Space) && grounded > 0) {
            grounded = 0;
            rb.velocity += Vector3.up * 5;
        }

        rb.velocity = rb.velocity.WithX(rb.velocity.x * 0.8f);
        rb.velocity = rb.velocity.WithZ(rb.velocity.z * 0.8f);

        rb.velocity += Physics.gravity * Time.fixedDeltaTime;
        var newPosition = rb.position + rb.velocity * Time.fixedDeltaTime;

        if (grounded > 0) grounded--;

        // Put a kinematic rigidbody dangling from the player's head to smash plates around.
        // TODO SphereCastAll because if you touch a wall then stuff breaks.
        RaycastHit hitInfo;
        if (Physics.SphereCast(newPosition, radius, Vector3.down, out hitInfo, extraHeight, LayerMask.GetMask("Default")))
        {
            var fixedCastPos = newPosition + Vector3.down * hitInfo.distance;
            newPosition = fixedCastPos + Vector3.up * extraHeight;
            rb.velocity = rb.velocity.WithY(0);
            rb.velocity += (newPosition - rb.position) * 10f;

            grounded = 2;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + Vector3.down * extraHeight, GetComponent<SphereCollider>().radius);
    }

    static Vector2 cameraBob(float t)
    {
        var x = Mathf.Sin(t);
        var y = -x * x;
        return new Vector2(x, y);
    }
}
