using UnityEngine;
using System.Collections;
using System;

public class RotateGear : ObjectEvent
{
    public enum AXIS { X, Y, Z };

    public AXIS ROT_AXIS;
    private Vector3 m_rotAxis;
    public float m_rotSpeed;
	// Use this for initialization
	void Start ()
    {
        init();
        setActive(false);
        switch (ROT_AXIS)
        {
            case AXIS.X :
                m_rotAxis = Vector3.right;
                break;
            case AXIS.Y :
                m_rotAxis = Vector3.up;
                break;
            case AXIS.Z :
                m_rotAxis = Vector3.forward;
                break;
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (isActive() == true)
            rotateGear();
	}

    public void rotateGear()
    {
        transform.Rotate(m_rotAxis * m_rotSpeed * Time.deltaTime);
    }

    public override void startEvent()
    {
        setActive(true);
    }

    public override void endEvent()
    {
        setActive(false);
    }
}
