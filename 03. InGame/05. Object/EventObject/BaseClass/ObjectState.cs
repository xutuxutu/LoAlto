using UnityEngine;
using System.Collections;

public class ObjectState : MonoBehaviour
{
    public enum EVENT_STATE { READY, WORKING, }
    EVENT_STATE m_eventState;

	// Use this for initialization
	void Start ()
    {
        m_eventState = EVENT_STATE.READY;
	}

    public void setEventState(EVENT_STATE eventState) { m_eventState = eventState; }
    public EVENT_STATE getEventState() { return m_eventState; }
}
