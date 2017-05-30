using UnityEngine;
using System.Collections;
using System;

public class QuestCreature : QuestCondition
{
    public Creature m_targetCreature;

	// Use this for initialization
	void Start ()
    {
        setActive(false);
	}

    public override void startEvent()
    {
        if (m_deActive == QuestTrigger.DEACTIVE.ONCE)
            setActive(false);

        if(m_mainQuest == QUEST.QUEST_TYPE.MAIN)
            QuestMgr.getInstance().setNextQuest();

        if (m_eventScript == QUEST.QUEST_TYPE.MAIN)
            QuestMgr.getInstance().printDialogueUI(getQuestNum());

        if (m_questTarget == QUEST.QUEST_TYPE.MAIN)
            QuestMgr.getInstance().setNextTargetUI();
    }

    public IEnumerator checkCreatureState()
    {
        bool checkState = true;

        while(checkState)
        {
            if (m_targetCreature.GetCreatureState() == CreatureMgr.CreatureState.DIE)
            {
                startEvent();
                checkState = false;
            }
            yield return null;
        }
    }

    public override void setActive(bool isActive)
    {
        m_isActive = isActive;

        if (isActive == true)
            StartCoroutine("checkCreatureState");
        else
            StopCoroutine("checkCreatureState");
    }
}
