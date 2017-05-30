using UnityEngine;
using System.Collections;
using System;

public class ChangeSceneActiveCtrl : ObjectEvent
{
    public enum ACTION_TYPE { ACTIVE, DEACTIVE }

    public ACTION_TYPE m_actionType; 
    public ChangeScene m_objectEvent;
	// Use this for initialization
	void Start ()
    {
        init();
	}

    public override void startEvent()
    {
        if (isActive() == false)
            return;

        setEventState(ObjectState.EVENT_STATE.WORKING);
        switch(m_actionType)
        {
            case ACTION_TYPE.ACTIVE :
                m_objectEvent.setActive(true);
                break;
            case ACTION_TYPE.DEACTIVE :
                m_objectEvent.setActive(false);
                break;
        }
        setEventState(ObjectState.EVENT_STATE.READY);
    }

    public override void endEvent()
    {
    }
}
