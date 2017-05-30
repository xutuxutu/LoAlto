using UnityEngine;
using System.Collections;

public class Floor : MonoBehaviour
{
    public float m_rotSpeed;
    private bool m_isOverlap;
    private Quaternion m_targetPosition;

	// Use this for initialization
	void Start ()
    {
        m_isOverlap = false;
        StartCoroutine(rotate());
	}

    public void init(float rotSpeed)
    {
        m_rotSpeed = rotSpeed;
        m_isOverlap = false;
    }

    public bool isOverlap() { return m_isOverlap; }
    public void rotateStart() { m_isOverlap = false; }
    public float getRotateSpeed() { return m_rotSpeed; }

    public void oveplapFloor(float targetPos)
    {
        m_isOverlap = true;
        m_targetPosition = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y +targetPos, transform.rotation.eulerAngles.z);
        Invoke("rotateStart", 3.0f);
    }

    public IEnumerator rotate()
    {
        while (true)
        {
            if (m_isOverlap == false)
                transform.Rotate(Vector3.up * m_rotSpeed);

            else
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, m_targetPosition, Time.deltaTime * 4f);
            }

            yield return null;
        }
    }
}
