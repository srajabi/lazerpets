using Game;
using System;
using System.Collections;
using UnityEngine;

public class CatProjectileController : MonoBehaviour
{
    private Rigidbody rb;
    private DamageOnCollisionEnter damager;

    public void Init(Vector3 direction)
    {
        rb = GetComponent<Rigidbody>();
        damager = GetComponent<DamageOnCollisionEnter>();
        damager.OnCollision += OnCollision;

        transform.position += direction * 0.1f;
        rb.velocity = direction * 5f;
        StartCoroutine(DelayedEnableColliderRoutine());
    }

    private void OnCollision(object sender, EventArgs e)
    {
        Destroy(gameObject);
    }

    IEnumerator DelayedEnableColliderRoutine()
    {
        yield return new WaitForSeconds(0.1f);
        GetComponent<Collider>().enabled = true;
    }
}
