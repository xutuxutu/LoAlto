using UnityEngine;
using System.Collections.Generic;

namespace OBJECT_EVENT
{
    public enum EVENT_START_TYPE { EARLY, COINCIDE, LATER }
}

public struct OBJECT_EVENTS
{
    private List<ObjectEvent> m_eventObject_Early;
    private List<ObjectEvent> m_eventObject_Coincide;
    private List<ObjectEvent> m_eventObject_Later;

    public void init(ObjectEvent[] objectEvents)
    {
        m_eventObject_Early = new List<ObjectEvent>();
        m_eventObject_Coincide = new List<ObjectEvent>();
        m_eventObject_Later = new List<ObjectEvent>();

        for (int i = 0; i < objectEvents.Length; ++i)
        {
            switch(objectEvents[i].EventStartTime)
            {
                case OBJECT_EVENT.EVENT_START_TYPE.EARLY :
                    m_eventObject_Early.Add(objectEvents[i]);
                    break;
                case OBJECT_EVENT.EVENT_START_TYPE.COINCIDE :
                    m_eventObject_Coincide.Add(objectEvents[i]);
                    break;
                case OBJECT_EVENT.EVENT_START_TYPE.LATER :
                    m_eventObject_Later.Add(objectEvents[i]);
                    break;
            }
        }
    }

    public void startEvent(OBJECT_EVENT.EVENT_START_TYPE type)
    {
        switch (type)
        {
            case OBJECT_EVENT.EVENT_START_TYPE.EARLY :
                for (int i = 0; i < m_eventObject_Early.Count; ++i)
                    m_eventObject_Early[i].startEvent();
                break;
            case OBJECT_EVENT.EVENT_START_TYPE.COINCIDE :
                for (int i = 0; i < m_eventObject_Coincide.Count; ++i)
                    m_eventObject_Coincide[i].startEvent();
                break;
            case OBJECT_EVENT.EVENT_START_TYPE.LATER :
                for (int i = 0; i < m_eventObject_Later.Count; ++i)
                    m_eventObject_Later[i].startEvent();
                break;
        }
    }

    public void endEvent(OBJECT_EVENT.EVENT_START_TYPE type)
    {
        switch (type)
        {
            case OBJECT_EVENT.EVENT_START_TYPE.EARLY:
                for (int i = 0; i < m_eventObject_Early.Count; ++i)
                    m_eventObject_Early[i].endEvent();
                break;
            case OBJECT_EVENT.EVENT_START_TYPE.COINCIDE:
                for (int i = 0; i < m_eventObject_Coincide.Count; ++i)
                    m_eventObject_Coincide[i].endEvent();
                break;
            case OBJECT_EVENT.EVENT_START_TYPE.LATER:
                for (int i = 0; i < m_eventObject_Later.Count; ++i)
                    m_eventObject_Later[i].endEvent();
                break;
        }
    }

    public bool checkAllEventEnd(OBJECT_EVENT.EVENT_START_TYPE type)
    {
        switch (type)
        {
            case OBJECT_EVENT.EVENT_START_TYPE.EARLY:
                for (int i = 0; i < m_eventObject_Early.Count; ++i)
                {
                    if (m_eventObject_Early[i].getEventState() == ObjectState.EVENT_STATE.WORKING)
                        return false;
                }
                break;
            case OBJECT_EVENT.EVENT_START_TYPE.COINCIDE:
                for (int i = 0; i < m_eventObject_Coincide.Count; ++i)
                {
                    if (m_eventObject_Coincide[i].getEventState() == ObjectState.EVENT_STATE.WORKING)
                        return false;
                }
                break;
            case OBJECT_EVENT.EVENT_START_TYPE.LATER:
                for (int i = 0; i < m_eventObject_Later.Count; ++i)
                {
                    if (m_eventObject_Later[i].getEventState() == ObjectState.EVENT_STATE.WORKING)
                    {
                        Debug.Log(m_eventObject_Later[i]);
                        return false;
                    }
                }
                break;
        }

        return true;
    }
}

public abstract class ObjectEvent : MonoBehaviour
{
    public OBJECT_EVENT.EVENT_START_TYPE EventStartTime;
    public float invokeTime;
    private ObjectState.EVENT_STATE m_eventState;

    private bool m_isActive;

    public abstract void startEvent();
    public abstract void endEvent();

    public void init()
    {
        m_isActive = true;
        setEventState(ObjectState.EVENT_STATE.READY);
    }
    //setter
    public void setActive(bool isActive) { m_isActive = isActive; }
    public void setEventState(ObjectState.EVENT_STATE state) { m_eventState = state; }

    //getter
    public bool isActive() { return m_isActive; }
    public ObjectState.EVENT_STATE getEventState() { return m_eventState; }
}
