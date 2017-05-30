using UnityEngine;
using System.Collections;

public class ExplosionBulletHit : MonoBehaviour
{
    public static float m_atkPoint = 35;
    public static float m_atkRange = 5;

    public float m_playTime;
    private AudioSource m_hitSound;
    private SphereCollider m_damageTrigger;

    public Transform[] m_effectObject;

    private int m_effectNum;
    private float[] m_effectMoveSpeed;
    private float[] m_effectDirection;

    public void init()
    {
        m_hitSound = GetComponent<AudioSource>();
        m_damageTrigger = GetComponent<SphereCollider>();

        m_effectNum = m_effectObject.Length;
        m_effectMoveSpeed = new float[m_effectNum];
        m_effectDirection = new float[m_effectNum];
    }

    public static void initStatus(float atkPoint, float atkRange)
    {
        m_atkPoint = atkPoint;
        m_atkRange = atkRange;
    }

    public void setPosition(Vector3 position)
    {
        transform.position = position;

        m_damageTrigger.radius = m_atkRange;
        m_damageTrigger.enabled = true;

        setEffectInfo();
        StartCoroutine("moveEffectPosition");

        if (m_hitSound != null)
            m_hitSound.Play();
        Invoke("deActiveCollider", 0.5f);
        Invoke("deActive", m_playTime);
    }

    public void setEffectInfo()
    {
        for (int i = 0; i < m_effectNum; i++)
        {
            m_effectMoveSpeed[i] = Random.Range(8f, 10f);
            m_effectDirection[i] = Random.Range(0f, 360f);
        }

        for(int i = 0; i < m_effectNum; i++)
        {
            m_effectObject[i].Rotate(Vector3.forward * m_effectDirection[i]);
        }
    }

    public IEnumerator moveEffectPosition()
    {
        while (true)
        {
            for(int i = 0; i < m_effectNum; i++)
            {
                m_effectObject[i].Translate(Vector3.up * m_effectMoveSpeed[i] * Time.deltaTime);
            }
            yield return null;
        }
    }

    public void deActive()
    {
        for(int i = 0; i < m_effectObject.Length; ++i)
        {
            m_effectObject[i].localPosition = Vector3.zero;
            m_effectObject[i].localRotation = Quaternion.Euler(Vector3.zero);
        }
        gameObject.SetActive(false);
    }

    public void OnTriggerEnter(Collider coll)
    {
        if (ProjectMgr.getInstance().getOwnCharacterType() == CHARACTER.TYPE.SPARKY)
        {
            if (coll.transform.root.CompareTag(TAG.CREATURE))
            {
                coll.transform.root.GetComponent<Creature>().damaged(ProjectMgr.getInstance().getOwnID(), (int)m_atkPoint, transform.position);
            }
        }
    }

    public void deActiveCollider()
    {
        m_damageTrigger.enabled = false;
    }
}
