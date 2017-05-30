using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ObjectState))]

public class SingleSlideDoor : OpenableDoor
{
    public OPENABLE_DOOR.BUTTON_TYPE m_buttonOperateType;
    public Transform m_door_1;

    public float m_moveSpeed;
    public float m_moveDistance;

    private OPENABLE_DOOR.STATE m_doorState;

    private Vector3 m_moveVector;
    private float m_curDistacne;

    private float m_openPoint;
    private float m_closePoint;

    // Use this for initialization
    void Start()
    {
        init();

        setState(ObjectState.EVENT_STATE.READY, OPENABLE_DOOR.STATE.CLOSE);

        //Door1의 로컬 좌표 기준.
        m_closePoint = m_door_1.localPosition.y;
        m_openPoint = m_closePoint + m_moveDistance;
        m_moveVector = Vector3.up;
    }

    public override void initChild()
    {
        ObjectState eventState = GetComponentInChildren<ObjectState>();
        setObjectStateScript(eventState);

        for (int i = 0; i < m_button.Length; i++)
            m_button[i].GetComponent<Button>().init(eventState, i + 1);
    }

    // Update is called once per frame
    void Update()
    {
        switch (m_doorState)
        {
            case OPENABLE_DOOR.STATE.OPENING:
                openDoor();
                break;
            case OPENABLE_DOOR.STATE.CLOSING:
                closeDoor();
                break;
        }
    }

    public override void startEvent()
    {
        if (m_buttonOperateType == OPENABLE_DOOR.BUTTON_TYPE.MULTIPLE)  //모두가 on상태가 아니면 return
        {
            for (int i = 0; i < m_button.Length; i++)
            {
                if (m_button[i].GetComponent<Button>().getButtonState() != BUTTON.STATE.ON)
                    return;
            }
        }
        setState(ObjectState.EVENT_STATE.WORKING, OPENABLE_DOOR.STATE.OPENING);
    }

    public override void endEvent()
    {
        setState(ObjectState.EVENT_STATE.WORKING, OPENABLE_DOOR.STATE.CLOSING);
    }

    public void openDoor()
    {
        m_curDistacne = m_door_1.localPosition.y - m_closePoint;
        if (m_curDistacne >= m_moveDistance)
        {
            setState(ObjectState.EVENT_STATE.READY, OPENABLE_DOOR.STATE.OPEN);
            m_curDistacne = 0;
            setPosition(m_openPoint);
            return;
        }

        else
        {
            m_door_1.position += m_moveVector * m_moveSpeed;
        }
    }

    public void closeDoor()
    {
        m_curDistacne = m_openPoint - m_door_1.localPosition.y;
        if (m_curDistacne >= m_moveDistance)
        {
            setState(ObjectState.EVENT_STATE.READY, OPENABLE_DOOR.STATE.CLOSE);
            m_curDistacne = 0;
            setPosition(m_closePoint);
            return;
        }

        else
        {
            m_door_1.position -= m_moveVector * m_moveSpeed;
        }
    }

    public void setPosition(float point)
    {
        m_door_1.localPosition = new Vector3(m_door_1.localPosition.x, point, m_door_1.localPosition.z);
    }
}
