using UnityEngine;
using System.Collections.Generic;

public class AttackArea_Pulverlize : MonoBehaviour
{
    private float m_atkRange = 2f;
    private float m_atkPoint = 30f;

    private List<GameObject> m_targetCreatureList;
    private SphereCollider m_atkTrigger;
    private Transform m_character;

    // Use this for initialization
    public void init (Transform characterTransform, float atkRanage, float atkPoint)
    {
        m_targetCreatureList = new List<GameObject>();
        m_atkTrigger = GetComponent<SphereCollider>();
        m_atkTrigger.radius = m_atkRange;

        m_atkPoint = atkPoint;
        m_character = characterTransform;
        gameObject.SetActive(false);
    }

    public void OnTriggerEnter(Collider coll)
    {
        if(coll.CompareTag(TAG.CREATURE))
        {
            if (m_targetCreatureList.Contains(coll.gameObject) == false)
            {
                m_targetCreatureList.Add(coll.gameObject);
                coll.transform.root.GetComponent<Creature>().damaged(ProjectMgr.getInstance().getOwnID(), (int)m_atkPoint, m_character.position);
            }
        }
    }

    public void activeTrigger()
    {
        gameObject.SetActive(true);
        Invoke("deActiveTrigger", 0.3f);
    }

    public void deActiveTrigger()
    {
        if (m_targetCreatureList.Count > 0)
            m_targetCreatureList.Clear();       

        gameObject.SetActive(false);
    }
}
