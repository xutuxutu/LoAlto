using UnityEngine;
using System.Collections;

namespace MOVING_OBJECT
{
    public enum TPYE { ACCEL, CONST }
}

public abstract class MovingObject : EventObject
{
    protected int m_wayPoint;
    public float m_moveSpeed;
    protected float m_curMoveSpeed;

    protected Vector3 m_moveVector;
    protected bool m_isArrive;

    private bool m_isOwner;

    protected delegate void MoveFuction();
    protected MoveFuction MoveTo;

    private bool m_isAuto;
    public new void init()
    {
        base.init();

        m_wayPoint = 0;
        m_isArrive = false;
        setActive(false);
        m_moveVector = Vector3.zero;
    }

    //setter
    public void setMoveVector(Vector3 moveVec, float speed)
    {
        m_moveVector = moveVec;
        m_curMoveSpeed = speed;
    }

    public void setOwner(bool isOwner)
    {
        m_isOwner = isOwner;
        if (m_isOwner == true)
            MoveTo = moveOwn;
        else
            MoveTo = moveOther;
    }

    public void arriveToPosition() { m_isArrive = true; }
    public void startMoving() { m_isArrive = false; }
    public void setIsAuto(bool isAuto) { m_isAuto = isAuto; }
    //getter
    public Vector3 getMoveVector()
    {
        if (m_isArrive == false)
            return m_moveVector * m_curMoveSpeed;
        else
            return Vector3.zero;
    }

    public int getState() { return m_wayPoint; }
    public bool getIsArrive() { return m_isArrive; }
    public bool isOwner() { return m_isOwner; }
    public bool isAuto() { return m_isAuto; }

    public abstract void moveOwn();
    public abstract void moveOther();
    public abstract void setState(int state);
}
