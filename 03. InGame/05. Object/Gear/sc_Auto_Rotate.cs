using UnityEngine;
using System.Collections;

public class sc_Auto_Rotate : MonoBehaviour {

    public bool Zrotate;
    public bool Yrotate;
    public bool Xrotate;

    public float rotate_speed;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Zrotate) transform.Rotate(Vector3.forward * Time.deltaTime * rotate_speed);
        if (Xrotate) transform.Rotate(Vector3.right * Time.deltaTime * rotate_speed);
        if (Yrotate) transform.Rotate(Vector3.up * Time.deltaTime * rotate_speed);
	}
}
