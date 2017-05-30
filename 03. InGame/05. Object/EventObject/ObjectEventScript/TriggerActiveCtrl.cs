using UnityEngine;
using System.Collections;
using System;

public class TriggerActiveCtrl : ObjectEvent
{
    public enum ACTION_TYPE { ACTIVE, DEACTIVE, ACTIVE_DEACTIVE, }
    public enum EVENT_TYPE { CONTINUE, ONCE, }

    public EVENT_TYPE m_eventType;
    public ACTION_TYPE m_actionType;
    public QuestCondition m_targetTrigger;

    public void Start()
    {
        init();
    }

    public override void startEvent()
    {
        if (isActive())
        {
            setEventState(ObjectState.EVENT_STATE.WORKING);
            switch (m_actionType)
            {
                case ACTION_TYPE.ACTIVE:
                case ACTION_TYPE.ACTIVE_DEACTIVE :
                    if (m_targetTrigger.isActive() == false)
                        Invoke("activeObject", invokeTime);
                    break;
                case ACTION_TYPE.DEACTIVE:
                    if (m_targetTrigger.isActive() == true)
                        Invoke("deActiveObject", invokeTime);
                    break;
            }

            if (m_eventType == EVENT_TYPE.ONCE)
                setActive(false);

            Invoke("setEventStateReady", invokeTime);
        }
    }

    public override void endEvent()
    {
        if (isActive())
        {
            if (m_actionType == ACTION_TYPE.ACTIVE_DEACTIVE)
            {
                setEventState(ObjectState.EVENT_STATE.WORKING);
                Invoke("deActiveObject", invokeTime);
                Invoke("setEventStateReady", invokeTime);
            }
        }
    }

    public void activeObject()
    {
        m_targetTrigger.setActive(true);
    }

    public void deActiveObject()
    {
        m_targetTrigger.setActive(false);
    }

    public void setEventStateReady()
    {
        setEventState(ObjectState.EVENT_STATE.READY);
    }
}
