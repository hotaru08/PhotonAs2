using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/* This is for Player Name */
public class PlayerName : MonoBehaviour
{
    [SerializeField]
    public TextMesh m_text;
    
    //private static GameObject m_player;
    //public void SetPlayer(GameObject _player)
    //{
    //    m_player = _player;
    //}

    //private string m_playerName;

	// Use this for initialization
	void Start ()
    {
        /* Add to method to carry out event */
        //PhotonNetwork.OnEventCall += this.OnEventHandler;
        //m_playerName = "";
        m_text.text = GetComponentInParent<PhotonView>().owner.NickName;
        //PrintName();
    }

    /* Print name of players */
    //private void PrintName()
    //{
    //    //if(PhotonNetwork.playerName == PlayerPrefs.GetString("playerName"))
    //    //m_text.text = PlayerPrefs.GetString("playerName"); // return player's name

    //    //Debug.Log(PhotonNetwork.player.ID + ", " + GetComponentInParent<PhotonView>().ownerId);

    //    if (GetComponentInParent<PhotonView>().isMine)
    //    {
    //        m_text.text = PhotonNetwork.playerName; // return player's name
    //        Debug.Log("This is player Name : " + m_text.text);
    //    }
    //    else if (!GetComponentInParent<PhotonView>().isMine)
    //    {
    //        // for each player in the room 
    //        foreach (var _player in PhotonNetwork.otherPlayers)
    //        {
    //            m_text.text = _player.NickName; //return others names
    //            Debug.Log("This is other Name : " + m_text.text);
    //        }
    //    }
    //}

    ///* Receive data of events from server */
    //private void OnEventHandler(byte eventCode, object content, int senderId)
    //{
    //    switch (eventCode)
    //    {
    //        case 2:
    //            m_playerName = (string)content;
    //            Debug.Log("Name : " + m_playerName);
    //            //Debug.Log(string.Format("Message from Server : {0}", (string)content));
    //            break;
    //    }
    //}

}
