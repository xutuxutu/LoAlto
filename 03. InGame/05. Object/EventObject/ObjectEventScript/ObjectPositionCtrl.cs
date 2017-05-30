using UnityEngine;
using System.Collections;
using System;

public class ObjectPositionCtrl : ObjectEvent
{
    public GameObject[] m_targetObject;
    public GameObject[] m_position;

    // Use this for initialization
    void Start ()
    {
        init();
    }

    public override void startEvent()
    {
        if (isActive() == false)
            return;

        setEventState(ObjectState.EVENT_STATE.WORKING);
        Invoke("setPosition", invokeTime);
    }

    public override void endEvent()
    {
    }

    public void setPosition()
    {
        for(int i = 0; i < m_targetObject.Length; ++i)
        {
            m_targetObject[i].transform.position = m_position[i].transform.position;
            m_targetObject[i].transform.rotation = m_position[i].transform.rotation;
        }
        setEventState(ObjectState.EVENT_STATE.READY);
    }
}
