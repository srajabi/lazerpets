using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BirdShit : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] float Mass = 0.1f;
    [SerializeField] float Drag = 10f;
    [SerializeField] float ExplosionRadius = 1f;
    [SerializeField] float ExplosionForce = 100f;

    float ttl = 10f;
    float creationTime = 0f;

    // Start is called before the first frame update
    void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.mass = Mass;
        rb.drag = Drag;
        creationTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - creationTime > ttl)
        {
            Destroy(this.gameObject);
        }
    }

    void OnCollisionEnter(Collision col)
    {
        Explode();
    }

    void Explode()
    {
        var caughtUpInABadTime = Physics.OverlapSphere(transform.position, ExplosionRadius, LayerMask.GetMask("Animals"));

        foreach (var c in caughtUpInABadTime)
        {
            var rbInExplosion = c.gameObject.GetComponent<Rigidbody>();

            if (rbInExplosion != null)
            {
                Debug.LogError("explosisdifisdf");
                rbInExplosion.AddExplosionForce(ExplosionForce, transform.position, ExplosionRadius);
            }
        }

        Destroy(this.gameObject);
    }

    public void SetInitialVelocity(Vector3 velocity)
    {
        rb.AddForce(velocity, ForceMode.Impulse);
    }
}
