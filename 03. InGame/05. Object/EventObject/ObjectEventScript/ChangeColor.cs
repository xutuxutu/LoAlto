using UnityEngine;
using System.Collections;
using System;

public class ChangeColor : ObjectEvent
{
    private Material m_material;

    public Color m_startColor;
    public Color m_endColor;

	// Use this for initialization
	void Start ()
    {
        init();
        m_material = GetComponent<Renderer>().material;
        setColor(m_startColor);
    }

    public override void startEvent()
    {
        setEventState(ObjectState.EVENT_STATE.WORKING);
        setColor(m_endColor);
        setEventState(ObjectState.EVENT_STATE.READY);
    }

    public override void endEvent()
    {
        setEventState(ObjectState.EVENT_STATE.WORKING);
        setColor(m_startColor);
        setEventState(ObjectState.EVENT_STATE.READY);
    }

    public void setColor(Color color)
    {
        m_material.color = color;
        m_material.SetColor("_EmissionColor", color);
    }
}
