using UnityEngine;
using System.Collections;

public abstract class QuestCondition : MonoBehaviour
{
    public enum DEACTIVE { ONCE, CONDITION, NONE, }
    public enum ACITIVE_NEXT { IMMEDIATLEY, ALL_EVENT_END }
    public bool preventCharacterCtrl;

    public QUEST.QUEST_TYPE m_mainQuest;
    public QUEST.QUEST_TYPE m_eventScript;
    public QUEST.QUEST_TYPE m_questTarget;
    public DEACTIVE m_deActive;

    public QuestCondition[] m_nextQuest;
    public ACITIVE_NEXT m_activeNextQuest;

    private int m_questNum;
    protected bool m_isActive;

    public abstract void startEvent();

    public void activeNextQuest()
    {
        for (int i = 0; i < m_nextQuest.Length; i++)
            QuestMgr.getInstance().setNextQuestTrigger(m_nextQuest[i].getQuestNum());
    }

    //setter
    public void setQuestNum(int qNum) { m_questNum = qNum; }
    public abstract void setActive(bool isActive);

    //getter
    public int getQuestNum() { return m_questNum; }
    public bool isActive() { return m_isActive; }
}
