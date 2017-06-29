using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;
public class Signboard : MonoBehaviour {
    Map map;
    public Poi poi;

    double lat, lon, bearing;
    float initialScale = 0.03f;
    public float d = 0f, b = 0f, h = 3.5f;
    // Use this for initialization
    void Start () {
		
	}
    public void init(double bb_lat, double bb_lon, double bb_bearing, Poi poi)
    {
        this.poi = poi;
        lat = bb_lat;
        lon = bb_lon;
        bearing = bb_bearing;
        if (map == null)
            map = GameObject.Find("Map").GetComponent<Map>();

        //rotate billboard

        transform.localScale = new Vector3(initialScale, initialScale, initialScale);
        transform.Rotate(Vector3.up, (float)bb_bearing);
        GameObject sign = transform.Find("signCube").gameObject;
        Texture2D texture = Resources.Load("Textures/shoppingcart", typeof(Texture2D)) as Texture2D;
        sign.GetComponent<Renderer>().material.mainTexture = texture;

        updateSignboard();
    }

    public void updateSignboard()
    {
        d = 1000 * (float)map.dist(lat, lon);
        b = (float)map.bearing(lat, lon);
        transform.localScale = new Vector3(initialScale, initialScale, initialScale);
        RectTransform t = transform as RectTransform;       
        transform.position = new Vector3(d * Mathf.Sin(Mathf.Deg2Rad * b), h , d * Mathf.Cos(Mathf.Deg2Rad * b));
    }

    public void scaleOut()
    {
        StartCoroutine(scaleOutRoutine());
    }

    IEnumerator scaleOutRoutine()
    {
        while (transform.localScale.x > 0)
        {
            transform.localScale -= new Vector3(0.001f, 0.001f, 0.001f);
            yield return new WaitForSeconds(0.02f);
        }
        transform.gameObject.SetActive(false);
        StopCoroutine(this.scaleOutRoutine());
    }
    public void scaleIn()
    {
        StartCoroutine(scaleInRoutine());
    }

    IEnumerator scaleInRoutine()
    {
        while (transform.localScale.x < initialScale)
        {
            transform.localScale += new Vector3(0.001f, 0.001f, 0.001f);
            yield return new WaitForSeconds(0.02f);
        }
        StopCoroutine(this.scaleInRoutine());
    }


    // Update is called once per frame
    void Update () {
		
	}
}
