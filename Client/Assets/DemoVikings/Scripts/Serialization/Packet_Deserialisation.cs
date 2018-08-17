
using System.IO;
using UnityEngine;

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
    ///// <summary>
    ///// Player Deseialisation
    ///// </summary>
    //public object DeserializeCustomPlayer(byte[] bytes)
    //{
    //    Player customObject = new Player(); // create new obj

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
    /// Custom Object Type Deseialisation
    /// </summary>
    public object DeserializeCustomObjects(byte[] bytes)
    {
        GameObject customObject = new GameObject(); // create new obj
        customObject.AddComponent<CustomObjectBase>();

        // start to construct back to custom type ( byte packets )
        using (var s = new MemoryStream(bytes))
        {
            using (var br = new BinaryReader(s))
            {
                // Target Packet
                customObject.GetComponent<CustomObjectBase>().SetTargetPacket(br.ReadString());

                // Name
                customObject.GetComponent<CustomObjectBase>().SetName(br.ReadString());

                // Position
                customObject.GetComponent<CustomObjectBase>().SetPositionX(br.ReadSingle());
                customObject.GetComponent<CustomObjectBase>().SetPositionY(br.ReadSingle());
                customObject.GetComponent<CustomObjectBase>().SetPositionZ(br.ReadSingle());

                // Rotation
                customObject.GetComponent<CustomObjectBase>().SetRotationX(br.ReadSingle());
                customObject.GetComponent<CustomObjectBase>().SetRotationY(br.ReadSingle());
                customObject.GetComponent<CustomObjectBase>().SetRotationZ(br.ReadSingle());

                // HEalth
                customObject.GetComponent<CustomObjectBase>().SetHealth(br.ReadInt32());

                // isinGrid
                customObject.GetComponent<CustomObjectBase>().SetGrid(br.ReadBoolean());

            }
        }
        return customObject;
    }

    /// <summary>
    /// Enemy AI Deseialisation
    /// </summary>
    public object DeserializeCustomAI(byte[] bytes)
    {
        GameObject customObject = new GameObject(); // create new obj
        customObject.AddComponent<EnemyAI>(); // add the component

        // start to construct back to custom type ( byte packets )
        using (var s = new MemoryStream(bytes))
        {
            using (var br = new BinaryReader(s))
            {
                // Target Packet
                customObject.GetComponent<EnemyAI>().SetTargetPacket(br.ReadString());
                //Debug.Log("Target Packet : " + customObject.GetComponent<EnemyAI>().GetTargetPacket());

                // Name
                customObject.GetComponent<EnemyAI>().SetName(br.ReadString());
                //Debug.Log("Name : " + customObject.GetComponent<EnemyAI>().GetName());

                // Position
                customObject.GetComponent<EnemyAI>().SetPositionX(br.ReadSingle());
                customObject.GetComponent<EnemyAI>().SetPositionY(br.ReadSingle());
                customObject.GetComponent<EnemyAI>().SetPositionZ(br.ReadSingle());
                //Debug.Log("Position : " + customObject.GetComponent<EnemyAI>().GetPosition());

                // Rotation
                customObject.GetComponent<EnemyAI>().SetRotationX(br.ReadSingle());
                customObject.GetComponent<EnemyAI>().SetRotationY(br.ReadSingle());
                customObject.GetComponent<EnemyAI>().SetRotationZ(br.ReadSingle());
                //Debug.Log("Rotation : " + customObject.GetComponent<EnemyAI>().GetTargetPacket());

                // HEalth
                customObject.GetComponent<EnemyAI>().SetHealth(br.ReadInt32());
                //Debug.Log("Health : " + customObject.GetComponent<EnemyAI>().GetHealth());

                // isinGrid
                customObject.GetComponent<EnemyAI>().SetGrid(br.ReadBoolean());
                //Debug.Log("inGrid : " + customObject.GetComponent<EnemyAI>().GetGrid());

                // pos of enemy
                customObject.GetComponent<EnemyAI>().SetPrevPosX(br.ReadSingle());
                customObject.GetComponent<EnemyAI>().SetPrevPosY(br.ReadSingle());
                customObject.GetComponent<EnemyAI>().SetPrevPosZ(br.ReadSingle());
                //Debug.Log("Prev pos : " + customObject.GetComponent<EnemyAI>().GetPrevPos());

                // pos of nearest player
                customObject.GetComponent<EnemyAI>().SetNearestPosX(br.ReadSingle());
                customObject.GetComponent<EnemyAI>().SetNearestPosY(br.ReadSingle());
                customObject.GetComponent<EnemyAI>().SetNearestPosZ(br.ReadSingle());
                //Debug.Log("Nearest Player : " + customObject.GetComponent<EnemyAI>().GetNearestPos());
            }
        }
        return customObject;
    }
}

