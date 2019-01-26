using System;
using UnityEngine;

[Serializable]
public class CritterMoverConfig
{
    public float extraHeight;
    public float suspensionRadiusRatio;
    public AttackKind attackKind;
}

public class CritterMover
{
    readonly GameObject critter;
    readonly CritterMoverConfig config;

    readonly GameObject childHead;
    readonly GameObject childCamera;
    readonly Rigidbody rb;
    readonly float radius;
    readonly float suspensionRadius;
    readonly IAttackLauncher launcher;

    int grounded;
    float cameraBobT;

    public CritterMover(GameObject critter, CritterMoverConfig config)
    {
        this.critter = critter;
        this.config = config;

        rb = critter.GetComponent<Rigidbody>();
        radius = critter.GetComponent<SphereCollider>().radius;
        childHead = critter.transform.Find("Head").gameObject;
        childCamera = critter.GetComponentInChildren<Camera>().gameObject;
        cameraBobT = 0;
        suspensionRadius = config.suspensionRadiusRatio * radius;
        launcher = AttackLauncherFactory.Create(config.attackKind);
    }

    public void UpdateImmediate(CritterInputPacket packet)
    {
        cameraBobT += rb.velocity.WithY(0).magnitude * 0.05f;
        childHead.transform.rotation = packet.headOrientation;
        childCamera.transform.localPosition = 0.02f * cameraBob(cameraBobT);
    }

    public CritterStatePacket UpdateTick(CritterInputPacket packet)
    {
        if (packet.forward) {
            rb.velocity += childHead.transform.forward * 0.1f;
        }
        else if (packet.backward) {
            rb.velocity += childHead.transform.forward * -0.1f;
        }

        if (packet.rightward) {
            rb.velocity += childHead.transform.right * 0.1f;
        }
        else if (packet.leftward) {
            rb.velocity += childHead.transform.right * -0.1f;
        }

        if (packet.jump && grounded > 0) {
            grounded = 0;
            rb.velocity += Vector3.up * 5;
        }

        rb.velocity = rb.velocity.WithX(rb.velocity.x * 0.8f);
        rb.velocity = rb.velocity.WithZ(rb.velocity.z * 0.8f);

        rb.velocity += Physics.gravity * Time.fixedDeltaTime;
        var newPosition = rb.position + rb.velocity * Time.fixedDeltaTime;

        if (grounded > 0) grounded--;

        var hits = Physics.SphereCastAll(newPosition, suspensionRadius, Vector3.down, config.extraHeight, LayerMask.GetMask("Default"));
        var hitInfo = findImportantHit(hits);
        if (hitInfo.HasValue)
        {
            var fixedCastPos = newPosition + Vector3.down * hitInfo.Value.distance;
            newPosition = fixedCastPos + Vector3.up * config.extraHeight;
            rb.velocity = rb.velocity.WithY(0);
            rb.velocity += (newPosition - rb.position) * 10f;

            grounded = 2;
        }

        launcher.Update(packet.shoot, critter.transform.position, packet.headOrientation * Vector3.forward);

        return new CritterStatePacket {
            position = newPosition,
            velocity = rb.velocity,
        };
    }

    public void TakeStateFromServer(CritterStatePacket state)
    {
        rb.MovePosition(state.position);
        rb.velocity = state.velocity;
    }

    static Vector2 cameraBob(float t)
    {
        var x = Mathf.Sin(t);
        var y = -x * x;
        return new Vector2(x, y);
    }

    RaycastHit? findImportantHit(RaycastHit[] hits)
    {
        foreach (var hit in hits) {
            if (hit.point == Vector3.zero) continue;

            if (hit.point.y < critter.transform.position.y - radius) {
                return hit;
            }
        }
        return null;
    }

    static public void DrawGizmos(Vector3 critterPosition, float critterRadius, CritterMoverConfig config)
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(critterPosition + Vector3.down * config.extraHeight, critterRadius * config.suspensionRadiusRatio);
    }
}
