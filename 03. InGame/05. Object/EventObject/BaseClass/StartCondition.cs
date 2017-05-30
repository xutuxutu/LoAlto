using UnityEngine;
using System.Collections;

public abstract class StartCondition : MonoBehaviour
{
    private bool m_isActive;
    private bool m_startCondition = false;

    public void init()
    {
        m_isActive = true;
    }

    //setter
    public void setReady(bool isReady) { m_startCondition = isReady; }
    public void setReady() { m_startCondition = true; }
    public void setNotReady() { m_startCondition = false; }

    public void setActive(bool isActive)
    {
        if (isActive == true)
            setActive();
        else
            deActive();
    }
    public void setActive() { m_isActive = true; }
    public void deActive()
    {
        m_isActive = false;
        reset();
    }

    //getter
    public bool isActive() { return m_isActive; }
    public bool isReady() { return m_startCondition; }
    protected bool getStartCondition() { return m_startCondition; }

    //가상함수
    public abstract void reset();
}
