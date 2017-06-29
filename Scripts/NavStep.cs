using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavStep : MonoBehaviour {
    public float lat;
    public float lon;
    public float length;
    public int step;
    public int leg;
    public int route;
    public float dist;
    public float bearing;
    public bool collides = false;
    private new Transform transform;
    private Map map;
	// Use this for initialization
	void Start () {
        transform=GetComponent<Transform>();
    }
	
	// Update is called once per frame
	void Update () {}

    public void initNavStep(float lat1, float lon1, float lat2, float lon2)
    {
        transform = GetComponent<Transform>();
        if(map==null)
            map = GameObject.Find("Map").GetComponent<Map>();
        this.lat = lat1;
        this.lon = lon1;
        bearing = (float)Map.bearing(lat, lon, lat2, lon2);
        this.length = (float)Map.dist(lat, lat2, lon, lon2) * 1000;
        transform.Rotate(Vector3.up, bearing + 180);
        transform.localScale = new Vector3(0.2f, 1, this.length / 10);
        GetComponent<Renderer>().material.mainTextureScale = new Vector2(1, Mathf.Floor(this.length/2));
        transform.position = new Vector3(0, 0, 0);
        updateNavStep();

    }

    public void initDirectionArrow(float lat1, float lon1, float lat2, float lon2)
    {
        transform = GetComponent<Transform>();
        if (map == null)
            map = GameObject.Find("Map").GetComponent<Map>();
        this.lat = lat1;
        this.lon = lon1;
        bearing = (float)Map.bearing(lat, lon, lat2, lon2);
        this.length = 10;
        transform.Rotate(new Vector3(90,bearing+270,270));
        //transform.Rotate(Vector3.left, 90);
        //transform.Rotate(Vector3.up, bearing + 180);
        //transform.Rotate(Vector3.forward, 270);
        transform.localScale = new Vector3(0.00001f, 10f, 5f);
        GetComponent<Renderer>().material.mainTextureScale = new Vector2(1, 2);
        transform.position = new Vector3(0, 4f, 0);
        updateNavStep();
    }

    public float anchorDistanceToCenter()
    {
        return (float)map.dist(lat, lon) * 1000;
    }

    public void updateNavStep()
    {
        float d = (float)map.dist(lat, lon) * 1000;
        float b1 = (float)map.bearing(lat, lon);
        transform.position = new Vector3(d * Mathf.Sin(Mathf.Deg2Rad * b1) + this.length / 2 * Mathf.Sin(Mathf.Deg2Rad * bearing), transform.position.y, d * Mathf.Cos(Mathf.Deg2Rad * b1) + this.length / 2 * Mathf.Cos(Mathf.Deg2Rad * bearing));
    }

    void OnCollisionEnter(Collision col)
    {
        
        if (col.gameObject.name.Equals("nav_collider"))
        {
            collides = true;
        }
    }

    private void OnCollisionExit(Collision col)
    {
        if (col.gameObject.name.Equals("nav_collider"))
        {
            collides = false;
        }
    }
}
