using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

public class PoiHandler : MonoBehaviour {
    public Map map;
    string base_url = "http://api.mapmo.de/mapmo/bv_pois?";
    public float del_radius = 0.2f;
    public float update_radius = 0.05f;
    public double lat_last, lon_last;
    public List<Poi> pois;
    public BillboardDetailScript billboard_detail_script;
    public InvisBuildingHandler invis_building_handler;

	// Use this for initialization
	void Start () {
        pois = new List<Poi>();
    }
	
    public void showBillboard(Poi poi, bool called_from_poi_name_bar)
    {
        billboard_detail_script.show(poi, called_from_poi_name_bar);
    }
    public void showBillboard(Poi poi)
    {
        billboard_detail_script.show(poi);
    }

    public void hideBillBoard(Poi poi)
    {
        billboard_detail_script.hide();
    }

    public void updatePois()
    {

        if (map.dist(lat_last, lon_last) > update_radius)
        {
            removePois();
            StartCoroutine(loadPoisCall(lat_last,lon_last));
            lat_last = map.lat;
            lon_last = map.lon;
        }
        for (int i = 0; i < pois.Count; ++i)
        {
            pois[i].updatePoi(0);

        }
    }

    void loadPois(string data)
    {
        PoiJsonResponse res = JsonUtility.FromJson<PoiJsonResponse>(data);
        if (res != null)
        {
            for (int i = 0; i < res.response.Length; ++i)
            {
                bool duplicate = false;
                for (int j = 0; j < pois.Count; ++j)
                {
                    if (res.response[i].id == pois[j].id)
                        duplicate = true;
                }
                if (!duplicate)
                {
                    pois.Add(new Poi(res.response[i]));
                }
            }
            invis_building_handler.addPois(pois);
        }        
    }

    IEnumerator loadPoisCall(double lat_last, double lon_last)
    {
        WWW www = new WWW(base_url + "lat=" + map.lat + "&lon=" + map.lon);// +"&lat_last="+lat_last+"&lon_last="+lon_last);
        yield return www;
        yield return new WaitForSeconds(0.5f);
        //string ret  = "{\"response\":[{\"id\" : 384746, \"original_name\" : \"Nikolaikirche\", \"bv_image\" : \"https://scontent-frt3-1.cdninstagram.com/t51.2885-15/e35/15538982_397954337262951_6855562085602951168_n.jpg\", \"icon\" : \"glyphicons-303-temple-christianity-church@3x\", \"super_categories\" : [111494], \"bearing\" : 79.8900000000000006, \"elevation\" : 106.32283, \"pos_lat\" : 50.1100489999999965, \"pos_lon\" : 8.68232300000000023}]}";
        loadPois(www.text); 
    }

    void removePois()
    {
        List<Poi> del = new List<Poi>();
        for (int i = 0; i < pois.Count; ++i)
        {
            if (map.dist(pois[i].lat, pois[i].lon) > del_radius)
                del.Add(pois[i]);
        }
        for (int i = 0; i < del.Count; ++i)
        {
            Poi deletePoi = del[i];
            pois.Remove(deletePoi);
            deletePoi.kill();
        }
    }

    // TODO: get categories from Server or config file
    Dictionary<string, List<int>> categories = new Dictionary<string, List<int>>
    { {"Gastronomie", new List<int> { 69537,69687 } },
        {"Shopping", new List<int> { 111492 } },
        {"Sight", new List<int> {111494}}
    };

    public Color getColorForCategory(int[] super_categories)
    {
        for (int i = 0; i < super_categories.Length; ++i)
        {
            int category_id = super_categories[i];
            string category = "";
            foreach (KeyValuePair<string, List<int>> entry in categories)
            {
                if(entry.Value.Contains(category_id))
                {
                    category = entry.Key;
                    break;
                }
            }
            switch (category)
            {
                case "Gastronomie": return new Color(159f / 255f, 1f / 255f, 1f / 255f);
                case "Sight": return new Color(131f / 255f, 122f / 255f, 0f / 255f);
                case "Shopping": return new Color(60f / 255f, 60f / 255f, 65f / 255f);
            }
        }
        return new Color(0f / 255f, 99f / 255f, 25f / 255f);
    }
}
