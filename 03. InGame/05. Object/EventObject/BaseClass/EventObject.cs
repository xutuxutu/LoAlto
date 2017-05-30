using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ObjectState))]

public abstract class EventObject : MonoBehaviour
{
    private bool m_isActive = false;
    private ObjectState m_eventState;
    public Button[] m_button;

    public void init()
    {
        initChild();
        setEventState(ObjectState.EVENT_STATE.READY);
    }

    public abstract void initChild();

    //setter
    public void setObjectStateScript(ObjectState objectState) { m_eventState = objectState; }
    public void setActive(bool isActive)
    {
        if (isActive == true)
            setActive();
        else
            setDeActive();
    }
    public void setActive()
    {
        m_isActive = true;

        for (int i = 0; i < m_button.Length; i++)
            m_button[i].setActive(m_isActive);
    }
    public void setDeActive()
    {
        m_isActive = false;

        for (int i = 0; i < m_button.Length; i++)
            m_button[i].setActive(m_isActive);
    }

    public void setEventState(ObjectState.EVENT_STATE eventState) { m_eventState.setEventState(eventState); }

    //getter
    public ObjectState.EVENT_STATE getEventState() { return m_eventState.getEventState(); }
    public bool isActive() { return m_isActive; }
    public int getButtonNum() { return m_button.Length; }
    public Button[] getButton() { return m_button; }
    public Button getButton(int id)
    {
        for (int i = 0; i < m_button.Length; i++)
        {
            if (m_button[i].getButtonID() == id)
                return m_button[i];
        }
        return null;
    }

    //가상 함수
    public abstract void startEvent();
    public abstract void endEvent();
}
