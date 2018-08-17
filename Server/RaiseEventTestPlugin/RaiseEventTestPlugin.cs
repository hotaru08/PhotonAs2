using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Photon.Hive;
using Photon.Hive.Plugin;

using MySql.Data;
using MySql.Data.MySqlClient;
using System.Threading;

namespace TestPlugin
{
    /* Body of Plugin - Hooking OnRaiseEvent */
    public class RaiseEventTestPlugin : PluginBase
    {
        // Variables 
        private string connStr;
        private MySqlConnection conn;
        private string sql;

        // Getters and Setters
        public string ServerString { get; private set; }
        public int CallsCount { get; private set; }
        public override string Name
        {
            get
            {
                return this.GetType().Name;
            }
        }

        // Constructor 
        public RaiseEventTestPlugin()
        {
            this.UseStrictMode = true;
            this.ServerString = "ServerMessage";
            this.CallsCount = 0;

            // --- Connect to MySQL.
            ConnectToMySQL();
        }

        // Instance of this class
        private static RaiseEventTestPlugin Instance = null;
        public static RaiseEventTestPlugin GetInstance()
        {
            if (Instance == null)
                Instance = new RaiseEventTestPlugin();
            return Instance;
        }

        /// <summary>
        /// List containing all players that joins the game
        /// </summary>
        public List<Player> m_playerList;

        /// <summary>
        /// New thread to be created and started
        /// </summary>
        public Thread thread;

        /// <summary>
        /// Call this to initialise variables
        /// </summary>
        public void Init()
        {
            // player list
            m_playerList = new List<Player>();

            // create a new thread, taking in ThreadStart ( the method to be executed on thread )
            try
            {
                ThreadStart methodExecuted = new ThreadStart(AI_Manager.GetInstance().ThreadOfAI);
                thread = new Thread(methodExecuted);
                thread.Name = "AIThread";
                thread.Start();
            }
            catch (ThreadStartException e)
            {
                this.PluginHost.BroadcastEvent(target: ReciverGroup.All,
                                               senderActor: 0,
                                               targetGroup: 0,
                                               data: new Dictionary<byte, object>() { { (byte)245, "Exception Thread : " + e.ToString() } },
                                               evCode: (byte)99,
                                               cacheOp: 0);
            }
        }


        public override void OnRaiseEvent(IRaiseEventCallInfo info)
        {
            try
            {
                base.OnRaiseEvent(info);
            }
            catch (Exception e)
            {
                this.PluginHost.BroadcastErrorInfoEvent(e.ToString(), info);
                return;
            }

            // Deserialise Packet received from Client
            CustomObjectBase data = null;
            try
            {
                data = Packet_Deserialisation.GetInstance().DeserializeCustomObjects((byte[])info.Request.Data) as CustomObjectBase;
            }
            catch (Exception e)
            {
                this.PluginHost.BroadcastEvent(target: ReciverGroup.All,
                                               senderActor: 0,
                                               targetGroup: 0,
                                               data: new Dictionary<byte, object>() { { (byte)245, "Exception : " + e.ToString() } },
                                               evCode: (byte)99,
                                               cacheOp: 0);
            }

            #region Assignment 1
            // Successful Hook
            if (info.Request.EvCode == 1) // simple plugin test
            {
                /// Convert from string to char array 
                string playerName = Encoding.Default.GetString((byte[])info.Request.Data);
                /// Query statement
                sql = "INSERT INTO users (name, date_created) VALUES ('" + playerName + "', now())";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();

                /// When Clicked, increase count and annouce
                ++this.CallsCount;
                int cnt = this.CallsCount;
                string ReturnMessage = info.Nickname + " clicked the button. Now the count is " + cnt.ToString();
                this.PluginHost.BroadcastEvent(target: ReciverGroup.All,
                                               senderActor: 0,
                                               targetGroup: 0,
                                               data: new Dictionary<byte, object>() { { (byte)245, ReturnMessage } },
                                               evCode: info.Request.EvCode,
                                               cacheOp: 0);
            }
            else if (info.Request.EvCode == 2) // Login of Viking 
            {
                /// Getting info from client and saving to SQL
                string playerInfo = Encoding.Default.GetString((byte[])info.Request.Data); /// Convert from string to char array 
                string playerPassword = "", playerName = "";
                bool isPassword = false;

                /// Seperate name and password
                for (int i = 0; i < playerInfo.Length; ++i)
                {
                    if (playerInfo[i] == ',')
                    {
                        isPassword = true;
                        continue;
                    }

                    /// Set name and password
                    if (!isPassword)
                        playerName += playerInfo[i];
                    else
                        playerPassword += playerInfo[i];
                }

                /// Check using playerName for existing accounts
                if (!NameExist(playerName, info.Request.EvCode))
                {
                    /// Query statement
                    sql = "INSERT INTO users (name, password, date_created) VALUES ('" + playerName + "','" + playerPassword + "', now())";
                    ServerString = "New Account created";
                }
                else
                {
                    /// Check if the password is correct
                    if (!PasswordMatch(playerName, playerPassword))
                    {
                        sql = "UPDATE users SET password='" + playerPassword + "' WHERE name='" + playerName + "'";
                        ServerString = "password is updated";
                    }
                    else
                    {
                        /// if username and password exists, send login is ok
                        ServerString = "Login Result = OK";
                    }
                }

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();

                /// send back message to server
                this.PluginHost.BroadcastEvent(target: ReciverGroup.All,
                                               senderActor: 0,
                                               targetGroup: 0,
                                               data: new Dictionary<byte, object>() { { (byte)245, ServerString } },
                                               evCode: info.Request.EvCode,
                                               cacheOp: 0);
            }

            else if (info.Request.EvCode == 3) // Variables of Viking
            {
                string playerInfo = Encoding.Default.GetString((byte[])info.Request.Data); /// Convert from string to char array 
                string playerPos = "", playerName = "";
                int playerHealth = 0, playerScore = 0;

                string[] m_storeInfo = playerInfo.Split(',');
                playerName = m_storeInfo[0];
                playerPos = m_storeInfo[1];
                playerHealth = int.Parse(m_storeInfo[2]);
                playerScore = int.Parse(m_storeInfo[3]);

                /// Check using playerName for existing accounts
                if (!NameExist(playerName, info.Request.EvCode))
                {
                    /// Query statement
                    sql = "INSERT INTO user_position (name, position, health, score) VALUES ('" + playerName + "','" + playerPos + "','" + playerHealth + "','" + playerScore + "')";
                }
                else
                {
                    sql = "UPDATE user_position SET position='" + playerPos + "', health='" + playerHealth.ToString() + "', score='" + playerScore.ToString() + "' WHERE name='" + playerName + "'";
                }

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();

                /// send back message to server
                this.PluginHost.BroadcastEvent(target: ReciverGroup.All,
                                               senderActor: 0,
                                               targetGroup: 0,
                                               data: new Dictionary<byte, object>() { { (byte)245, null } },
                                               evCode: info.Request.EvCode,
                                               cacheOp: 0);

            }

            else if (info.Request.EvCode == 4) // Get position of player
            {
                string playerInfo = Encoding.Default.GetString((byte[])info.Request.Data); /// Convert from string to char array 
                string playerName = playerInfo;

                if (NameExist(playerName, 3))
                {
                    /// Query statement
                    sql = "SELECT position FROM user_position WHERE name='" + playerName + "'";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    object position = cmd.ExecuteScalar();


                    /// send back message to server
                    this.PluginHost.BroadcastEvent(target: ReciverGroup.All,
                                                   senderActor: 0,
                                                   targetGroup: 0,
                                                   data: new Dictionary<byte, object>() { { (byte)245, position } },
                                                   evCode: info.Request.EvCode,
                                                   cacheOp: 0);
                }
                else
                {
                    /// send back message to server
                    this.PluginHost.BroadcastEvent(target: ReciverGroup.All,
                                                   senderActor: 0,
                                                   targetGroup: 0,
                                                   data: new Dictionary<byte, object>() { { (byte)245, null } },
                                                   evCode: info.Request.EvCode,
                                                   cacheOp: 0);
                }
            }

            else if (info.Request.EvCode == 5) // get health of player 
            {
                string playerInfo = Encoding.Default.GetString((byte[])info.Request.Data); /// Convert from string to char array 
                string playerName = playerInfo;

                if (NameExist(playerName, 3))
                {
                    /// Query statement
                    sql = "SELECT health FROM user_position WHERE name='" + playerName + "'";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    object hp = cmd.ExecuteScalar();

                    /// send back message to server
                    this.PluginHost.BroadcastEvent(target: ReciverGroup.All,
                                                   senderActor: 0,
                                                   targetGroup: 0,
                                                   data: new Dictionary<byte, object>() { { (byte)245, hp } },
                                                   evCode: info.Request.EvCode,
                                                   cacheOp: 0);
                }
                else
                {
                    /// send back message to server
                    this.PluginHost.BroadcastEvent(target: ReciverGroup.All,
                                                   senderActor: 0,
                                                   targetGroup: 0,
                                                   data: new Dictionary<byte, object>() { { (byte)245, null } },
                                                   evCode: info.Request.EvCode,
                                                   cacheOp: 0);
                }
            }

            else if (info.Request.EvCode == 6) // Add friends to vikings
            {
                string playerInfo = Encoding.Default.GetString((byte[])info.Request.Data); /// Convert from string to char array 
                string[] store = playerInfo.Split(',');
                string playerName = store[0];
                string friendName = store[1];

                sql = "INSERT INTO user_friends (name, friend_name) VALUES ('" + playerName + "','" + friendName + "')";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();

                /// send back message to server
                this.PluginHost.BroadcastEvent(target: ReciverGroup.All,
                                               senderActor: 0,
                                               targetGroup: 0,
                                               data: new Dictionary<byte, object>() { { (byte)245, null } },
                                               evCode: info.Request.EvCode,
                                               cacheOp: 0);
            }

            else if (info.Request.EvCode == 7) // get score of player
            {
                string playerInfo = Encoding.Default.GetString((byte[])info.Request.Data); /// Convert from string to char array 
                string playerName = playerInfo;

                if (NameExist(playerName, 3))
                {
                    /// Query statement
                    sql = "SELECT score FROM user_position WHERE name='" + playerName + "'";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    object score = cmd.ExecuteScalar();

                    /// send back message to server
                    this.PluginHost.BroadcastEvent(target: ReciverGroup.All,
                                                   senderActor: 0,
                                                   targetGroup: 0,
                                                   data: new Dictionary<byte, object>() { { (byte)245, score } },
                                                   evCode: info.Request.EvCode,
                                                   cacheOp: 0);
                }
                else
                {
                    /// send back message to server
                    this.PluginHost.BroadcastEvent(target: ReciverGroup.All,
                                                   senderActor: 0,
                                                   targetGroup: 0,
                                                   data: new Dictionary<byte, object>() { { (byte)245, null } },
                                                   evCode: info.Request.EvCode,
                                                   cacheOp: 0);
                }
            }


            else if (info.Request.EvCode == 8) // Add friends to vikings
            {
                string playerInfo = Encoding.Default.GetString((byte[])info.Request.Data); /// Convert from string to char array 
                string playerName = playerInfo;

                if (NameExist(playerName, 4))
                {
                    sql = "SELECT GROUP_CONCAT(friend_name) FROM user_friends WHERE name='" + playerName + "'";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    object Friends = cmd.ExecuteScalar();
                    string toreturn = playerName + " " + Friends.ToString();
                    /// send back message to server
                    this.PluginHost.BroadcastEvent(target: ReciverGroup.All,
                                                   senderActor: 0,
                                                   targetGroup: 0,
                                                   data: new Dictionary<byte, object>() { { (byte)245, toreturn } },
                                                   evCode: info.Request.EvCode,
                                                   cacheOp: 0);
                }
                else
                {
                    /// send back message to server
                    this.PluginHost.BroadcastEvent(target: ReciverGroup.All,
                                                   senderActor: 0,
                                                   targetGroup: 0,
                                                   data: new Dictionary<byte, object>() { { (byte)245, null } },
                                                   evCode: info.Request.EvCode,
                                                   cacheOp: 0);
                }


            }

            else if (info.Request.EvCode == 9) // Add friends to vikings
            {
                string playerInfo = Encoding.Default.GetString((byte[])info.Request.Data); /// Convert from string to char array 
                string[] store = playerInfo.Split(',');
                string playerName = store[0];
                string partyName = store[1];

                sql = "INSERT INTO user_party (name, party_name) VALUES ('" + playerName + "','" + partyName + "')";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();

                /// send back message to server
                this.PluginHost.BroadcastEvent(target: ReciverGroup.All,
                                               senderActor: 0,
                                               targetGroup: 0,
                                               data: new Dictionary<byte, object>() { { (byte)245, null } },
                                               evCode: info.Request.EvCode,
                                               cacheOp: 0);
            }

            else if (info.Request.EvCode == 10) // Add friends to vikings
            {
                string playerInfo = Encoding.Default.GetString((byte[])info.Request.Data); /// Convert from string to char array 
                string playerName = playerInfo;

                if (NameExist(playerName, 5))
                {
                    sql = "SELECT GROUP_CONCAT(party_name) FROM user_party WHERE name='" + playerName + "'";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    object Party = cmd.ExecuteScalar();
                    string toreturn = playerName + " " + Party.ToString();

                    /// send back message to server
                    this.PluginHost.BroadcastEvent(target: ReciverGroup.All,
                                                   senderActor: 0,
                                                   targetGroup: 0,
                                                   data: new Dictionary<byte, object>() { { (byte)245, toreturn } },
                                                   evCode: info.Request.EvCode,
                                                   cacheOp: 0);
                }
                else
                {
                    this.PluginHost.BroadcastEvent(target: ReciverGroup.All,
                                                   senderActor: 0,
                                                   targetGroup: 0,
                                                   data: new Dictionary<byte, object>() { { (byte)245, null } },
                                                   evCode: info.Request.EvCode,
                                                   cacheOp: 0);
                }
            }
            #endregion
            // ************************************** Assignment 2 
            else if (info.Request.EvCode == 11) // when player login, client send info over and server stores them to player obj which is added to list
            {
                //CustomObjectBase addPlayer = (CustomObjectBase)Packet_Deserialisation.GetInstance().DeserializeCustomObjects((byte[])info.Request.Data); // breaks here
                if (data == null)
                {
                    this.PluginHost.BroadcastEvent(target: ReciverGroup.All,
                                     senderActor: 0,
                                     targetGroup: 0,
                                     evCode: (byte)99,
                                     data: new Dictionary<byte, object>() { { (byte)245, "data data data is null" } },
                                     cacheOp: 0);
                    return;
                }

                // Check for duplicates when list is not empty
                if (m_playerList.Count > 0)
                {
                    foreach (Player player in m_playerList)
                    {
                        if (player.GetName() == data.GetName())
                        {
                            this.PluginHost.BroadcastEvent(target: ReciverGroup.All,
                                senderActor: 0,
                                targetGroup: 0,
                                evCode: (byte)99,
                                data: new Dictionary<byte, object>() { { (byte)245, "Player Name already in list : "
                                                                                            + player.GetName() } },
                                cacheOp: 0);
                            return;
                        }
                    }
                }

                // Add to list 
                m_playerList.Add(data as Player);
                //foreach(Player player in m_playerList)
                //{
                    this.PluginHost.BroadcastEvent(target: ReciverGroup.All,
                                senderActor: 0,
                                targetGroup: 0,
                                evCode: (byte)99,
                                data: new Dictionary<byte, object>() { { (byte)245, "Player list: " + m_playerList.Count } },
                                cacheOp: 0);
                //}
            }
            else if (info.Request.EvCode == 12) // sending enemy info so that client can spawn them
            {
                // ensure AI_Manager is running to continue
                while (AI_Manager.GetInstance() == null)
                    Thread.Sleep(100);
                
                this.PluginHost.BroadcastEvent(target: ReciverGroup.All,
                                      senderActor: 0,
                                      targetGroup: 0,
                                      evCode: (byte)99,
                                      data: new Dictionary<byte, object>() { { (byte)245, "Thread Info: " + thread.IsAlive + " / " + thread.ThreadState
                                                                                            + " / " + thread.Name } },
                                       cacheOp: 0);

                try
                {
                    if (AI_Manager.GetInstance().GetAIList() == null)
                    {
                        this.PluginHost.BroadcastEvent(target: ReciverGroup.All,
                                    senderActor: 0,
                                    targetGroup: 0,
                                    evCode: (byte)99,
                                    data: new Dictionary<byte, object>() { { (byte)245, "Enemy list is null" } },
                                     cacheOp: 0);
                    }

                    // send information to client about each Enemy AI
                    foreach (EnemyAI enemy in AI_Manager.GetInstance().GetAIList())
                    {
                        enemy.SetTargetPacket(data.GetName()); // player's name
                        this.PluginHost.BroadcastEvent(target: ReciverGroup.All,
                                    senderActor: 0,
                                    targetGroup: 0,
                                    evCode: (byte)99,
                                    data: new Dictionary<byte, object>() { { (byte)245, "Enemy target packet: " + enemy.GetTargetPacket() } },
                                     cacheOp: 0);

                        byte[] store = Packet_Serialisation.GetInstance().SerializeCustomAI((object)enemy);
                        //if (store != null)
                        //{
                        //    this.PluginHost.BroadcastEvent(target: ReciverGroup.All,
                        //            senderActor: 0,
                        //            targetGroup: 0,
                        //            evCode: 99,
                        //            data: new Dictionary<byte, object>() { { (byte)245, "this is store that is not null " + store.Length + " / " + enemy.GetName() } },
                        //            cacheOp: 0); // send to client each enemy in AI list
                        //}
                        //else
                        //{
                        //    this.PluginHost.BroadcastEvent(target: ReciverGroup.All,
                        //            senderActor: 0,
                        //            targetGroup: 0,
                        //            evCode: 99,
                        //            data: new Dictionary<byte, object>() { { (byte)245, "this is store that is null" } },
                        //            cacheOp: 0); // send to client each enemy in AI list
                        //}

                        this.PluginHost.BroadcastEvent(target: ReciverGroup.All,
                                    senderActor: 0,
                                    targetGroup: 0,
                                    evCode: info.Request.EvCode,
                                    data: new Dictionary<byte, object>() { { (byte)245, store } },
                                    cacheOp: 0); // send to client each enemy in AI list
                    }
                }
                catch (Exception e)
                {
                    this.PluginHost.BroadcastEvent(target: ReciverGroup.All,
                                senderActor: 0,
                                targetGroup: 0,
                                evCode: (byte)99,
                                data: new Dictionary<byte, object>() { { (byte)245, "Exception : " + e.ToString() } },
                                 cacheOp: 0);
                }
            }
        }

        /* Linking to SQL */
        public void ConnectToMySQL()
        {
            // Connect to MySQL
            connStr = "server=localhost;user=root;database=photon;port=3306;password=marrot4299";
            conn = new MySqlConnection(connStr);

            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        /* Un-link from Server */
        public void DisconnectFromMySQL()
        {
            conn.Close();
        }

        /* Checking if Name exists */
        public bool NameExist(string _playerName, int _type)
        {
            /// Query Statement
            switch (_type)
            {
                case 2:
                    sql = "SELECT name FROM users WHERE name='" + _playerName + "'";
                    break;
                case 3:
                    sql = "SELECT name FROM user_position WHERE name='" + _playerName + "'";
                    break;
                case 4:
                    sql = "SELECT name FROM user_friends WHERE name='" + _playerName + "'";
                    break;
                case 5:
                    sql = "SELECT name FROM user_party WHERE name='" + _playerName + "'";
                    break;
            }

            MySqlCommand cmd = new MySqlCommand(sql, conn);
            object obj = cmd.ExecuteScalar();

            if (obj != null)
                return true;

            return false;
        }

        /* Checking if Password exists */
        public bool PasswordMatch(string _playerName, string _password)
        {
            sql = "SELECT password FROM users WHERE name='" + _playerName + "' and password='" + _password + "'";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            object obj = cmd.ExecuteScalar();

            if (obj != null)
                return true;

            return false;
        }

        /// <summary>
        /// Overrides SetupInstance function in default plugin lib
        /// Any new Custom Type Objects are to be registered here ( for Serialization / Deserialization )
        /// This is called as soon as plugin is initialised
        /// </summary>
        public override bool SetupInstance(IPluginHost host, Dictionary<string, string> config, out string errorMsg)
        {
            // Player
            //host.TryRegisterType(typeof(Player), 1,
            //                        Packet_Serialisation.GetInstance().SerializeCustomPlayer,
            //                        Packet_Deserialisation.GetInstance().DeserializeCustomPlayer 
            //                        );

            // Custom Object 
            host.TryRegisterType(typeof(CustomObjectBase), 1,
                                    Packet_Serialisation.GetInstance().SerializeCustomObjects,
                                    Packet_Deserialisation.GetInstance().DeserializeCustomObjects
                                    );

            // Enemy AI
            host.TryRegisterType(typeof(EnemyAI), 1,
                                    Packet_Serialisation.GetInstance().SerializeCustomAI,
                                    Packet_Deserialisation.GetInstance().DeserializeCustomAI
                                    );

            return base.SetupInstance(host, config, out errorMsg);
        }

    } // end of namespace
}