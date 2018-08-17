using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This simple chat example showcases the use of RPC targets and targetting certain players via RPCs.
/// </summary>
public class ChatVik : Photon.MonoBehaviour
{

    public static ChatVik SP;
    public List<string> messages = new List<string>();

    private int chatHeight = (int)140;
    private Vector2 scrollPos = Vector2.zero;
    private string chatInput = "";
    private float lastUnfocusTime = 0;

    void Awake()
    {
        SP = this;
    }

    void OnGUI()
    {
        GUI.SetNextControlName("");

        GUILayout.BeginArea(new Rect(0, Screen.height - chatHeight, Screen.width, chatHeight));

        //Show scroll list of chat messages
        scrollPos = GUILayout.BeginScrollView(scrollPos);
        GUI.color = Color.red;
        for (int i = messages.Count - 1; i >= 0; i--)
        {
            GUILayout.Label(messages[i]);
        }
        GUILayout.EndScrollView();
        GUI.color = Color.white;

        //Chat input
        GUILayout.BeginHorizontal();
        GUI.SetNextControlName("ChatField");
        chatInput = GUILayout.TextField(chatInput, GUILayout.MinWidth(200));

        if (Event.current.type == EventType.KeyDown && Event.current.character == '\n')
        {
            if (GUI.GetNameOfFocusedControl() == "ChatField")
            {
                SendChat(PhotonTargets.All);
                lastUnfocusTime = Time.time;
                GUI.FocusControl("");
                GUI.UnfocusWindow();
            }
            else
            {
                if (lastUnfocusTime < Time.time - 0.1f)
                {
                    GUI.FocusControl("ChatField");
                }
            }
        }

        //if (GUILayout.Button("SEND", GUILayout.Height(17)))
        //   SendChat(PhotonTargets.All);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();



        GUILayout.EndArea();
    }

    public static void AddMessage(string text)
    {
        SP.messages.Add(text);
        if (SP.messages.Count > 15)
            SP.messages.RemoveAt(0);
    }


    [PunRPC]
    void SendChatMessage(string text, PhotonMessageInfo info)
    {
        AddMessage("[" + info.sender + "] " + text);
    }

    void SendChat(PhotonTargets target)
    {
        if (chatInput != "")
        {
            photonView.RPC("SendChatMessage", target, chatInput);
            chatInput = "";
        }
    }

    void SendChat(PhotonPlayer target)
    {
        if (chatInput != "")
        {
            chatInput = "[PM] " + chatInput;
            photonView.RPC("SendChatMessage", target, chatInput);
            chatInput = "";
        }
    }

    void OnLeftRoom()
    {
        this.enabled = false;
    }

    void OnJoinedRoom()
    {
        this.enabled = true;
    }
    void OnCreatedRoom()
    {
        this.enabled = true;
    }
}


//using UnityEngine;
//using System.Collections;
//using System.Text;
//using System.Collections.Generic;

///// <summary>
///// This simple chat example showcases the use of RPC targets and targetting certain players via RPCs.
///// </summary>
//public class ChatVik : Photon.MonoBehaviour
//{

//    public static ChatVik SP;
//    public List<string> messages = new List<string>();

//    private int chatHeight = (int)140;
//    private Vector2 scrollPos = Vector2.zero;
//    private string chatInput = "";
//    private float lastUnfocusTime = 0;
//    private static string[] Friends;

//    public static string[] PlayerParty;
//    public static string[] Party;

//    public string friendlist;

//    private bool showFriend = false;

//    public string[] returnParty()
//    {
//        return Party;
//    }

//    public string[] returnPlayerParty()
//    {
//        return PlayerParty;
//    }


//    void Awake()
//    {
//        SP = this;
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

//        for (int i = messages.Count - 1; i >= 0; i--)
//        {
//            GUILayout.Label(messages[i]);
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
//                SendChat(PhotonNetwork.player);
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

//    public static void AddMessage(string text)
//    {
//        SP.messages.Add(text);
//        if (SP.messages.Count > 15)
//            SP.messages.RemoveAt(0);
//    }


//    [PunRPC]
//    void SendChatMessage(string text, PhotonMessageInfo info)
//    {
//        AddMessage("[" + info.sender + "] " + text);
//    }

//    void SendChat(PhotonTargets target)
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

//            else if (checkCommand[0] == "/List")
//            {
//                GetFriend();
//                GetParty();
//            }

//            else if (checkCommand[0] == "/onList")
//            {
//                showFriend = true;
//            }

//            else if (checkCommand[0] == "/offList")
//            {
//                showFriend = false;
//            }
//            chatInput = "";
//        }

//    }

//    void SendChat(PhotonPlayer target)
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

//            else if (checkCommand[0] == "/List")
//            {
//                GetFriend();
//                GetParty();

//                string friend = "Friends: " + friendlist;
//                photonView.RPC("SendChatMessage", target, friend);
//                //photonView.RPC("SendChatMessage", target, chatInput);

//            }

//            else if (checkCommand[0] == "/onList")
//            {
//                showFriend = true;
//            }

//            else if (checkCommand[0] == "/offList")
//            {
//                showFriend = false;
//            }

//            chatInput = "";
//        }
//    }

//    void OnLeftRoom()
//    {
//        //this.enabled = false;
//    }
//    void OnJoinedRoom()
//    {
//        // this.enabled = true;
//        PhotonNetwork.OnEventCall += this.OnEventHandler;
//    }
//    void OnCreatedRoom()
//    {
//        //this.enabled = true;
//    }

//    public void DoAddFriend(string FriendName)
//    {
//        byte evCode = 6; // evCode for saving position
//        string contentMessage = PhotonNetwork.playerName + "," + FriendName;
//        byte[] content = Encoding.UTF8.GetBytes(contentMessage);
//        bool reliable = true;
//        PhotonNetwork.RaiseEvent(evCode, content, reliable, null);
//    }

//    public void GetFriend()
//    {
//        byte evCode = 8; // evCode for saving position
//        string contentMessage = PhotonNetwork.playerName;
//        byte[] content = Encoding.UTF8.GetBytes(contentMessage);
//        bool reliable = true;
//        PhotonNetwork.RaiseEvent(evCode, content, reliable, null);
//    }

//    public void DoAddParty(string FriendName)
//    {
//        byte evCode = 9; // evCode for saving position
//        string contentMessage = PhotonNetwork.playerName + "," + FriendName;
//        byte[] content = Encoding.UTF8.GetBytes(contentMessage);
//        bool reliable = true;
//        PhotonNetwork.RaiseEvent(evCode, content, reliable, null);
//    }

//    public void DoAddParty2(string FriendName)
//    {
//        byte evCode = 9; // evCode for saving position
//        string contentMessage = FriendName + "," + PhotonNetwork.playerName;
//        byte[] content = Encoding.UTF8.GetBytes(contentMessage);
//        bool reliable = true;
//        PhotonNetwork.RaiseEvent(evCode, content, reliable, null);
//    }

//    public void GetParty()
//    {
//        byte evCode = 10; // evCode for saving position
//        string contentMessage = PhotonNetwork.playerName;
//        byte[] content = Encoding.UTF8.GetBytes(contentMessage);
//        bool reliable = true;
//        PhotonNetwork.RaiseEvent(evCode, content, reliable, null);
//    }

//    private void OnEventHandler(byte eventCode, object content, int senderId)
//    {
//        switch (eventCode)
//        {
//            case 8:
//                if (content != null)
//                    friendlist = content.ToString();
//                //Friends = content.ToString().Split(',');
//                break;

//            case 10:
//                if (content != null)
//                    PlayerParty = content.ToString().Split(' ');

//                if (PlayerParty.Length > 1)
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