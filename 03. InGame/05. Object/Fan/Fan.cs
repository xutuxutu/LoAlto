using UnityEngine;
using System.Collections;

public struct SEND_DATA
{
    public float force;
    public float fallSpeedCorrection;
}
public class Fan : MonoBehaviour
{
    public float ROT_SPEED;
    public float STAY_FORCE;
    public float FALL_SPEED_CORRECTION;
    private SEND_DATA m_sendData;
	// Use this for initialization
	void Start ()
    {
        m_sendData.force = STAY_FORCE;
        m_sendData.fallSpeedCorrection = FALL_SPEED_CORRECTION;
	}
	
	// Update is called once per frame
	void Update ()
    {
        rotate();
	}

    public void rotate()
    {
        transform.Rotate(Vector3.up * ROT_SPEED);
    }

    public void blowUp(GameObject target)
    {
       target.SendMessage("riseUp", m_sendData);
    }

    public void OnTriggerStay(Collider coll)
    {
        if (coll.tag == TAG.CHARACTER_OWN)
            blowUp(coll.gameObject);
    }
}
