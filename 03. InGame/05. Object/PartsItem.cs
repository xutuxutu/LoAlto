using UnityEngine;
using System.Collections;

public class PartsItem : MonoBehaviour
{
    public CreatureMgr.CreatureDropItemType m_type;
    private SphereCollider m_collider;
    private Rigidbody m_rigidbody;

    private int m_gainValue = 0;
    private bool m_isContact;
    private float m_moveSpeed;
    private float m_accelSpeed;
    private bool m_gainable = false;

    public AudioSource[] m_audioSource;
	void Start ()
    {
        m_collider = GetComponent<SphereCollider>();
        m_rigidbody = GetComponent<Rigidbody>();
        Invoke("BeGainable", 0.8f);
        m_audioSource = GetComponents<AudioSource>();
    }

    public void Init(DB_DATA.CREATURE_ITEM_DATA _data)
    {
        m_type = (CreatureMgr.CreatureDropItemType)_data.ITEM_TYPE;
        m_accelSpeed = _data.CHSE_ACCL;
        m_moveSpeed = _data.CHSE_SPED;
        m_gainValue = _data.GAIN_VALU;
    }

    public void OnTriggerStay(Collider coll)
    {
        if(coll.CompareTag(TAG.CHARACTER_OWN) && m_gainable)
        {
            if (m_isContact == false)
            {
                m_isContact = true;
                // Rigidbody 끄기
                //m_rigidbody.useGravity = false;
                m_rigidbody.constraints = RigidbodyConstraints.FreezeAll;
                m_rigidbody.velocity = Vector3.zero;
                StartCoroutine("chaseCharacter");
                m_collider.radius = 0.5f;
                InGameMgr.getInstance().getOwnCharacterCtrl().getItem(m_type, m_gainValue);

                int random = UnityEngine.Random.Range(0, 1);
                m_audioSource[random].Play();
            }
            else
            {
                Destroy(transform.parent.gameObject);
            }
        }
    }

    public IEnumerator chaseCharacter()
    {
        bool isContact = false;
        float moveSpeed = m_moveSpeed;

        while (isContact == false)
        {
            Vector3 targetPosition = InGameMgr.getInstance().getOwnCharacterCtrl().transform.position + Vector3.up * 1f;
            Vector3 moveVector = (targetPosition - transform.position).normalized;

            transform.parent.position += moveVector * moveSpeed*Time.deltaTime;
            moveSpeed += m_accelSpeed * Time.deltaTime;

            yield return null;
        }
    }

    private void BeGainable()
    {
        m_gainable = true;
    }
}
