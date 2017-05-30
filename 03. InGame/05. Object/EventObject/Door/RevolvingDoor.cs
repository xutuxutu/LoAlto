using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(ObjectState))]

public class RevolvingDoor : OpenableDoor
{
    public OPENABLE_DOOR.BUTTON_TYPE m_buttonOperateType;
    private OBJECT_EVENTS m_eventObject;

    public enum Axis { X, Y, Z};
    public Axis ROT_AXIS;

    public float m_rotDegree;
    public float m_rotSpeed;

    public Transform m_door;

    private Vector3 m_rotVector;
    private float m_openDegree;
    private float m_closeDegree;

    private Vector3 m_openRotation;
    private Vector3 m_closeRotation;
    public void Start()
    {
        init();

        setState(ObjectState.EVENT_STATE.READY, OPENABLE_DOOR.STATE.CLOSE);

        m_openDegree = m_rotDegree;
        m_closeDegree = -m_rotDegree;

        switch (ROT_AXIS)
        {
            case Axis.X:
                m_rotVector = Vector3.right;
                m_openRotation = m_door.rotation.eulerAngles + new Vector3(m_rotDegree, 0, 0);
                break;
            case Axis.Y :
                m_rotVector = Vector3.up;
                m_openRotation = m_door.rotation.eulerAngles + new Vector3(0, m_rotDegree, 0);
                break;
            case Axis.Z :
                m_rotVector = Vector3.forward;
                m_openRotation = m_door.rotation.eulerAngles + new Vector3(0, 0, m_rotDegree);
                break;
        }
        m_closeRotation = m_door.rotation.eulerAngles;

        if (m_rotDegree < 0)
            m_rotSpeed = -m_rotSpeed;
    }

    public override void initChild()
    {
        ObjectState eventState = GetComponent<ObjectState>();
        setObjectStateScript(eventState);

        for (int i = 0; i < m_button.Length; i++)
            m_button[i].init(eventState, i + 1);

        m_eventObject.init(GetComponentsInChildren<ObjectEvent>());
    }

    public override void startEvent()
    {
        if (getEventState() == ObjectState.EVENT_STATE.WORKING)
            return;

        setState(ObjectState.EVENT_STATE.WORKING, OPENABLE_DOOR.STATE.OPENING);

        StartCoroutine("startEvent_Concide");
    }

    public override void endEvent()
    {
        Debug.Log("end");
        if (getEventState() == ObjectState.EVENT_STATE.WORKING)
            return;

        setState(ObjectState.EVENT_STATE.WORKING, OPENABLE_DOOR.STATE.CLOSING);

        StartCoroutine("startEvent_Concide");
    }


    public IEnumerator startEvent_Concide()
    {
        bool earlyEventEnd = false;
        while (earlyEventEnd == false)
        {
            if (m_eventObject.checkAllEventEnd(OBJECT_EVENT.EVENT_START_TYPE.EARLY))
            {
                earlyEventEnd = true;
                m_eventObject.startEvent(OBJECT_EVENT.EVENT_START_TYPE.COINCIDE);
                StartCoroutine("revolvingDoor");          //움직임 시작.
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
            if (m_eventObject.checkAllEventEnd(OBJECT_EVENT.EVENT_START_TYPE.COINCIDE))
            {
                if (getEventState() == ObjectState.EVENT_STATE.READY)
                {
                    coincideEventEnd = true;
                    m_eventObject.startEvent(OBJECT_EVENT.EVENT_START_TYPE.LATER);
                }
            }

            yield return null;
        }
        StartCoroutine("endObjectEvent");
    }

    public IEnumerator endObjectEvent()
    {
        bool laterEventEnd = false;
        while (laterEventEnd == false)
        {
            if (m_eventObject.checkAllEventEnd(OBJECT_EVENT.EVENT_START_TYPE.LATER))
            {
                laterEventEnd = true;
                m_eventObject.endEvent(OBJECT_EVENT.EVENT_START_TYPE.EARLY);
                m_eventObject.endEvent(OBJECT_EVENT.EVENT_START_TYPE.COINCIDE);
                m_eventObject.endEvent(OBJECT_EVENT.EVENT_START_TYPE.LATER);
            }

            yield return null;
        }
    }

    public IEnumerator revolvingDoor()
    {
        float curDegree = 0;
        float rotSpeed = 0;

        float maxDegree = 0;

        Vector3 nextRot = Vector3.zero;
        OPENABLE_DOOR.STATE nextState = getDoorState();

        switch(getDoorState())
        {
            case OPENABLE_DOOR.STATE.OPENING :
                maxDegree = m_openDegree;
                rotSpeed = m_rotSpeed;
                nextState = OPENABLE_DOOR.STATE.OPEN;
                nextRot = m_openRotation;
                break;
            case OPENABLE_DOOR.STATE.CLOSING :
                maxDegree = m_closeDegree;
                rotSpeed = -m_rotSpeed;
                nextState = OPENABLE_DOOR.STATE.CLOSE;
                nextRot = m_closeRotation;
                break;
        }

        while (checkMaxDegree(maxDegree, curDegree))
        {
            Debug.Log(curDegree);
            curDegree += rotSpeed * Time.deltaTime;

            m_door.Rotate(m_rotVector * rotSpeed * Time.deltaTime);
            yield return null;
        }

        m_door.rotation = Quaternion.Euler(nextRot);
        setState(ObjectState.EVENT_STATE.READY, nextState);
    }

    public bool checkMaxDegree(float maxDegree, float curDegree)
    {
        if(maxDegree > 0)
        {
            if (curDegree < maxDegree)
                return true;
            return false;
        }
        
        if(maxDegree < 0)
        {
            if (curDegree > maxDegree)
                return true;
            return false;
        }
        return false;
    }
}
