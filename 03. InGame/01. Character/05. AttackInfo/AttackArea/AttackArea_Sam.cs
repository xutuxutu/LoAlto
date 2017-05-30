using UnityEngine;
using System.Collections.Generic;

public class AttackArea_Sam : MonoBehaviour
{
    private Collider m_collider;
    private List<GameObject> m_attackedCreature;
    private float m_atkPoint;
    private int m_atkType;
    private Character_Own_Sam m_characterCtrl;
    // Use this for initialization

    public void init(Character_Own_Sam characterCtrl)
    {
        m_characterCtrl = characterCtrl;
        m_attackedCreature = new List<GameObject>();
        m_collider = GetComponent<Collider>();
        m_collider.enabled = false;
    }

    public void OnTriggerEnter(Collider coll)
    {
        if (m_collider.enabled == false)
            return;

        if (coll.transform.root.gameObject.tag == TAG.CREATURE)
        {
            if (m_attackedCreature.Contains(coll.transform.root.gameObject))
                return;
            else
            {
                if (m_atkType == (int)CHARACTER_SAM.ATTACK_TYPE.STEAM_BLOW)
                {
                    m_characterCtrl.skill_SteamBlow_Contact();
                }

                m_attackedCreature.Add(coll.transform.root.gameObject);
                coll.transform.root.gameObject.GetComponent<Creature>().damaged(ProjectMgr.getInstance().getOwnID(), (int)m_atkPoint, m_characterCtrl.transform.position);

                if (m_atkType < 4)
                {
                    if(coll.transform.root.gameObject.GetComponent<Creature>().GetCreatureType() == (int)CreatureMgr.CreatureType.OWL)
                        m_characterCtrl.printCreatureHitEffet(transform.position, 1);
                    else
                        m_characterCtrl.printCreatureHitEffet(coll.transform.root.position + Vector3.up, 1);
                }
            }
        }
    }

    public void resetList()
    {
        if (m_attackedCreature.Count > 0)
            m_attackedCreature.Clear();
    }

    public void setActive(bool isActive)
    {
        if (isActive == false)
            resetList();

        m_collider.enabled = isActive;
    }

    public void setAttackPoint(float atkPoint, int atkType)
    {
        m_atkPoint = atkPoint;
        m_atkType = atkType;
    }

    public bool isActive() { return m_collider.enabled; }
}
