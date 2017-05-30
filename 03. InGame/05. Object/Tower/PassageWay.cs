using UnityEngine;
using System.Collections;

public class PassageWay : MonoBehaviour
{
    public float m_degreeToParents;
    float m_targetDegree;
    float m_rotSpeed;

	// Use this for initialization
	void Start ()
    {
        m_rotSpeed = transform.parent.GetComponent<Floor>().getRotateSpeed();
	}
	
	// Update is called once per frame
	void Update ()
    {
	}

    public void OnTriggerEnter(Collider coll)
    {
        if(coll.gameObject.CompareTag(TAG.PASSAGE_WAY))
        {
            PassageWay otherObj = coll.gameObject.GetComponent<PassageWay>();
            float degree = getLocalDegree() + otherObj.getLocalDegree();

            transform.parent.SendMessage("oveplapFloor", (degree / 2) - getLocalDegree());
        }
    }

    public float getRotateSpeed() { return m_rotSpeed; }
    public float getLocalDegree()
    {
        float degree;
        if (m_rotSpeed < 0)
            degree = transform.parent.rotation.eulerAngles.y + m_degreeToParents - 360;
        else
            degree = transform.parent.rotation.eulerAngles.y + m_degreeToParents;

       while(degree > 180)
          degree = degree - 360;

        while (degree < -180)
            degree = degree + 360;

        return degree;
    }

    public float calTotalSpeed(float speedA, float speedB)
    {
        if (speedA < 0)
            speedA = -speedA;
        if (speedB < 0)
            speedB = -speedB;

        return speedA + speedB;
    }
}
