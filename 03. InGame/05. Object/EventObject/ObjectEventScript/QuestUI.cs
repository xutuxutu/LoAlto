using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class QuestUI : ObjectEvent
{
    public QUEST.QUEST_TYPE m_mainQuest;
    public QUEST.QUEST_TYPE m_questTarget;
    public Vector3 m_uiPosition;

    private int m_questNum;
    public void Start()
    {
        init();
    }

    public override void startEvent()
    {
        if (isActive() == false)
            return;

        setActive(false);
        setEventState(ObjectState.EVENT_STATE.WORKING);
        Invoke("setEvent", invokeTime);
    }

    public override void endEvent()
    {
        setActive(false);
    }

    public void setEvent()
    {
        if (m_mainQuest == QUEST.QUEST_TYPE.MAIN)
            QuestMgr.getInstance().setNextQuest();

        if (m_questTarget == QUEST.QUEST_TYPE.MAIN)
            QuestMgr.getInstance().setNextTargetUI();

        setEventState(ObjectState.EVENT_STATE.READY);
    }
}
