using UnityEngine;
using System.Collections;
using System;
using OPENABLE_DOOR;

public class AnimationDoor : OpenableDoor
{
    public OPENABLE_DOOR.BUTTON_TYPE m_buttonOperateType;

    public Animator m_doorAnimator;

    //private ObjectEvent[] m_eventObject;
    // Use this for initialization
    void Start()
    {
        init();
        initChild();

        if (m_doorAnimator.GetCurrentAnimatorStateInfo(0).IsName("Open"))
        {
            setDoorState(OPENABLE_DOOR.STATE.OPEN);
            for (int i = 0; i < m_button.Length; i++)
                m_button[i].setButtonState(BUTTON.STATE.ON);
        }

        if (m_doorAnimator.GetCurrentAnimatorStateInfo(0).IsName("Close"))
        {
            setDoorState(OPENABLE_DOOR.STATE.CLOSE);
            for (int i = 0; i < m_button.Length; i++)
                m_button[i].setButtonState(BUTTON.STATE.OFF);
        }
    }

    // Update is called once per frame
    //후에 코루틴으로 변경
    void Update()
    {
        switch (getDoorState())
        {
            case OPENABLE_DOOR.STATE.OPENING:
                openDoor();
                break;
            case OPENABLE_DOOR.STATE.CLOSING:
                closeDoor();
                break;
        }
    }

    public override void initChild()
    {
        GetComponent<Animator>();
        ObjectState eventState = GetComponent<ObjectState>();
        setObjectStateScript(eventState);

        for (int i = 0; i < m_button.Length; i++)
            m_button[i].GetComponent<Button>().init(eventState, i + 1);

        //m_eventObject = GetComponentsInChildren<ObjectEvent>();
        //m_audioSource = GetComponent<AudioSource>();
    }

    public override void startEvent()
    {
        for (int i = 0; i < m_button.Length; i++)
        {
            if (m_button[i].getButtonState() != BUTTON.STATE.ON)
            {
                if (m_buttonOperateType == OPENABLE_DOOR.BUTTON_TYPE.MULTIPLE)  //모두가 on상태가 아니면 return
                    return;
                else if (m_buttonOperateType == OPENABLE_DOOR.BUTTON_TYPE.SINGLE)
                    m_button[i].setButtonState(BUTTON.STATE.ON);
            }
        }

        setState(ObjectState.EVENT_STATE.WORKING, OPENABLE_DOOR.STATE.OPENING);
        //printSoundEffect(OPENABLE_DOOR.SOUND.OPERATE);
    }

    public override void endEvent()
    {
        for (int i = 0; i < m_button.Length; i++)
        {
            if (m_button[i].getButtonState() != BUTTON.STATE.OFF)
            {
                if (m_buttonOperateType == OPENABLE_DOOR.BUTTON_TYPE.MULTIPLE)  //모두가 off상태가 아니면 return
                    return;
                else if (m_buttonOperateType == OPENABLE_DOOR.BUTTON_TYPE.SINGLE)
                    m_button[i].setButtonState(BUTTON.STATE.OFF);
            }
        }

        setState(ObjectState.EVENT_STATE.WORKING, OPENABLE_DOOR.STATE.CLOSING);
    }

    public void openDoor()
    {
        m_doorAnimator.SetBool("isOpen", true);
    }

    public void closeDoor()
    {
        m_doorAnimator.SetBool("isClose", true);
    }

    public void eventEnd()
    {
        switch (getDoorState())
        {
            case OPENABLE_DOOR.STATE.OPENING :
                m_doorAnimator.SetBool("isOpen", false);
                setState(ObjectState.EVENT_STATE.READY, OPENABLE_DOOR.STATE.OPEN);
                break;
            case OPENABLE_DOOR.STATE.CLOSING :
                m_doorAnimator.SetBool("isClose", false);
                setState(ObjectState.EVENT_STATE.READY, OPENABLE_DOOR.STATE.CLOSE);
                break;
        }
    }
}
