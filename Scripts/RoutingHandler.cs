using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoutingHandler : MonoBehaviour {

    public int step_threshold = 20;
    public float dest_lat;
    public float dest_lon;
    public GameObject routingDialogue;
    public GameObject calculatingRouteDialogue;
    OSRMDrivingJsonResponse route;
    public List<GameObject> nav_steps;
    public List<GameObject> nav_step_dir_arrows;
    public bool initiated = false;
    public int reroute_threshold = 10;
    public int remove_threshold = 1;
    public float street_width = 20;
    private int out_of_bounds_count = 0;
    private int current_step = 0;
    private Map map;
    private bool aborted = false;
    // Use this for initialization
    void Start () {
        map = GameObject.Find("Map").GetComponent<Map>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void clearRoute()
    {
        for (int i = 0; i < nav_steps.Count; ++i)
            DestroyImmediate(nav_steps[i], true);
        for (int i = 0; i < nav_step_dir_arrows.Count; ++i)
            DestroyImmediate(nav_step_dir_arrows[i], true);

        nav_steps.Clear();
        nav_step_dir_arrows.Clear();
        out_of_bounds_count = 0;
        initiated = false;
        aborted = false;
    }

    public void buildRoute(float lat1, float lon1, float lat2, float lon2)
    {
        aborted = false;
        initiated = true;
        dest_lat = lat2;
        dest_lon = lon2;
        StartCoroutine(getRoute(lat1, lon1, lat2, lon2));
    }

    private int inBoundsOf()
    {
        int ibo = -1;
        for(int i = 0; i < nav_steps.Count; ++i)
            if (nav_steps[i].GetComponent<NavStep>().collides && nav_steps[i].GetComponent<NavStep>().step > ibo) {
                ibo = nav_steps[i].GetComponent<NavStep>().step;
            }
                
        return ibo;
    }

    private void updateGameObjects()
    {
        for (int i = 0; i < nav_steps.Count; ++i)
            nav_steps[i].GetComponent<NavStep>().updateNavStep();
        for (int i = 0; i < nav_step_dir_arrows.Count; ++i)
            nav_step_dir_arrows[i].GetComponent<NavStep>().updateNavStep();
    }

    public void updateRoute()
    {
        if (!initiated) return;
        int s_idx = inBoundsOf();
        if(s_idx == -1)
        {
            // out of bounds
            if(out_of_bounds_count >= reroute_threshold)
            {
                // calculate new route
                clearRoute();
                buildRoute((float)map.lat, (float)map.lon, dest_lat, dest_lon);
            }
            else
            {
                updateGameObjects();
                out_of_bounds_count++;
            }
            
        } else
        {
            out_of_bounds_count = 0;
            if (s_idx == current_step)
            {
                dist(route.routes[0].legs[0].steps[((s_idx + 1 < route.routes[0].legs[0].steps.Length) ? s_idx + 1 : s_idx)].maneuver.location);
                // still in same step
                updateGameObjects();
            }
            else
            {
                // new step
                routingDialogue.transform.Find("img_dialogue_bg/txt_next_step").GetComponent<UnityEngine.UI.Text>().text = route.routes[0].legs[0].steps[((s_idx + 1 < route.routes[0].legs[0].steps.Length) ? s_idx + 1 : s_idx)].maneuver.instruction;
                dist(route.routes[0].legs[0].steps[((s_idx+1 < route.routes[0].legs[0].steps.Length) ?s_idx + 1:s_idx)].maneuver.location);
                current_step = s_idx;
                // in different step
                // remove steps farther away from current_step than remove_threshold
                removeSteps();
                // add steps
                addSteps();
            }
        }
        
    }

    private void addSteps()
    {
        for(int i = current_step; i < ((current_step + remove_threshold > route.routes[0].legs[0].steps.Length) ? route.routes[0].legs[0].steps.Length : current_step + remove_threshold); ++i)
        {
            bool has = false;
            for (int j = 0; j < nav_steps.Count; ++j)
                if (nav_steps[j].GetComponent<NavStep>().step == i)
                {
                    has = true;
                }
            if(!has)
            {
                List<float[]> coordinates = Polyline.decode(route.routes[0].legs[0].steps[i].geometry);
                if (i != 0 && coordinates.Count > 1)
                {
                    buildDirectionArrow(coordinates[0], coordinates[1], 0, 0, i);
                }
                for (int j = 0; j < coordinates.Count - 1; ++j)
                {
                    buildNavStep(coordinates[j], coordinates[j + 1], 0, 0, i);
                }
            }        
        }
    }
    private void removeSteps()
    {
        List<GameObject> ns_rmv = new List<GameObject>();
        List<GameObject> da_rmv = new List<GameObject>();
        for(int i = 0; i< nav_steps.Count;++i)
            if (Mathf.Abs(nav_steps[i].GetComponent<NavStep>().step - current_step) > 1)
            {
                ns_rmv.Add(nav_steps[i]);
            }
                
        for (int i = 0; i < nav_step_dir_arrows.Count; ++i)
            if (Mathf.Abs(nav_step_dir_arrows[i].GetComponent<NavStep>().step - current_step) > 1)
            {
                da_rmv.Add(nav_step_dir_arrows[i]);
                
            }
                
        for (int i = 0; i < ns_rmv.Count; ++i)
        {       
            nav_steps.Remove(ns_rmv[i]);
            DestroyImmediate(ns_rmv[i], true);
        }
            
        for (int i = 0; i < da_rmv.Count; ++i)
        {
            nav_step_dir_arrows.Remove(da_rmv[i]);
            DestroyImmediate(da_rmv[i], true);
        }
            
    }
    private IEnumerator getRoute(float lat1, float lon1, float lat2, float lon2) 
    {
        string url = "https://api.mapbox.com/directions/v5/mapbox/cycling/" + lon1 + "," + lat1 + ";" + lon2 + "," + lat2 + "?overview=false&steps=true&access_token=pk.eyJ1IjoicGhpbGlwMTMxIiwiYSI6InJsSDE4LWsifQ.qAjbT5HjX4kuZ7pvlNw-FQ";
        Debug.Log(url);
        WWW www = new WWW(url);
        yield return www;
        loadRoute(www.text);
    }

    private void buildDirectionArrow(float[] coords_start, float[] coords_end, int route_idx, int leg_idx, int step_idx)
    {
        GameObject step = Instantiate(Resources.Load("direction_arrow"), new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        step.name = "direction_arrow_" + route_idx + "_" + leg_idx + "_" + step_idx;
        NavStep ns = step.GetComponent<NavStep>();
        ns.initDirectionArrow(coords_start[0], coords_start[1], coords_end[0], coords_end[1]);
        ns.route = route_idx;
        ns.leg = leg_idx;
        ns.step = step_idx;
        nav_step_dir_arrows.Add(step);
    }

    private void buildNavStep(float[] coords_start, float[] coords_end, int route_idx, int leg_idx, int step_idx)
    {
        GameObject step = Instantiate(Resources.Load("nav_step"), new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        step.name = "nav_step_" + route_idx + "_" + leg_idx + "_" + step_idx;
        NavStep ns = step.GetComponent<NavStep>();
        ns.initNavStep(coords_start[0], coords_start[1], coords_end[0], coords_end[1]);
        ns.route = route_idx;
        ns.leg = leg_idx;
        ns.step = step_idx;
        nav_steps.Add(step);
    }

    private void loadRoute(string data)
    {
        if (aborted) {
            clearRoute();
            return;
        }
        calculatingRouteDialogue.GetComponent<CalculatingRouteDialogueScript>().hide();
        routingDialogue.SetActive(true);
        routingDialogue.GetComponent<RoutingDialogueScript>().show();


        route = JsonUtility.FromJson<OSRMDrivingJsonResponse>(data);
        current_step = 0;
        addSteps();
        routingDialogue.transform.Find("img_dialogue_bg/txt_next_step").GetComponent<UnityEngine.UI.Text>().text = route.routes[0].legs[0].steps[0].maneuver.instruction;
        dist(route.routes[0].legs[0].steps[0].maneuver.location);
        //map.startFakeGPS();
        

    }

    private void dist(float[] loc)
    {
        routingDialogue.transform.Find("img_dialogue_bg/txt_distance").GetComponent<UnityEngine.UI.Text>().text = Mathf.Ceil((float)map.dist(loc[1], loc[0]) * 1000).ToString() + "m";
    }
    public void abort()
    {
        clearRoute();
    }
}
