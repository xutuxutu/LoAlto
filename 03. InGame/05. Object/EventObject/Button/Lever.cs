using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Lever : Button
{
    public bool m_preventCharacterCtrl;
    private Animator m_leverAnimator;

    // Use this for initialization
    void Start()
    {
        init();
        initChild();

        setButtonState(BUTTON.STATE.OFF);
        setButtonType();

        m_leverAnimator = GetComponentInChildren<Animator>();
        setButtonState(BUTTON.STATE.OFF);
        m_animationObject = true;
    }

    public override void startEvent()
    {
        if (isActive() == false)
            return;

        if (checkStartCondition() == false)
            return;

        if (m_parentEventState.getEventState() == ObjectState.EVENT_STATE.READY)
        {
            if (getEventState() == ObjectState.EVENT_STATE.READY)
            {
                setEventState(ObjectState.EVENT_STATE.WORKING);
                m_leverAnimator.SetBool("operator", true);
                printSoundEffect();
                if (m_preventCharacterCtrl == true)
                    InGameMgr.getInstance().getOwnCharacterCtrl().setIsActive(false);
            }
        }
    }

    public override void endEvent()
    {
        if (isActive() == false)
            return;

        if (checkStartCondition() == false)
            return;

        if (m_parentEventState.getEventState() == ObjectState.EVENT_STATE.READY)
        {
            if (getEventState() == ObjectState.EVENT_STATE.READY)
            {
                setEventState(ObjectState.EVENT_STATE.WORKING);
                m_leverAnimator.SetBool("operator", true);
                printSoundEffect();
                if (m_preventCharacterCtrl == true)
                    InGameMgr.getInstance().getOwnCharacterCtrl().setIsActive(false);
            }
        }
    }

    public void parentsEventStart()
    {
        switch(getButtonState())
        {
            case BUTTON.STATE.ON :
                base.endEvent();
                break;
            case BUTTON.STATE.OFF :
                base.startEvent();
                break;
        }
        setEventState(ObjectState.EVENT_STATE.READY);
    }
}
