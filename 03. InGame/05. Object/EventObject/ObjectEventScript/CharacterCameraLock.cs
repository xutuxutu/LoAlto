using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class CharacterCameraLock : ObjectEvent
{
    public enum ACTION_TYPE { ACTIVE, DEACTIVE, ACTIVE_DEACTIVE, DEACTIVE_ACTIVE };
    public enum COMPULSORY { NONE, FORCE }

    public ACTION_TYPE m_actionType;
    public COMPULSORY m_compulsory;
    public float m_angle;
	// Use this for initialization
	void Start ()
    {
        init();
    }

    public override void startEvent()
    {
        setEventState(ObjectState.EVENT_STATE.WORKING);

        switch (m_actionType)
        {
            case ACTION_TYPE.ACTIVE:
            case ACTION_TYPE.ACTIVE_DEACTIVE :
                Invoke("activeCtrl", invokeTime);
                break;
            case ACTION_TYPE.DEACTIVE:
            case ACTION_TYPE.DEACTIVE_ACTIVE :
                Invoke("deActiveCtrl", invokeTime);
                break;
        }

        if(m_compulsory == COMPULSORY.FORCE)
        {
            InGameMgr.getInstance().getOwnCharacterCtrl().transform.parent.rotation = Quaternion.Euler(0, m_angle, 0);
        }
        setEventState(ObjectState.EVENT_STATE.READY);
    }

    public override void endEvent()
    {
        setEventState(ObjectState.EVENT_STATE.WORKING);

        switch(m_actionType)
        {
            case ACTION_TYPE.ACTIVE_DEACTIVE :
                Invoke("deActiveCtrl", invokeTime);
                break;
            case ACTION_TYPE.DEACTIVE_ACTIVE :
                Invoke("activeCtrl", invokeTime);
                break;
        }

        setEventState(ObjectState.EVENT_STATE.READY);
    }

    public void activeCtrl()
    {
        InGameMgr.getInstance().getOwnCharacterCtrl().setCameraCtrlLock(false);
        InGameMgr.getInstance().getOwnCharacterCtrl().setDynamicCameraLock(false);
    }

    public void deActiveCtrl()
    {
        InGameMgr.getInstance().getOwnCharacterCtrl().setCameraCtrlLock(true);
        InGameMgr.getInstance().getOwnCharacterCtrl().setDynamicCameraLock(true);
    }
}
