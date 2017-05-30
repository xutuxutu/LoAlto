using UnityEngine;
using System.Collections;

public class QuestTrigger : QuestCondition
{
    public enum TARGET_CHARACTER { ANY, SAM, SPARKY, ALL }
    private Collider[] m_curTrigger;
    private OBJECT_EVENTS m_eventObject;

    public TARGET_CHARACTER m_targetChararcter;
    private bool[] m_enterUser;
    public bool sendPacket;
    // Use this for initialization
    void Start()
    {
        m_enterUser = new bool[2];
        m_curTrigger = GetComponents<Collider>();
        m_eventObject.init(GetComponents<ObjectEvent>());

        for (int i = 0; i < m_curTrigger.Length; i++)
            m_curTrigger[i].isTrigger = true;

        setActive(false);
    }

    public void OnTriggerEnter(Collider coll)
    {
        if (isActive() == false)
            return;

        if (coll.gameObject.CompareTag(TAG.CHARACTER_OWN))
        {
            m_enterUser[(int)USER_INFO.TYPE.OWN] = true;
            switch (m_targetChararcter)
            {
                case TARGET_CHARACTER.ANY:
                    break;
                case TARGET_CHARACTER.SAM:
                    if (coll.gameObject.GetComponentInChildren<CharacterCtrl_Own>().getCharacterType() == CHARACTER.TYPE.SAM)
                        break;
                    else
                        return;
                case TARGET_CHARACTER.SPARKY:
                    if (coll.gameObject.GetComponentInChildren<CharacterCtrl_Own>().getCharacterType() == CHARACTER.TYPE.SPARKY)
                        break;
                    else
                        return;
                case TARGET_CHARACTER.ALL :
                    if (checkAllUserEnter() == true)
                        break;
                    else
                        return;
            }
            if (sendPacket == true)
            {
                InGameServerMgr.getInstance().SendPacket(NET_INGAME.SEND.PACKET_TYPE.REQUEST_TRIGGER_ACTIVE_EM, getQuestNum());
                Debug.Log("sendTriggerActive : " + getQuestNum());
            }
            startEvent();
        }
        else if (coll.gameObject.CompareTag(TAG.CHARACTER_OTHER))
        {
            m_enterUser[(int)USER_INFO.TYPE.OTHER] = true;
            switch (m_targetChararcter)
            {
                case TARGET_CHARACTER.ANY:
                    break;
                case TARGET_CHARACTER.SAM:
                    if (coll.gameObject.GetComponentInChildren<CharacterCtrl_Other>().getCharacterType() == CHARACTER.TYPE.SAM)
                        break;
                    else
                        return;
                case TARGET_CHARACTER.SPARKY:
                    if (coll.gameObject.GetComponentInChildren<CharacterCtrl_Other>().getCharacterType() == CHARACTER.TYPE.SPARKY)
                        break;
                    else
                        return;
                case TARGET_CHARACTER.ALL :
                    if (checkAllUserEnter() == true)
                        break;
                    else
                        return;
            }
            startEvent();
        }
    }

    public void OnTriggerExit(Collider coll)
    {
        if (isActive() == false)
            return;

        if (m_targetChararcter == TARGET_CHARACTER.ALL)
        {
            if (coll.gameObject.CompareTag(TAG.CHARACTER_OWN))
                m_enterUser[(int)USER_INFO.TYPE.OWN] = false;

            else if (coll.gameObject.CompareTag(TAG.CHARACTER_OTHER))
                m_enterUser[(int)USER_INFO.TYPE.OTHER] = false;
        }
    }

    public bool checkAllUserEnter()
    {
#if SERVER_ON
        for(int i = 0; i < m_enterUser.Length; ++i)
        {
            if (m_enterUser[i] == false)
                return false;
        }
#endif
        return true;
    }
    public override void setActive(bool isActive)
    {
        m_isActive = isActive;

        if (m_curTrigger != null)
        {
            for (int i = 0; i < m_curTrigger.Length; i++)
                m_curTrigger[i].enabled = isActive;
        }
    }
    
    public override void startEvent()
    {
        if (isActive() == false)
            return;

        m_eventObject.startEvent(OBJECT_EVENT.EVENT_START_TYPE.EARLY);

        if (m_deActive == QuestTrigger.DEACTIVE.ONCE)
            setActive(false);

        StartCoroutine("startEvent_Concide");
    }

    public void startTriggerEvent()
    {
        if(m_mainQuest == QUEST.QUEST_TYPE.MAIN)
            QuestMgr.getInstance().setNextQuest();

        if (m_eventScript == QUEST.QUEST_TYPE.MAIN)
            QuestMgr.getInstance().printDialogueUI(getQuestNum());

        if (m_questTarget == QUEST.QUEST_TYPE.MAIN)
            QuestMgr.getInstance().setNextTargetUI();

        if(m_activeNextQuest == ACITIVE_NEXT.IMMEDIATLEY)
            activeNextQuest();  
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
