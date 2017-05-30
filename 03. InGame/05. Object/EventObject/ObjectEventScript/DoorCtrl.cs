using UnityEngine;
using System.Collections;
using System;

public class DoorCtrl : ObjectEvent
{
    public enum ACTION_TIME { START, END, ALL }
    public enum CONTROL_TYPE { OPEN, CLOSE, OPEN_CLOSE, CLOSE_OPEN };

    public ACTION_TIME m_actionTime;
    public CONTROL_TYPE m_ctrlType;

    public OpenableDoor m_doorCtrl;

    // Use this for initialization
    void Start()
    {
        init();
	}

    public override void startEvent()
    {
        if (m_actionTime == ACTION_TIME.END)
            return;

        Invoke("eventStart", invokeTime);
    }

    public void eventStart()
    {
        switch (m_ctrlType)
        {
            case CONTROL_TYPE.CLOSE:
            case CONTROL_TYPE.CLOSE_OPEN:
                closeDoor();
                break;
            case CONTROL_TYPE.OPEN:
            case CONTROL_TYPE.OPEN_CLOSE:
                openDoor();
                break;
        }
    }

    public override void endEvent()
    {
        if (m_actionTime == ACTION_TIME.START)
            return;

        switch (m_ctrlType)
        {
            case CONTROL_TYPE.OPEN:
            case CONTROL_TYPE.CLOSE_OPEN:
                openDoor();
                break;
            case CONTROL_TYPE.CLOSE:
            case CONTROL_TYPE.OPEN_CLOSE:
                closeDoor();
                break;
        }
    }

    public void openDoor()
    {
        if (m_doorCtrl.getDoorState() == OPENABLE_DOOR.STATE.CLOSE || m_doorCtrl.getDoorState() == OPENABLE_DOOR.STATE.CLOSING)
        {
            setEventState(ObjectState.EVENT_STATE.WORKING);
            m_doorCtrl.doorCtrl(OPENABLE_DOOR.STATE.OPEN);
            m_doorCtrl.setButtonState(BUTTON.STATE.ON);
            StartCoroutine("checkDoorState");
        }
    }

    public void closeDoor()
    {
        if (m_doorCtrl.getDoorState() == OPENABLE_DOOR.STATE.OPEN || m_doorCtrl.getDoorState() == OPENABLE_DOOR.STATE.OPENING)
        {
            setEventState(ObjectState.EVENT_STATE.WORKING);
            m_doorCtrl.doorCtrl(OPENABLE_DOOR.STATE.CLOSE);
            m_doorCtrl.setButtonState(BUTTON.STATE.OFF);
            StartCoroutine("checkDoorState");
        }
    }

    public IEnumerator checkDoorState()
    {
        bool isCheck = false;
        while (isCheck == false)
        {
            if (m_doorCtrl.getEventState() == ObjectState.EVENT_STATE.READY)
            {
                setEventState(ObjectState.EVENT_STATE.READY);
                isCheck = true;
            }
            yield return null;
        }
    }
}
