using UnityEngine;
using System.Collections;

public class FoldingFootHold : MonoBehaviour
{
    public enum FOLDING_STATE { FOLDING, UNFOLDING };
    public enum WORKING_STATE { WORKING, READY };

    public Transform m_platform;
    public float m_foldingDegree;
    public float m_unFoldingDegree;

    public float m_foldingTime;
    public float m_unFoldingTime;

    public float m_foldingSpeed;

    private Quaternion m_foldingRotation;
    private Quaternion m_unFoldingRotation;

    private FOLDING_STATE m_foldingState;
    private WORKING_STATE m_workingState;
    // Use this for initialization
    void Start ()
    {
        m_foldingState = FOLDING_STATE.UNFOLDING;
        m_workingState = WORKING_STATE.READY;

        m_foldingRotation = Quaternion.Euler(0, 0, m_foldingDegree - 5);
        m_unFoldingRotation = Quaternion.Euler(0, 0, m_unFoldingDegree + 5);
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (m_workingState == WORKING_STATE.WORKING)
        {
            switch (m_foldingState)
            {
                case FOLDING_STATE.FOLDING:
                    folding();
                    break;
                case FOLDING_STATE.UNFOLDING:
                    unfolding();
                    break;
            }
        }
	}

    public void OnTriggerEnter(Collider coll)
    {
        if (m_foldingState == FOLDING_STATE.UNFOLDING)
        {
            if (m_workingState == WORKING_STATE.READY)
            {
                m_foldingState = FOLDING_STATE.FOLDING;
                Invoke("setWorking", m_foldingTime);
            }
        }
    }

    public void startEvent()
    {

    }

    public void endEvent()
    {

    }

    public void folding()
    {

        if (Quaternion.Angle(m_platform.transform.rotation, m_foldingRotation) < 5)
        {
            m_platform.transform.rotation = Quaternion.Euler(0, 0, m_foldingDegree);
            setState(FOLDING_STATE.UNFOLDING, WORKING_STATE.READY);
            Invoke("setWorking", m_unFoldingTime);
        }
        else
        {
            m_platform.Rotate(-Vector3.forward * m_foldingSpeed * Time.deltaTime);
        }

    }

    public void unfolding()
    {
        if (Quaternion.Angle(m_platform.transform.rotation, m_unFoldingRotation) < 5)
        {
            m_platform.transform.rotation = Quaternion.Euler(0, 0, m_unFoldingDegree);
            m_workingState = WORKING_STATE.READY;
        }
        else
        {
            m_platform.Rotate(Vector3.forward * m_foldingSpeed * Time.deltaTime);
        }
    }

    public void setState(FOLDING_STATE foldingState, WORKING_STATE workingState)
    {
        m_foldingState = foldingState;
        m_workingState = workingState;
    }

    public void setWorking()
    {
        m_workingState = WORKING_STATE.WORKING;
    }
}
