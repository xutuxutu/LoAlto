using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    Vector3 m_startPos;
    Vector3 m_fireDir;
	// Use this for initialization
	
	// Update is called once per frame
	void Update ()
    {
        transform.Translate(m_fireDir * 70f * Time.deltaTime);

        float distance = Vector3.Distance(m_startPos, transform.position);

        if (distance > 15.0f)
            gameObject.SetActive(false);

        checkCollision();
    }

    public void setStartPosition(Vector3 position, Vector3 direction)
    {
        m_startPos = position;
        transform.position = position;

        m_fireDir = direction.normalized;

        transform.forward = m_fireDir.normalized;
        m_fireDir = transform.InverseTransformDirection(m_fireDir);

        checkCollision();
    }

    public void checkCollision()
    {
        RaycastHit hit;
        LayerMask mask = (1 << 8) | (1 << 10);
        mask = ~mask;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 1.0f, mask))
        {
            gameObject.SetActive(false);
        }
    }
}