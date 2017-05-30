using UnityEngine;
using System.Collections;
using System;

public class AnimToObjectEvent : EventObject
{
    private OBJECT_EVENTS m_eventObject;

	// Use this for initialization

    public override void initChild()
    {
        m_eventObject.init(GetComponents<ObjectEvent>());
        setObjectStateScript(GetComponent<ObjectState>());
    }

    public override void startEvent()
    {
        setEventState(ObjectState.EVENT_STATE.WORKING);
        m_eventObject.startEvent(OBJECT_EVENT.EVENT_START_TYPE.EARLY);

        StartCoroutine("startEvent_Concide");
    }

    public override void endEvent()
    {
        setEventState(ObjectState.EVENT_STATE.READY);
    }


    public IEnumerator startEvent_Concide()
    {
        bool earlyEventEnd = false;
        while (earlyEventEnd == false)
        {
            if (m_eventObject.checkAllEventEnd(OBJECT_EVENT.EVENT_START_TYPE.EARLY))
            {
                earlyEventEnd = true;
                m_eventObject.startEvent(OBJECT_EVENT.EVENT_START_TYPE.COINCIDE);
            }

            yield return null;
        }

        StartCoroutine("startEvent_Later");
    }

    public IEnumerator startEvent_Later()
    {
        bool coincideEventEnd = false;
        while (coincideEventEnd == false)
        {
            if (m_eventObject.checkAllEventEnd(OBJECT_EVENT.EVENT_START_TYPE.COINCIDE))
            {
                if (getEventState() == ObjectState.EVENT_STATE.READY)
                {
                    coincideEventEnd = true;
                    m_eventObject.startEvent(OBJECT_EVENT.EVENT_START_TYPE.LATER);
                }
            }

            yield return null;
        }
        StartCoroutine("endObjectEvent");
    }

    public IEnumerator endObjectEvent()
    {
        bool laterEventEnd = false;
        while (laterEventEnd == false)
        {
            if (m_eventObject.checkAllEventEnd(OBJECT_EVENT.EVENT_START_TYPE.LATER))
            {
                laterEventEnd = true;
                m_eventObject.endEvent(OBJECT_EVENT.EVENT_START_TYPE.EARLY);
                m_eventObject.endEvent(OBJECT_EVENT.EVENT_START_TYPE.COINCIDE);
                m_eventObject.endEvent(OBJECT_EVENT.EVENT_START_TYPE.LATER);
            }

            yield return null;
        }
    }
}
