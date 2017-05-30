using UnityEngine;
using System.Collections;

public class FootholdTrap : MonoBehaviour
{
    public enum TRAP_STATE { READY, RUN, STOP }
    public Transform m_foothold;
    public Rigidbody m_trap;
    public float m_footholdDown;

    private TRAP_STATE m_state;
    private GameObject m_collider;
	void Start ()
    {
        m_trap.useGravity = false;
        m_state = TRAP_STATE.READY;
        m_collider = null;
	}
	
	// Update is called once per frame
	void Update ()
    {

	}

    public void OnTriggerEnter(Collider coll)
    {
        if (m_collider == null)
        {
            m_foothold.position -= Vector3.up * m_footholdDown;
            m_trap.useGravity = true;
            m_collider = coll.gameObject;
        }
    }

    public void OnTriggerExit(Collider coll)
    {
        if (coll.gameObject == m_collider)
        {
            m_foothold.position += Vector3.up * m_footholdDown;
            m_collider = null;
        }
    }
}
