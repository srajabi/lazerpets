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
        critterMover.UpdateImmediate(inputGrabber.UpdateImmediate());
    }

    private void FixedUpdate()
    {
        critterMover.UpdateTick(inputGrabber.UpdateTick());
    }

    private void OnDrawGizmosSelected()
    {
        CritterMover.DrawGizmos(transform.position, GetComponent<SphereCollider>().radius, critterConfig);
    }
}
