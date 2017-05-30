using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(AudioSource))]

public class Elevator : MovingObject
{
    private Vector3 m_startPoint;
    public Transform m_foorhold;
    public Transform[] m_stopPoint;

    public float m_accelSpeed;
    public float m_stopTime;
    public MOVING_OBJECT.TPYE m_moveType;

    private float m_totalDistance;
    private float m_curDistance;
    private float m_squeakDistance;
    private bool m_isSqueak;

    private Vector3[] m_moveVectorList;
    private int m_movePointNum;
    private int m_moveDirection;

    private MoveFuction Move;

    private OBJECT_EVENTS m_eventObject;

    private AudioSource[] m_audioSource;
    // Use this for initialization
    void Start()
    {
        init();
        initChild();

        m_isSqueak = false;
        m_moveDirection = 0;
        m_startPoint = m_foorhold.position;                  //초기위치 저장 
        m_movePointNum = m_stopPoint.Length;
        m_moveVectorList = new Vector3[m_movePointNum + 1];     //이동 벡터리스트

        if (m_moveType == MOVING_OBJECT.TPYE.ACCEL)          //가속운동일 경우
        {
            Move = move_Accel;
        }
        else if (m_moveType == MOVING_OBJECT.TPYE.CONST)     //등속운동일 경우
        {
            m_accelSpeed = 0;
            Move = move_Const;
        }

        setMoveVectorList();
        setMoveVector(m_moveDirection);
    }

    public override void initChild()
    {
        m_audioSource = GetComponents<AudioSource>();
        m_eventObject.init(GetComponentsInChildren<ObjectEvent>());
        ObjectState eventState = GetComponent<ObjectState>();
        setObjectStateScript(eventState);

        if (m_button == null)   //버튼이 없는경우 자동으로 움직임.
            setIsAuto(true);
        else
        {
            setIsAuto(false);
            arriveToPosition();

            for (int i = 0; i < m_button.Length; i++)
                m_button[i].init(eventState, i + 1);
        }
    }

    public void setMoveVectorList()
    {
        int index = 0;

        m_moveVectorList[index] = m_stopPoint[index].position - m_startPoint; //시작점에서 첫 정지점까지의 벡터

        index += 1;
        for (; index < m_movePointNum; ++index)                             //원위치로 돌아오기전까지 순차적으로 이동벡터 구함
        {
            //stop포인트는 0번이 1번째 위치. startPoint -> 0번째 정지위치 , stopPoint[0] -> 1번째 정지위치
            m_moveVectorList[index] = m_stopPoint[index].position - m_stopPoint[index - 1].position;
        }

        m_moveVectorList[index] = m_startPoint - m_stopPoint[index - 1].position;     //원위치로 돌아오는 벡터
    }

    public void setMoveVector(int direction)
    {
        //현재 속도를 초기속도로 지정.
        m_curMoveSpeed = m_moveSpeed;
        m_totalDistance = Vector3.SqrMagnitude(m_moveVectorList[direction]);
        m_moveVector = m_moveVectorList[direction].normalized;

        m_squeakDistance = ((m_totalDistance / m_moveSpeed) - 35) * m_moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive() == true)
            MoveTo();
    }

    public override void moveOwn()
    {
        //0번째 플레이어의 이동 기능
        if (m_isArrive == false)
        {
            if (checkDistance() == true)
                Move();

            else
            {
                m_isSqueak = false;
                arriveToPosition(); //m_isArrive = true;
                setMoveDirection(m_moveDirection + 1);
#if SERVER_ON   
                int[] objID = ObjectMgr.getInstance().getObjectID(gameObject.name);
                InGameServerMgr.getInstance().SendPacket(NET_INGAME.SEND.PACKET_TYPE.REQUEST_OBJECT_STATE_EM, objID[0], objID[1], m_moveDirection);
                Debug.Log("send");
#endif
                setPosition();
                changeDirection();
                setEventState(ObjectState.EVENT_STATE.READY);
            }
        }
    }

    public override void moveOther()
    {
        if (getEventState() == ObjectState.EVENT_STATE.WORKING)
        {
            //1번째 플레이어의 이동기능. 방향 변경은 네트워크 패킷수신으로 설정.
            if (m_isArrive == false)
            {
                if (checkDistance() == true)
                    Move();

                else
                {
                    m_isSqueak = false;
                    arriveToPosition();
                }
            }
        }
    }

    public override void setState(int state)
    {
        Debug.Log("Elevator : " + state);
        //패킷 수신시 호출함수.
        //상태 설정 -> 정지 위치, 이동 방향 설정 및 이동 시작.
        setMoveDirection(state);
        changeDirection();
        setPosition();
        setEventState(ObjectState.EVENT_STATE.READY);
    }

    public void move_Const()
    {
        //등속 이동
        m_foorhold.Translate(m_moveVector * m_curMoveSpeed * Time.deltaTime);
    }

    public void move_Accel()
    {
        //거리에따라 가속했다가 감속
        if (m_curDistance < m_totalDistance / 2)
            m_curMoveSpeed += m_accelSpeed * Time.deltaTime;
        else if (m_curMoveSpeed > m_moveSpeed)
            m_curMoveSpeed -= m_accelSpeed * Time.deltaTime;

        m_foorhold.Translate(m_moveVector * m_curMoveSpeed * Time.deltaTime);
    }

    public void updateCurDistance()
    {
        //현재 이동거리 업데이트.
        Vector3 startPoint = Vector3.zero;
        if (m_moveDirection == 0)
            startPoint = m_startPoint;
        else
            startPoint = m_stopPoint[m_moveDirection - 1].position;

        m_curDistance = Vector3.SqrMagnitude(m_foorhold.position - startPoint);
    }

    public bool checkDistance()
    {
        //거리 측정.
        updateCurDistance();
        if (m_isSqueak == false)
        {
            //Debug.Log(m_curDistance + " /  " + m_squeakDistance + " / " + m_totalDistance);
            if (m_curDistance >= m_squeakDistance)
            {
                Debug.Log("squeak");
                m_isSqueak = true;
                printSoundEffect(SOUND_POOL.OBJECT.ELEVATOR.SQUEAK);
            }
        }
        if (m_curDistance > m_totalDistance)
            return false;

        return true;
    }

    public void setMoveDirection(int direction)
    {
        //이동 방향 설정
        m_moveDirection = direction;

        if (m_moveDirection > m_movePointNum)
            m_moveDirection = 0;
    }

    public void changeDirection()
    {
        //방향 변경
        setMoveVector(m_moveDirection);
        if(isAuto() == true)                    //자동일 경우 자동으로 방향전환.
            Invoke("startMoving", m_stopTime);
    }

    public void setPosition()
    {
        //위치 설정
        if (m_moveDirection == 0)
            m_foorhold.position = m_startPoint;
        else
            m_foorhold.position = m_stopPoint[m_moveDirection - 1].position;
    }

    public override void startEvent()
    {
        setEventState(ObjectState.EVENT_STATE.WORKING);
        m_eventObject.startEvent(OBJECT_EVENT.EVENT_START_TYPE.EARLY);

        StartCoroutine("startEvent_Concide");
    }

    public override void endEvent()
    {
        setEventState(ObjectState.EVENT_STATE.WORKING);
        m_eventObject.startEvent(OBJECT_EVENT.EVENT_START_TYPE.EARLY);

        StartCoroutine("startEvent_Concide");
    }

    public IEnumerator startEvent_Concide()
    {
        Debug.Log("elevatorCoincide");
        bool earlyEventEnd = false;
        while (earlyEventEnd == false)
        {
            if (m_eventObject.checkAllEventEnd(OBJECT_EVENT.EVENT_START_TYPE.EARLY))
            {
                earlyEventEnd = true;
                m_eventObject.startEvent(OBJECT_EVENT.EVENT_START_TYPE.COINCIDE);
                printSoundEffect(SOUND_POOL.OBJECT.ELEVATOR.OPERATE);
                startMoving();          //움직임 시작.
            }

            yield return null;
        }

        StartCoroutine("startEvent_Later");
    }

    public IEnumerator startEvent_Later()
    {
        Debug.Log("elevatorEnd");
        bool coincideEventEnd = false;
        while (coincideEventEnd == false)
        {
            if (m_eventObject.checkAllEventEnd(OBJECT_EVENT.EVENT_START_TYPE.COINCIDE))
            {
                if (getEventState() == ObjectState.EVENT_STATE.READY)
                {
                    coincideEventEnd = true;
                    printSoundEffect(SOUND_POOL.OBJECT.ELEVATOR.STOP);
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

    public void printSoundEffect(SOUND_POOL.OBJECT.ELEVATOR type)
    {
        switch (type)
        {
            case SOUND_POOL.OBJECT.ELEVATOR.OPERATE:
                m_audioSource[0].Stop();
                m_audioSource[0].PlayOneShot(SoundMgr.getInstance().getObjectAudioClip((int)SOUND_POOL.OBJECT.ELEVATOR.OPERATE), 0.5f);
                StartCoroutine(startLoop());
                break;
            case SOUND_POOL.OBJECT.ELEVATOR.OPERATING:
                m_audioSource[0].Stop();
                m_audioSource[0].Play();
                break;
            case SOUND_POOL.OBJECT.ELEVATOR.SQUEAK:
                StartCoroutine("fadeOutOperatingSound");
                m_audioSource[1].Play();
                break;
            case SOUND_POOL.OBJECT.ELEVATOR.STOP :
                StopCoroutine("fadeOutOperatingSound");
                m_audioSource[0].Stop();
                m_audioSource[1].Stop();
                m_audioSource[0].volume = 1f;
                m_audioSource[0].PlayOneShot(SoundMgr.getInstance().getObjectAudioClip((int)SOUND_POOL.OBJECT.ELEVATOR.STOP), 0.5f);
                break;
        }
    }

    public IEnumerator startLoop()
    {
        yield return new WaitForSeconds(0.05f);

        if (getEventState() == ObjectState.EVENT_STATE.WORKING)
            printSoundEffect(SOUND_POOL.OBJECT.ELEVATOR.OPERATING);
    }

    public IEnumerator fadeOutOperatingSound()
    {
        while(true)
        {
            m_audioSource[0].volume -= 0.01f;
            yield return null;
        }
    }
}
