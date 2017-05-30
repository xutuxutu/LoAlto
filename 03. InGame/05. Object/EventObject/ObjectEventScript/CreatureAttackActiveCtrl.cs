using UnityEngine;
using System.Collections;

public class CreatureAttackActiveCtrl : ObjectEvent
{
    public enum ACTION_TYPE { START, END, }
    public enum STATE_CTRL { FIXED, CLEAR }

    public ACTION_TYPE m_actionType;
    public STATE_CTRL m_stateCtrl;

    public Creature[] m_targetCreature;

    // Use this for initialization
    void Start()
    {
        init();
    }


    public override void startEvent()
    {
        if (isActive() == false)
            return;

        if (m_actionType == ACTION_TYPE.END)
            return;
        setEventState(ObjectState.EVENT_STATE.WORKING);

        Invoke("setCreatureState", invokeTime);
    }

    public override void endEvent()
    {
        if (isActive() == false)
            return;

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
                for (int i = 0; i < m_targetCreature.Length; ++i)
                { 
                    m_targetCreature[i].AttackActive(false);
                }
                break;
            case STATE_CTRL.CLEAR:
                for (int i = 0; i < m_targetCreature.Length; ++i)
                    m_targetCreature[i].AttackActive(true);
                Debug.Log("ActiveAttack : " + m_targetCreature[0]);
                break;
        }

        setEventState(ObjectState.EVENT_STATE.READY);
    }
}
