using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompassTextScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void LateUpdate () {
        transform.LookAt(Camera.main.transform.position);
        transform.rotation = Quaternion.Euler(90, transform.rotation.eulerAngles.y + 180, 0);
	}
}
