using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Assets.Scripts;

public class TileHandler : MonoBehaviour, IPointerDownHandler, IPointerExitHandler, IPointerUpHandler, IDragHandler, IEndDragHandler, IBeginDragHandler {
    public bool mouse_pressed = false;
    private Dictionary<string, GameObject> tiles;
    public bool gps_fixed = false;
    // destroy radius (in tiles to center) SHOULD ALWAYS BE GREATER THAN LOADING RADIUS!!
    public float destroy_r = 6;
    // loading radius (in tiles to center)
    public float load_r = 3;
    // center tile 
    public float c_x, c_y, c_tx, c_ty;
    public int zoom = 15;

    public float min_drag_distance = 2f;
    public float longtap_start = 0;
    public Vector3 nav_hit;
    public Vector3 initial_pos = new Vector3(0,-15f,0);
    public Vector3 drag_from = new Vector3();
    public GameObject navigateDialogue;
    public Marker goalMarker;
    public Map map;
    public PoiHandler poiHandler;
    public float init_c_x, init_c_y;
    // Use this for initialization

    //count
    int count = 0;

    void Start () {
        tiles = new Dictionary<string, GameObject>();
        goalMarker = (Instantiate(Resources.Load("Marker"), new Vector3(0, -14f, 0), Quaternion.identity) as GameObject).GetComponent<Marker>();
        Texture2D texture = Resources.Load("Textures/mapmo_goal", typeof(Texture2D)) as Texture2D;
        if (texture != null)
            goalMarker.transform.Find("img_marker").GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
        goalMarker.gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (!navigateDialogue.activeSelf && Physics.Raycast(ray, out hit) && hit.transform.gameObject == gameObject)
        {
            nav_hit = hit.point;
            longtap_start = Time.time;
            Debug.Log(nav_hit);
        }
        count++;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        count--;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        longtap_start = 0;
        count--;
        drag_from = new Vector3();
    }

    public void OnDrag(PointerEventData eventData)
    {
        
        Ray ray = Camera.main.ScreenPointToRay(eventData.currentInputModule.input.mousePosition);
        RaycastHit hit;
        //Debug.Log(Physics.Raycast(ray, out hit));
        if((initial_pos - eventData.pointerCurrentRaycast.worldPosition).magnitude > min_drag_distance)
        {
            longtap_start = 0;
            gps_fixed = true;
            if (drag_from.magnitude == 0) { 
                drag_from = eventData.pointerCurrentRaycast.worldPosition;
                init_c_x = c_x;
                init_c_y = c_y;
            }
            Vector3 drag = eventData.pointerCurrentRaycast.worldPosition - drag_from;
            //Debug.Log(init_c_x +" " + init_c_y + " " + drag.x + " " + drag.y);
            if(Mathf.Floor(c_x) != Mathf.Floor(init_c_x - drag.x / 10f) || Mathf.Floor(c_y) != Mathf.Floor(init_c_y + drag.z / 10f))
            {
                c_x = init_c_x - drag.x / 10.0f;
                c_y = init_c_y + drag.z / 10.0f;
                c_tx = Mathf.Floor(c_x);
                c_ty = Mathf.Floor(c_y);
                updateTiles();
            } else
            {
                c_x = init_c_x - drag.x / 10.0f;
                c_y = init_c_y + drag.z / 10.0f;
                moveTiles();
            }
        }
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        initial_pos = new Vector3(0, -15f, 0);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        initial_pos = eventData.pointerCurrentRaycast.worldPosition;
        //navigateDialogue.GetComponent<NavigationDialogueScript>().hide();
        goalMarker.gameObject.SetActive(false);
    }

 

    // Update is called once per frame
    void Update()
    {
        if(longtap_start != 0)
        {
           
            if(Time.time - longtap_start > 1)
            {
                goalMarker.gameObject.SetActive(true);
                
                longtap_start = 0;
                navigateDialogue.SetActive(true);
                float[] lonlat = map.TileToWorldPos(c_x + nav_hit.x/10, c_y - nav_hit.z / 10, zoom);
                goalMarker.init(lonlat[1], lonlat[0]);
                //navigateDialogue.GetComponent<NavigationDialogueScript>().lat = lonlat[1];
                //navigateDialogue.GetComponent<NavigationDialogueScript>().lon = lonlat[0];
                //navigateDialogue.GetComponent<NavigationDialogueScript>().show();
            }
        }



        //foreach (Touch t in Input.touches)
        //{
        //    Debug.Log("We have touches!");
        //    Ray ray = Camera.main.ScreenPointToRay(t.position);
        //    RaycastHit hit;
        //    Debug.Log(Physics.Raycast(ray, out hit));
        //    if (Physics.Raycast(ray, out hit))
        //    {
                
        //        Debug.Log("We have a hit!");
        //        switch (t.phase)
        //        {
        //            case TouchPhase.Began:
        //                hit.collider.SendMessage("OnTouchDown", null, SendMessageOptions.DontRequireReceiver);

        //                break;
        //            case TouchPhase.Moved:
        //            case TouchPhase.Stationary:
        //                break;
        //            default:
        //                break;
        //        }
        //    }
        //}

    }

    
    private void OnGUI()
    {
        if(Event.current.type.Equals(EventType.KeyUp) && Event.current.keyCode == KeyCode.Escape && c_x != map.c_x && c_y != map.c_y)
        {
            Event.current.Use();
            zoom = map.zoom;
            c_x = map.c_x;
            c_y = map.c_y;
            c_tx = map.c_tx;
            c_ty = map.c_ty;
            gps_fixed = false;
            updateTiles();
        }
    }

    public void moveTiles()
    {
        foreach(GameObject t in tiles.Values)
            t.GetComponent<TileScript>().moveTile(c_x, c_y);
        foreach (Poi p in GameObject.Find("PoiHandler").GetComponent<PoiHandler>().pois)
        {
            p.marker.moveMarker();
        }

    }

    public void updateTiles()
    {
        // for all tiles in loading radius load tiles
        for (float x = c_tx - load_r; x < c_tx + load_r; ++x)
            for (float y = c_ty - load_r; y < c_ty + load_r; ++y)
            {
                string name = zoom + "_" + x + "_" + y;
                if (!tiles.ContainsKey(name))
                {
                    GameObject pln = Instantiate(Resources.Load("tile"), new Vector3(0, -15, 0), Quaternion.identity) as GameObject;
                    pln.transform.name = name;
                    pln.GetComponent<TileScript>().init(zoom, x, y, c_x, c_y);
                    tiles.Add(pln.name, pln);
                }
            }

        List<string> dl = new List<string>();
        // for all tiles out of destroy radius: destroy
        foreach (string t_k in tiles.Keys)
        {
            GameObject t;
            tiles.TryGetValue(t_k, out t);
            TileScript ts = t.GetComponent<TileScript>();
            if (ts.t_x > c_tx + destroy_r || ts.t_y > c_ty + destroy_r || ts.t_x < c_tx - destroy_r || ts.t_y < c_ty - destroy_r)
                dl.Add(t_k);
        }
        foreach (string t_k in dl)
        {
            GameObject t = tiles[t_k];
            tiles.Remove(t_k);
            Texture texture = t.GetComponent<Renderer>().material.mainTexture;
            if (texture != null)
            {
                Destroy(texture);
            }
            DestroyImmediate(t, true);
        }
        moveTiles();
    }

    
}
