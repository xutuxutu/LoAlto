using UnityEngine;
using System.Collections;
using System;

public class EventObjectActiveCtrl : ObjectEvent
{
    public enum ACTION_TYPE { ACTIVE, DEACTIVE, }

    public ACTION_TYPE m_actionType;
    public EventObject m_eventObject;

    void Start()
    {
        init();
    }

    public override void startEvent()
    {
        switch (m_actionType)
        {
            case ACTION_TYPE.ACTIVE :
                m_eventObject.setActive(true);
                break;
            case ACTION_TYPE.DEACTIVE :
                m_eventObject.setActive(false);
                break;
        }
    }

    public override void endEvent()
    {
    }
}
