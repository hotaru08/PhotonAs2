using Photon.Hive.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPlugin
{
    /* Enemy AI Custom Type Object class */
    public class EnemyAI : CustomObjectBase
    {
        /// <summary>
        /// Previous Position of AI ( used to return back to last state change pos )
        /// </summary>
        private Vector3 m_prevPos;
        public void SetPrevPos(Vector3 _prev) { m_prevPos = _prev; }
        public void SetPrevPosX(float _x) { m_prevPos.x = _x; }
        public void SetPrevPosY(float _y) { m_prevPos.y = _y; }
        public void SetPrevPosZ(float _z) { m_prevPos.z = _z; }
        public Vector3 GetPrevPos() { return m_prevPos; }

        /// <summary>
        /// Store the nearest Player position
        /// </summary>
        private Vector3 m_nearestPlayerPos;
        public void SetNearestPos(Vector3 _nearest) { m_nearestPlayerPos = _nearest; }
        public void SetNearestPosX(float _x) { m_nearestPlayerPos.x = _x; }
        public void SetNearestPosY(float _y) { m_nearestPlayerPos.y = _y; }
        public void SetNearestPosZ(float _z) { m_nearestPlayerPos.z = _z; }
        public Vector3 GetNearestPos() { return m_nearestPlayerPos; }

        /// <summary>
        /// Bool which is true to save last pos before state change
        /// </summary>
        private bool m_bSavePrevPos;
        public void SetSave(bool _save) { m_bSavePrevPos = _save; }
        public bool GetSave() { return m_bSavePrevPos; }

        /// <summary>
        /// State of the Enemy AI
        /// </summary>
        public string m_state;

        /// <summary>
        /// Store the player that is target and its position
        /// </summary>
        public Vector3 m_targetPos;
        private Player m_playerTarget;

        /// <summary>
        /// To make Enemy Idle 
        /// </summary>
        private bool m_bIdle;

        // ------------------------------------------------------------------------
        /// <summary>
        /// Constructor 
        /// </summary>
        public EnemyAI() 
        {
            // from parent, but initialise child inherited varaibles
            this.SetTargetPacket("ALL");
            this.SetName("");
            this.SetPosition(new Vector3());
            this.SetRotation(new Vector3());
            this.SetHealth(0);
            this.SetGrid(true);

            // child class variables
            m_prevPos = new Vector3();
            m_nearestPlayerPos = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue); // set nearest to be a huge number
            m_bSavePrevPos = false;
            m_state = "ROAM";
            m_targetPos = new Vector3();
            m_playerTarget = null;
            m_bIdle = false;
        }

        /// <summary>
        /// Update method to be sent to client constantly to update AI 
        /// </summary>
        public void Update()
        {
            // ----- reset variables when each update
            //Reset();

            // ----- Changing of the states 
            //ChangingOfStates();

            // ---- Update of the States
            switch (m_state)
            {
                case "ROAM":
                    EnemyRoam();
                    break;
                case "CHASE":
                    EnemyChase();
                    break;
                case "ATTACK":
                    EnemyAttack();
                    break;
                case "RETURN":
                    EnemyReturn();
                    break;
                case "IDLE":
                    EnemyIdle();
                    break;
            }
        }

        /// <summary>
        /// Handles the Changing of Enemy States
        /// </summary>
        private void ChangingOfStates()
        {
            // ---- Check for all Players in the PlayerList
            foreach (Player player in RaiseEventTestPlugin.GetInstance().m_playerList)
            {
                // ---- If the Distance from Enemy AI to Player is lesser than (_blank_), change to CHASE
                if (DistanceFromPlayer(player) > 4 * 4 && DistanceFromPlayer(player) < 10 * 10)
                {
                    // ---- Choose the nearest Player
                    if (player.GetPosition().x < m_nearestPlayerPos.x &&
                        player.GetPosition().z < m_nearestPlayerPos.z)
                    {
                        // update nearest player pos
                        m_nearestPlayerPos = player.GetPosition();
                        // set new target 
                        m_targetPos = player.GetPosition();
                        m_playerTarget = player;
                    }

                    // save previous position of AI
                    if (!m_bSavePrevPos)
                    {
                        m_prevPos = this.GetPosition();
                        m_bSavePrevPos = true;
                    }

                    m_state = "CHASE";
                }
                // ---- If the Distance from Enemy AI to Player is lesser than (_blank_), change to ATTACK
                else if (DistanceFromPlayer(player) < 4 * 4)
                {
                    // If a Player suddenly intercepts, change target
                    if (player.GetPosition().x < m_nearestPlayerPos.x &&
                        player.GetPosition().z < m_nearestPlayerPos.z)
                    {
                        // update nearest player pos
                        m_nearestPlayerPos = player.GetPosition();
                        // set new target 
                        m_targetPos = player.GetPosition();
                        m_playerTarget = player;
                    }

                    m_state = "ATTACK";
                }
                // ---- If the Distance from Enemy AI to Player is more than (_blank_), change to RETURN
                else if (DistanceFromPlayer(player) >= 10 * 10
                        && !(this.GetPosition().x == m_prevPos.x
                        && this.GetPosition().z == m_prevPos.z))
                {

                    m_state = "RETURN";
                }
                else
                {
                    m_state = "ROAM";
                }
            }

            RaiseEventTestPlugin.GetInstance().PluginHost.BroadcastEvent(target: ReciverGroup.All,
                               senderActor: 0,
                               targetGroup: 0,
                               evCode: (byte)99,
                               data: new Dictionary<byte, object>() { { (byte)245, "State : " + m_state } },
                               cacheOp: 0);
        }
        

        /// <summary>
        /// Method to handle Roaming State of AI ( 
        /// </summary>
        public void EnemyRoam()
        {
            Random rand = new Random();
            float number = (float)rand.NextDouble();

            if (number < 0.25f)
            {
                // ---- move to right
                this.GetPosition().x += 2.0f;
            }
            else if (number >= 0.25f && number < 0.5f)
            {
                // ---- move to left
                this.GetPosition().x -= 2.0f;
            }
            else if (number >= 0.5f && number < 0.75f)
            {
                // ---- move to up
                this.GetPosition().z += 2.0f;
            }
            else
            {
                // ---- move to down
                this.GetPosition().z -= 2.0f;
            }
        }

        /// <summary>
        ///  Method to handle Chasing State of AI
        /// </summary>
        public void EnemyChase()
        {
            // ---- Update position of Enemy
            Vector3 viewVector = (m_targetPos - this.GetPosition()).Normalised();
            this.GetPosition().x += viewVector.x * 1.0f;
            this.GetPosition().z += viewVector.z * 1.0f;
        }

        /// <summary>
        ///  Method to handle Attacking State of AI
        /// </summary>
        public void EnemyAttack()
        {

        }

        /// <summary>
        ///  Method to handle Returning State of AI
        /// </summary>
        public void EnemyReturn()
        {
            // ---- If AI is not at prev pos, return back
            if (this.GetPosition() != m_prevPos)
            {
                m_targetPos = m_prevPos;
                EnemyChase();
            }
        }

        /// <summary>
        /// Method to handle Idling of AI
        /// </summary>
        public void EnemyIdle()
        {

        }

        /// <summary>
        /// Reset Variables in EnemyAI
        /// </summary>
        public void Reset()
        {
            m_nearestPlayerPos.Set(float.MaxValue, float.MaxValue, float.MaxValue);
            //m_state = "ROAM";
            m_bSavePrevPos = false;
            m_playerTarget = null;
        }

        /// <summary>
        /// Calculates the Distance Enemy AI is from Player ( a^2 + b^2 )
        /// </summary>
        public float DistanceFromPlayer(Player _player)
        {
            return ((_player.GetPosition().x - this.GetPosition().x) * (_player.GetPosition().x - this.GetPosition().x) +
                    (_player.GetPosition().z - this.GetPosition().z) * (_player.GetPosition().z - this.GetPosition().z));
        }
    }
}
