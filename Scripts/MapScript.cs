using System.Collections;
using System.Collections.Generic;
using DeadMosquito.AndroidGoodies;
using UnityEngine;

public class MapScript : MonoBehaviour {
    string url = "";
    double lat;
    double lon;
    LocationInfo li;
    public GameObject go;

    void Start()
    {
        GetComponent<Renderer>().material.SetFloat("_Cutoff", 10f);
        // Minimum time interval between location updates, in milliseconds.
        const long minTimeInMillis = 200;
        // Minimum distance between location updates, in meters.
        const float minDistanceInMetres = 1;
        AGGPS.RequestLocationUpdates(minTimeInMillis, minDistanceInMetres, OnLocationChanged);
        li = new LocationInfo();

        //lat = li.latitude; lon = li.longitude;
        
        loadTile();
        test();
        
    }

    

    private float[] tilepos(double lon, double lat, int zoom)
    {
            float[] pos = new float[2];
            //x
            pos[0] = (float)((lon + 180.0) / 360.0 * (1 << zoom));
            //y
            pos[1] = (float)((1.0 - System.Math.Log(System.Math.Tan(lat * System.Math.PI / 180.0) +
            1.0 / System.Math.Cos(lat * System.Math.PI / 180.0)) / System.Math.PI) / 2.0 * (1 << zoom));

            return pos;
    }

    void test()
    {
        int a = 0;
        StartCoroutine(loadTile());
    }

    IEnumerator loadTile()
    {
        int zoom = 18;
        double lat = 51.0292021;
        double lon = 13.7194791;

        float[] pos = tilepos(lon, lat, zoom);
        
        // set tile pos
        float p_x = (pos[0] % 1) * ((GetComponent<Renderer>().transform.localScale.x * 10));
        float p_y = -1 * (pos[1] % 1) * ((GetComponent<Renderer>().transform.localScale.z * 10));
        GetComponent<Renderer>().transform.position = new Vector3(p_x, -15, p_y);

        // load tile texture
        url = "http://tile.openstreetmap.org/" + ("" + zoom + "/" + Mathf.Floor(pos[0]) + "/" + Mathf.Floor(pos[1])) + ".png";
        WWW www = new WWW(url);
        yield return www;
        GetComponent<Renderer>().material.mainTexture = www.texture;
    }

    private void OnLocationChanged(AGGPS.Location location)
    {
        lat = location.Latitude;
        lon = location.Longitude;
        loadTile();
    }

    // Update is called once per frame
    void Update () {
		
	}
}
