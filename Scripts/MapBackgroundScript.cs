using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBackgroundScript : MonoBehaviour {


    // Use this for initialization
    void Start () {
        GetComponent<Renderer>().material.SetFloat("_Cutoff", 12f);
    }
	

}
