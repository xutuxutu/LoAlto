using UnityEngine;
using System.Collections;
using System;

public class QuestItem : EventObject
{
    private OBJECT_EVENTS m_eventObject;
    public GameObject m_sparkyPosition;
    public GameObject m_samPosition;

    // Use this for initialization

    void Start ()
    {
        init();

        m_eventObject.init(GetComponentsInChildren<ObjectEvent>());
        setEventState(ObjectState.EVENT_STATE.READY);

        GameObject questItem = null;

        switch(InGameMgr.getInstance().getOwnCharacterCtrl().getCharacterType())
        {
            case CHARACTER.TYPE.SAM :
                transform.position = m_samPosition.transform.position;
                questItem = Resources.Load(PREFAB_PATH.QUEST_ITEM_SAM_STEAMBALL) as GameObject;
                break;
            case CHARACTER.TYPE.SPARKY :
                transform.position = m_sparkyPosition.transform.position;
                questItem = Resources.Load(PREFAB_PATH.QUEST_ITEM_SPARKY_CARTRIDGE) as GameObject;
                break;
        }

        if (questItem != null)
        {
            questItem = GameObject.Instantiate(questItem);
            questItem.transform.parent = transform;
            questItem.transform.localPosition = Vector3.zero;
        }
        else
            Debug.Log("Target Not exist : QuestItem");
    }

    public override void initChild()
    {
        ObjectState eventState = GetComponent<ObjectState>();
        setObjectStateScript(eventState);

        for (int i = 0; i < m_button.Length; i++)
            m_button[i].GetComponent<Button>().init(eventState, i + 1);
    }

    public new void setActive(bool isActive)
    {
        base.setActive(isActive);

        for (int i = 0; i < m_button.Length; i++)
            m_button[i].setActive(isActive);
    }

    public override void startEvent()
    {
        m_eventObject.startEvent(OBJECT_EVENT.EVENT_START_TYPE.EARLY);

        StartCoroutine("startEvent_Concide");
    }

    public override void endEvent()
    {
        for (int i = 0; i < m_button.Length; ++i)
            m_button[i].setActive(false);

        gameObject.SetActive(false);
    }

    public IEnumerator startEvent_Concide()
    {
        bool earlyEventEnd = false;
        while (earlyEventEnd == false)
        {
            if (m_eventObject.checkAllEventEnd(OBJECT_EVENT.EVENT_START_TYPE.EARLY))
            {
                earlyEventEnd = true;
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
                endEvent();
                m_eventObject.endEvent(OBJECT_EVENT.EVENT_START_TYPE.EARLY);
                m_eventObject.endEvent(OBJECT_EVENT.EVENT_START_TYPE.COINCIDE);
                m_eventObject.endEvent(OBJECT_EVENT.EVENT_START_TYPE.LATER);
            }

            yield return null;
        }
    }
}
