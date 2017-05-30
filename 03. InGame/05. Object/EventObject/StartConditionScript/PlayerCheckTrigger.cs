using UnityEngine;
using System.Collections;
using System;

public class PlayerCheckTrigger : StartCondition
{
    private bool m_existPlayerOwn;
    private bool m_existPlayerOther;

	// Use this for initialization
	void Start ()
    {
        init();
        m_existPlayerOwn = false;
        m_existPlayerOther = false;
	}


    public void OnTriggerEnter(Collider coll)
    {
        if (isActive() == false)
            return;

        if (coll.CompareTag(TAG.CHARACTER_OWN))
        {
            if (m_existPlayerOwn == false)
            {
                Debug.Log("check");
                m_existPlayerOwn = true;
                checkStartCondition();
            }
        }

        if(coll.CompareTag(TAG.CHARACTER_OTHER))
        {
            if (m_existPlayerOther == false)
            {
                m_existPlayerOther = true;
                checkStartCondition();
            }
        }
    }

    public void OnTriggerExit(Collider coll)
    {
        if (isActive() == false)
            return;

        if (coll.CompareTag(TAG.CHARACTER_OWN))
        {
            if (m_existPlayerOwn == true)
            {
                m_existPlayerOwn = false;
                setNotReady();
            }
        }

        if (coll.CompareTag(TAG.CHARACTER_OTHER))
        {
            if (m_existPlayerOther == true)
            {
                m_existPlayerOther = false;
                setNotReady();
            }
        }
    }

    public void checkStartCondition()
    {
#if SERVER_ON
        if (m_existPlayerOwn == true && m_existPlayerOther == true)
            setReady();
#else
        if (m_existPlayerOwn == true)
            setReady();
#endif
        else
            setNotReady();
    }

    public override void reset()
    {
        setNotReady();
        m_existPlayerOwn = false;
        m_existPlayerOther = false;
    }
}
