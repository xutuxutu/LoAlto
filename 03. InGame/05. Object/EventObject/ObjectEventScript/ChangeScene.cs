using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class ChangeScene : ObjectEvent
{
    public float m_startTime;
    public float m_fadeOutTime;

    private Image m_blackScreen;
    // Use this for initialization
    void Start ()
    {
        init();
        m_blackScreen = GameObject.Find(OBJECT_NAME.BLACK_SCREEN).GetComponent<Image>();
    }

    public void fadeOut() //검어짐
    {
        m_blackScreen.CrossFadeAlpha(1.0f, m_fadeOutTime, false);
    }

    public override void startEvent()
    {
        if (isActive() == false)
            return;

        setEventState(ObjectState.EVENT_STATE.WORKING);

        Invoke("fadeOut", m_startTime);
        Invoke("changeScene", m_fadeOutTime + m_startTime);
    }

    public override void endEvent()
    {
    }

    public void changeScene()
    {
        ProjectMgr.getInstance().setPartsNum(InGameMgr.getInstance().getOwnCharacterCtrl().getPartsNumber());

        ProjectMgr.getInstance().transform.parent = null;
        DontDestroyOnLoad(ProjectMgr.getInstance().gameObject);
#if SERVER_ON
        if (Application.loadedLevel != 4)
        {
            InGameServerMgr.getInstance().transform.parent = null;
            DontDestroyOnLoad(InGameServerMgr.getInstance().gameObject);
            InGameMgr.getInstance().resetSceneData();
        }
#endif
        Application.LoadLevel(Application.loadedLevel + 1);
    }

    public void setEventStateReady() { setEventState(ObjectState.EVENT_STATE.READY); }
}
