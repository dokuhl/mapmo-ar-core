using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeBuilding : MonoBehaviour {

    public float extrude = 1f;
    public int id;
    public float b, d, lat, lon;
    Map map;
    private new Transform transform;

    // Use this for initialization
    void Start()
    {
        transform = GetComponent<Transform>();
        // Create Vector2 vertices
        Vector2[] vertices2D = new Vector2[] {
            new Vector2(0,0),
            new Vector2(8.34294589f,8.53140899f),
            new Vector2(15.9759875f,1.06799749f),
            new Vector2(7.63304161f,-7.46970621f)
        };

    }
    public void init(InvisBuildingJsonRepsonse_Building building)
    {
        this.lat = building.startPoint[1];
        this.lon = building.startPoint[0];
        this.id = building.id;

        Vector2[] vertices2D = new Vector2[building.building.Length / 2];
        for (int i = 0; i < building.building.Length; i += 2)
        {
            vertices2D[i / 2] = new Vector2(building.building[i], building.building[i + 1]);
        }

        if (map == null)
            map = GameObject.Find("Map").GetComponent<Map>();
        MeshFilter filter = gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
        filter.mesh = CreateMesh(vertices2D);
        updateInvisBuilding();

    }
    public void updateInvisBuilding()
    {
        b = (float)map.bearing(lat, lon);
        d = (float)map.dist(lat, lon) * 1000;

        transform.position = new Vector3(d * Mathf.Sin(Mathf.Deg2Rad * b), 0, d * Mathf.Cos(Mathf.Deg2Rad * b));
    }

    Mesh CreateMesh(Vector2[] poly)
    {
        // convert polygon to triangles
        Triangulator triangulator = new Triangulator(poly);
        int[] tris = triangulator.Triangulate();
        Mesh m = new Mesh();
        Vector3[] vertices = new Vector3[poly.Length * 2];
        Vector2[] uvs = new Vector2[poly.Length * 2];

        int alternate = 0;
        for (int i = 0; i < poly.Length; i++)
        {
            vertices[i].x = poly[i].x;
            vertices[i].z = poly[i].y;
            vertices[i].y = 0; // front vertex
            vertices[i + poly.Length].x = poly[i].x;
            vertices[i + poly.Length].z = poly[i].y;
            vertices[i + poly.Length].y = extrude;  // back vertex    
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
        return m;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
