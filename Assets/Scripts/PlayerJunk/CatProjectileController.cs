using Game;
using System;
using System.Collections;
using UnityEngine;

public class CatProjectileController : MonoBehaviour
{
    private Rigidbody rb;
    private DamageOnCollisionEnter damager;
    private Collider myCollider;
    private int hits = 2;

    public void Init(Vector3 direction)
    {
        rb = GetComponent<Rigidbody>();
        damager = GetComponent<DamageOnCollisionEnter>();
        myCollider = GetComponent<Collider>();

        damager.OnCollision += OnCollision;

        transform.position += direction * 0.1f;
        rb.velocity = direction * 5f;
        StartCoroutine(DelayedEnableColliderRoutine());
        StartCoroutine(DestroyAfterLifeTime());
    }

    private void OnCollision(bool hitPlayer)
    {
        if (hits <= 0 || hitPlayer)
        {
            Destroy(gameObject);
        }
        else
        {
            hits--;
        }
    }

    IEnumerator DelayedEnableColliderRoutine()
    {
        yield return new WaitForSeconds(0.1f);
        myCollider.enabled = true;
    }

    IEnumerator DestroyAfterLifeTime()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
