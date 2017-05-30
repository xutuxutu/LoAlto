using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class TutorialGuideUICtrl : ObjectEvent
{
    public enum ACTIVE_TYPE { ACTIVE, DEACTIVE }
    public enum EVENT_TYPE { START, END }

    public EVENT_TYPE m_eventType;
    public ACTIVE_TYPE m_activeType;

    private TutorialGuideUI m_tutorialGuideUI;
    public GameObject m_tutorialQuestTarget;
    public string m_questString;

	// Use this for initialization
	void Start ()
    {
        init();
        m_tutorialGuideUI = GameObject.Find(OBJECT_NAME.TUTORIAL_GUIDE_UI).GetComponent<TutorialGuideUI>();
	}

    public override void startEvent()
    {
        if (isActive() == false)
            return;

        if (m_eventType == EVENT_TYPE.END)
            return;

        setEventState(ObjectState.EVENT_STATE.WORKING);
        Invoke("UI_ActiveCtrl", invokeTime);
    }

    public override void endEvent()
    {
        if (isActive() == false)
            return;

        if (m_eventType == EVENT_TYPE.START)
            return;

        setEventState(ObjectState.EVENT_STATE.WORKING);
        Invoke("UI_ActiveCtrl", invokeTime);
    }

    public void UI_ActiveCtrl()
    {
        switch(m_activeType)
        {
            case ACTIVE_TYPE.ACTIVE :
                m_tutorialGuideUI.setTargetObject(m_tutorialQuestTarget);
                m_tutorialGuideUI.setQuestString(m_questString);
                m_tutorialGuideUI.fadeInUI();
                break;
            case ACTIVE_TYPE.DEACTIVE :
                Debug.Log("DeActive");
                m_tutorialGuideUI.fadeOutUI();
                break;
        }
        setEventState(ObjectState.EVENT_STATE.READY);
    }
}
