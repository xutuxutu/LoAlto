using UnityEngine;
using System.Collections;
using System;

public class CreatureStateCtrl : ObjectEvent
{
    public enum ACTION_TYPE { START, END, START_END }
    public enum STATE_CTRL { FIXED, CLEAR }

    public ACTION_TYPE m_actionType;
    public STATE_CTRL m_stateCtrl;

	// Use this for initialization
	void Start ()
    {
        init();
	}
	

    public override void startEvent()
    {
        if (m_actionType == ACTION_TYPE.END)
            return;
        setEventState(ObjectState.EVENT_STATE.WORKING);

        Invoke("setCreatureState", invokeTime);
    }

    public override void endEvent()
    {
        if (m_actionType == ACTION_TYPE.START)
            return;

        setEventState(ObjectState.EVENT_STATE.WORKING);

        Invoke("setCreatureState", invokeTime);
    }

    public void setCreatureState()
    {
        switch (m_stateCtrl)
        {
            case STATE_CTRL.FIXED:
                CreatureMgr.getInstance().AllCreatureFixedIdleState();
                break;
            case STATE_CTRL.CLEAR:
                CreatureMgr.getInstance().AllCreatureClearFixedState();
                break;
        }

        setEventState(ObjectState.EVENT_STATE.READY);
    }
}
