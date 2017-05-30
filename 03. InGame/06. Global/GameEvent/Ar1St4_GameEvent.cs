using UnityEngine;
using System.Collections;
using System;

public class Ar1St4_GameEvent : GameEvent
{
    public Button m_eventObject;
    public GameObject[] m_directCamera;
    public QuestTrigger[] m_questTrigger;
    public ChangeScene m_objectEvent;
    public StartCondition m_startCondition;

    // Use this for initialization
    void Start()
    {
        init();
        m_objectEvent.setActive(false);
        m_startCondition.setActive(false);

        for (int i = 0; i < m_directCamera.Length; ++i)
        {
            //m_directCamera[i].GetComponent<Animator>().enabled = true;
            m_directCamera[i].GetComponent<AnimToObjectEvent>().init();
            m_directCamera[i].SetActive(false);
        }
    }

    public override void startEvent()
    {
        initAllEvent();
        setTriggerOption();
        ObjectMgr.getInstance().setActiveObjectPool();
        CreatureMgr.getInstance().AllCreatureClearFixedState();
    }

    public override void objectActive()
    {
#if SERVER_ON
        InGameServerMgr.getInstance().sendObjectActiveMessage(m_eventObject.m_EventObject.name, m_eventObject.getButtonID());
#else
        m_eventObject.startEvent();
#endif
        fadeInLoadingImage();
        Invoke("fadeIn", m_fadeStartTime);
        Invoke("fadeInVolume", m_fadeStartTime);
    }

    public void setTriggerOption()
    {
        switch(ProjectMgr.getInstance().getOwnCharacterType())
        {
            case CHARACTER.TYPE.SAM :
                m_questTrigger[1].GetComponent<ObjectActiveCtrl>().setActive(false);
                break;
            case CHARACTER.TYPE.SPARKY :
                m_questTrigger[0].GetComponent<ObjectActiveCtrl>().setActive(false);
                break;
        }
    }
}
