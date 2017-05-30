using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

namespace BUTTON
{
    public enum STATE { ON, OFF }
    public enum OPERATING_TYPE { HAND_OPERATE, AUTO_OFF, KEEP_PRESS, }
}

public class Button : EventObject
{
    private OBJECT_EVENTS m_objectEvent;
    public EventObject m_EventObject;
    private int m_buttonID;
    private BUTTON.STATE m_buttonState;
    public BUTTON.OPERATING_TYPE m_buttonType;
    protected ObjectState m_parentEventState;

    private delegate void ButtonFunction();
    private ButtonFunction TurnOn;
    private ButtonFunction TurnOff;

    public float m_turnOffTime;

    public Vector2 m_UIPosition;
    private AudioSource m_audioSource;

    protected bool m_animationObject;

    public StartCondition m_startCondition;
    public string m_errorMessage;
    // Use this for initialization
    void Awake()
    {
        m_animationObject = false;
        m_objectEvent.init(GetComponentsInChildren<ObjectEvent>());
    }

    void Start()
    {
        init();
        initChild();

        setButtonState(BUTTON.STATE.OFF);
        setButtonType();
    }

    public override void initChild()
    {
        m_audioSource = GetComponent<AudioSource>();
        setObjectStateScript(GetComponent<ObjectState>());
    }

    public void setButtonID(int id) { m_buttonID = id; }
    public int getButtonID() { return m_buttonID; }

    public void setButtonType()
    {
        switch(m_buttonType)
        {
            case BUTTON.OPERATING_TYPE.HAND_OPERATE :
                TurnOn = new ButtonFunction(handOperateButton);
                TurnOff = new ButtonFunction(handOperateButton);
                break;
            case BUTTON.OPERATING_TYPE.AUTO_OFF :
                TurnOn = new ButtonFunction(autoOffButton);
                TurnOff = null;
                break;
            case BUTTON.OPERATING_TYPE.KEEP_PRESS :
                TurnOn = new ButtonFunction(keepPressButton);
                TurnOff = new ButtonFunction(keepPressButton);
                break;
        }
    }

    public void init(ObjectState eventState, int buttonID)
    {
        setButtonID(buttonID);
        m_parentEventState = eventState;
    }

    public override void startEvent()
    {
        if (isActive() == false)
            return;

        if (m_animationObject == false)
        {
            if (checkStartCondition() == false)
                return;
        }
        m_objectEvent.startEvent(OBJECT_EVENT.EVENT_START_TYPE.EARLY);
        StartCoroutine("startEvent_Concide");
    }

    public override void endEvent()
    {
        if (isActive() == false)
            return;

        if (m_animationObject == false)
        {
            if (checkStartCondition() == false)
                return;
        }
        m_objectEvent.startEvent(OBJECT_EVENT.EVENT_START_TYPE.EARLY);
        StartCoroutine("startEvent_Concide");
    }

    public void startButtonState()
    {
        switch(m_buttonState)
        {
            case BUTTON.STATE.ON :
                if (TurnOff != null)
                        TurnOff();
                break;
            case BUTTON.STATE.OFF :
                if (TurnOn != null)
                        TurnOn();
                break;
        }
    }

    public IEnumerator startEvent_Concide()
    {
        bool earlyEventEnd = false;
        while (earlyEventEnd == false)
        {
            if (m_objectEvent.checkAllEventEnd(OBJECT_EVENT.EVENT_START_TYPE.EARLY))
            {
                earlyEventEnd = true;
                startButtonState();
                m_objectEvent.startEvent(OBJECT_EVENT.EVENT_START_TYPE.COINCIDE);
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
            if (m_objectEvent.checkAllEventEnd(OBJECT_EVENT.EVENT_START_TYPE.COINCIDE))
            {
                coincideEventEnd = true;
                m_objectEvent.startEvent(OBJECT_EVENT.EVENT_START_TYPE.LATER);
            }

            yield return null;
        }
    }

    public void turnOn()
    {
        setButtonState(BUTTON.STATE.ON);
        m_EventObject.startEvent();
        if (m_animationObject == false)
            printSoundEffect();
    }

    public void turnOff()
    {
        setButtonState(BUTTON.STATE.OFF);
        m_EventObject.endEvent();
        if (m_animationObject == false)
            printSoundEffect();
    }

    public void handOperateButton() //수동 버튼 -> on, off모두 이 함수가 호출.
    {
        if (m_parentEventState.getEventState() == ObjectState.EVENT_STATE.READY)
        {
            switch (m_buttonState)
            {
                case BUTTON.STATE.ON:
                    turnOff();
                    break;
                case BUTTON.STATE.OFF:
                    turnOn();
                    break;
            }
        }
    }

    public void autoOffButton() //자동 닫힘 버튼 -> 닫기 버튼 == null, 일정시간후에 자동으로 닫힘.
    {
        if (m_parentEventState.getEventState() == ObjectState.EVENT_STATE.READY)
        {
            if (m_buttonState == BUTTON.STATE.OFF)
            {
                turnOn();
                Invoke("turnOff", m_turnOffTime);
            }
        }
    }

    public void keepPressButton()
    {
        switch(m_buttonState)
        {
            case BUTTON.STATE.ON :
                turnOff();
                break;
            case BUTTON.STATE.OFF :
                turnOn();
                break;
        }
    }
    public void setButtonState(BUTTON.STATE buttonState) { m_buttonState = buttonState; }

    public BUTTON.OPERATING_TYPE getButtonOperatingType() { return m_buttonType; }
    public BUTTON.STATE getButtonState() { return m_buttonState; }

    public void printSoundEffect()
    {
        if(m_audioSource != null)
            m_audioSource.Play();
    }

    public bool checkStartCondition()
    {
        if (m_startCondition != null)
        {
            if (m_startCondition.isActive() == true)
            {
                if (m_startCondition.isReady() == false)
                {
                    printErrorMessage();
                    return false;
                }
            }
        }
        return true;
    }
    
    public void printErrorMessage()
    {
        InGameMgr.getInstance().printErrorMessage(m_errorMessage, false);
    }

    public Vector2 getUI_Position() { return m_UIPosition; }
}
