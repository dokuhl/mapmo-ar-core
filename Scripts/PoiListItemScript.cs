using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoiListItemScript : MonoBehaviour {
    public int id;
    public PoiNameBarScript poi_name_bar_script;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void openPoi()
    {
        poi_name_bar_script.openPoi(id);
    }
}
