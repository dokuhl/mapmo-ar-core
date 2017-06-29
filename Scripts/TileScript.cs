using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour {
    public float t_x, t_y, z, s_x, s_z;
    private Transform renderer_transform;
	// Use this for initialization
	void Start () {
        renderer_transform = GetComponent<Renderer>().transform;
    }

    public void init(float z, float t_x, float t_y, float c_x, float c_y)
    {
        renderer_transform = GetComponent<Renderer>().transform;
        //load tile texture in background
        StartCoroutine(loadTile(z, t_x, t_y));

        // rotate tile to north
        renderer_transform.Rotate(Vector3.up, 180);
        s_x = ((renderer_transform.localScale.x * 10));
        s_z = ((renderer_transform.localScale.z * 10));

        // set tile pos
        this.t_x = t_x;
        this.t_y = t_y;
        moveTile(c_x, c_y);
    }

    IEnumerator loadTile(float z, float x, float y)
    {
        // DeadMosquito.AndroidGoodies.AGUIMisc.ShowToast(z + "_" + x + "_" + y, DeadMosquito.AndroidGoodies.AGUIMisc.ToastLength.Short);
        // load tile texture https://api.mapbox.com/v4/mapbox.streets/3/2/3.png32?access_token=your-access-token
        WWW www = new WWW("https://api.mapbox.com/v4/mapbox.streets/" + z + "/" + x + "/" + y + ".png32?access_token=pk.eyJ1IjoicGhpbGlwMTMxIiwiYSI6InJsSDE4LWsifQ.qAjbT5HjX4kuZ7pvlNw-FQ");
        yield return www;
        GetComponent<Renderer>().material.mainTexture = www.texture;
    }

    public void moveTile(float c_x, float c_y)
    {
        float p_x = -1 * (c_x % 1) * s_x + s_x * (t_x - Mathf.Floor(c_x)) + s_x/2;
        float p_y =  (c_y % 1) * s_z + s_z * (Mathf.Floor(c_y) - t_y) - s_z/2 ;
        renderer_transform.position = new Vector3(p_x, -15, p_y);
    }

    
    // Update is called once per frame
    void Update () {
        //GetComponent<Renderer>().transform.Translate
    }
}
