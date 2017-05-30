using UnityEngine;
using System.Collections;
using System;

public class TriggerObject : EventObject
{
    public enum ACTIVE_TYPE { ONCE, CONTINUE, }

    public ACTIVE_TYPE m_activeType;
    private OBJECT_EVENTS m_eventObject;

    public void Start()
    {
        init();
        setActive(true);
    }

    public override void initChild()
    {
        m_eventObject.init(GetComponents<ObjectEvent>());

        ObjectState objectState = GetComponent<ObjectState>();
        setObjectStateScript(objectState);

        for (int i = 0; i < m_button.Length; i++)
            m_button[i].init(objectState, i);
    }

    public override void startEvent()
    {
        if (getEventState() == ObjectState.EVENT_STATE.WORKING)
            return;

        setEventState(ObjectState.EVENT_STATE.WORKING);
        m_eventObject.startEvent(OBJECT_EVENT.EVENT_START_TYPE.EARLY);

        StartCoroutine("startEvent_Concide");
    }

    public override void endEvent()
    {
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
        setEventState(ObjectState.EVENT_STATE.READY);

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

    public void OnTriggerEnter(Collider coll)
    {
        if (isActive() == false)
            return;

        if(coll.CompareTag(TAG.CHARACTER_OWN) || coll.CompareTag(TAG.CHARACTER_OTHER))
        {
            startEvent();
            if (m_activeType == ACTIVE_TYPE.ONCE)
                setActive(false);
        }
    }
}
