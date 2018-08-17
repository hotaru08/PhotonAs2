using System.IO;
using UnityEngine;

/* This class handles data synchronisation - serialise custom types */
/* Transform custom type text packets to byte array to send to server */
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
    ///// <summary>
    ///// Player Serialisation
    ///// </summary>
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
    public byte[] SerializeCustomObjects(GameObject o)
    {
        GameObject customObject = o as GameObject; // casting
        if (customObject == null)
            return null;

        // conversion to byte array
        using (var s = new MemoryStream())
        {
            using (var bw = new BinaryWriter(s))
            {
                // Target Packet
                bw.Write(customObject.GetComponent<CustomObjectBase>().GetTargetPacket());
                //Debug.Log("Target Packet : " + customObject.GetComponent<CustomObjectBase>().GetTargetPacket());

                // Name
                bw.Write(customObject.GetComponent<CustomObjectBase>().GetName());
                //Debug.Log("Name : " + customObject.GetComponent<CustomObjectBase>().GetName());

                // Position
                bw.Write(customObject.GetComponent<CustomObjectBase>().GetPosition().x);
                bw.Write(customObject.GetComponent<CustomObjectBase>().GetPosition().y);
                bw.Write(customObject.GetComponent<CustomObjectBase>().GetPosition().z);
                //Debug.Log("Position : " + customObject.GetComponent<CustomObjectBase>().GetPosition());

                // Rotation
                bw.Write(customObject.GetComponent<CustomObjectBase>().GetRotation().x);
                bw.Write(customObject.GetComponent<CustomObjectBase>().GetRotation().y);
                bw.Write(customObject.GetComponent<CustomObjectBase>().GetRotation().z);
                //Debug.Log("Rotation : " + customObject.GetComponent<CustomObjectBase>().GetRotation());

                // Health
                bw.Write(customObject.GetComponent<CustomObjectBase>().GetHealth());
                //Debug.Log("Health : " + customObject.GetComponent<CustomObjectBase>().GetHealth());

                // isinGrid
                bw.Write(customObject.GetComponent<CustomObjectBase>().GetGrid());
                //Debug.Log("IsInGrid : " + customObject.GetComponent<CustomObjectBase>().GetGrid());

                return s.ToArray(); // this will start conversion
            }
        }
    }

    /// <summary>
    /// Enemy AI Serialisation
    /// </summary>
    public byte[] SerializeCustomAI(object o)
    {
        GameObject customObject = o as GameObject; // casting
        if (customObject == null) { return null; } // if no obj

        // conversion to byte array
        byte[] store;
        using (var s = new MemoryStream())
        {
            using (var bw = new BinaryWriter(s))
            {
                store = SerializeCustomObjects(customObject);

                // pos of enemy
                bw.Write(customObject.GetComponent<EnemyAI>().GetPrevPos().x);
                bw.Write(customObject.GetComponent<EnemyAI>().GetPrevPos().y);
                bw.Write(customObject.GetComponent<EnemyAI>().GetPrevPos().z);

                // pos of nearest player 
                bw.Write(customObject.GetComponent<EnemyAI>().GetNearestPos().x);
                bw.Write(customObject.GetComponent<EnemyAI>().GetNearestPos().y);
                bw.Write(customObject.GetComponent<EnemyAI>().GetNearestPos().z);

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
            Debug.Log("Array is empty : " + _array1 + " / " + _array2);
            return null; // when empty arraies
        }

        int temp = _array1.Length + _array2.Length; // add both array tgt
        byte[] sum = new byte[temp]; // new byte array of temp number of slots
        _array1.CopyTo(sum, 0);
        _array2.CopyTo(sum, _array1.Length);
        return sum; // return new array
    }
}

