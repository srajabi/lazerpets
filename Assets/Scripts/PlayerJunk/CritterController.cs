using UnityEngine;

public class CritterController : MonoBehaviour
{
    [SerializeField] float mouseSensitivity = 4f;
    [SerializeField] CritterMoverConfig critterConfig;

    CritterInputGrabber inputGrabber;
    CritterMover critterMover;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        inputGrabber = new CritterInputGrabber(mouseSensitivity);
        critterMover = new CritterMover(gameObject, critterConfig);
    }

    private void Update()
    {
        // Always use the local input grabber to drive the mover with UpdateImmediate
        critterMover.UpdateImmediate(inputGrabber.UpdateImmediate());
    }

    private void FixedUpdate()
    {
        // If we're the server do this but don't use input grabber, use the remote packet input packet
        critterMover.UpdateTick(inputGrabber.UpdateTick());

        // if we're a client and taking state from the server then run {
        //     server.SendCritterInputPacket( inputGrabber.UpdateTick() );
        //     critterMover.TakeStateFromServer( ... );
        // }
    }

    private void OnDrawGizmosSelected()
    {
        CritterMover.DrawGizmos(transform.position, GetComponent<SphereCollider>().radius, critterConfig);
    }
}
