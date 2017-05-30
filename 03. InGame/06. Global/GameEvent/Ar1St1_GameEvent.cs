using UnityEngine;
using System.Collections;
using System;

public class Ar1St1_GameEvent : GameEvent
{
    public Button[] m_eventObject;
    public GameObject m_directionCamera;
    public Creature m_tutorialCreature;

    public GameObject m_creatrueStartPosition_Sam;
    public GameObject m_creatrueStartPosition_Sparky;

    public GameObject[] m_creatureWayPoint_Sam;
    public GameObject[] m_creatureWayPoint_Sparky;

    public GameObject m_lastDoorEventObject_Sam;
    public GameObject m_lastDoorEventObject_Sparky;
    public GameObject m_tutorialQuestTarget_LastDoor;

    public GameObject m_directionElevator;
    public CreatureStateCheck m_tutorialCreatureStateCheck;
    public Creature[] m_lastCreature;
    public QuestTrigger[] m_lastTrigger;
    // Use this for initialization
    void Start ()
    {
        init();
        InGameMgr.getInstance().getOwnCharacterCtrl().setCameraCtrlLock(true);
        InGameMgr.getInstance().getCharacterCamera().SetActive(false);
    }

    public override void startEvent()
    {
        initAllEvent();

        setTutorialOption();
        ObjectMgr.getInstance().setActiveObjectPool();
        m_directionElevator.SetActive(false);
    }

    public override void objectActive()
    {
#if SERVER_ON
        for (int i = 0; i < m_eventObject.Length; ++i)
            InGameServerMgr.getInstance().sendObjectActiveMessage(m_eventObject[i].m_EventObject.name, m_eventObject[i].getButtonID());
#else
        m_eventObject[0].startEvent();
        m_eventObject[1].startEvent();
        m_eventObject[2].startEvent();
#endif

        for (int i = 0; i < m_lastCreature.Length; ++i)
            m_lastCreature[i].gameObject.SetActive(false);

        fadeInLoadingImage();
        Invoke("fadeIn", m_fadeStartTime);
        Invoke("fadeInVolume", m_fadeStartTime);
        Invoke("activeDirectionCamera", 3.3f);
    }

    public void activeDirectionCamera()
    {
        switch (InGameMgr.getInstance().getOwnCharacterCtrl().getCharacterType())
        {
            case CHARACTER.TYPE.SAM:
                m_directionCamera.GetComponent<Animator>().SetTrigger("sam");
                break;
            case CHARACTER.TYPE.SPARKY:
                m_directionCamera.GetComponent<Animator>().SetTrigger("sparky");
                break;
        }
    }

    public void setTutorialOption()
    {
        QuestUI newTarget = null;
        m_tutorialCreature.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
        switch (InGameMgr.getInstance().getOwnCharacterCtrl().getCharacterType())
        {
            case CHARACTER.TYPE.SAM :
                m_tutorialCreature.transform.position = m_creatrueStartPosition_Sam.transform.position;
                m_tutorialCreature.m_wayPointArray = m_creatureWayPoint_Sam;
                m_lastDoorEventObject_Sam.transform.parent.GetComponentInChildren<Button>().m_startCondition = m_tutorialCreatureStateCheck;
                Debug.Log(m_lastDoorEventObject_Sam.transform.parent.GetComponentInChildren<Button>().m_startCondition);
                newTarget = m_lastDoorEventObject_Sam.GetComponent<QuestUI>();
                m_lastTrigger[0].GetComponent<ObjectActiveCtrl>().setActive(false);
                m_lastTrigger[0].GetComponent < TutorialGuideUICtrl>().setActive(false);
                m_tutorialQuestTarget_LastDoor.transform.parent = m_lastDoorEventObject_Sam.transform;
                m_lastDoorEventObject_Sparky.GetComponentInChildren<TutorialGuideUICtrl>().setActive(false);
                break;
            case CHARACTER.TYPE.SPARKY :
                m_tutorialCreature.transform.position = m_creatrueStartPosition_Sparky.transform.position;
                m_tutorialCreature.m_wayPointArray = m_creatureWayPoint_Sparky;
                m_lastDoorEventObject_Sparky.transform.parent.GetComponentInChildren<Button>().m_startCondition = m_tutorialCreatureStateCheck;
                newTarget = m_lastDoorEventObject_Sparky.GetComponent<QuestUI>();
                m_lastTrigger[1].GetComponent<ObjectActiveCtrl>().setActive(false);
                m_lastTrigger[1].GetComponent<TutorialGuideUICtrl>().setActive(false);
                m_tutorialQuestTarget_LastDoor.transform.parent = m_lastDoorEventObject_Sparky.transform;
                m_lastDoorEventObject_Sam.GetComponentInChildren<TutorialGuideUICtrl>().setActive(false);
                break;
        }
        m_tutorialQuestTarget_LastDoor.transform.localPosition = new Vector3(0, 0, 0.4f);

        QuestMgr.getInstance().addQuestTargetObject(newTarget, 3);
        newTarget.m_questTarget = QUEST.QUEST_TYPE.MAIN;

        Invoke("PatrolActive", 0.5f);
    }
    public void PatrolActive()
    {
        ((Creature_Tutorial_Drone)m_tutorialCreature).b_patrolActive = true;
    }
}