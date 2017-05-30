using UnityEngine;
using System.Collections;

public class CamTime : MonoBehaviour {

	public bool paused;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (!paused) {
			Time.timeScale = 0;
				
		} else {
			Time.timeScale = 1;
		}
	}
	}
