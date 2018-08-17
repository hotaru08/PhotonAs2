using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetMovement : MonoBehaviour {

    public Rigidbody target;
    public float speed = 1.0f, walkSpeedDownscale = 2.0f, turnSpeed = 2.0f, mouseTurnSpeed = 0.3f, jumpSpeed = 1.0f;
    // Tweak to ajust character responsiveness
    public LayerMask groundLayers = -1;
    // Which layers should be walkable?
    // NOTICE: Make sure that the target collider is not in any of these layers!
    public float groundedCheckOffset = 0.7f;
    // Tweak so check starts from just within target footing
    public bool
        showGizmos = true,
        // Turn this off to reduce gizmo clutter if needed
        requireLock = true,
        // Turn this off if the camera should be controllable even without cursor lock
        controlLock = false;
    // Turn this on if you want mouse lock controlled by this script

    private const float inputThreshold = 0.01f,
    groundDrag = 5.0f,
    directionalJumpFactor = 0.7f;
    // Tweak these to adjust behaviour relative to speed
    private const float groundedDistance = 0.5f;
    // Tweak if character lands too soon or gets stuck "in air" often
    
    private bool grounded, walking;

    private float DistanceToPlayer;
    private float minDist = 3;
    public GameObject playerToFollow = null;

    public bool Grounded
    // Make our grounded status available for other components
    {
        get
        {
            return grounded;

        }
    }

    void Reset()
    // Run setup on component attach, so it is visually more clear which references are used
    {
        Setup();
    }


    void Setup()
    // If target is not set, try using fallbacks
    {
        if (target == null)
        {
            target = GetComponent<Rigidbody>();
        }
    }

    // Use this for initialization
    void Start () {
        Setup();
        //Debug.Log("PET: " + transform.position);
        if (target == null)
        {
            Debug.LogError("No target assigned. Please correct and restart.");
            enabled = false;
            return;
        }

        target.freezeRotation = true;
        // We will be controlling the rotation of the target, so we tell the physics system to leave it be
        walking = false;
    }
	
	// Update is called once per frame
	void Update () {
        transform.LookAt(new Vector3(playerToFollow.transform.position.x, transform.position.y, playerToFollow.transform.position.z));
        DistanceToPlayer = (transform.position - playerToFollow.transform.position).magnitude;

        if(DistanceToPlayer > minDist)
        {
            Vector3 movement = target.transform.forward;
            target.AddForce(movement.normalized * speed, ForceMode.VelocityChange);
        }
	}

    public void SetPlayer(GameObject player)
    {
        if(playerToFollow == null)
            playerToFollow = player;
    }
}
