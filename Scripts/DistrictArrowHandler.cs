using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistrictArrowHandler : MonoBehaviour {
    public string base_url = "http://api.mapmo.de/mapmo/districts_around?";
    public float lat, lon;
    public List<GameObject> district_arrows;
    public List<DistrictArrowJsonResponse_DistrictArrow> district_arrows_loaded;
    public Map map;
    public float closest = float.MaxValue;
    
    public int closest_id;
    // Use this for initialization
	void Start () {
        if (this.map == null)
            this.map = GameObject.Find("Map").GetComponent<Map>();
        //if (this.district_arrows == null)
            this.district_arrows = new List<GameObject>();
	}
	
    public void updateData()
    {
        if (district_arrows.Count == 0 || map.dist(lat,lon) > 1000)
        {
            lat = (float) map.lat;
            lon = (float) map.lon;
            StartCoroutine(load_districts_call());
        }
        else
        {
            updateArrows();  
        }
    }

    public void updateArrows()
    {
        for (int i = 0; i < district_arrows_loaded.Count; ++i)
        {
            district_arrows_loaded[i].setDistToCenter((float)GameObject.Find("Map").GetComponent<Map>().dist(district_arrows_loaded[i].lat, district_arrows_loaded[i].lon));
            district_arrows_loaded[i].setBearing((float)GameObject.Find("Map").GetComponent<Map>().bearing(district_arrows_loaded[i].lat, district_arrows_loaded[i].lon));
        }

        for (int i = 0; i < district_arrows.Count; ++i)
        {
            district_arrows[i].GetComponent<DistrictArrow>().updatePos();
        }

        district_arrows_loaded.Sort();

        List<GameObject> del = new List<GameObject>();
        for (int i = 0; i < district_arrows_loaded.Count; ++i)
        {
            bool create = true;
            for (int j = 0; j < district_arrows.Count; ++j)
            {
                if(district_arrows[j].GetComponent<DistrictArrow>().dist_to_center==0)
                {
                    del.Add(district_arrows[j]);
                    continue;
                }
                if (district_arrows[j].GetComponent<DistrictArrow>().dist_to_center <= district_arrows_loaded[i].getDistToCenter() && Mathf.Abs(district_arrows_loaded[i].getBearing() - district_arrows[j].GetComponent<DistrictArrow>().bearing) < 25 || district_arrows_loaded[i].original_name.Equals(district_arrows[j].GetComponent<DistrictArrow>().district_name))
                {
                    create = false;
                }
                else if (district_arrows[j].GetComponent<DistrictArrow>().dist_to_center > district_arrows_loaded[i].getDistToCenter() && Mathf.Abs(district_arrows_loaded[i].getBearing() - district_arrows[j].GetComponent<DistrictArrow>().bearing) < 25)
                {
                    del.Add(district_arrows[j]);
                }

            }
            if (create)
            {
                GameObject b = Instantiate(Resources.Load("DistrictArrow"), new Vector3(0, -15, 0), Quaternion.identity) as GameObject;
                b.GetComponent<DistrictArrow>().init(district_arrows_loaded[i]);
                district_arrows.Add(b);

            }
            for (int j = 0; j < del.Count; ++j)
            {
                DestroyImmediate(del[j], true);
                district_arrows.Remove(del[j]);
            }
               
        }

        
    }

    public void load_districts(DistrictArrowJsonResponse res)
    {
        for (int i = 0; i < res.response.Length; ++i)
        {
            district_arrows_loaded.Add(res.response[i]);
            updateArrows();
            
        }
    }

    IEnumerator load_districts_call()
    {
        WWW www = new WWW(base_url + "lat=" + map.lat + "&lon=" + map.lon);
        yield return www;
        load_districts(JsonUtility.FromJson<DistrictArrowJsonResponse>(www.text));
        //DistrictArrowJsonResponse res = new DistrictArrowJsonResponse();
        //res.response = new DistrictArrowJsonResponse_DistrictArrow[1];
        //DistrictArrowJsonResponse_DistrictArrow arr = new DistrictArrowJsonResponse_DistrictArrow();
        //arr.name = "columbia";
        //arr.lat = 50.1126f;
        //arr.lon = 8.6821f;
        //res.response[0] = arr;

    }

    public void enableDistrictArrowsCollider()
    {
        for (int i = 0; i < district_arrows.Count; ++i)
        {
            //enable colliders
            if (!district_arrows[i].GetComponent<Collider>().enabled)
                district_arrows[i].GetComponent<Collider>().enabled = true;

            //adapt opacity TODO
        }
    }
    public void disableDistrictArrowsCollider()
    {
        for (int i = 0; i < district_arrows.Count; ++i)
        {
            //disable colliders for drag
            if (district_arrows[i].GetComponent<Collider>().enabled)
                district_arrows[i].GetComponent<Collider>().enabled = false;
        }
    }

	// Update is called once per frame
	void LateUpdate () {
        //float α = Camera.main.transform.eulerAngles.x;
        //if(α > 65f)
        //{
        //    if(α > 80f)
        //    {
        //        disableDistrictArrowsCollider();
        //    } else if(α < 75f && α > 65)
        //    {
        //        enableDistrictArrowsCollider();
        //    } else if(α >=75f && α < 80f)
        //    {
        //        for (int i = 0; i < district_arrows.Count; ++i)
        //        {
                    
        //            //adapt opacity TODO
        //        }
        //    }
        //}
	}
}
