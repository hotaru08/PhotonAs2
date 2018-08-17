using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading; // for threading
using Photon.Hive.Plugin;

/* Handles the Update of the AI - Singleton */
namespace TestPlugin
{
    class AI_Manager
    {
        /// <summary>
        /// ID of the AI in the list ( as like the name of the AI )
        /// </summary>
        private int m_ID;
        public int GetID() { return m_ID; }

        /// <summary>
        /// List of EnemyAI ( to store all EnemyAI that is present )
        /// </summary>
        private List<EnemyAI> m_AIList/* = new List<EnemyAI>()*/;
        public List<EnemyAI> GetAIList() { return m_AIList; }

        /// <summary>
        /// Website say to do this
        /// </summary>
        private object obj = new object();
        
        private static AI_Manager Instance = null;
        private AI_Manager() { }

        /// <summary>
        /// Get the Instance of AI_Manager
        /// </summary>
        public static AI_Manager GetInstance()
        {
            if (Instance == null)
                Instance = new AI_Manager();
            return Instance;
        }

        /// <summary>
        /// Adding Enemy Ai into AIList
        /// </summary>
        public void AddEnemy(EnemyAI _ai)
        {
            if (m_AIList == null) return;
            m_AIList.Add(_ai);
        }

        /// <summary>
        /// Use this to Spawn enemies
        /// </summary>
        public void SpawnEnemy(string _name, Vector3 _pos)
        {
            // ---- Create new enemy to spawn
            EnemyAI newEnemy = new EnemyAI();
            newEnemy.SetName(_name);
            newEnemy.SetPosition(_pos);
            ++m_ID;

            // ---- Add it to List
            AddEnemy(newEnemy);
        }

        /// <summary>
        /// This is the other thread that handles the AI
        /// Main thread is the RaiseEvent
        /// </summary>
        public void ThreadOfAI()
        {
            // ---- Website did this, so i do this ( smth abt ensuring that update only this )
            lock (obj)
            {
                // Get the currently running thread
                var thread = Thread.CurrentThread;

                // ensure main thread is started before updating this thread
                while (RaiseEventTestPlugin.GetInstance() == null)
                    Thread.Sleep(100);

                // ---- Start updating this thread
                // Initialise list
                m_AIList = new List<EnemyAI>();

                // Spawn enemies in list
                SpawnEnemy("EnemyName " + m_ID.ToString(), new Vector3(10.0f, 0.0f, 10.0f));
                SpawnEnemy("EnemyName " + m_ID.ToString(), new Vector3(20.0f, 0.0f, 20.0f));
                SpawnEnemy("EnemyName " + m_ID.ToString(), new Vector3(30.0f, 0.0f, 30.0f));

                // Update the enemies
                while (true)
                {
                    //RaiseEventTestPlugin.GetInstance().PluginHost.BroadcastEvent(target: ReciverGroup.All,
                    //                                                                 senderActor: 0,
                    //                                                                 targetGroup: 0,
                    //                                                                 evCode: (byte)99,
                    //                                                                 data: new Dictionary<byte, object>() { { (byte)245, "entered while loop" } },
                    //                                                                 cacheOp: 0);

                    // Update each enemy
                    foreach (EnemyAI enemy in Instance.m_AIList)
                    {
                        enemy.Update();

                        // set target packet to send to be player 
                        //foreach (Player player in RaiseEventTestPlugin.GetInstance().m_playerList)
                        //{
                        //enemy.SetTargetPacket(player.GetName());


                        // send over to client to update the Enemy obj position
                        byte[] store = Packet_Serialisation.GetInstance().SerializeCustomAI(enemy);
                        //RaiseEventTestPlugin.GetInstance().PluginHost.BroadcastEvent(target: ReciverGroup.All,
                        //                                                                 senderActor: 0,
                        //                                                                 targetGroup: 0,
                        //                                                                 evCode: (byte)99,
                        //                                                                 data: new Dictionary<byte, object>() { { (byte)245, "entered while loop" } },
                        //                                                                 cacheOp: 0);
                        //RaiseEventTestPlugin.GetInstance().PluginHost.BroadcastEvent(target: ReciverGroup.All,
                        //                                                             senderActor: 0,
                        //                                                             targetGroup: 0,
                        //                                                             evCode: (byte)13,
                        //                                                             data: new Dictionary<byte, object>() { { (byte)245, store } },
                        //                                                             cacheOp: 0);
                        //}
                    }
                    Thread.Sleep(100); // 0.1s
                }
            }
        }
    }
}
