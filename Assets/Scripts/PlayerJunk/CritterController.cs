using UnityEngine;

public class CritterController : MonoBehaviour
{
    [SerializeField] float mouseSensitivity = 4f;
    [SerializeField] CritterMoverConfig critterConfig;
    [SerializeField] CatAudioManager audioManager;

    CritterInputGrabber inputGrabber;
    public CritterMover Mover { get; private set; }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        inputGrabber = new CritterInputGrabber(mouseSensitivity);
        Mover = new CritterMover(gameObject, critterConfig, audioManager);
    }

    private void Update()
    {
        // Always use the local input grabber to drive the mover with UpdateImmediate
        Mover.UpdateImmediate(inputGrabber.UpdateImmediate());
    }

    private void FixedUpdate()
    {
        // If we're the server do this but don't use input grabber, use the remote packet input packet
        var critterNewState = Mover.UpdateTick(inputGrabber.UpdateTick());
        // broadcastToClients( critterNewState );

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
