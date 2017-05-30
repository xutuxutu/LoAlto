using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class ChangeCamera : ObjectEvent
{
    public enum USE_MAIN_CAMERA { NONE, PREVIOUS, NEXT }

    public USE_MAIN_CAMERA m_useMainCamera;

    public GameObject m_previousCamera;
    public GameObject m_nextCamera;
    public float m_fadeInTime;
    public float m_fadeOutTime;

    private Image m_blackScreen;
	// Use this for initialization
	void Start ()
    {
        init();

        if (m_useMainCamera == USE_MAIN_CAMERA.PREVIOUS)
            m_previousCamera = GameObject.Find(OBJECT_NAME.CAMERA);
        else if(m_useMainCamera == USE_MAIN_CAMERA.NEXT)
            m_nextCamera = GameObject.Find(OBJECT_NAME.CAMERA);

        m_blackScreen = GameObject.Find(OBJECT_NAME.BLACK_SCREEN).GetComponent<Image>();
    }

    public override void startEvent()
    {
        if (isActive() == false)
            return;

        Invoke("eventStart", invokeTime);
    }

    public void eventStart()
    {
        setEventState(ObjectState.EVENT_STATE.WORKING);
        fadeOut();
    }

    public override void endEvent()
    {
    }

    public void fadeIn() //밝아짐
    {
        m_blackScreen.CrossFadeAlpha(0.0f, m_fadeInTime, false);
        Invoke("setEventStateReady", m_fadeInTime);
    }

    public void fadeOut()
    {
        m_blackScreen.CrossFadeAlpha(1.0f, m_fadeOutTime, false);
        Invoke("setNextCamera", m_fadeOutTime);
    }

    public void setNextCamera()
    {
        if (InGameMgr.getInstance().getOwnCharacterCtrl().isDie() == true)
        {
            switch(m_useMainCamera)
            {
                case USE_MAIN_CAMERA.NONE :
                    m_previousCamera.SetActive(false);
                    m_nextCamera.SetActive(true);
                    break;
                case USE_MAIN_CAMERA.PREVIOUS :
                    m_nextCamera.SetActive(true);
                    InGameMgr.getInstance().getOwnCharacterCtrl().deActiveDeathUI();
                    break;
                case USE_MAIN_CAMERA.NEXT :
                    m_previousCamera.SetActive(false);
                    InGameMgr.getInstance().getOwnCharacterCtrl().activeDeathUI();
                    break;
            }
        }
        else
        {
            m_previousCamera.SetActive(false);
            m_nextCamera.SetActive(true);
        }

        fadeIn();
    }

    public void setEventStateReady() { setEventState(ObjectState.EVENT_STATE.READY); }
}
