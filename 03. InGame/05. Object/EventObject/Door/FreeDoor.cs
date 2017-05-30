using UnityEngine;
using System.Collections;

public class FreeDoor : OpenableDoor
{
    public OPENABLE_DOOR.BUTTON_TYPE m_buttonOperateType;

    public Transform[] m_door;
    public Transform[] m_openPoint;

    //문 배치좌표
    private Vector3[] m_closePoint;

    public float m_moveSpeed;
    private Vector3[] m_moveVector;

    private OBJECT_EVENTS m_objectEvent;
    private AudioSource m_audioSource;
    // Use this for initialization
    void Start()
    {
        init();

        setState(ObjectState.EVENT_STATE.READY, OPENABLE_DOOR.STATE.CLOSE);
        setMoveVector();

        m_objectEvent.init(GetComponentsInChildren<ObjectEvent>());
        setEventState(ObjectState.EVENT_STATE.READY);
    }

    public override void initChild()
    {
        ObjectState eventState = GetComponent<ObjectState>();
        m_audioSource = GetComponent<AudioSource>();
        setObjectStateScript(eventState);

        for (int i = 0; i < m_button.Length; i++)
            m_button[i].GetComponent<Button>().init(eventState, i + 1);
    }

    public void setMoveVector()
    {
        m_closePoint = new Vector3[m_door.Length];
        m_moveVector = new Vector3[m_door.Length];

        for (int i = 0; i < m_door.Length; i++)
        {
            m_closePoint[i] = m_door[i].localPosition;
            m_moveVector[i] = m_openPoint[i].localPosition - m_closePoint[i];
        }
    }
    // Update is called once per frame
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

    public override void startEvent()   //문 여는 이벤트
    {
        if (m_buttonOperateType == OPENABLE_DOOR.BUTTON_TYPE.MULTIPLE)  //모두가 on상태가 아니면 return
        {
            for (int i = 0; i < m_button.Length; i++)
            {
                if (m_button[i].GetComponent<Button>().getButtonState() != BUTTON.STATE.ON)
                    return;
            }
        }
        m_objectEvent.startEvent(OBJECT_EVENT.EVENT_START_TYPE.EARLY);

        StartCoroutine("startEvent_Concide");
        printSoundEffect(SOUND_POOL.OBJECT.DOOR.OPERATE);
    }

    public override void endEvent() //문 닫는 이벤트
    {
        m_objectEvent.startEvent(OBJECT_EVENT.EVENT_START_TYPE.EARLY);
        StartCoroutine("startEvent_Concide");
        printSoundEffect(SOUND_POOL.OBJECT.DOOR.OPERATE);
    }

    public void moveDoor()
    {
        switch (getDoorState())
        {
            case OPENABLE_DOOR.STATE.OPEN :
                setState(ObjectState.EVENT_STATE.WORKING, OPENABLE_DOOR.STATE.CLOSING);   
                break;
            case OPENABLE_DOOR.STATE.CLOSE :
                setState(ObjectState.EVENT_STATE.WORKING, OPENABLE_DOOR.STATE.OPENING);
                break;
        }
    }

    public void openDoor()
    {
        float curDistacne;

        for(int i = 0; i< m_door.Length; i++)
        {
            curDistacne = (m_door[i].localPosition - m_closePoint[i]).magnitude;
            if (curDistacne >= m_moveVector[i].magnitude)
            {
                setState(ObjectState.EVENT_STATE.READY, OPENABLE_DOOR.STATE.OPEN);
                setPosition(i, m_openPoint[i].localPosition);
                printSoundEffect(SOUND_POOL.OBJECT.DOOR.STOP);
                continue;
            }
            else
            {
                m_door[i].localPosition += m_moveVector[i].normalized * m_moveSpeed * Time.deltaTime;
            }
        }
    }

    public void closeDoor()
    {
        float curDistacne;

        for (int i = 0; i < m_door.Length; i++)
        {
            curDistacne = ( m_door[i].localPosition - m_openPoint[i].localPosition).magnitude;
            if (curDistacne >= m_moveVector[i].magnitude)
            {
                setState(ObjectState.EVENT_STATE.READY, OPENABLE_DOOR.STATE.CLOSE);
                setPosition(i, m_closePoint[i]);
                printSoundEffect(SOUND_POOL.OBJECT.DOOR.STOP);
                continue;
            }
            else
            {
                m_door[i].localPosition -= m_moveVector[i].normalized * m_moveSpeed * Time.deltaTime;
            }
        }
    }

    public void setPosition(int index, Vector3 position)
    {
        m_door[index].localPosition = position;
    }

    public IEnumerator startEvent_Concide()
    {
        bool earlyEventEnd = false;
        while (earlyEventEnd == false)
        {
            if (m_objectEvent.checkAllEventEnd(OBJECT_EVENT.EVENT_START_TYPE.EARLY))
            {
                earlyEventEnd = true;
                moveDoor();
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
        StartCoroutine("endObjectEvent");
    }

    public IEnumerator endObjectEvent()
    {
        bool laterEventEnd = false;
        while (laterEventEnd == false)
        {
            if (m_objectEvent.checkAllEventEnd(OBJECT_EVENT.EVENT_START_TYPE.LATER))
            {
                laterEventEnd = true;
                m_objectEvent.endEvent(OBJECT_EVENT.EVENT_START_TYPE.EARLY);
                m_objectEvent.endEvent(OBJECT_EVENT.EVENT_START_TYPE.COINCIDE);
                m_objectEvent.endEvent(OBJECT_EVENT.EVENT_START_TYPE.LATER);
            }

            yield return null;
        }
    }

    public void printSoundEffect(SOUND_POOL.OBJECT.DOOR type)
    {
        if (m_audioSource == null)
            return;

        m_audioSource.Stop();
        switch (type)
        {
            case SOUND_POOL.OBJECT.DOOR.OPERATE:
                m_audioSource.PlayOneShot(SoundMgr.getInstance().getObjectAudioClip((int)SOUND_POOL.OBJECT.DOOR.OPERATE), 0.5f);
                StartCoroutine(startLoop());
                break;
            case SOUND_POOL.OBJECT.DOOR.OPERATING:
                m_audioSource.Play();
                break;
            case SOUND_POOL.OBJECT.DOOR.STOP:
                m_audioSource.PlayOneShot(SoundMgr.getInstance().getObjectAudioClip((int)SOUND_POOL.OBJECT.DOOR.STOP), 0.5f);
                break;
        }
    }
    public IEnumerator startLoop()
    {
        yield return new WaitForSeconds(0.05f);

        if (getEventState() == ObjectState.EVENT_STATE.WORKING)
            printSoundEffect(SOUND_POOL.OBJECT.DOOR.OPERATING);
    }
}
