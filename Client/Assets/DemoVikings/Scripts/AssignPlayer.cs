using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssignPlayer : Photon.MonoBehaviour {
    public string nameofPlayer;

    // Use this for initialization
    void Start () {
        nameofPlayer = photonView.owner.NickName;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
