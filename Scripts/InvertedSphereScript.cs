using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvertedSphereScript : MonoBehaviour {

    void InvertSphere()
    {
        Vector3[] normals = GetComponent<MeshFilter>().mesh.normals;
        for (int i = 0; i < normals.Length; i++)
        {
            normals[i] = -normals[i];
        }
        GetComponent<MeshFilter>().sharedMesh.normals = normals;

        int[] triangles = GetComponent<MeshFilter>().sharedMesh.triangles;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            int t = triangles[i];
            triangles[i] = triangles[i + 2];
            triangles[i + 2] = t;

        }

        GetComponent<MeshFilter>().sharedMesh.triangles = triangles;
        transform.GetComponent<Renderer>().material.renderQueue = 2002;
    }
    // Use this for initialization
    void Start()
    {
        InvertSphere();
        gameObject.AddComponent<MeshCollider>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
