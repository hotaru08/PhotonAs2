using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxNetwork : Photon.MonoBehaviour {
    AssignPlayer aPlayer;
    private bool appliedInitialUpdate;

    // Use this for initialization
    void Awake () {
        aPlayer = GetComponent<AssignPlayer>();
    }

    void Start()
    {
        //TODO: Bugfix to allow .isMine and .owner from AWAKE!
        if (photonView.isMine)
        {
            //MINE: local player, simply enable the local scripts
            aPlayer.enabled = true;
        }
        else
        {
            aPlayer.enabled = false;

        }
        gameObject.name = gameObject.name + photonView.viewID;
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            //We own this player: send the others our data
            // stream.SendNext((int)controllerScript._characterState);
            //stream.SendNext(aPlayer.transform.position);
            stream.SendNext(aPlayer.nameofPlayer);
        }
        else
        {
            //Network player, receive data
            //controllerScript._characterState = (CharacterState)(int)stream.ReceiveNext();
            //correctPos = (Vector3)stream.ReceiveNext();
            correctName = (string)stream.ReceiveNext();

            if (!appliedInitialUpdate)
            {
                appliedInitialUpdate = true;
                //aPlayer.transform.position = correctPos;
                aPlayer.nameofPlayer = correctName;
            }
        }
    }

    private Vector3 correctPos = Vector3.zero; //We lerp towards this
    private string correctName = null;

    void Update()
    {
        if (!photonView.isMine)
        {
            //Update remote player (smooth this, this looks good, at the cost of some accuracy)
            //aPlayer.transform.position = Vector3.Lerp(aPlayer.transform.position, correctPos, Time.deltaTime * 5);
            aPlayer.nameofPlayer = correctName;
        }
    }

    void OnPhotonInstantiate(PhotonMessageInfo info)
    {
       
    }
}
