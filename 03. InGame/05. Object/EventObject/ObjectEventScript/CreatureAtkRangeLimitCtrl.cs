using UnityEngine;
using System.Collections;
using System;

public class CreatureAtkRangeLimitCtrl : ObjectEvent
{
    public enum LIMIT { LOCK, UNLOCK }
    public enum TARGET { ALL, SELECT }
    public LIMIT m_limit;
    public TARGET m_target;
    public Creature[] m_targetCreature;

	// Use this for initialization
	void Start ()
    {
        init();
	}

    public override void startEvent()
    {
        if (isActive() == false)
            return;

        switch(m_target)
        {
            case TARGET.ALL :
                Invoke("setLimitAll", invokeTime);
                break;
            case TARGET.SELECT:
                Invoke("setLimitTarget", invokeTime);
                break;
        }
    }

    public override void endEvent()
    {
    }

    public void setLimitTarget()
    {
        switch(m_limit)
        {
            case LIMIT.LOCK :
                for (int i = 0; i < m_targetCreature.Length; ++i)
                    m_targetCreature[i].TakeImmediateAction(false);
                break;
            case LIMIT.UNLOCK :
                for (int i = 0; i < m_targetCreature.Length; ++i)
                    m_targetCreature[i].TakeImmediateAction(true);
                break;
        }
    }

    public void setLimitAll()
    {
        Debug.Log("공격 거리 제한 해제 : " + gameObject);
        switch (m_limit)
        {
            case LIMIT.LOCK:
                CreatureMgr.getInstance().AllCreatureTakeImmediateAction(false);
                break;
            case LIMIT.UNLOCK:
                CreatureMgr.getInstance().AllCreatureTakeImmediateAction(true);
                break;
        }
    }
}
