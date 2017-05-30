using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using System;

public class Ar1St2_GameEvent : GameEvent
{
    public Button m_eventObject;
    public ChangeScene m_objectEvent;
    public GameObject m_directCamera;
    public StartCondition m_startCondition;

    public Creature[] m_lastCreature;
    public void Start()
    {
        init();
        m_directCamera.SetActive(false);
    }

    public override void startEvent()
    {
        initAllEvent();

        m_startCondition.setActive(false);
        m_objectEvent.setActive(false);
        ObjectMgr.getInstance().setActiveObjectPool();
    }

    public override void objectActive()
    {
#if SERVER_ON
        InGameServerMgr.getInstance().sendObjectActiveMessage(m_eventObject.m_EventObject.name, m_eventObject.getButtonID());
#else
        m_eventObject.startEvent();
#endif

        for(int i = 0; i < m_lastCreature.Length; ++i)
        {
            m_lastCreature[i].AttackActive(false);
            m_lastCreature[i].gameObject.SetActive(false);
        }
        fadeInLoadingImage();
        Invoke("fadeIn", m_fadeStartTime);
        Invoke("fadeInVolume", m_fadeStartTime);
    }
}


