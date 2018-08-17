using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Melee of the player */
public class Melee : Photon.MonoBehaviour
{
    public string meleeBox = "MeleeBox";
    public GameObject hitbox;

    private float time;
    private bool hitboxSpawn;

    // Use this for initialization
    void Start ()
    {
        time = 0.0f;
        hitboxSpawn = false;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Health.isDied) return;

		/* Checking if mouse left click is down */
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Left mouse is pressed!");
            if(hitbox)
                PhotonNetwork.Destroy(hitbox);

            hitbox = PhotonNetwork.Instantiate(meleeBox, transform.position, Quaternion.identity, 0);
            hitbox.transform.parent = transform;

            hitbox.transform.forward = transform.forward;
            hitboxSpawn = true;
        }

        if (hitboxSpawn)
            time += Time.deltaTime;

        if (time > 0.5f &&  hitbox)
        {
            PhotonNetwork.Destroy(hitbox);
            hitboxSpawn = false;
            time = 0.0f;
        }

        else if (time > 0.5f && !hitbox)
        {
            hitboxSpawn = false;
            time = 0.0f;
        }
    }
    
}
