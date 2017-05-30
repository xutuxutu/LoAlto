using UnityEngine;
using System.Collections;
using System;

public class CreatureStateCheck : StartCondition
{
    public QUEST.QUEST_TYPE m_mainQuest;
    public QUEST.QUEST_TYPE m_questTarget;
    public Creature[] m_conditionCreture;
    public CreatureMgr.CreatureState m_conditionState;

    private OBJECT_EVENTS m_eventObject;
    private int m_creatureNum;
	// Use this for initialization
	void Start ()
    {
        init();
        m_eventObject.init(GetComponents<ObjectEvent>());
        m_creatureNum = m_conditionCreture.Length;
        StartCoroutine("checkCondition");
	}

    public void startTriggerEvent()
    {
        if (m_mainQuest == QUEST.QUEST_TYPE.MAIN)
            QuestMgr.getInstance().setNextQuest();

        if (m_questTarget == QUEST.QUEST_TYPE.MAIN)
            QuestMgr.getInstance().setNextTargetUI();
    }

    public override void reset()
    {
        m_conditionCreture = null;
        m_creatureNum = 0;
    }

    public IEnumerator checkCondition()
    {
        bool isReady = false;
        while(getStartCondition() == false)
        {
            isReady = true;
            yield return null;
            for (int i = 0; i < m_creatureNum; i++)
            {
                if (m_conditionCreture[i].GetCreatureState() != m_conditionState)
                    isReady = false;
            }
            if(isReady == true)
                setReady(); 
        }
        m_eventObject.startEvent(OBJECT_EVENT.EVENT_START_TYPE.EARLY);
        StartCoroutine("startEvent_Concide");
    }

    public IEnumerator startEvent_Concide()
    {
        bool earlyEventEnd = false;
        while (earlyEventEnd == false)
        {
            if (m_eventObject.checkAllEventEnd(OBJECT_EVENT.EVENT_START_TYPE.EARLY))
            {
                earlyEventEnd = true;
                startTriggerEvent();
                m_eventObject.startEvent(OBJECT_EVENT.EVENT_START_TYPE.COINCIDE);
            }

            yield return null;
        }

        StartCoroutine("startEvent_Later");
    }

    public IEnumerator startEvent_Later()
    {
        bool coincideEventEnd = false;
        while (coincideEventEnd == false)
        {
            if (m_eventObject.checkAllEventEnd(OBJECT_EVENT.EVENT_START_TYPE.COINCIDE))
            {
                coincideEventEnd = true;
                m_eventObject.startEvent(OBJECT_EVENT.EVENT_START_TYPE.LATER);
            }

            yield return null;
        }
        StartCoroutine("endObjectEvent");
    }

    public IEnumerator endObjectEvent()
    {
        bool laterEventEnd = false;
        while (laterEventEnd == false)
        {
            if (m_eventObject.checkAllEventEnd(OBJECT_EVENT.EVENT_START_TYPE.LATER))
            {
                laterEventEnd = true;
                m_eventObject.endEvent(OBJECT_EVENT.EVENT_START_TYPE.EARLY);
                m_eventObject.endEvent(OBJECT_EVENT.EVENT_START_TYPE.COINCIDE);
                m_eventObject.endEvent(OBJECT_EVENT.EVENT_START_TYPE.LATER);
            }

            yield return null;
        }
    }
}
