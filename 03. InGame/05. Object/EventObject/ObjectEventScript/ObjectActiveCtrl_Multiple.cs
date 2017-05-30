using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class ObjectActiveCtrl_Multiple : ObjectEvent
{
    public enum ACTION_TYPE { ACTIVE, DEACTIVE, ACTIVE_DEACTIVE, DEACTIVE_ACTIVE };

    public ACTION_TYPE m_actionType;

    public GameObject[] m_object;
    private Text m_errorMessage;
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

        switch (m_actionType)
        {
            case ACTION_TYPE.ACTIVE:
            case ACTION_TYPE.ACTIVE_DEACTIVE :
                Invoke("activeObject", invokeTime);
                break;
            case ACTION_TYPE.DEACTIVE:
            case ACTION_TYPE.DEACTIVE_ACTIVE :
                Invoke("deActiveObject", invokeTime);
                break;
        }
    }

    public override void endEvent()
    {
        setEventState(ObjectState.EVENT_STATE.WORKING);

        switch(m_actionType)
        {
            case ACTION_TYPE.ACTIVE_DEACTIVE :
                Invoke("deActiveObject", invokeTime);
                break;
            case ACTION_TYPE.DEACTIVE_ACTIVE :
                Invoke("activeObject", invokeTime);
                break; 
        }
    }

    public void activeObject()
    {
        for (int i = 0; i < m_object.Length; ++i)
            m_object[i].SetActive(true);

        setEventState(ObjectState.EVENT_STATE.READY);
    }

    public void deActiveObject()
    {
        for (int i = 0; i < m_object.Length; ++i)
            m_object[i].SetActive(false);

        setEventState(ObjectState.EVENT_STATE.READY);
    }
}
