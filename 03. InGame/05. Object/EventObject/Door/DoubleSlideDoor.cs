using UnityEngine;
using System.Collections;


[RequireComponent(typeof(AudioSource))]

public class DoubleSlideDoor : OpenableDoor
{
    public OPENABLE_DOOR.BUTTON_TYPE m_buttonOperateType;

    private ObjectEvent[] m_eventObject;

    public Transform m_door_1;
    public Transform m_door_2;

    public float m_moveSpeed;
    public float m_moveDistance;

    private Vector3 m_moveVector;
    private float m_curDistacne;

    private float m_openPoint;
    private float m_closePoint;

    private Vector3[] m_openPosition;
    private Vector3[] m_closePosition;

    private AudioSource m_audioSource;
    // Use this for initialization
    void Start()
    {
        init();
        m_openPosition = new Vector3[2];
        m_closePosition = new Vector3[2];

        setState(ObjectState.EVENT_STATE.READY, OPENABLE_DOOR.STATE.CLOSE);

        m_closePosition[0] = m_door_1.localPosition;
        m_closePosition[1] = m_door_2.localPosition;

        //Door1의 로컬 좌표 기준.
        m_closePoint = m_door_1.localPosition.x;
        if (m_door_1.forward == new Vector3(0, 0, 1))
        {
            m_openPoint = m_closePoint + m_moveDistance;
            m_moveVector = m_door_1.right;
        }
        else
        {
            m_openPoint = m_closePoint - m_moveDistance; 
            m_moveVector = -m_door_1.right;
        }

        m_openPosition[0] = m_door_1.localPosition + m_moveVector * m_moveDistance;
        m_openPosition[1] = m_door_2.localPosition - m_moveVector * m_moveDistance;
    }

    public override void initChild()
    {
        ObjectState eventState = GetComponent<ObjectState>();
        setObjectStateScript(eventState);

        for (int i = 0; i < m_button.Length; i++)
            m_button[i].init(eventState, i + 1);

        m_eventObject = GetComponentsInChildren<ObjectEvent>();
        m_audioSource = GetComponent<AudioSource>();
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

    public override void startEvent()
    {
        for (int i = 0; i < m_button.Length; i++)
        {
            if (m_buttonOperateType == OPENABLE_DOOR.BUTTON_TYPE.MULTIPLE)  //모두가 on상태가 아니면 return
            {
                if (m_button[i].GetComponent<Button>().getButtonState() != BUTTON.STATE.ON)
                    return;
            }
            
            if (m_buttonOperateType == OPENABLE_DOOR.BUTTON_TYPE.SINGLE)    //모든 버튼의 상태를 ON
                m_button[i].GetComponent<Button>().setButtonState(BUTTON.STATE.ON);
        }
        

        for (int i = 0; i < m_eventObject.Length; i++)
            m_eventObject[i].SendMessage("startEvent");

        setState(ObjectState.EVENT_STATE.WORKING, OPENABLE_DOOR.STATE.OPENING);
        printSoundEffect(SOUND_POOL.OBJECT.DOOR.OPERATE);
    }

    public override void endEvent()
    {
        for (int i = 0; i < m_button.Length; i++)   //모든 버튼의 상태를 OFF
        {
            if (m_button[i].GetComponent<Button>().getButtonState() == BUTTON.STATE.ON)
                m_button[i].GetComponent<Button>().setButtonState(BUTTON.STATE.OFF);
        }

        for (int i = 0; i < m_eventObject.Length; i++)
            m_eventObject[i].SendMessage("startEvent");

        setState(ObjectState.EVENT_STATE.WORKING, OPENABLE_DOOR.STATE.CLOSING);
        printSoundEffect(SOUND_POOL.OBJECT.DOOR.OPERATE);
    }

    public void openDoor()
    {
        m_curDistacne = Mathf.Abs(m_closePoint - m_door_1.localPosition.x);
        if (m_curDistacne >= m_moveDistance)
        {
            setState(ObjectState.EVENT_STATE.READY, OPENABLE_DOOR.STATE.OPEN);
            m_curDistacne = 0;
            setPosition();
            return;
        }

        else
        {
            m_door_1.position += m_moveVector * m_moveSpeed * Time.deltaTime;
            m_door_2.position -= m_moveVector * m_moveSpeed * Time.deltaTime;
        }
    }

    public void closeDoor()
    {
        m_curDistacne = Mathf.Abs(m_door_1.localPosition.x - m_openPoint);
        if (m_curDistacne >= m_moveDistance)
        {
            setState(ObjectState.EVENT_STATE.READY, OPENABLE_DOOR.STATE.CLOSE);
            m_curDistacne = 0;
            setPosition();
            return;
        }

        else
        {
            m_door_1.position -= m_moveVector * m_moveSpeed * Time.deltaTime;
            m_door_2.position += m_moveVector * m_moveSpeed * Time.deltaTime;
        }
    }

    public void setPosition()
    {
        if (getDoorState() == OPENABLE_DOOR.STATE.CLOSE)
        {
            m_door_1.localPosition = m_closePosition[0];
            m_door_2.localPosition = m_closePosition[1];
        }
        if(getDoorState() == OPENABLE_DOOR.STATE.OPEN)
        {
            m_door_1.localPosition = m_openPosition[0];
            m_door_2.localPosition = m_openPosition[1];
        }

        for (int i = 0; i < m_eventObject.Length; i++)
            m_eventObject[i].SendMessage("endEvent");

        printSoundEffect(SOUND_POOL.OBJECT.DOOR.STOP);
    }

    public void printSoundEffect(SOUND_POOL.OBJECT.DOOR type)
    {
        m_audioSource.Stop();
        switch (type)
        {
            case SOUND_POOL.OBJECT.DOOR.OPERATE :
                m_audioSource.PlayOneShot(SoundMgr.getInstance().getObjectAudioClip((int)SOUND_POOL.OBJECT.DOOR.OPERATE), 0.5f);
                StartCoroutine(startLoop());
                break;
            case SOUND_POOL.OBJECT.DOOR.OPERATING :
                m_audioSource.Play();
                break;
            case SOUND_POOL.OBJECT.DOOR.STOP :
                m_audioSource.PlayOneShot(SoundMgr.getInstance().getObjectAudioClip((int)SOUND_POOL.OBJECT.DOOR.STOP), 0.5f);
                break;
        }
    }

    public IEnumerator startLoop()
    {
        yield return new WaitForSeconds(0.05f);

        if(getEventState() == ObjectState.EVENT_STATE.WORKING)
            printSoundEffect(SOUND_POOL.OBJECT.DOOR.OPERATING);
    }
}
