//using System.Collections;
//using System.Collections.Generic;
//using System.Text;
//using UnityEngine;

//public class SetFriendParty : Photon.MonoBehaviour
//{

//    private int chatHeight = (int)140;
//    private Vector2 scrollPos = Vector2.zero;
//    private string chatInput = "";
//    private float lastUnfocusTime = 0;

//    private string[] PlayerFriends;
//    private string[] Friends;
//    public string[] PlayerParty;
//    public string[] Party;
//    private bool showFriend = true;

//    public string[] returnParty()
//    {
//        return Party;
//    }

//    public string[] returnPlayerParty()
//    {
//        return PlayerParty;
//    }

//    // Use this for initialization
//    void Start()
//    {
//        GetFriend();
//        GetParty();
//        PhotonNetwork.OnEventCall += this.OnHandle;
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        if (transform.gameObject)
//        {
//            GetFriend();
//            GetParty();
//        }
//    }

//    void Awake()
//    {

//    }

//    void OnGUI()
//    {
//        GUI.SetNextControlName("");

//        GUILayout.BeginArea(new Rect(0, Screen.height - chatHeight, Screen.width, chatHeight));

//        //Show scroll list of chat messages
//        scrollPos = GUILayout.BeginScrollView(scrollPos);
//        GUI.color = Color.white;
//        GUILayout.Label("PARTY");
//        if (Party != null && showFriend)
//        {
//            for (int i = 0; i < Party.Length; i++)
//            {
//                GUILayout.Label(Party[i]);
//            }
//        }

//        GUI.color = Color.red;
//        GUILayout.Label("FRIENDS");
//        if (Friends != null && showFriend)
//        {
//            for (int i = 0; i < Friends.Length; i++)
//            {
//                GUILayout.Label(Friends[i]);
//            }
//        }

//        GUILayout.EndScrollView();
//        GUI.color = Color.white;

//        //Chat input
//        GUILayout.BeginHorizontal();
//        GUI.SetNextControlName("ChatField");
//        chatInput = GUILayout.TextField(chatInput, GUILayout.MinWidth(200));

//        if (Event.current.type == EventType.KeyDown && Event.current.character == '\n')
//        {
//            if (GUI.GetNameOfFocusedControl() == "ChatField")
//            {
//                SendChat();
//                lastUnfocusTime = Time.time;
//                GUI.FocusControl("");
//                GUI.UnfocusWindow();
//            }
//            else
//            {
//                if (lastUnfocusTime < Time.time - 0.1f)
//                {
//                    GUI.FocusControl("ChatField");
//                }
//            }
//        }

//        //if (GUILayout.Button("SEND", GUILayout.Height(17)))
//        //   SendChat(PhotonTargets.All);
//        GUILayout.FlexibleSpace();
//        GUILayout.EndHorizontal();

//        GUILayout.EndArea();
//    }

//    void SendChat()
//    {
//        if (chatInput != "")
//        {
//            //photonView.RPC("SendChatMessage", target, chatInput);
//            //chatInput = "";
//            string[] checkCommand = chatInput.Split(' ');

//            if (checkCommand[0] == "/Friend")
//            {
//                foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
//                {
//                    if (player.GetComponentInChildren<PlayerName>().m_text.text == checkCommand[1])
//                    {
//                        DoAddFriend(checkCommand[1]);
//                    }
//                }
//            }

//            else if (checkCommand[0] == "/Party")
//            {
//                foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
//                {
//                    if (player.GetComponentInChildren<PlayerName>().m_text.text == checkCommand[1])
//                    {
//                        DoAddParty(checkCommand[1]);
//                        DoAddParty2(checkCommand[1]);
//                    }
//                }
//            }
            
//            chatInput = "";
//        }
//    }

//    public void DoAddFriend(string FriendName)
//    {
//        byte evCode = 6; // evCode for saving position
//        string contentMessage = GetComponent<PhotonView>().owner.NickName + "," + FriendName;
//        byte[] content = Encoding.UTF8.GetBytes(contentMessage);
//        bool reliable = true;
//        PhotonNetwork.RaiseEvent(evCode, content, reliable, null);
//    }

//    public void GetFriend()
//    {
//        byte evCode = 8; // evCode for saving position
//        string contentMessage = GetComponent<PhotonView>().owner.NickName;
//        byte[] content = Encoding.UTF8.GetBytes(contentMessage);
//        bool reliable = true;
//        PhotonNetwork.RaiseEvent(evCode, content, reliable, null);
//    }

//    public void DoAddParty(string FriendName)
//    {
//        byte evCode = 9; // evCode for saving position
//        string contentMessage = GetComponent<PhotonView>().owner.NickName + "," + FriendName;
//        byte[] content = Encoding.UTF8.GetBytes(contentMessage);
//        bool reliable = true;
//        PhotonNetwork.RaiseEvent(evCode, content, reliable, null);
//    }

//    public void DoAddParty2(string FriendName)
//    {
//        byte evCode = 9; // evCode for saving position
//        string contentMessage = FriendName + "," + GetComponent<PhotonView>().owner.NickName;
//        byte[] content = Encoding.UTF8.GetBytes(contentMessage);
//        bool reliable = true;
//        PhotonNetwork.RaiseEvent(evCode, content, reliable, null);
//    }

//    public void GetParty()
//    {
//        byte evCode = 10; // evCode for saving position
//        string contentMessage = GetComponent<PhotonView>().owner.NickName;
//        byte[] content = Encoding.UTF8.GetBytes(contentMessage);
//        bool reliable = true;
//        PhotonNetwork.RaiseEvent(evCode, content, reliable, null);
//    }

//    private void OnHandle(byte eventCode, object content, int senderId)
//    {
//        switch (eventCode)
//        {
//            case 8:
//                //if (content != null)
//                //    Friends = content.ToString().Split(',');
//                //else
//                //    Friends = null;

//                if (content != null)
//                    PlayerFriends = content.ToString().Split(' ');
//                else break;

//                if (PlayerFriends.Length > 1 && PlayerFriends[0] == GetComponent<PhotonView>().owner.NickName)
//                {
//                    if (PlayerFriends[1].Length > 1) // if > 1 friend
//                        Friends = PlayerFriends[1].Split(',');
//                    else if (PlayerFriends[1].Length == 1) // if only 1 friend
//                        Friends[0] = PlayerFriends[1];
//                }
//                break;

//            case 10:
//                if (content != null)
//                    PlayerParty = content.ToString().Split(' ');
//                else break;

//                if (PlayerParty.Length > 1 && PlayerParty[0] == GetComponent<PhotonView>().owner.NickName)
//                {
//                    if (PlayerParty[1].Length > 1)
//                        Party = PlayerParty[1].Split(',');
//                    else if (PlayerParty[1].Length == 1)
//                        Party[0] = PlayerParty[1];
//                }
//                break;
//        }
//    }
//}
