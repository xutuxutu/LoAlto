using UnityEngine;
using System.Collections.Generic;

public class AttackArea_SteamBlow : MonoBehaviour
{
    private MeshCollider m_collider;
    private List<GameObject> m_attackedCreature;
    private float m_atkPoint = 30f;
    private Transform m_character;
    // Use this for initialization

    public void init(Transform characterTransform, float atkPoint, float width, float height)
    {
        m_atkPoint = atkPoint;

        Mesh ms = new Mesh();
        ms.vertices = new Vector3[]
        {
             //윗면
            new Vector3(width / 2, 1, 0),           //Root 0
            new Vector3(-width / 2, 1, 0),           //Root 1
            new Vector3(-0.8f - width, 1, 0.8f + height), //2
            new Vector3(-0.5f - width, 1, 1.1f + height), //3

            new Vector3(0, 1, 1.3f + height), //4
            new Vector3(0.5f + width, 1, 1.1f + height), //5
            new Vector3(0.8f + width, 1, 0.8f + height), //6
            //아랫면
            new Vector3(width / 2, 0, 0), //7
            new Vector3(-width / 2, 0, 0), //8
            new Vector3(-0.8f - width, 0, 0.8f + height), //9
            new Vector3(-0.5f - width, 0, 1.1f + height), //10

            new Vector3(0, 0, 1.3f + height), //11
            new Vector3(0.5f + width, 0, 1.1f + height),//12
            new Vector3(0.8f + width, 0, 0.8f + height),//13
        };

        ms.triangles = new int[] { 1,2,3, 0,1,3, 0,3,4, 0,4,5, 0,5,6 };
        ms.RecalculateBounds();

        m_collider = GetComponent<MeshCollider>();
        m_collider.sharedMesh = null;
        m_collider.sharedMesh = ms;
        //m_collider.enabled = false;

        m_character = characterTransform;
        m_attackedCreature = new List<GameObject>();
        gameObject.SetActive(false);
    }
    
    public void OnTriggerEnter(Collider coll)
    {
        if (gameObject.activeSelf == false)
            return;

        if (coll.transform.root.gameObject.tag == TAG.CREATURE)
        {
            if (m_attackedCreature.Contains(coll.transform.root.gameObject))
                return;
            else
            {
                m_attackedCreature.Add(coll.transform.root.gameObject);
                coll.transform.root.gameObject.GetComponent<Creature>().damaged(ProjectMgr.getInstance().getOwnID(), (int)m_atkPoint, m_character.position);
            }
        }
    }

    public void resetList()
    {
        if (m_attackedCreature.Count > 0)
            m_attackedCreature.Clear();
    }

    public void active()
    {
        gameObject.SetActive(true);
        //m_collider.enabled = true;

        Invoke("deActive", 0.2f);
    }

    public void deActive()
    {
        resetList();
        gameObject.SetActive(false);
        //m_collider.enabled = false;
    }
}
