using System.Collections;
using UnityEngine;

public class CatProjectileController : MonoBehaviour
{
    Rigidbody rb;

    public void Init(Vector3 direction)
    {
        transform.position += direction * 0.1f;
        rb = GetComponent<Rigidbody>();
        rb.velocity = direction * 5f;
        StartCoroutine(delayedEnableColliderRoutine());
    }

    IEnumerator delayedEnableColliderRoutine()
    {
        yield return new WaitForSeconds(0.1f);
        GetComponent<Collider>().enabled = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
