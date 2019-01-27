using Game;
using System.Linq;
using UnityEngine;

public class BirdShit : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] float Mass = 0.1f;
    [SerializeField] float Drag = 10f;
    [SerializeField] float ExplosionRadius = 2f;
    [SerializeField] float ExplosionForce = 100f;
    [SerializeField] float Damage = 10f;

    float ttl = 10f;
    float delay = 0.1f;
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
        if (Time.time - creationTime > delay)
        {
            GetComponent<Collider>().enabled = true;
        }

        if (Time.time - creationTime > ttl)
        {
            Destroy(this.gameObject);
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (GameObject.ReferenceEquals(col.gameObject, gameObject))
        {
            return;
        }

        Explode();
    }

    void Explode()
    {
        var caughtUpInABadTime = Physics.OverlapSphere(transform.position, ExplosionRadius, LayerMask.GetMask("Animals"));

        foreach (var c in caughtUpInABadTime.Where(c => !GameObject.ReferenceEquals(c.gameObject, this.gameObject)))
        {
            var rbInExplosion = c.gameObject.GetComponent<Rigidbody>();

            if (rbInExplosion != null)
            {
                rbInExplosion.AddExplosionForce(ExplosionForce, transform.position, ExplosionRadius);
            }

            var player = c.gameObject.GetComponentInParent<Player>();
            if (player != null)
            {
                player.Health.Modify(-Damage, new Damager(gameObject));
            }
        }

        Destroy(this.gameObject);
    }
}
