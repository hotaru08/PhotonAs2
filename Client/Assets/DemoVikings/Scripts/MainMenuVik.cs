using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Text;

public class MainMenuVik : MonoBehaviour
{

    void Awake()
    {
        //PhotonNetwork.logLevel = NetworkLogLevel.Full;

        //Connect to the main photon server. This is the only IP and port we ever need to set(!)
        if (!PhotonNetwork.connected)
            PhotonNetwork.ConnectUsingSettings("v1.0"); // version of the game/demo. used to separate older clients from newer ones (e.g. if incompatible)

        //Load name from PlayerPrefs
        PhotonNetwork.playerName = PlayerPrefs.GetString("playerName", "Guest" + Random.Range(1, 9999));

        //Set camera clipping for nicer "main menu" background
        Camera.main.farClipPlane = Camera.main.nearClipPlane + 0.1f;
    }

    private string roomName = "myRoom";
    private Vector2 scrollPos = Vector2.zero;
    private string password = "";

    // Login
    private bool isLogin = false;
    private bool isRegistering = false;
    private bool isLeaderboard = false;
    private bool isRendered = false;

    // LeaderBoard
    private int startingNumber = 0;
    private List<string> names = new List<string>();
    private List<string> scores = new List<string>();
    private List<int> ranks = new List<int>();

    // Achievements
    private bool isAchievement = false;
    private string playerName = "";
    private List<string> m_achievementList = new List<string>();

    void OnGUI()
    {
        if (!PhotonNetwork.connected)
        {
            ShowConnectingGUI();
            return;   //Wait for a connection
        }
        if (PhotonNetwork.room != null)
            return; //Only when we're not in a Room

        GUILayout.BeginArea(new Rect((Screen.width - 400) / 2, (Screen.height - 300) / 2, 400, 300));

        if (!isRegistering && !isLeaderboard && !isAchievement)
        {
            GUILayout.Label("Main Menu");

            //Player name
            GUILayout.BeginHorizontal();
            GUILayout.Label("Player name:", GUILayout.Width(150));
            PhotonNetwork.playerName = GUILayout.TextField(PhotonNetwork.playerName);
            if (GUI.changed)//Save name
                PlayerPrefs.SetString("playerName", PhotonNetwork.playerName);
            GUILayout.EndHorizontal();

            //Player password
            GUILayout.BeginHorizontal();
            GUILayout.Label("Password:", GUILayout.Width(150)); // render text
            password = GUILayout.TextField(password); // render the previous pw and input field
            if (GUI.changed) // password is changed, save it
                PlayerPrefs.SetString("playerPassword", password);
            GUILayout.EndHorizontal();

            // Player Authentication ( login )
            GUILayout.BeginHorizontal();
            GUILayout.Space(100);
            if (GUILayout.Button("Login"))
            {
                Debug.Log("Authenticating Player " + PhotonNetwork.playerName + " ...");

                /// Lambda function to send and authenticate users that had registered
                new GameSparks.Api.Requests.AuthenticationRequest()
                    .SetUserName(PhotonNetwork.playerName)
                    .SetPassword(password)
                    .Send((response) =>
                    {
                        if (!response.HasErrors)
                        {
                            Debug.Log("Authentication is successful! User : " + response.DisplayName);
                            isLogin = true;
                        }
                        else
                        {
                            Debug.Log("Authentication failed! Error : " + response.Errors.JSON.ToString());
                        }
                    }
                    );
            }
            GUILayout.EndHorizontal();

            // Player Registration
            GUILayout.BeginHorizontal();
            GUILayout.Label("Don't have an account? Register here!");
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Go to Register"))
            {
                isRegistering = true;
            }

            // Button to press to show Leaderboard
            if (GUILayout.Button("Leaderboard"))
            {
                isLeaderboard = true;
            }
        }

        /* GUI Updating */
        if (isRegistering)
        {
            GUILayout.Label("Registration");
            RegistrationLayout();
        }

        if (isLeaderboard)
        {
            GUILayout.Label("Leaderboard");
            RenderLeaderboard();

            for (int i = 0; i < startingNumber; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(ranks[i].ToString());
                GUILayout.Label(names[i]);
                GUILayout.Label(scores[i]);
                GUILayout.EndHorizontal();
            }

            // Back button to go back to main menu
            if (GUILayout.Button("Main Menu"))
            {
                // reset display
                ranks.Clear();
                names.Clear();
                scores.Clear();
                startingNumber = 0;

                isLeaderboard = false;
                isRendered = false;
            }
        }

        // ---------------- After Login
        if (isLogin && !isAchievement && !isLeaderboard && !isRegistering)
        {
            // Show the list of achievements of the player
            if (GUILayout.Button("Get Achievements"))
            {
                isAchievement = true;
            }

            //Join room by title
            GUILayout.BeginHorizontal();
            GUILayout.Label("JOIN ROOM:", GUILayout.Width(150));
            roomName = GUILayout.TextField(roomName);
            if (GUILayout.Button("GO"))
            {
                PhotonNetwork.JoinRoom(roomName);
            }
            GUILayout.EndHorizontal();

            //Create a room (fails if exist!)
            GUILayout.BeginHorizontal();
            GUILayout.Label("CREATE ROOM:", GUILayout.Width(150));
            roomName = GUILayout.TextField(roomName);
            if (GUILayout.Button("GO"))
            {
                // using null as TypedLobby parameter will also use the default lobby
                PhotonNetwork.CreateRoom(roomName, new RoomOptions() { MaxPlayers = 10 }, TypedLobby.Default);
            }
            GUILayout.EndHorizontal();

            //Join random room
            GUILayout.BeginHorizontal();
            GUILayout.Label("JOIN RANDOM ROOM:", GUILayout.Width(150));
            if (PhotonNetwork.GetRoomList().Length == 0)
            {
                GUILayout.Label("..no games available...");
            }
            else
            {
                if (GUILayout.Button("GO"))
                {
                    PhotonNetwork.JoinRandomRoom();
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Label("ROOM LISTING:");
            if (PhotonNetwork.GetRoomList().Length == 0)
            {
                GUILayout.Label("..no games available..");
            }
            else
            {
                //Room listing: simply call GetRoomList: no need to fetch/poll whatever!
                scrollPos = GUILayout.BeginScrollView(scrollPos);
                foreach (RoomInfo game in PhotonNetwork.GetRoomList())
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(game.name + " " + game.playerCount + "/" + game.maxPlayers);
                    if (GUILayout.Button("JOIN"))
                    {
                        PhotonNetwork.JoinRoom(game.name);
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndScrollView();
            }
        }

        // For Achievements
        if (isAchievement)
        {
            GUILayout.Label("Achievements");
            RenderAchievements();
        }

        GUILayout.EndArea();
    }

    /* Loading GUI before main menu */
    void ShowConnectingGUI()
    {
        GUILayout.BeginArea(new Rect((Screen.width - 400) / 2, (Screen.height - 300) / 2, 400, 300));

        GUILayout.Label("Connecting to Photon server.");
        GUILayout.Label("Hint: This demo uses a settings file and logs the server address to the console.");

        GUILayout.EndArea();
    }

    /* Call this to change to main menu */
    public void OnConnectedToMaster()
    {
        // this method gets called by PUN, if "Auto Join Lobby" is off.
        // this demo needs to join the lobby, to show available rooms!

        PhotonNetwork.JoinLobby();  // this joins the "default" lobby
    }

    /* Render Registration Menu */
    private void RegistrationLayout()
    {
        //Player name
        GUILayout.BeginHorizontal();
        GUILayout.Label("New Player Name:", GUILayout.Width(150));
        PhotonNetwork.playerName = GUILayout.TextField(PhotonNetwork.playerName);
        if (GUI.changed)//Save name
            PlayerPrefs.SetString("playerName", PhotonNetwork.playerName);
        GUILayout.EndHorizontal();

        //Player password
        GUILayout.BeginHorizontal();
        GUILayout.Label("New Password:", GUILayout.Width(150)); // render text
        password = GUILayout.TextField(password); // render the previous pw and input field
        if (GUI.changed) // password is changed, save it
            PlayerPrefs.SetString("playerPassword", password);
        GUILayout.EndHorizontal();

        // Player Register
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Register"))
        {
            Debug.Log("Registering ...");

            /// Lambda function to register username and password and send to GameSparks
            new GameSparks.Api.Requests.RegistrationRequest()
                .SetDisplayName(PhotonNetwork.playerName)
                .SetUserName(PhotonNetwork.playerName)
                .SetPassword(password)
                .Send((response) =>
                {
                    if (!response.HasErrors)
                    {
                        Debug.Log("Registered Player! Username is : " + response.DisplayName);
                        //DoRaiseEvent(); /// Send event to SQL
                        isRegistering = false; /// set false and go back main menu
                    }
                    else
                    {
                        Debug.Log("Unable to register! Error : " + response.Errors.JSON.ToString());
                        GUILayout.Label("Error : " + response.Errors.JSON.ToString()); // render text
                    }
                }
                );

        }

        // Back button to go back to main menu
        if (GUILayout.Button("Main Menu"))
        {
            isRegistering = false;
        }

        GUILayout.EndHorizontal();
    }

    /* Render Leaderboard GUI Layout */
    public void RenderLeaderboard()
    {
        // Layout of the Leaderboard
        GUILayout.BeginHorizontal();
        // Player Rank
        GUILayout.Label("RANK", GUILayout.Width(150));
        GUILayout.FlexibleSpace();

        // Player Name
        GUILayout.Label("NAME", GUILayout.Width(150));
        GUILayout.FlexibleSpace();

        // Player Score
        GUILayout.Label("SCORE", GUILayout.Width(150));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        // Print the information
        if (!isRendered)
        {
            // get Data from GameSparks to print Leaderboard
            new GameSparks.Api.Requests.LeaderboardDataRequest()
                .SetLeaderboardShortCode("HIGH_SCORE_LB")
                .SetEntryCount(5)
                .Send((response) =>
                {
                    if (!response.HasErrors)
                    {
                        Debug.Log("Found Leaderboard Data...");

                        // for each data that is in leaderboard in Gamespark, do smth 
                        foreach (GameSparks.Api.Responses.LeaderboardDataResponse._LeaderboardData entry in response.Data)
                        {
                            ranks.Add((int)entry.Rank);
                            names.Add(entry.UserName);
                            scores.Add(entry.JSONData["SCORE"].ToString());
                            startingNumber++;
                            Debug.Log("Rank:" + (int)entry.Rank + " Name:" + entry.UserName + " \n Score:" + entry.JSONData["SCORE"].ToString());
                        }
                    }
                    else
                    {
                        Debug.Log("Error Retrieving Leaderboard Data..." + response.Errors.JSON.ToString());
                    }
                });
            isRendered = true;
        }
    }

    /* Render Achievements GUI */
    public void RenderAchievements()
    {
        /// Get data from GameSparks
        new GameSparks.Api.Requests.AccountDetailsRequest()
            .Send(response =>
            {
                if (!response.HasErrors)
                {
                    playerName = response.DisplayName; // we can get the display name
                    if (response.Achievements.Count != 0)
                        m_achievementList = response.Achievements; // we can get a list of achievements earned
                }
                else
                {
                    Debug.Log("Error Retrieving Account Details... " + response.Errors.JSON.ToString());
                }
            });

        // Print out Achievements
        GUI.color = Color.green;
        GUILayout.Label(playerName);
        GUI.color = Color.yellow;
        foreach (var ach in m_achievementList)
        {
            if (m_achievementList.Count == 0) break;
            GUILayout.Label(ach);
        }
        GUI.color = Color.white;

        // Back button to go back to main menu
        if (GUILayout.Button("Main Menu"))
        {
            isAchievement = false;
        }
    }
    
}

     