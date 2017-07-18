using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets;
using Assets.Scripts;
using UnityEngine.EventSystems;

public class InvisBuilding : MonoBehaviour {

    public float extrude_top = 1f;
    public float extrude_bot = 1f;
    public float max_x, max_z;
    public float x_off = 0, z_off = 0;
    public int id = -1;
    public float b, d, lat, lon;
    public bool blocked;
    Map map;
    private new Transform transform;
    public GameObject building_icon;
    public UnityEngine.UI.Text poi_amount;
    public GameObject poi_amount_icon;
    public PoiHandler poi_handler;
    public List<Poi> pois;
    public float s = 0.00025f;
    public float building_icon_width = 600;
    Color highlight_color = new Color(65f / 255f, 133f / 255f, 227f / 255f, 133f / 255f);
    Color unhighlight_color = new Color(0, 0, 0, 133f / 255f);
    // Use this for initialization
    void Start () {
               
    }
    public void kill()
    {
        DestroyImmediate(building_icon, true);
        DestroyImmediate(poi_amount_icon, true);
        poi_handler = null;
        pois = null;
        map = null;
    }
    public override bool Equals(System.Object obj)
    {
        if (obj == null)
        {
            return false;
        }
        InvisBuilding i = obj as InvisBuilding;
        return Equals(this.id, i.id);
    }

    public void highlight()
    {
        if (building_icon.activeSelf)
        {
            building_icon.GetComponent<UnityEngine.UI.Image>().color = highlight_color;
        }
    }

    public void unhighlight()
    {
        if (building_icon.activeSelf)
        {
            building_icon.GetComponent<UnityEngine.UI.Image>().color = unhighlight_color;
        }
    }

    public void addPois()
    {
        if (poi_handler.pois == null)
            return;

        for (int i = 0; i < poi_handler.pois.Count; ++i)
        {
            if (poi_handler.pois[i].basic_poi.parent_polygon == this.id && !this.pois.Contains(poi_handler.pois[i]))
                this.pois.Add(poi_handler.pois[i]);

        }

        switchIconVisibility();
    }

    public void switchIconVisibility()
    {
        if (this.pois.Count > 0 && !blocked)
        {
            if(this.pois.Count > 1)
            {
                poi_amount_icon.SetActive(true);
                poi_amount.text = "+" + (pois.Count - 1).ToString();
                if (this.pois.Count > 9)
                {
                    poi_amount.fontSize = 48;

                } else
                {
                    poi_amount.fontSize = 72;
                }
                
            } else
            {
                poi_amount_icon.SetActive(false);
            }
            building_icon.SetActive(true);

        }
        else
            building_icon.SetActive(false);
    }

    public void init(InvisBuildingJsonRepsonse_Building building, PoiHandler poi_handler)
    {
        this.poi_handler = poi_handler;
        addPois();
        transform = GetComponent<Transform>();
        this.lat = building.startPoint[1];
        this.lon = building.startPoint[0];
        this.id = building.id;
        this.name = "fade_building_" + building.id;

        Vector2[] vertices2D = new Vector2[building.building.Length/2];
        for(int i = 0; i < building.building.Length; i+=2)
        {
            vertices2D[i / 2] = new Vector2(building.building[i], building.building[i+1]);
        }

        if (map == null)
            map = GameObject.Find("Map").GetComponent<Map>();
        MeshFilter filter = gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
        filter.mesh = CreateMesh(vertices2D);
        updateInvisBuilding();

    }
    public void updateInvisBuilding()
    {

        blocked = false;

        b = (float)map.bearing(lat, lon);
        d = (float)map.dist(lat, lon) * 1000;

        transform.position = new Vector3((d) * Mathf.Sin(Mathf.Deg2Rad * b), 0, (d) * Mathf.Cos(Mathf.Deg2Rad * b));



        RaycastHit[] hits;
        hits = Physics.RaycastAll(new Vector3(0, 0, 0), new Vector3(transform.position.x + x_off, transform.position.y, transform.position.z + z_off), 350.0F);
        float min_d = float.MaxValue;
        RaycastHit hit = new RaycastHit();
        Vector3 p = new Vector3();
        for (int i = 0; i < hits.Length; i++)
        {
            hit = hits[i];
            Transform objectHit = hit.transform;
            if (objectHit.name.Contains("fade_building"))
            {
                if (objectHit.gameObject.GetComponent<InvisBuilding>().id == this.id)
                {
                    p = hit.point;
                }
                else if (hit.point.magnitude < min_d)
                    min_d = hit.point.magnitude;
            }
        }
        if (min_d < p.magnitude)
        {
            
            blocked = true;
            switchIconVisibility();
            return;
        }
        switchIconVisibility();

        building_icon.transform.position = new Vector3(p.x + ((building_icon_width / 1) * s * p.magnitude) * -p.normalized.x, 1, p.z + ((building_icon_width / 1) * s * p.magnitude) * -p.normalized.z);
        
        building_icon.transform.localScale = new Vector3(s * p.magnitude, s * p.magnitude, s * p.magnitude);
        //building_icon.transform.localPosition = new Vector3((transform.position.x - hit.point.x) - 0.5f * hit.point.normalized.x, 0.5f, (transform.position.z - hit.point.z) -0.5f*hit.point.normalized.z);
        //building_icon.GetComponent<Renderer>().sharedMaterial.renderQueue = 2000;
        building_icon.transform.LookAt(new Vector3(0, 1.65f, 0));
        building_icon.transform.rotation = Quaternion.Euler(0, building_icon.transform.rotation.eulerAngles.y + 180, 0);
    }

    Mesh CreateMesh(Vector2[] poly)
    {

        

        // convert polygon to triangles
        Triangulator triangulator = new Triangulator(poly);
        int[] tris = triangulator.Triangulate();
        Mesh m = new Mesh();
        Vector3[] vertices = new Vector3[poly.Length * 2];
        Vector2[] uvs = new Vector2[poly.Length * 2];

        for (int i = 2; i < tris.Length; i+=3)
        {
            x_off += (poly[tris[i - 2]].x + poly[tris[i - 1]].x + poly[tris[i]].x) / 3;
            z_off += (poly[tris[i - 2]].y + poly[tris[i - 1]].y + poly[tris[i]].y) / 3;
        }

        x_off = x_off / (tris.Length / 3);
        z_off = z_off / (tris.Length / 3);

        int alternate = 0;
        for (int i = 0; i < poly.Length; i++)
        {
            if (poly[i].x > max_x)
                max_x = poly[i].x;
            if (poly[i].y > max_z)
                max_z = poly[i].y;
            vertices[i].x = poly[i].x;
            vertices[i].z = poly[i].y;
            vertices[i].y = -extrude_bot; // front vertex
            vertices[i + poly.Length].x = poly[i].x;
            vertices[i + poly.Length].z = poly[i].y;
            vertices[i + poly.Length].y = extrude_top;  // back vertex    
            uvs[i] = new Vector2(alternate, 0);
            uvs[i + poly.Length] = new Vector2(alternate, 1);
            alternate = (alternate == 0) ? 1 : 0;

        }
        int[] triangles = new int[tris.Length * 2 + poly.Length * 6];
        int count_tris = 0;
        //for (int i = 0; i < tris.Length; i += 3)
        //{
        //    triangles[i] = tris[i];
        //    triangles[i + 1] = tris[i + 1];
        //    triangles[i + 2] = tris[i + 2];
        //} // front vertices
        count_tris += tris.Length;
        //for (int i = 0; i < tris.Length; i += 3)
        //{
        //    triangles[count_tris + i] = tris[i + 2] + poly.Length;
        //    triangles[count_tris + i + 1] = tris[i + 1] + poly.Length;
        //    triangles[count_tris + i + 2] = tris[i] + poly.Length;
        //} // back vertices
        count_tris += tris.Length;
        for (int i = 0; i < poly.Length; i++)
        {
            int n = (i + 1) % poly.Length;
            triangles[count_tris] = i;
            triangles[count_tris + 1] = n;
            triangles[count_tris + 2] = i + poly.Length;
            triangles[count_tris + 3] = n;
            triangles[count_tris + 4] = n + poly.Length;
            triangles[count_tris + 5] = i + poly.Length;
            count_tris += 6;
        }
        m.vertices = vertices;
        m.triangles = triangles;
        m.uv = uvs;
        m.RecalculateNormals();
        m.RecalculateBounds();
        MeshCollider myMC = GetComponent<MeshCollider>();
        Mesh newMesh = new Mesh();
        newMesh = new Mesh();
        newMesh.vertices = vertices;
        newMesh.triangles = triangles;
        newMesh.RecalculateBounds();
        myMC.sharedMesh = newMesh;
        return m;
    }
    

    // Update is called once per frame
    void Update () {
        
    }   
}
