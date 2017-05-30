using UnityEngine;
using System.Collections;

public class Drum : MonoBehaviour
{

	// Use this for initialization
	void Start ()
    { 
	}
	
	// Update is called once per frame
	void Update ()
    {
        GetComponent<Rigidbody>().AddTorque(Vector3.right, ForceMode.Acceleration);
	}
}
