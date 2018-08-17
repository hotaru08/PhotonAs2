using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Damage for melee */
public class Damage : Photon.MonoBehaviour
{
    private bool isDamaged;
    private float GodMode;

	// Use this for initialization
	void Start ()
    {
        isDamaged = false;
        GodMode = 0.0f;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (isDamaged)
            GodMode += Time.deltaTime;

        if(GodMode > 0.6f)
        {
            GodMode = 0.0f;
            isDamaged = false;
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Hitbox" &&!isDamaged)
        {
            GameObject player = null;
            foreach(var _player in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (_player.GetComponent<PhotonView>().owner.NickName == other.GetComponent<PhotonView>().owner.NickName)
                {
                    player = _player;
                    break;
                }
            }
            Debug.Log(player.GetComponent<PhotonView>().owner.NickName);

            //if (photonView.isMine)
            //    return;
            //Debug.Log("CHECKING " + other.transform.parent.gameObject);
            //if (GetComponent<SetFriendParty>().returnParty() != null)
            //{
            //    foreach (var partyMemName in GetComponent<SetFriendParty>().returnParty())
            //    {
            //        //Debug.Log(player.GetComponent<SetFriendParty>().returnParty()[i]);
            //        if (player.GetComponent<PhotonView>().owner.NickName == partyMemName)
            //            return;
            //    }
            //}
            GetComponentInChildren<Health>().m_health--;


            //if (player)
            //    player.GetComponent<Highscore>().m_score++;
            //other.transform.parent.parent.GetComponent<Highscore>().m_score++;
            isDamaged = true;
        }

    }
}
