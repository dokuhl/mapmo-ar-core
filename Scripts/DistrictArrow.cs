using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class DistrictArrow : MonoBehaviour, IPointerClickHandler {
    public float lat;
    public float lon;
    public string district_name;
    public string skybox_img;
    public float dist_to_game_center = 14.5f;
    public float dist_to_center = 0;
    public float offset_x = 0;
    public float bearing;
    private new Transform transform;
    private Map map;
	// Use this for initialization
	void Start () {
        transform = GetComponent<Transform>();
        if (map == null)
            map = GameObject.Find("Map").GetComponent<Map>();
    }

    public override bool Equals(System.Object obj)
    {
        if (obj == null)
        {
            return false;
        }
        DistrictArrow p = obj as DistrictArrow;
        return Equals(this.lat, p.lat) && Equals(this.lon, p.lon) && Equals(this.district_name, p.district_name);
    }

    public void init(DistrictArrowJsonResponse_DistrictArrow arrow)
    {
        transform = GetComponent<Transform>();
        if (map == null)
            map = GameObject.Find("Map").GetComponent<Map>();
        this.lat = arrow.lat;
        this.lon = arrow.lon;
        this.district_name = arrow.original_name;
        this.skybox_img = arrow.skybox_image;
        this.offset_x = arrow.image_offset;
        transform.FindChild("txt_district").GetComponent<TextMesh>().text = district_name;
        updatePos();
    }

    public float updatePos( )
    {
        bearing = (float)map.bearing(lat, lon);
        dist_to_center = (float)map.dist(lat, lon);
        transform.position = new Vector3(Mathf.Sin(Mathf.Deg2Rad * bearing) * dist_to_game_center, 0.7f , Mathf.Cos(Mathf.Deg2Rad * bearing) * dist_to_game_center);
        transform.eulerAngles = new Vector3(0, bearing+180, 0);//new Quaternion(0, 0, 0, 0);
        //transform.Rotate(Vector3.up, bearing + 180);
        transform.FindChild("txt_distance").GetComponent<TextMesh>().text =  Mathf.Round(dist_to_center * 1000) + "m";
        return dist_to_center;

    }


	// Update is called once per frame
	void Update () {
		
	}

    public void OnPointerClick(PointerEventData eventData)
    {
        map.gps_fixed = true;
        Debug.Log(map.lat +" " +GameObject.Find("Map").GetComponent<Map>().lat);
        if (map.orig_lat == 0f && map.orig_lon == 0f)
        {
            map.orig_lat = (float)map.lat;
            map.orig_lon = (float)map.lon;
        }
        map.lat = this.lat;
        map.lon = this.lon;
        map.updateCenter();

        float offset = (360 + this.offset_x) / 360;
        GameObject.Find("skybox_container").GetComponent<Skybox>().showDistrict(this.skybox_img, offset);
        GameObject.Find("DistrictArrowHandler").GetComponent<DistrictArrowHandler>().enableDistrictArrowsCollider();
    }
}
