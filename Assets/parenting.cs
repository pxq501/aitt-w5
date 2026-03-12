using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class Parenting : MonoBehaviour
{
    private GameObject collidingObject;
    private GameObject objectInHand;

    private Hand hand;
    private SpringJoint joint;

    void Start()
    {
        hand = GetComponent<Hand>();
        joint = GetComponent<SpringJoint>(); // get the spring joint on the controller
    }

    void Update()
    {
        if (SteamVR_Input.GetStateDown("GrabPinch", SteamVR_Input_Sources.RightHand))
        {
            if (collidingObject)
            {
                Grab();
            }
        }

        if (SteamVR_Input.GetStateUp("GrabPinch", SteamVR_Input_Sources.RightHand))
        {
            if (objectInHand)
            {
                Release();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Rigidbody>())
        {
            collidingObject = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (collidingObject == other.gameObject)
        {
            collidingObject = null;
        }
    }


    void Grab()
    {
        objectInHand = collidingObject;
        collidingObject = null;

        Rigidbody rb = objectInHand.GetComponent<Rigidbody>();

        // Attach spring to object
        joint.connectedBody = rb;
    }

    void Release()
    {
        Rigidbody rb = objectInHand.GetComponent<Rigidbody>();

        // Detach spring
        joint.connectedBody = null;

        if (hand != null)
        {
            rb.velocity = hand.trackedObject.GetVelocity();
            rb.angularVelocity = hand.trackedObject.GetAngularVelocity();
        }

        objectInHand = null;
    }
}