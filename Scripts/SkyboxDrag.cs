using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkyboxDrag : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler {
    Vector3 initial_pos = new Vector3();
    Vector3 drag_from = new Vector3();
    public GameObject maincamera;
    bool dragging = false;
    float min_drag_angle = 1f;
    float r = 55f;
    float speedH, speedV;

    public void OnBeginDrag(PointerEventData eventData)
    {
        speedH = GameObject.Find("Map").GetComponent<Map>().horizontalFov / Screen.width;
        speedV = GameObject.Find("Map").GetComponent<Map>().fov / Screen.height;
        if (maincamera == null)
            maincamera = GameObject.Find("Main Camera");
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragging || (Mathf.Abs(eulerAngles(eventData.pointerCurrentRaycast.worldPosition, drag_from).x) + Mathf.Abs(eulerAngles(eventData.pointerCurrentRaycast.worldPosition, drag_from).z)) / 2 > min_drag_angle)
        {
            
            dragging = true;
            maincamera.GetComponent<ARCamera>().camera_fixed = true;


            //if (drag_from.magnitude == 0)
            //{
            //    drag_from = eventData.pointerCurrentRaycast.worldPosition;
            //}
            //Vector3 drag = eulerAngles(eventData.pointerCurrentRaycast.worldPosition, drag_from);
            //Debug.Log(drag);
            //drag_from = eventData.pointerCurrentRaycast.worldPosition;
            maincamera.GetComponent<ARCamera>().rotateByDrag(new Vector3(speedV*eventData.delta.y, -speedH*eventData.delta.x, 0));
            
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        ARCamera arc = maincamera.GetComponent<ARCamera>();

        Quaternion compass = new Quaternion(maincamera.GetComponent<ARCamera>().o[0], maincamera.GetComponent<ARCamera>().o[1], maincamera.GetComponent<ARCamera>().o[2], maincamera.GetComponent<ARCamera>().o[3]);
        if (!GameObject.Find("skybox_container").GetComponent<Skybox>().isDistrict && !GameObject.Find("skybox_container").GetComponent<Skybox>().isInside) {
            maincamera.GetComponent<ARCamera>().heading_correction = maincamera.transform.rotation.eulerAngles.y - compass.eulerAngles.y;
            //GameObject.Find("Main Camera").GetComponent<ARCamera>().heading_correction = GameObject.Find("Main Camera").transform.rotation.eulerAngles.y - GameObject.Find("Main Camera").GetComponent<ARCamera>().compassHeading;
            maincamera.GetComponent<ARCamera>().camera_fixed = false;
        }
        dragging = false;
        drag_from = new Vector3();
    }

    public Vector3 eulerAngles (Vector3 from, Vector3 to)
    {
        Vector2 x_0 = new Vector2(r, 0);
        Vector2 z_0 = new Vector2(0, r);
        Vector2 from_x = new Vector2(from.y, from.z);
        Vector2 from_z = new Vector2(from.y, from.x);
        Vector2 to_x = new Vector2(to.y, to.z);
        Vector2 to_z = new Vector2(to.y, to.x);
        //Debug.Log(from_x + " " + to_x);
        float α_x =-1 * Mathf.Rad2Deg * (Mathf.Atan2(to_x.x - x_0.x, to_x.y - x_0.y) - Mathf.Atan2(from_x.x - x_0.x, from_x.y - x_0.y));
        float α_z = Mathf.Rad2Deg * (Mathf.Atan2(to_z.x - x_0.x, to_z.y - x_0.y) - Mathf.Atan2(from_z.x - x_0.x, from_z.y - x_0.y));

        return new Vector3(α_x,  α_z, 0);
    }
    
    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
