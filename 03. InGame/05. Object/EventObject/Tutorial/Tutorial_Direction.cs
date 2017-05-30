using UnityEngine;
using System.Collections;

public class Tutorial_Direction : MonoBehaviour
{
    private bool m_arriveSparky = false;
    private bool m_arriveSam = false;
    
    public GameObject m_directionObject;
    private OBJECT_EVENTS m_eventObject;

    public GameObject m_directionElevator;
    private bool m_isEndDirection;

    void Start()
    {
        m_isEndDirection = false;
        m_directionObject.SetActive(false);
        m_directionObject.GetComponent<AnimToObjectEvent>().init();

        m_eventObject.init(GetComponents<ObjectEvent>());
    }

    public void checkStartDirection()
    {
#if SERVER_ON
        if(m_arriveSam == true && m_arriveSparky == true)
        {
            m_eventObject.startEvent(OBJECT_EVENT.EVENT_START_TYPE.EARLY);
            StartCoroutine("startEvent_Concide");
        }
#elif SERVER_OFF
        if (m_arriveSam == true || m_arriveSparky == true)
        {
            m_eventObject.startEvent(OBJECT_EVENT.EVENT_START_TYPE.EARLY);
            StartCoroutine("startEvent_Concide");
        }
#endif
    }

    public void arriveSam()
    {
        m_arriveSam = true;
        checkStartDirection();
    }
    public void arriveSparky()
    {
        m_arriveSparky = true;
        checkStartDirection();
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
            if (m_eventObject.checkAllEventEnd(OBJECT_EVENT.EVENT_START_TYPE.COINCIDE) && m_isEndDirection)
            {
                coincideEventEnd = true;
                m_eventObject.startEvent(OBJECT_EVENT.EVENT_START_TYPE.LATER);
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
}
