using UnityEngine;
using System.Collections.Generic;

namespace OPENABLE_DOOR
{
    public enum STATE { OPEN, CLOSE, OPENING, CLOSING };
    public enum BUTTON_TYPE { SINGLE, MULTIPLE, }
}

public abstract class OpenableDoor : EventObject
{
    private OPENABLE_DOOR.STATE m_doorState;

    public void doorCtrl(OPENABLE_DOOR.STATE operateType)
    {
        switch (operateType)
        {
            case OPENABLE_DOOR.STATE.OPEN:
                setState(ObjectState.EVENT_STATE.WORKING, OPENABLE_DOOR.STATE.OPENING);
                break;
            case OPENABLE_DOOR.STATE.CLOSE:
                setState(ObjectState.EVENT_STATE.WORKING, OPENABLE_DOOR.STATE.CLOSING);
                break;
        }
    }
    //getter
    public OPENABLE_DOOR.STATE getDoorState() { return m_doorState; }

    //setter
    public new void setActive(bool isActive)
    {
        base.setActive(isActive);

        for (int i = 0; i < m_button.Length; i++)
            m_button[i].setActive(isActive);
    }

    public void setDoorState(OPENABLE_DOOR.STATE state) { m_doorState = state; }

    public void setButtonState(BUTTON.STATE state)
    {
        for (int i = 0; i < m_button.Length; ++i)
            m_button[i].setButtonState(state);
    }

    public void setState(ObjectState.EVENT_STATE eventState, OPENABLE_DOOR.STATE doorState)
    {
        setEventState(eventState);
        setDoorState(doorState);
    }
}
