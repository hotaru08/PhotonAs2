using UnityEngine;
using System.Collections;

/* Enemy AI class -  to get info from Server */
public class EnemyAI : CustomObjectBase
{
    /// <summary>
    /// Previous Position of AI ( used to return back to last state change pos )
    /// </summary>
    [SerializeField]
    private Vector3 m_prevPos;
    public void SetPrevPos(Vector3 _prev) { m_prevPos = _prev; }
    public void SetPrevPosX(float _x) { m_prevPos.x = _x; }
    public void SetPrevPosY(float _y) { m_prevPos.y = _y; }
    public void SetPrevPosZ(float _z) { m_prevPos.z = _z; }
    public Vector3 GetPrevPos() { return m_prevPos; }

    /// <summary>
    /// Store the nearest Player position
    /// </summary>
    [SerializeField]
    private Vector3 m_nearestPlayerPos;
    public void SetNearestPos(Vector3 _nearest) { m_nearestPlayerPos = _nearest; }
    public void SetNearestPosX(float _x) { m_nearestPlayerPos.x = _x; }
    public void SetNearestPosY(float _y) { m_nearestPlayerPos.y = _y; }
    public void SetNearestPosZ(float _z) { m_nearestPlayerPos.z = _z; }
    public Vector3 GetNearestPos() { return m_nearestPlayerPos; }

    public EnemyAI()
    {
        // parent
        this.SetTargetPacket("ALL");
        this.SetName("");
        this.SetPosition(new Vector3());
        this.SetRotation(new Vector3());
        this.SetHealth(0);
        this.SetGrid(true);

        // child class variables
        m_prevPos = new Vector3();
        m_nearestPlayerPos = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue); // set nearest to be a huge number
       
    }
}
