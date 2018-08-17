using UnityEngine;
using System.Collections;
using System.Text;
using GameSparks.Core;
using System.Collections.Generic;

public class GameManagerVik : Photon.MonoBehaviour {

    // this is a object name (must be in any Resources folder) of the prefab to spawn as player avatar.
    // read the documentation for info how to spawn dynamically loaded game objects at runtime (not using Resources folders)
    public string playerPrefabName = "Charprefab";
    public string petPrefabName = "Pet";
    private float m_achievementTimer;
    private bool m_bAtimerStart;
    private static GameObject player;
    private static GameObject pet;

    private static string m_PlayerPosition;
    private static string m_PlayerHealth;
    public static string m_PlayerScore;
    private static bool HasPet = false;

    private int m_typeMsg = 0;
    private string m_announcement = "";
    private bool m_isShown = false;

    // get player pos
    private Vector3 m_playerPos;

    // Enemy Prefab
    [SerializeField]
    private GameObject enemyPrefab;

    void OnJoinedRoom()
    {
        // to initialise variables in the Server 
        //byte evCode = 13;
        //PhotonNetwork.RaiseEvent(evCode, null, true, null);

        // get data from events
        DoRaiseEvent();
        GetStorePosition();
        GetStoreHealth();
        GetStoreScore();

        PhotonNetwork.OnEventCall += this.OnEventHandler; // server event handler
        GameSparks.Api.Messages.NewHighScoreMessage.Listener += HighScoreMessageHandler; // GameSparks Leaderboard event handler
        GameSparks.Api.Messages.AchievementEarnedMessage.Listener += AchievementMessageHandler; // New Achievenment event handler

        StartGame();
        TriggerEnemySpawn();
    }

#region Assignment 1
    /* Store login info */
    public void DoRaiseEvent()
    {
        byte evCode = 2;
        string contentMessage = PhotonNetwork.playerName + "," + PlayerPrefs.GetString("playerPassword"); //content of message to be sent
        byte[] content = Encoding.UTF8.GetBytes(contentMessage); // convert to array
        bool reliable = true;
        PhotonNetwork.RaiseEvent(evCode, content, reliable, null); // send to plugin
    }

    /* Store position and health of player */
    public void DoStoreVariables()
    {
        byte evCode = 3; // evCode for saving position
        string contentMessage = PhotonNetwork.playerName + "," + PlayerPrefs.GetString("playerPos") 
                                + "," + PlayerPrefs.GetString("playerHealth") + "," + PlayerPrefs.GetString("playerScore");
        byte[] content = Encoding.UTF8.GetBytes(contentMessage);
        bool reliable = true;
        PhotonNetwork.RaiseEvent(evCode, content, reliable, null);
    }

    public void GetStorePosition()
    {
        byte evCode = 4; // evCode for saving position
        string contentMessage = PhotonNetwork.playerName;
        byte[] content = Encoding.UTF8.GetBytes(contentMessage);
        bool reliable = true;
        PhotonNetwork.RaiseEvent(evCode, content, reliable, null);
    }

    /* Get health of player */
    public void GetStoreHealth()
    {
        byte evCode = 5;
        string contentMessage = PhotonNetwork.playerName;
        byte[] content = Encoding.UTF8.GetBytes(contentMessage);
        bool reliable = true;
        PhotonNetwork.RaiseEvent(evCode, content, reliable, null);
    }

    /* Get score of player */
    public void GetStoreScore()
    {
        byte evCode = 7;
        string contentMessage = PhotonNetwork.playerName;
        byte[] content = Encoding.UTF8.GetBytes(contentMessage);
        bool reliable = true;
        PhotonNetwork.RaiseEvent(evCode, content, reliable, null);
    }
#endregion

    /* Send to Server Player Info - Name, Pos and Health */
    public void SendPlayerInfo(GameObject _playerToSet)
    {
        // code of the event
        byte evCode = 11;

        // set player info
        _playerToSet.GetComponent<CustomObjectBase>().InitCustomObjectBase();
        _playerToSet.GetComponent<CustomObjectBase>().SetTargetPacket(PhotonNetwork.playerName);
        _playerToSet.GetComponent<CustomObjectBase>().SetName(PhotonNetwork.playerName);
        _playerToSet.GetComponent<CustomObjectBase>().SetPosition(m_playerPos);
        _playerToSet.GetComponent<CustomObjectBase>().SetHealth(player.GetComponentInChildren<Health>().PlayerHealth);

        // raise event to send ( nid serialise first to make into byte array )
        byte[] content = Packet_Serialisation.GetInstance().SerializeCustomObjects(_playerToSet);
        PhotonNetwork.RaiseEvent(evCode, content, true, null);
    }

    /* Send over a Trigger to spawn enemies */
    public void TriggerEnemySpawn()
    {
        byte evCode = 12;

        // send in Player name 
        byte[] store = Packet_Serialisation.GetInstance().SerializeCustomObjects(player);
        PhotonNetwork.RaiseEvent(evCode, store, true, null);

        //Debug.Log("Message sent in TriggerEnemySpawn : " + evCode);
    }

    IEnumerator OnLeftRoom()
    {
        //Easy way to reset the level: Otherwise we'd manually reset the camera

        //Wait until Photon is properly disconnected (empty room, and connected back to main server)
        while(PhotonNetwork.room!=null || PhotonNetwork.connected==false)
            yield return 0;
        Application.LoadLevel(Application.loadedLevel);

    }

    private void LoadPlayer()
    {
        //prepare instantiation data for the viking: Randomly diable the axe and/or shield
        bool[] enabledRenderers = new bool[2];
        enabledRenderers[0] = Random.Range(0, 2) == 0;//Axe
        enabledRenderers[1] = Random.Range(0, 2) == 0;//Shield

        object[] objs = new object[1]; // Put our bool data in an object array, to send
        objs[0] = enabledRenderers;

        // Spawn our local player
        if (player == null)
        {
            if (m_PlayerPosition != null)
            {
                //Debug.Log("Creating player at its saved position : " + m_PlayerPosition);
                string[] positions = m_PlayerPosition.Split(' ');
                Vector3 positionForPlayer = new Vector3(float.Parse(positions[0]), float.Parse(positions[1]), float.Parse(positions[2]));

                player = PhotonNetwork.Instantiate(this.playerPrefabName, positionForPlayer, Quaternion.identity, 0, objs);
                pet = PhotonNetwork.Instantiate(this.petPrefabName, positionForPlayer + new Vector3(0, 0, -1), Quaternion.identity, 0);
                pet.GetComponent<PetMovement>().SetPlayer(player);

                // health, read from server
                player.GetComponentInChildren<Health>().PlayerHealth = int.Parse(m_PlayerHealth);

                // score, read from server
                player.GetComponent<Highscore>().m_score = int.Parse(m_PlayerScore);

            }
            else
            {
                player = PhotonNetwork.Instantiate(this.playerPrefabName, transform.position, Quaternion.identity, 0, objs);

                pet = PhotonNetwork.Instantiate(this.petPrefabName, transform.position + new Vector3(0, 0, -1), Quaternion.identity, 0);
                pet.GetComponent<PetMovement>().SetPlayer(player);
                // set health
                player.GetComponentInChildren<Health>().PlayerHealth = 10;
                
                // set score
                player.GetComponent<Highscore>().m_score = 0;
               
            }
        }
        // Get Player position 
        m_playerPos = player.transform.position;

        // Sent Player Info to Server
        SendPlayerInfo(player);
    }

    void StartGame()
    {
        Camera.main.farClipPlane = 1000; //Main menu set this to 0.4 for a nicer BG    

        //// get data from GameSpark
        //new GameSparks.Api.Requests.LogEventRequest()
        //    .SetEventKey("LOAD_PLAYER")
        //    .Send((response) =>
        //    {
        //        if (!response.HasErrors)
        //        {
        //            Debug.Log("Received Player Data From GameSparks!");

        //            GSData data = response.ScriptData.GetGSData("player_Data");
        //            m_PlayerPosition = data.GetString("playerPosition");
        //            m_PlayerHealth = data.GetString("playerHealth");

        //            //print("Player Position: " + data.GetString("playerPosition"));
        //            //print("Player Health: " + data.GetString("playerHealth"));
        //        }
        //        else
        //        {
        //            Debug.Log("Error Loading Player Data : " + response.Errors.JSON.ToString());
        //        }
        //    });

        // Load Player to Scene
        LoadPlayer();

        // Get Achievement for login
        new GameSparks.Api.Requests.LogEventRequest()
            .SetEventKey("AWARD_ACHIEVEMENT_LOGIN")
            .Send((response) =>
            {
                if (!response.HasErrors)
                {
                    Debug.Log("Achievement achieved!");
                }
                else
                {
                    Debug.Log("Error : " + response.Errors.JSON.ToString());
                }
            });

        // start timer
        m_achievementTimer = 0.0f;
        m_bAtimerStart = false;
    }

    void OnGUI()
    {
        if (PhotonNetwork.room == null) return; //Only display this GUI when inside a room

        // Button to leave room
        if (GUILayout.Button("Leave Room"))
        {
            //// store position
            //string playerPos = player.transform.position.x + " " + player.transform.position.y + " " + player.transform.position.z;
            //PlayerPrefs.SetString("playerPos", playerPos);

            //// store health 
            //int playerHealth = player.GetComponentInChildren<Health>().PlayerHealth;
            //PlayerPrefs.SetString("playerHealth", playerHealth.ToString());

            //// store score
            //int playerScore = player.GetComponent<Highscore>().m_score;
            //PlayerPrefs.SetString("playerScore", playerScore.ToString());

            //// store to GameSparks player's pos and health using cloud code
            //new GameSparks.Api.Requests.LogEventRequest()
            //    .SetEventKey("SAVE_PLAYER")
            //    .SetEventAttribute("POSITION", playerPos)
            //    .SetEventAttribute("HEALTH", playerHealth.ToString())
            //    .Send((response) =>
            //    {
            //        if (!response.HasErrors)
            //            Debug.Log("Player has been saved to GameSparks! Player Pos: "
            //                + playerPos + " / Health : " + playerHealth.ToString());
            //        else
            //            Debug.Log("Error Saving Player Data : " + response.Errors.JSON.ToString());
            //    });
            

            // send event to server to store variables to SQL
            //DoStoreVariables();
            //HasPet = false;
            
            PhotonNetwork.LeaveRoom();
        }

        // Print "U DIED" when died
        if (Health.isDied)
        {
            GUILayout.BeginArea(new Rect(Screen.width / 2 - 50, Screen.height / 2 - 50, 400, 300));

            GUILayout.BeginHorizontal();
            GUI.color = Color.red;
            GUILayout.Label("YOU DIED!!!");
            GUI.color = Color.white;
            GUILayout.EndHorizontal();
            
            GUILayout.EndArea();
        }

        // Print out the achievement / highscore that is achieved
        if (m_isShown)
        {
            GUILayout.BeginArea(new Rect(Screen.width / 3 + 50, Screen.height / 3, 500, 500));

            // Achievement
            GUILayout.BeginHorizontal();
            GUI.color = Color.yellow;
            GUILayout.Label("AWARDED ACHIEVEMENT! " + m_announcement);

            //switch (m_typeMsg)
            //{
            //    case 1: // login
            //        GUILayout.Label("AWARDED ACHIEVEMENT : FIRST LOGIN ");
            //        break;
            //    case 2: // 100 pts
            //        GUILayout.Label("AWARDED ACHIEVEMENT : 100POINTS ");
            //        break;
            //    case 3: // first blood
            //        GUILayout.Label("AWARDED ACHIEVEMENT : FRIST_BLOOD ");
            //        break;
            //    case 4: // kill enemy
            //        GUILayout.Label("AWARDED ACHIEVEMENT : KILL_ENEMY ");
            //        break;
            //    case 5: // top 10
            //        GUILayout.Label("AWARDED ACHIEVEMENT : TOP10 ");
            //        break;
            //}

            GUI.color = Color.white;
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        
    }

    void OnDisconnectedFromPhoton()
    {
        Debug.LogWarning("OnDisconnectedFromPhoton");
    }

    public void Update()
    {
        // For quick highscore
        if (Input.GetKeyDown(KeyCode.E))
        {
            player.GetComponentInChildren<Health>().m_health = 0;
        }

        // For duration to show Achievement
        if (m_bAtimerStart)
        {
            m_isShown = true;
            m_achievementTimer += Time.deltaTime;
            if (m_achievementTimer > 5.0f)
            {
                m_bAtimerStart = false;
                m_achievementTimer = 0.0f;
                m_isShown = false;
            }
        }
    }

    /* Receive data of events from server */
    private void OnEventHandler(byte eventCode, object content, int senderId)
    {
        switch (eventCode)
        {
#region Assignment 1
            //case 2:
            //    Debug.Log(string.Format("Message from Server : {0}", (string)content));
            //    break;
            //case 3:
            //    m_PlayerPosition = (string)content;
            //    Debug.Log("position : " + m_PlayerPosition);
            //    break;
            case 4:
                if(content != null)
                    m_PlayerPosition = content.ToString();
                //Debug.Log("position : " + m_PlayerPosition);
                break;
            case 5:
                if (content != null)
                    m_PlayerHealth = content.ToString();
                //Debug.Log("health : " + m_PlayerHealth);
                break;
            case 7:
                if (content != null)
                    m_PlayerScore = content.ToString();
                //Debug.Log("Score: " + m_PlayerScore);
                //LoadPlayer();
                break;
#endregion
            // --------------------------- Assignment 2 
            case 12: // spawn Enemy AI
                if (content == null)
                {
                    Debug.Log("content is null");
                    break;
                }

                // Deserialise info received from server
                GameObject enemy = Packet_Deserialisation.GetInstance().DeserializeCustomAI((byte[])content) as GameObject;

                // Check if Enemy is already spawned
                foreach (GameObject _enemy in GameObject.FindGameObjectsWithTag("Enemy"))
                {
                    if (_enemy.GetComponent<EnemyAI>().GetName() == enemy.GetComponent<EnemyAI>().GetName())
                    {
                        Debug.Log("Already in Scene!");
                        Debug.Log("Enemy name : " + enemy.GetComponent<EnemyAI>().GetName());
                        Debug.Log("Enemy in scene name : " + _enemy.GetComponent<EnemyAI>().GetName());
                        return;
                    }
                }
                    // instantiate enemies
                    GameObject newEnemy = Instantiate(enemyPrefab, GameObject.Find("EnemyHolder").transform);
                    // set variables
                    newEnemy.GetComponent<EnemyAI>().SetTargetPacket(enemy.GetComponent<EnemyAI>().GetTargetPacket());
                    newEnemy.GetComponent<EnemyAI>().SetName(enemy.GetComponent<EnemyAI>().GetName());
                    newEnemy.GetComponent<EnemyAI>().SetPosition(enemy.GetComponent<EnemyAI>().GetPosition());
                    newEnemy.GetComponent<EnemyAI>().SetHealth(enemy.GetComponent<EnemyAI>().GetHealth());
                    newEnemy.GetComponent<EnemyAI>().SetGrid(enemy.GetComponent<EnemyAI>().GetGrid());
                    newEnemy.GetComponent<EnemyAI>().SetPrevPos(enemy.GetComponent<EnemyAI>().GetPrevPos());
                    newEnemy.GetComponent<EnemyAI>().SetNearestPos(enemy.GetComponent<EnemyAI>().GetNearestPos());

                    // set position of newEnemy
                    newEnemy.transform.position = newEnemy.GetComponent<EnemyAI>().GetPosition();
                    Debug.Log("Position : " + newEnemy.transform.position);

                // destroy the enemy, which will then destroy the gameobject created in deserialise
                DestroyImmediate(enemy);
                break;
            case 13: // updating the enemy AI
                if (content == null)
                {
                    Debug.Log("Content is null");
                    return;
                }

                // Deserialise info received from server
                GameObject infoEnemy = Packet_Deserialisation.GetInstance().DeserializeCustomAI((byte[])content) as GameObject;
                    Debug.Log("enemy in server info : " + infoEnemy.GetComponent<EnemyAI>().GetName());

                // Check if infoEnemy has same name as Enemy in Scene
                foreach (GameObject _enemy in GameObject.FindGameObjectsWithTag("Enemy"))
                {
                    Debug.Log("enemy in scene : " + _enemy.GetComponent<EnemyAI>().GetName());
                    if (infoEnemy.GetComponent<EnemyAI>().GetName() == _enemy.GetComponent<EnemyAI>().GetName())
                    {
                        Debug.Log("Enemy has same name as sent info");
                        _enemy.transform.position = infoEnemy.GetComponent<EnemyAI>().GetPosition();
                    }
                }
                DestroyImmediate(infoEnemy);

                break;
            case 99:
                //Decode string packet
                string serverMessage = "";
                try
                {
                    serverMessage = string.Format("{0}", (string)content);
                    Debug.Log("From Server: " + serverMessage);

                }
                catch (System.Exception e)
                {
                    Debug.Log("Exception : " + e.ToString());
                }
                break;
        }
    }

    /* When there is a new highscore, this message is sent to notify */
    void HighScoreMessageHandler(GameSparks.Api.Messages.NewHighScoreMessage _message)
    {
        Debug.Log("NEW HIGH SCORE \n " + _message.LeaderboardName);
    }

    /* When there is new achievements, this is sent to notify */
    void AchievementMessageHandler(GameSparks.Api.Messages.AchievementEarnedMessage _message)
    {
        m_bAtimerStart = true;
        m_announcement = _message.AchievementName;
    }

}
