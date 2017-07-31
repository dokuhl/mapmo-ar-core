using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Assets.Scripts;

public class Map : MonoBehaviour {

    public Transform img_unprecision;
    public GameObject gps_marker;
    public bool gps_fixed = false;
    public float orig_lat = 0;
    public float orig_lon = 0;

    private static float R = 6371;
    public TileHandler tile_handler;
    private List<GameObject> markers;
    private List<GameObject> billboards;
    private bool fakeGPSrunning = false;
    public Poi selectedPoi;
    public GameObject building_handler;
    public InvisBuildingHandler building_handler_script;
    public GameObject poi_handler;
    public PoiHandler poi_handler_script;
    public GameObject routing_handler;
    //public RoutingHandler routing_handler_script;
    public GameObject district_arrow_handler;
    public DistrictArrowHandler district_arrow_handler_script;
    private List<Poi> pois;
    public GameObject prefabTile;
    
    public double lat, lon, elevation;
    public float fov, horizontalFov;
    private float gauss_alpha = Mathf.PI / (Mathf.Sqrt(Mathf.Log(2)));
    // AGGPS settings
    public long updateMinTimeMillis = 30;
    public float updateMinMeters = 0.1f;

    // destroy radius (in tiles to center) SHOULD ALWAYS BE GREATER THAN LOADING RADIUS!!
    public float destroy_r = 6;
    // loading radius (in tiles to center)
    public float load_r = 3;
    public int zoom = 15;
    // center of view in tile coordinates
    public float c_x;
    public float c_y;

    // current center tile
    public float c_tx;
    public float c_ty;

    // Use this for initialization
    private static Map instance = null;
    public static Map Instance
    {
        get
        {
            if (null == (object)instance)
            {
                instance = FindObjectOfType(typeof(Map)) as Map;
                if (null == (object)instance)
                {
                    var go = new GameObject("[Map]");
                    instance = go.AddComponent<Map>();
                    instance.EnsureMap();
                }
            }

            return instance;
        }
    }
    private void EnsureMap(){}
    private Map(){}
    private void OnDestroy()
    {
        instance = null;
    }

    public float[] tilepos(double lon, double lat, int zoom)
    {
        
        float[] pos = new float[2];
        //x
        pos[0] = (float)((lon + 180.0) / 360.0 * (1 << zoom));
        //y
        pos[1] = (float)((1.0 - System.Math.Log(System.Math.Tan(lat * System.Math.PI / 180.0) +
        1.0 / System.Math.Cos(lat * System.Math.PI / 180.0)) / System.Math.PI) / 2.0 * (1 << zoom));

        return pos;
    }
    public float[] WorldToTilePos(double lon, double lat, int zoom)
    {
        float[] p = new float[2];
        p[0] = (float)((lon + 180.0) / 360.0 * (1 << zoom));
        p[1] = (float)((1.0 - Math.Log(Math.Tan(lat * Math.PI / 180.0) +
            1.0 / Math.Cos(lat * Math.PI / 180.0)) / Math.PI) / 2.0 * (1 << zoom));

        return p;
    }

    public float[] TileToWorldPos(double tile_x, double tile_y, int zoom)
    {
        float[] p = new float[2];
        double n = Math.PI - ((2.0 * Math.PI * tile_y) / Math.Pow(2.0, zoom));

        p[0] = (float)((tile_x / Math.Pow(2.0, zoom) * 360.0) - 180.0);
        p[1] = (float)(180.0 / Math.PI * Math.Atan(Math.Sinh(n)));
        
        return p;
    }

    public static double dist(double lat1, double lat2, double lon1, double lon2)
    {
        double dLat = (lat2 - lat1) * Mathf.Deg2Rad;
        double dLon = (lon2 - lon1) * Mathf.Deg2Rad;
        lat1 = lat1 * Mathf.Deg2Rad;
        lat2 = lat2 * Mathf.Deg2Rad;
        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }
    public double dist(double lat, double lon)
    {
        return dist(this.lat, lat, this.lon, lon);
    }
    public double dist2(double lat, double lon)
    {
        return dist(lat, this.lat, lon, this.lon);
    }


    public static double bearing(double lat1, double lon1, double lat2, double lon2)
    {
        double φ1 = Mathf.Deg2Rad * lat1;
        double φ2 = Mathf.Deg2Rad * lat2;
        double Δλ = Mathf.Deg2Rad * (lon2 - lon1);
        double y = Math.Sin(Δλ) * Math.Cos(φ2);
        double x = Math.Cos(φ1) * Math.Sin(φ2) - Math.Sin(φ1) * Math.Cos(φ2) * Math.Cos(Δλ);

        return (Math.Atan2(y, x) * Mathf.Rad2Deg + 360) % 360;
    }
    
    public double[] midpoint(double lat1, double lon1, double lat2, double lon2)
    {   
        double φ1 = lat1 * Mathf.Deg2Rad;
        double φ2 = lat2 * Mathf.Deg2Rad;
        double λ1 = lon1 * Mathf.Deg2Rad;
        double λ2 = lon2 * Mathf.Deg2Rad;

        double Bx = Math.Cos(φ2) * Math.Cos(λ2 - λ1);
        double By = Math.Cos(φ2) * Math.Sin(λ2 - λ1);
        double φ3 = Math.Atan2(Math.Sin(φ1) + Math.Sin(φ2),
                            Math.Sqrt((Math.Cos(φ1) + Bx) * (Math.Cos(φ1) + Bx) + By * By));
        double λ3 = λ1 + Math.Atan2(By, Math.Cos(φ1) + Bx);
        double[] m = new double[2];
        m[0] = φ3 * Mathf.Rad2Deg;
        m[1] = λ3 * Mathf.Rad2Deg;
        return m;
    }

    public double bearing(double lat, double lon)
    {
        return bearing(this.lat, this.lon, lat, lon);
    }

    void nerv() { }

    void Start() {
        building_handler_script = building_handler.GetComponent<InvisBuildingHandler>();
        building_handler_script.initHandler(this);

        poi_handler_script = poi_handler.GetComponent<PoiHandler>();
        //routing_handler_script = routing_handler.GetComponent<RoutingHandler>();
        district_arrow_handler_script = district_arrow_handler.GetComponent<DistrictArrowHandler>();
        ////hardcoded latlon + updateCenter once
        tile_handler = GameObject.Find("MapBackground/TileHandler").GetComponent<TileHandler>();

        lat = 51.049456;
        lon = 13.737441;
        GameObject.Find("skybox_container").GetComponent<Skybox>().showDistrict("http://mapmo.de/wp-content/uploads/R0010189_20170721150835.jpg", 1.845f);

        ////lat = 51.0291588;
        ////lon = 13.7194578;
        ////lat = 50.110479;
        ////lon = 8.682195;
        //orig_lat = (float)lat;
        //orig_lon = (float)lon;



        //++++++++++++++
        // hardcoded FOV
        fov = 60;
        // calculate horizontalFOV
        float radAngle = fov * Mathf.Deg2Rad;
        float radHFOV = 2 * Mathf.Atan(Mathf.Tan(radAngle / 2) * Screen.width / Screen.height);
        horizontalFov = Mathf.Rad2Deg * radHFOV;
        selectedPoi = null;


        updateCenter();
        StartCoroutine(gpsDebugUpdateRoutine());
        ////startFakeGPS();


    }

    public void startFakeGPS()
    {
        List<float[]> coords = new List<float[]>();
        coords.Add(new float[] { 51.0535469055176f, 13.735987663269f });
        coords.Add(new float[] { 51.054095f, 13.735120f });
        coords.Add(new float[] { 51.053915f, 13.737164f });
        if (!fakeGPSrunning)
            StartCoroutine(fakeGPSCoroutine(coords));
    }

    public void updateCenter()
    {
        float[] c = tilepos(lon, lat, zoom);
        this.c_x = c[0];
        this.c_y = c[1];
        float c_ntx = Mathf.Floor(c_x);
        float c_nty = Mathf.Floor(c_y);
        bool ctchng = (c_ntx != c_tx || c_nty != c_ty);
        this.c_tx = c_ntx;
        this.c_ty = c_nty;
        // if new tile check for load & discarding of tiles
       
        if(!tile_handler.gps_fixed)
        {
            tile_handler.c_x = c[0];
            tile_handler.c_y = c[1];
            tile_handler.c_tx = c_ntx;
            tile_handler.c_ty = c_nty;
            if (ctchng)
            {
                tile_handler.updateTiles();
            } else
            {
                tile_handler.moveTiles();
            }
        }


        building_handler_script.updateBuildings();
        poi_handler_script.updatePois();
        district_arrow_handler_script.updateData();
        ////StartCoroutine(getUserElevation());
        //routing_handler_script.updateRoute();
        //TODO: remove fake
        //if (!routing_handler.GetComponent<RoutingHandler>().initiated && lat!=0 && lon!=0)
        //    routing_handler.GetComponent<RoutingHandler>().buildRoute((float)lat, (float)lon, 51.0353867f, 13.7191067f);
    }

    IEnumerator gpsDebugUpdateRoutine()
    {
        while(true)
        {
            updateCenter();
            yield return new WaitForSeconds(2f);
        }
        
    }

    //IEnumerator getUserElevation()
    //{

    //    WWWForm wwwForm = new WWWForm();
    //    string data = "{\"type\": \"Point\",\"coordinates\": [" + this.lon + "," + this.lat + "]}";

    //    Dictionary<string,string> headers = new Dictionary<string, string>();
    //    headers.Add("Content-Type", "application/json");
    //    WWW www = new WWW("https://data.cykelbanor.se/elevation/geojson", Encoding.ASCII.GetBytes(data),headers);
    //    yield return www;

    //    ElevationJsonResponse res = JsonUtility.FromJson<ElevationJsonResponse>(www.text);
    //    if(res.coordinates!=null && res.coordinates.Length>0)
    //        this.elevation = res.coordinates[2];
    //}



    public IEnumerator fakeGPSCoroutine(List<float[]> coords)
    {
        fakeGPSrunning = true;
        int idx = 0;
        lat = coords[0][0];
        lon = coords[0][1];
        float times = 700;

        while(idx < coords.Count -1)
        {
            lat += (coords[idx + 1][0] - coords[idx][0]) / times;
            lon += (coords[idx + 1][1] - coords[idx][1]) / times;
            updateCenter();
            yield return new WaitForSeconds(0.03f);
            if ((coords[idx + 1][0] - lat) + (coords[idx + 1][1] - lon) < 0.00005f)
                ++idx;
        }
        fakeGPSrunning = false;
    }

    int count = 0;
    public void locationChanged(string loc_str)
    {
        if(!gps_fixed)
        {
            string[] l_str = loc_str.Split('_');
            float acc;
            double lat, lon;
            float.TryParse(l_str[0], out acc);
            string prov = l_str[1];
            double.TryParse(l_str[2], out lat);
            double.TryParse(l_str[3], out lon);

            if (acc == 0 || prov == null || lat == 0 || lon == 0)
                return;

            if (this.lat == 0 || this.lon == 0)
            {
                this.lat = lat;
                this.lon = lon;
            }

            img_unprecision.localScale = new Vector3(acc * 0.005f, acc * 0.005f, acc * 0.005f);
            this.lat = ((acc < 1) ? lat : this.lat + 1f / Math.Sqrt(acc) * (lat - this.lat));
            this.lon = ((acc < 1) ? lon : this.lon + 1f / Math.Sqrt(acc) * (lon - this.lon));
            updateCenter();
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
//class ElevationJsonResponse
//{
//    public string type;
//    public double[] coordinates;
//}
