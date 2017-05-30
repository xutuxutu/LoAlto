using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class ObjectActiveCtrl : ObjectEvent
{
    public enum ACTION_TYPE { ACTIVE, DEACTIVE, ACTIVE_DEACTIVE, DEACTIVE_ACTIVE };
    public enum OBJECT_TYPE { SELECT, CHARACTER, MAIN_UI, OTHER }

    public ACTION_TYPE m_actionType;
    public OBJECT_TYPE m_objectType;

    public GameObject m_object;
    private Text m_errorMessage;
	// Use this for initialization
	void Start ()
    {
        init();
        switch(m_objectType)
        {
            case OBJECT_TYPE.MAIN_UI :
                m_object = GameObject.Find(OBJECT_NAME.COMMON_UI);
                m_errorMessage = GameObject.Find(OBJECT_NAME.ERROR_MESSGE).GetComponent<Text>();
                break;
        }
	}

    public override void startEvent()
    {
        if (isActive() == false)
            return;

        setEventState(ObjectState.EVENT_STATE.WORKING);

        switch (m_actionType)
        {
            case ACTION_TYPE.ACTIVE:
            case ACTION_TYPE.ACTIVE_DEACTIVE :
                Invoke("activeObject", invokeTime);
                break;
            case ACTION_TYPE.DEACTIVE:
            case ACTION_TYPE.DEACTIVE_ACTIVE :
                Invoke("deActiveObject", invokeTime);
                break;
        }
    }

    public override void endEvent()
    {
        setEventState(ObjectState.EVENT_STATE.WORKING);

        switch(m_actionType)
        {
            case ACTION_TYPE.ACTIVE_DEACTIVE :
                Invoke("deActiveObject", invokeTime);
                break;
            case ACTION_TYPE.DEACTIVE_ACTIVE :
                Invoke("activeObject", invokeTime);
                break; 
        }
    }

    public void activeObject()
    {
        switch(m_objectType)
        {
            case OBJECT_TYPE.CHARACTER :
                if(InGameMgr.getInstance().getOwnCharacterCtrl().isDie() == false)
                    InGameMgr.getInstance().getOwnCharacterCtrl().setIsActive(true);   
                break;
            case OBJECT_TYPE.MAIN_UI :
                m_object.SetActive(true);
                m_errorMessage.canvasRenderer.SetAlpha(0f);
                InGameMgr.getInstance().setActiveMainUI(true);
                break;
            case OBJECT_TYPE.SELECT :
                m_object.SetActive(true);
                break;
            case OBJECT_TYPE.OTHER :
                InGameMgr.getInstance().getOtherCharacterCtrl().setActive();
                break;
        }
        setEventState(ObjectState.EVENT_STATE.READY);
    }

    public void deActiveObject()
    {
        switch (m_objectType)
        {
            case OBJECT_TYPE.CHARACTER:
                if (InGameMgr.getInstance().getOwnCharacterCtrl().isDie() == false)
                    InGameMgr.getInstance().getOwnCharacterCtrl().setIsActive(false);
                break;
            case OBJECT_TYPE.MAIN_UI:
                Debug.Log(m_object);
                m_object.SetActive(false);
                InGameMgr.getInstance().getMainUI().SetActive(false);
                break;
            case OBJECT_TYPE.SELECT:
                m_object.SetActive(false);
                break;
            case OBJECT_TYPE.OTHER:
                InGameMgr.getInstance().getOtherCharacterCtrl().setDeActive();
                break;
        }
        setEventState(ObjectState.EVENT_STATE.READY);
    }
}
