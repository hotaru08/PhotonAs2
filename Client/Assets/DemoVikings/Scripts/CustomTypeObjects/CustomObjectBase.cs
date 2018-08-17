using UnityEngine;
using System.Collections;

/* Base class template for Objects that are custom types - shared variables */
public class CustomObjectBase : MonoBehaviour
{
    /// <summary>
    /// Target client that packets will be sent to
    /// </summary>
    [SerializeField]
    private string m_targetPacket;
    public void SetTargetPacket(string _target) { m_targetPacket = _target; }
    public string GetTargetPacket() { return m_targetPacket; }


    /// <summary>
    /// Name of the Custom Type GameObject
    /// </summary>
    [SerializeField]
    private string m_name;
    public void SetName(string _name) { m_name = _name; }
    public string GetName() { return m_name; }

    /// <summary>
    /// Position of the Custom Type GameObject
    /// </summary>
    [SerializeField]
    private Vector3 m_position;
    public void SetPosition(Vector3 _position) { m_position = _position; }
    public void SetPositionX(float _x) { m_position.x = _x; }
    public void SetPositionY(float _y) { m_position.y = _y; }
    public void SetPositionZ(float _z) { m_position.z = _z; }
    public Vector3 GetPosition() { return m_position; }

    /// <summary>
    /// Rotation of the Custom Type GameObject
    /// </summary>
    [SerializeField]
    private Vector3 m_rotation;
    public void SetRotation(Vector3 _rotation) { m_rotation = _rotation; }
    public void SetRotationX(float _x) { m_rotation.x = _x; }
    public void SetRotationY(float _y) { m_rotation.y = _y; }
    public void SetRotationZ(float _z) { m_rotation.z = _z; }
    public Vector3 GetRotation() { return m_rotation; }

    /// <summary>
    /// Health of the Custom Type GameObject
    /// </summary>
    [SerializeField]
    private int m_health;
    public void SetHealth(int _health) { m_health = _health; }
    public int GetHealth() { return m_health; }

    /// <summary>
    /// If Object is able to be seen or not
    /// </summary>
    [SerializeField]
    private bool m_isinGrid;
    public void SetGrid(bool _isin) { m_isinGrid = _isin; }
    public bool GetGrid() { return m_isinGrid; }

    /// <summary>
    /// Constructor ( intialise variables )
    /// </summary>
    public void InitCustomObjectBase()
    {
        m_targetPacket = "ALL";
        m_name = "";
        m_position = new Vector3();
        m_rotation = new Vector3();
        m_health = 0;
        m_isinGrid = true;
    }
}


