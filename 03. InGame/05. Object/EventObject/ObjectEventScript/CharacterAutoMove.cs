using UnityEngine;
using System.Collections;
using System;

public class CharacterAutoMove : ObjectEvent
{
    public CHARACTER.TYPE m_characterType;
    public GameObject m_directionCamera;

    public GameObject[] m_wayPoint;
    private float m_moveSpeed;
    private Transform m_character;
    private CharacterController m_characterCtrl;
    private Animator m_characterAnimCtrl;
     
	// Use this for initialization
	void Start ()
    {
        init();
	}

    public void setCharacter()
    {
        if (ProjectMgr.getInstance().getOwnCharacterType() == m_characterType)
        {
            m_character = InGameMgr.getInstance().getOwnCharacterCtrl().transform.parent;
            m_moveSpeed = InGameMgr.getInstance().getOwnCharacterCtrl().getMoveSpeed();

            m_characterCtrl = m_character.GetComponent<CharacterController>();
            m_characterAnimCtrl = m_character.GetComponentInChildren<Animator>();
            Debug.Log(m_characterAnimCtrl);
        }
    }

    public override void startEvent()
    {
        if(isActive())
        {
            setEventState(ObjectState.EVENT_STATE.WORKING);
            setCharacter();
            if (InGameMgr.getInstance().getOwnCharacterCtrl().getCharacterType() == m_characterType)
                Invoke("autoMove", invokeTime);
        }
    }

    public override void endEvent()
    {
    }

    public void autoMove()
    {
        StartCoroutine("move");
        Invoke("startDirection", 0.7f);
    }

    public void startDirection()
    {
        m_directionCamera.GetComponent<Animator>().SetTrigger("active");
    }

    public IEnumerator move()
    {
        int curWayPoint = 0;
        bool arrive = false;

        while(arrive == false)
        {
            float dist = Vector3.Distance(m_character.position, m_wayPoint[curWayPoint].transform.position);
            if(dist < 1)
            {
                curWayPoint += 1;
                if (curWayPoint >= m_wayPoint.Length)
                {
                    arrive = true;
                    InGameMgr.getInstance().getOwnCharacterCtrl().setMoveAnim(0, 0, false);
                    continue;
                }
            }
            Vector3 forward = (m_wayPoint[curWayPoint].transform.position - m_character.position).normalized;

            forward = new Vector3(forward.x, 0, forward.z);

            m_character.forward = Vector3.Lerp(m_character.forward, forward, Time.deltaTime * 10f);
            InGameMgr.getInstance().getOwnCharacterCtrl().setMoveAnim(forward.z, forward.x, true);
            m_characterCtrl.Move(forward * m_moveSpeed * Time.deltaTime);
            yield return null;
        }
        setEventState(ObjectState.EVENT_STATE.READY);
    }
}
