using UnityEngine;
using System.Collections;

public abstract class RotatingObject : EventObject
{
    public enum ROT_DIRECTION { LEFT, RIGHT };
    public ROT_DIRECTION m_rotDirection;

    public int[] m_stopAngle;
    public float m_rotSpeed;

    private Vector3 m_rotVector;

    private int m_maxRotCycle;
    private int m_curRotCycle;

    public new void init()
    {
        base.init();

        m_maxRotCycle = m_stopAngle.Length;
        m_curRotCycle = 0;
    }

    public void rotationObject()
    {
        m_curRotCycle += 1;
        if (m_curRotCycle >= m_maxRotCycle)
            m_curRotCycle = 0;

        StartCoroutine(rotate());
    }

    public Vector3 getRotVector() { return m_rotVector * m_rotSpeed; }

    public abstract bool checkRotAngle();
    public abstract IEnumerator rotate();

    public int getCurRotCycle() { return m_curRotCycle; }
    public int getMaxRotCycle() { return m_maxRotCycle; }
}
