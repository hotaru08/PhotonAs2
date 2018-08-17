using Photon.Hive.Plugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPlugin
{
    /* This class handles data synchronisation - serialise custom types */
    /* Transform custom type text packets to byte array to send to client */
    class Packet_Serialisation
    {
        /// <summary>
        /// Singleton Class - one instance of this class
        /// </summary>
        private static Packet_Serialisation Instance = null;
        private Packet_Serialisation() { }

        /// <summary>
        /// Get the instance of this class
        /// </summary>
        public static Packet_Serialisation GetInstance()
        {
            if (Instance == null)
                Instance = new Packet_Serialisation();
            return Instance; 
        }

        /************************************* Serialisation of Packets *************************************/
        /// <summary>
        /// Player Serialisation
        /// </summary>
        //public byte[] SerializeCustomPlayer(object o)
        //{
        //    Player customObject = o as Player; // casting
        //    if (customObject == null) { return null; } // no obj

        //    // start conversion to byte array
        //    byte[] store;
        //    using (var s = new MemoryStream())
        //    {
        //        using (var bw = new BinaryWriter(s))
        //        {
        //            // Write Player Data here ( call custom obj serialise as its the same )
        //            store = SerializeCustomObjects(customObject);
        //            // add to s to convert to byte array
        //            return ConCatArray(store, s.ToArray());
        //        }
        //    }
        //}

        /// <summary>
        /// Custom Objects Serialisation
        /// </summary>
        public byte[] SerializeCustomObjects(object o)
        {
            CustomObjectBase customObject = o as CustomObjectBase; // casting
            if (customObject == null) { return null; } // if no obj

            // conversion to byte array
            using (var s = new MemoryStream())
            {
                using (var bw = new BinaryWriter(s))
                {
                    // Target Packet
                    bw.Write(customObject.GetTargetPacket());

                    // Name
                    bw.Write(customObject.GetName());

                    // Position
                    bw.Write(customObject.GetPosition().x);
                    bw.Write(customObject.GetPosition().y);
                    bw.Write(customObject.GetPosition().z);

                    // Rotation
                    bw.Write(customObject.GetRotation().x);
                    bw.Write(customObject.GetRotation().y);
                    bw.Write(customObject.GetRotation().z);

                    // Health
                    bw.Write(customObject.GetHealth());

                    // isinGrid
                    bw.Write(customObject.GetGrid());
                    
                    return s.ToArray(); // this will start conversion
                }
            }
        }


        /// <summary>
        /// Enemy AI Serialisation
        /// </summary>
        public byte[] SerializeCustomAI(object o)
        {
            EnemyAI customObject = o as EnemyAI; // casting
            if (customObject == null) { return null; } // if no obj

            // conversion to byte array
            byte[] store;
            using (var s = new MemoryStream())
            {
                using (var bw = new BinaryWriter(s))
                {
                    store = SerializeCustomObjects(customObject);

                    // pos of enemy
                    bw.Write(customObject.GetPrevPos().x);
                    bw.Write(customObject.GetPrevPos().y);
                    bw.Write(customObject.GetPrevPos().z);

                    // pos of nearest player 
                    bw.Write(customObject.GetNearestPos().x);
                    bw.Write(customObject.GetNearestPos().y);
                    bw.Write(customObject.GetNearestPos().z);

                    return ConCatArray(store, s.ToArray()); // this will start conversion
                }
            }
        }

        /// <summary>
        /// Adding 2 Arrays
        /// </summary>
        private byte[] ConCatArray(byte[] _array1, byte[] _array2)
        {
            if (_array1 == null || _array2 == null)
            {
                Console.WriteLine("array is empty");
                return null; // when empty arraies
            }

            int temp = _array1.Length + _array2.Length; // add both array tgt
            byte[] sum = new byte[temp]; // new byte array of temp number of slots
            _array1.CopyTo(sum, 0);
            _array2.CopyTo(sum, _array1.Length);
            return sum; // return new array
        }
    }
}
