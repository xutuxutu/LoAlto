using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class FadeInNOut : ObjectEvent
{
    public enum CAMERA { CHARACTER, SELECT, }
    public enum FADE_TYPE { F_IN, F_OUT, F_IN_OUT, F_OUT_IN}

    public CAMERA m_targetCamera;
    public FADE_TYPE m_fadeType;

    public float m_eventStartTime;
    public float m_fadeInTime;
    public float m_fadeOutTime;

    private Camera m_camera;

    private Image m_blackScreen;
    // Use this for initialization
    void Start ()
    {
        init();
        if (m_targetCamera == CAMERA.CHARACTER)
            m_camera = InGameMgr.getInstance().getCharacterCamera().GetComponent<Camera>();

        m_blackScreen = GameObject.Find(OBJECT_NAME.BLACK_SCREEN).GetComponent<Image>();
    }

    public override void startEvent()
    {
        setEventState(ObjectState.EVENT_STATE.WORKING);
        switch (m_fadeType)
        {
            case FADE_TYPE.F_IN :
            case FADE_TYPE.F_IN_OUT :
                fadeIn();
                break;
            case FADE_TYPE.F_OUT :
            case FADE_TYPE.F_OUT_IN:
                fadeOut();
                break; 
        }
    }

    public override void endEvent()
    {
        setEventState(ObjectState.EVENT_STATE.WORKING);
        switch (m_fadeType)
        {
            case FADE_TYPE.F_IN_OUT :
                fadeOut();
                break;
            case FADE_TYPE.F_OUT_IN :
                fadeIn();
                break;
        }
    }

    public void fadeIn() //밝아짐
    {
        m_blackScreen.CrossFadeAlpha(0.0f, m_fadeInTime, false);
        Invoke("setEventStateReady", m_fadeInTime);
    }

    public void fadeOut()   //어두워짐
    {
        m_blackScreen.CrossFadeAlpha(1.0f, m_fadeOutTime, false);
        Invoke("setEventStateReady", m_fadeOutTime);
    }

    public void setEventStateReady() { setEventState(ObjectState.EVENT_STATE.READY); }
}
