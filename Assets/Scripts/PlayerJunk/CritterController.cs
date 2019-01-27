using System;
using UnityEngine;

public class CritterController : MonoBehaviour
{
    [SerializeField] CritterMoverConfig critterConfig;
    [SerializeField] CatAudioManager audioManager;

    public IInputGrabber localInputGrabber;
    public CritterMover Mover { get; private set; }

    public bool IsServer;
    internal CritterInputPacket? InputPacketOveride;

    public event Action<CritterStatePacket> OnCritterStatePacket;
    public event Action<CritterInputPacket> OnCritterInputPacket;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        //inputGrabber = new CritterInputGrabber(mouseSensitivity);
        Mover = new CritterMover(gameObject, critterConfig, audioManager);
    }

    private void Update()
    {
        // Always use the local input grabber to drive the mover with UpdateImmediate
        Mover.UpdateImmediate(localInputGrabber.UpdateImmediate());
    }

    public void UpdateViaCritterStatePacket(CritterStatePacket critterStatePacket)
    {
        Mover.TakeStateFromServer(critterStatePacket);
    }

    private void FixedUpdate()
    {
        var inputPacket = localInputGrabber.UpdateTick();

        OnCritterInputPacket?.Invoke(inputPacket);

        if (IsServer)
        {
            var statePacket = Mover.UpdateTick(inputPacket);
            OnCritterStatePacket?.Invoke(statePacket);
        }
        else
        {
            if (InputPacketOveride != null)
            {
                Mover.UpdateTick(InputPacketOveride.Value);
            }
        }
        // If we're the server do this but don't use input grabber, use the remote packet input packet
        //var critterNewState = critterMover.UpdateTick(authorativeInputGrabber.UpdateTick());
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
