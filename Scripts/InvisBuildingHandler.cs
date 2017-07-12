using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

public class InvisBuildingHandler : MonoBehaviour {
    public string base_url = "http://api.mapmo.de/mapmo/location_buildings?";
    public List<InvisBuilding> all_buildings;
    public float del_radius = 0.2f;
    public float update_radius = 0.05f;
    public Map map;
    public double lat_last, lon_last;
    public float smaller = 0.5f; // make occlusion 50cm smaller than building
    public float prec = 1000;
    private Transform building_container;
    public PoiHandler poi_handler;

    public void addPois(List<Poi> pois)
    {
        for(int i = 0; i < all_buildings.Count; ++i)
        {
            all_buildings[i].GetComponent<InvisBuilding>().addPois();
        }
    }

    public void initHandler(Map map)
    {
        if (building_container == null)
            building_container = GameObject.Find("BuildingContainer").transform;
        if(all_buildings==null)
            all_buildings = new List<InvisBuilding>();
    }

    public void removeBuildings()
    {
        List<InvisBuilding> del = new List<InvisBuilding>();
        for(int i= 0; i < all_buildings.Count; ++i)
        {
            if (map.dist(all_buildings[i].lat, all_buildings[i].lon) > del_radius)
                del.Add(all_buildings[i]);       
        }
        for(int i = 0; i < del.Count; ++i) {
            GameObject b = del[i].gameObject;
            all_buildings.Remove(del[i]);
            del[i].kill();
            DestroyImmediate(b, true);
        }
    }

    public void updateBuildings()
    {
        if (map.dist(lat_last, lon_last) > update_radius) {
            removeBuildings();
            StartCoroutine(loadBuildingsCall(lat_last,lon_last));
            lat_last = map.lat;
            lon_last = map.lon;
        }
        for(int i = 0; i < all_buildings.Count; ++i)
        {
            all_buildings[i].updateInvisBuilding();

        }
    }

    IEnumerator loadBuildingsCall(double lat_last, double lon_last)
    {
        WWW www = new WWW(base_url + "lat=" + map.lat + "&lon=" + map.lon + "&lat_last=" + lat_last + "&lon_last=" + lon_last);
        yield return www;
        assignNewBuildings(www.text);
    }

    public void assignNewBuildings(string data)
    {
        ClipperLib.ClipperOffset co = new ClipperLib.ClipperOffset();
        InvisBuildingJsonRepsonse res =  JsonUtility.FromJson<InvisBuildingJsonRepsonse>(data);
        for(int i= 0; i < res.response.Length; ++i)
        {
            bool duplicate = false;
            for(int j=0; j < all_buildings.Count; ++j)
            {
                if (res.response[i].id == all_buildings[j].id)
                    duplicate = true;
            }
            if (!duplicate) {


                // invisible building and fade building
                GameObject fb = Instantiate(Resources.Load("FadeBuilding"), new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                fb.transform.SetParent(building_container);
                List<ClipperLib.IntPoint> path = new List<ClipperLib.IntPoint>();
                for (int k = 0; k < res.response[i].building.Length; k  += 2)
                {
                    path.Add(new ClipperLib.IntPoint(res.response[i].building[k] * prec, res.response[i].building[k + 1] * prec));
                }

                co.AddPath(path, ClipperLib.JoinType.jtRound, ClipperLib.EndType.etClosedPolygon);
                List<List<ClipperLib.IntPoint>> l = new List<List<ClipperLib.IntPoint>>();
                co.Execute(ref l, -smaller*prec);
                co.Clear();
                InvisBuildingJsonRepsonse_Building ibb = new InvisBuildingJsonRepsonse_Building();
                ibb.startPoint = res.response[i].startPoint;
                ibb.id = res.response[i].id;
                ibb.building = new float[l[0].Count*2];
                //ibb.building = 

                for (int k = 0; k < l[0].Count; ++k)
                {
                    ibb.building[k * 2] = l[0][k].X / prec;
                    ibb.building[k * 2 + 1] = l[0][k].Y / prec;
                }
                

                fb.GetComponent<InvisBuilding>().init(res.response[i], poi_handler);
                
                all_buildings.Add(fb.GetComponent<InvisBuilding>());
            }
        }
     }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
