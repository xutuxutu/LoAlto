using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class C4_Bomb : MonoBehaviour
{
    public static float m_atkPoint = 40;
    public static float m_movePower = 1800;
    public static float m_atkRange = 6;

    private bool m_possibleDetonate;
    private float m_timeLimit = 5f;
    private List<GameObject> m_targetCreatureList;
    private SphereCollider m_trigger;

    public void init()
    {
        m_targetCreatureList = new List<GameObject>();
        SphereCollider[] coll = GetComponents<SphereCollider>();
        for(int i = 0; i < coll.Length; ++i)
        {
            if(coll[i].isTrigger == true)
            {
                m_trigger = coll[i];
                break;
            }
        }
    }
    
    public static void initStatus(float atkPoint, float movePower, float atkRange)
    {
        m_atkPoint = atkPoint;
        m_movePower = movePower;
        m_atkRange = atkRange;
    }
    public void setMoveVector(Vector3 startPosition, Vector3 moveVector)
    {
        transform.position = startPosition;
        m_trigger.radius = m_atkRange;

        if (ProjectMgr.getInstance().getOwnCharacterType() == CHARACTER.TYPE.SPARKY)
        {
            if(GetComponent<Rigidbody>() == null)
                gameObject.AddComponent<Rigidbody>();
            GetComponent<Rigidbody>().drag = 1.3f;
            GetComponent<Rigidbody>().AddForce(moveVector * m_movePower);

            Invoke("detonationPossible", 0.5f);
            StartCoroutine("checkTimeLimit");
        }
    }

    public IEnumerator checkTimeLimit()
    {
        float curTime = 0f;
        while(curTime < m_timeLimit)
        {
            curTime += 0.03f;
#if SERVER_ON
            Vector3 rotation = transform.rotation.eulerAngles;
            InGameServerMgr.getInstance().SendPacket_UDP(NET_INGAME.SEND.PACKET_TYPE.REQUEST_CHARCATER_SPARKY_C4_BOMB_TRANSFORM_INFO_EM, 
                transform.position.x, transform.position.y, transform.position.z, rotation.x, rotation.y, rotation.z);
#endif
            yield return new WaitForSeconds(0.03f);
        }

        InGameMgr.getInstance().getOwnCharacterCtrl().SendMessage("skill_C4Bomb_Detonate");
    }

    public bool isPossibleDetonation() { return m_possibleDetonate; }

    public void detonationPossible() { m_possibleDetonate = true; }
    public void detonation()
    {
        if (ProjectMgr.getInstance().getOwnCharacterType() == CHARACTER.TYPE.SPARKY)
        {
            if (m_possibleDetonate == true)
            {
                StopCoroutine("checkTimeLimit");
                gameObject.SetActive(false);

                if (ProjectMgr.getInstance().getOwnCharacterType() == CHARACTER.TYPE.SPARKY)
                {
                    for (int i = 0; i < m_targetCreatureList.Count; ++i)
                        m_targetCreatureList[i].GetComponent<Creature>().damaged(ProjectMgr.getInstance().getOwnID(), (int)m_atkPoint, transform.position);
                }
                m_targetCreatureList.Clear();
                Vector3 detonatePos = transform.position + (-Vector3.up) * 0.5f;
                ObjectPool_Sparky.getInstance().printC4BombExplosion(detonatePos);
                InGameMgr.getInstance().getOwnCharacterCtrl().camShakeEvent(CAM_SHAKE_EVENT.TYPE.SPARKY_C4, 1f, true, 0.15f);
#if SERVER_ON
                InGameServerMgr.getInstance().SendPacket(NET_INGAME.SEND.PACKET_TYPE.REQUEST_CHARCATER_SPARKY_C4_BOMB_DETONATION_ORDER_EM, detonatePos.x, detonatePos.y, detonatePos.z);
#endif
            }
        }
    }

    public void OnTriggerEnter(Collider coll)
    {
        if (coll.transform.root.CompareTag(TAG.CREATURE))
        {
            if (m_targetCreatureList.Contains(coll.gameObject) == false)
                m_targetCreatureList.Add(coll.transform.root.gameObject);
        }
    }

    public void OnTriggerExit(Collider coll)
    {
        if (coll.transform.root.CompareTag(TAG.CREATURE))
        {
            if (m_targetCreatureList.Contains(coll.gameObject) == true)
                m_targetCreatureList.Remove(coll.transform.root.gameObject);
        }
    }
}
