using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ObjectState))]

public class RotateTower : RotatingObject
{
    public Transform m_tower;
    public Animator m_directCamera;

    private OBJECT_EVENTS m_eventObject;
    private AudioSource m_audioSource;

    // Use this for initialization
    void Start()
    {
        init();
        initInfo();
    }

    public override void initChild()
    {
        m_eventObject.init(GetComponentsInChildren<ObjectEvent>());
        m_audioSource = GetComponent<AudioSource>();

        ObjectState eventState = GetComponent<ObjectState>();
        setObjectStateScript(eventState);

        for (int i = 0; i < m_button.Length; i++)
            m_button[i].init(eventState, i + 1);
    }

    private void initInfo()
    {
        if (m_rotDirection == ROT_DIRECTION.LEFT)
        {
            m_rotSpeed = -m_rotSpeed;
            for (int i = 0; i < m_stopAngle.Length; i++)
            {
                m_stopAngle[i] = 360 - m_stopAngle[i];
            }
        }

        for (int i = 0; i < m_stopAngle.Length; i++)
        {
            if (m_stopAngle[i] >= 360)
                m_stopAngle[i] = 360 - m_stopAngle[i];
        }
    }

    public override void startEvent()
    {
        m_eventObject.startEvent(OBJECT_EVENT.EVENT_START_TYPE.EARLY);

        rotationObject();

        StartCoroutine("startEvent_Concide");
    }

    public override void endEvent()
    {
    }

    public IEnumerator startEvent_Concide()
    {
        bool earlyEventEnd = false;
        while(earlyEventEnd == false)
        {
            if(m_eventObject.checkAllEventEnd(OBJECT_EVENT.EVENT_START_TYPE.EARLY))
            {
                earlyEventEnd = true;
                m_eventObject.startEvent(OBJECT_EVENT.EVENT_START_TYPE.COINCIDE);
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

    public override IEnumerator rotate()
    {
        setEventState(ObjectState.EVENT_STATE.WORKING);
        printSound();
        while (checkRotAngle() == false)
        {
            m_tower.Rotate(Vector3.up * m_rotSpeed * Time.deltaTime);
            yield return null;
        }

        for (int i = 0; i < m_button.Length; i++)
            m_button[i].GetComponent<Button>().turnOff();

        m_directCamera.SetTrigger("rotateEnd");

        m_tower.rotation = Quaternion.Euler(0, m_stopAngle[getCurRotCycle()], 0);
        setEventState(ObjectState.EVENT_STATE.READY);
        printSound();
    }

    public override bool checkRotAngle()
    {
        float curRot = m_tower.eulerAngles.y;
        float nextRot = curRot + (m_rotSpeed * Time.deltaTime);

        if (nextRot < 0)
            nextRot = 360 + nextRot;

        switch (m_rotDirection)
        {
            case ROT_DIRECTION.LEFT:
                if (m_stopAngle[getCurRotCycle()] >= 0 && m_stopAngle[getCurRotCycle()] <= 3)
                {
                    if (nextRot <= 3)
                        return true;
                }

                if (nextRot < m_stopAngle[getCurRotCycle()] && curRot > m_stopAngle[getCurRotCycle()])
                    return true;
                break;
            case ROT_DIRECTION.RIGHT:
                if (m_stopAngle[getCurRotCycle()] >= 358 && m_stopAngle[getCurRotCycle()] <= 360)
                {
                    if (nextRot >= 358)
                        return true;
                }

                if (m_stopAngle[getCurRotCycle()] == 0 && m_stopAngle[getCurRotCycle()] <= 3)
                {
                    if (nextRot >= 0 && nextRot <= 3)
                        return true;
                }

                if (nextRot > m_stopAngle[getCurRotCycle()] && curRot < m_stopAngle[getCurRotCycle()])
                    return true;
                break;
        }
        return false;
    }

    public new void setActive(bool isActive)
    {
        base.setActive(isActive);

        for (int i = 0; i < m_button.Length; i++)
            m_button[i].GetComponent<Button>().setActive(isActive);
    }

    public void printSound()
    {
        m_audioSource.Stop();

        switch (getEventState())
        {
            case ObjectState.EVENT_STATE.READY:
                m_audioSource.PlayOneShot(SoundMgr.getInstance().getObjectAudioClip((int)SOUND_POOL.OBJECT.DOOR.STOP), 0.5f);
                break;
            case ObjectState.EVENT_STATE.WORKING:
                    m_audioSource.Play();
                break;
        }
    }
}
