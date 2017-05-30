using UnityEngine;
using System.Collections;
using System;

public class Ar1St3_GameEvent : GameEvent
{
    public Button m_eventObject;
    public GameObject[] m_directCamera;

    public Creature[] m_lastCreature;
    // Use this for initialization
    void Start()
    {
        init();

        for (int i = 0; i < m_directCamera.Length; ++i)
            m_directCamera[i].GetComponent<Animator>().enabled = false;
    }

    public override void startEvent()
    {
        initAllEvent();

        ObjectMgr.getInstance().setActiveObjectPool();

        for (int i = 0; i < m_directCamera.Length; ++i)
        {
            m_directCamera[i].GetComponent<Animator>().enabled = true;
            m_directCamera[i].GetComponent<AnimToObjectEvent>().init();
            m_directCamera[i].SetActive(false);
        }
    }

    public override void objectActive()
    {
#if SERVER_ON
        InGameServerMgr.getInstance().sendObjectActiveMessage(m_eventObject.m_EventObject.name, m_eventObject.getButtonID());
#else
        m_eventObject.startEvent();
#endif
        for (int i = 0; i < m_lastCreature.Length; ++i)
        {
            m_lastCreature[i].gameObject.SetActive(false);
            m_lastCreature[i].AttackActive(false);
        }

        fadeInLoadingImage();
        Invoke("fadeIn", m_fadeStartTime);
        Invoke("fadeInVolume", m_fadeStartTime);
    }
}
