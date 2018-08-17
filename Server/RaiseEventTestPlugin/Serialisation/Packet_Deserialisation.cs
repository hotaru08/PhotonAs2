using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.Hive;
using Photon.Hive.Plugin;

namespace TestPlugin
{
    /* This class handles data synchronisation - deserialise custom types */
    /* Receive packets from client and contructs back custom type objects */
    class Packet_Deserialisation
    {
        /// <summary>
        /// Singleton class - only one instance of this class
        /// </summary>
        private static Packet_Deserialisation Instance = null;
        private Packet_Deserialisation() { }

        /// <summary>
        /// Get Instance of this class
        /// </summary>
        public static Packet_Deserialisation GetInstance()
        {
            if (Instance == null)
                Instance = new Packet_Deserialisation();
            return Instance;
        }

        /************************************* De - Serialisation of Packets *************************************/
        /// <summary>
        /// Player Deseialisation
        /// </summary>
        //public object DeserializeCustomPlayer(byte[] bytes)
        //{
        //    Player customObject = new Player(); // create new obj
        //    if (customObject == null) { return null; }

        //    // start to construct back to custom type ( byte packets )
        //    using (var s = new MemoryStream(bytes))
        //    {
        //        using (var br = new BinaryReader(s))
        //        {
        //            // Target Packet
        //            customObject.SetTargetPacket(br.ReadString());

        //            // Name
        //            customObject.SetName(br.ReadString());

        //            // Position
        //            customObject.SetPositionX(br.ReadSingle());
        //            customObject.SetPositionY(br.ReadSingle());
        //            customObject.SetPositionZ(br.ReadSingle());

        //            // Rotation
        //            customObject.SetRotationX(br.ReadSingle());
        //            customObject.SetRotationY(br.ReadSingle());
        //            customObject.SetRotationZ(br.ReadSingle());

        //            // HEalth
        //            customObject.SetHealth(br.ReadInt32());

        //            // isinGrid
        //            customObject.SetGrid(br.ReadBoolean());
        //        }
        //    }
        //    return customObject;
        //}

        /// <summary>
        /// Custom Object Type Deserialisation
        /// </summary>
        public object DeserializeCustomObjects(byte[] bytes)
        {
            CustomObjectBase customObject = new CustomObjectBase(); // create new obj
            if (customObject == null) return null;
            
            // start to construct back to custom type ( byte packets )
            using (var s = new MemoryStream(bytes))
            {
                using (var br = new BinaryReader(s))
                {
                    // Target Packet
                    customObject.SetTargetPacket(br.ReadString());

                    // Name
                    customObject.SetName(br.ReadString());

                    // Position
                    customObject.SetPositionX(br.ReadSingle());
                    customObject.SetPositionY(br.ReadSingle());
                    customObject.SetPositionZ(br.ReadSingle());

                    // Rotation
                    customObject.SetRotationX(br.ReadSingle());
                    customObject.SetRotationY(br.ReadSingle());
                    customObject.SetRotationZ(br.ReadSingle());

                    // HEalth
                    customObject.SetHealth(br.ReadInt32());

                    // isinGrid
                    customObject.SetGrid(br.ReadBoolean());

                }
            }
            return customObject;
        }

        /// <summary>
        /// Enemy AI Deseialisation
        /// </summary>
        public object DeserializeCustomAI(byte[] bytes)
        {
            EnemyAI customObject = new EnemyAI(); // create new obj
            if (customObject == null) return null;

            // start to construct back to custom type ( byte packets )
            using (var s = new MemoryStream(bytes))
            {
                using (var br = new BinaryReader(s))
                {
                    // Target Packet
                    customObject.SetTargetPacket(br.ReadString());

                    // Name
                    customObject.SetName(br.ReadString());

                    // Position
                    customObject.SetPositionX(br.ReadSingle());
                    customObject.SetPositionY(br.ReadSingle());
                    customObject.SetPositionZ(br.ReadSingle());

                    // Rotation
                    customObject.SetRotationX(br.ReadSingle());
                    customObject.SetRotationY(br.ReadSingle());
                    customObject.SetRotationZ(br.ReadSingle());

                    // Health
                    customObject.SetHealth(br.ReadInt32());

                    // isinGrid
                    customObject.SetGrid(br.ReadBoolean());

                    // pos of enemy
                    customObject.SetPrevPosX(br.ReadSingle());
                    customObject.SetPrevPosY(br.ReadSingle());
                    customObject.SetPrevPosZ(br.ReadSingle());

                    // pos of nearest player
                    customObject.SetNearestPosX(br.ReadSingle());
                    customObject.SetNearestPosY(br.ReadSingle());
                    customObject.SetNearestPosZ(br.ReadSingle());
                }
            }
            return customObject;
        }
    }
}
