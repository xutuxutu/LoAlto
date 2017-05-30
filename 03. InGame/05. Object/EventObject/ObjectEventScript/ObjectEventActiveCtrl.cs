using UnityEngine;
using System.Collections;
using System;

public class ObjectEventActiveCtrl : ObjectEvent
{
    public enum EVENT_TYPE { ACTIVE, DEACTIVE }
    public enum START_TYPE { START, END }

    public START_TYPE m_startType;
    public EVENT_TYPE m_eventType;
    public ObjectEvent[] m_targetObjectEvent;

	// Use this for initialization
	void Start ()
    {
        init();	
	}
    public override void startEvent()
    {
        if (isActive() == false)
            return;
        if(m_startType == START_TYPE.START)
            Invoke("setActiveObjectEvent", invokeTime);
    }
    public override void endEvent()
    {
        if (isActive() == false)
            return;
        if(m_startType == START_TYPE.END)
            Invoke("setActiveObjectEvent", invokeTime);
    }

    public void setActiveObjectEvent()
    {
        bool isActive = false;
        if (m_eventType == EVENT_TYPE.ACTIVE)
            isActive = true;

        for (int i = 0; i < m_targetObjectEvent.Length; ++i)
            m_targetObjectEvent[i].setActive(isActive);
    }
}
