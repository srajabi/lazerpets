using UnityEngine;

public class CatController : MonoBehaviour
{
    [SerializeField] float extraHeight;

    Rigidbody rb;
    float radius;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        radius = GetComponent<SphereCollider>().radius;
    }

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.W)) {
            rb.velocity += transform.forward * 0.1f;
        }
        else if (Input.GetKey(KeyCode.S)) {
            rb.velocity += transform.forward * -0.1f;
        }

        if (Input.GetKey(KeyCode.D)) {
            rb.velocity += transform.right * 0.1f;
        }
        else if (Input.GetKey(KeyCode.A)) {
            rb.velocity += transform.right * -0.1f;
        }

        rb.velocity = rb.velocity.WithX(rb.velocity.x * 0.8f);
        rb.velocity = rb.velocity.WithZ(rb.velocity.z * 0.8f);

        rb.velocity += Physics.gravity * Time.fixedDeltaTime;
        var newPosition = rb.position + rb.velocity * Time.fixedDeltaTime;

        RaycastHit hitInfo;
        if (Physics.SphereCast(newPosition, radius, Vector3.down, out hitInfo, extraHeight, LayerMask.GetMask("Default")))
        {
            var fixedCastPos = newPosition + Vector3.down * hitInfo.distance;
            newPosition = fixedCastPos + Vector3.up * extraHeight;
            rb.velocity = rb.velocity.WithY(0);
            rb.velocity += (newPosition - rb.position) * 10f;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + Vector3.down * extraHeight, GetComponent<SphereCollider>().radius);
    }
}
