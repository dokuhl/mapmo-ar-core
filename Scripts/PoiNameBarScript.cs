using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class PoiNameBarScript : MonoBehaviour {
    public int polygon_id = 0;
    public Text min_poi_title;
    public Image min_poi_image;
    public GameObject expand_view;
    public Text expand_poi_title;
    public Image expand_poi_image;
    public GameObject minimize_view;
    public GameObject list_container;
    public ARCamera camera_script;
    public PoiHandler poi_handler_script;
    public Texture2D default_tex;
    public Map map;
    InvisBuilding invis_building;
    Poi min_poi;
    List<Poi> pois = new List<Poi>();
    List<GameObject> list_items = new List<GameObject>();
	// Use this for initialization
	void Start () {
        default_tex = Resources.Load("icons/default", typeof(Texture2D)) as Texture2D;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void hide()
    {
        minimize_view.SetActive(false);
        expand_view.SetActive(false);
    }

    public void show()
    {
        highlight();
        if (pois.Count == 1 || list_items.Count == 0)
            minimize_view.SetActive(true);
        else if(pois.Count > 1)
            expand_view.SetActive(true);
    }

    public void switchBuilding(InvisBuilding invis_building)
    {
        
        if (expand_view.activeSelf)
            return;
        
        if (this.polygon_id == 0)
            enableView();
        else
        {
            unhighlight();
            pois.Clear();
        }

        this.polygon_id = invis_building.id;
        this.invis_building = invis_building;
        addPois();
        highlight();
        if(pois.Count > 1)
            min_poi_title.text = pois[0].basic_poi.original_name + " (+" + (pois.Count-1) + " others)";
        else
            min_poi_title.text = pois[0].basic_poi.original_name;
        min_poi = pois[0];
        min_poi_image.sprite = icon(pois[0].basic_poi.icon);

        if (pois.Count == 0)
        {
            disableView();
            return;
        } else
            if (!minimize_view.activeSelf)
                enableView();

        

       
        
        // set polygon id
        
    }

    public void highlight()
    {
        if (invis_building != null)
            invis_building.highlight();

        ////highlight new pois
        //for (int i = 0; i < poi_handler_script.pois.Count; ++i)
        //{
        //    if (poi_handler_script.pois[i].basic_poi.parent_polygon == polygon_id)
        //    {
        //        poi_handler_script.pois[i].billboard.GetComponent<Light>().enabled = true;
        //    }
        //}
    }
    public void addPois()
    {
        for (int i = 0; i < poi_handler_script.pois.Count; ++i)
        {
            if (poi_handler_script.pois[i].basic_poi.parent_polygon == polygon_id)
            {
                pois.Add(poi_handler_script.pois[i]);
            }
        }
    }

    public void unhighlight()
    {
        if (invis_building != null)
            invis_building.unhighlight();
        //// unhighlight pois
        //for (int i = 0; i < pois.Count; ++i)
        //    if (pois[i].billboard != null)
        //        pois[i].billboard.GetComponent<Light>().enabled = false;
    }

    private Sprite icon(string icon)
    {
        Texture2D texture = (!icon.Equals("") && !icon.Equals("null")) ? Resources.Load("icons/" + icon.Substring(0, icon.Length - 4), typeof(Texture2D)) as Texture2D : default_tex;
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
    }

    public void expand()
    {
        if (pois.Count == 1)
        {
            openPoi(0);
            return;
        }
            

        camera_script.camera_fixed = true;
        if(NatCamU.Core.NatCam.Camera != null)
            NatCamU.Core.NatCam.Pause();
        minimize_view.SetActive(false);
        expand_view.SetActive(true);
        for (int i = 0; i < pois.Count; ++i)
        {
            GameObject item = Instantiate(Resources.Load("PoiListItem"), new Vector3(0, -15, 0), Quaternion.identity) as GameObject;
            Transform item_transform = item.transform;
            item_transform.Find("txt_title").GetComponent<Text>().text = pois[i].basic_poi.original_name;
            item.transform.Find("img_pictogram").GetComponent<Image>().sprite = icon(pois[i].basic_poi.icon);

            item_transform.Find("txt_distance").GetComponent<Text>().text = Mathf.Ceil( (float)map.dist(pois[i].basic_poi.pos_lat, pois[i].basic_poi.pos_lon) * 1000) + "m";
            item_transform.SetParent(list_container.transform);
            item_transform.localScale = new Vector3(1, 1, 1);
            item_transform.localEulerAngles = new Vector3(0, 0, 0);

            item.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
            item.GetComponent<PoiListItemScript>().id = pois[i].id;
            item.GetComponent<PoiListItemScript>().poi_name_bar_script = this;
            list_items.Add(item);
        }
    }

    public void minimize()
    {
        camera_script.camera_fixed = false;
        if (NatCamU.Core.NatCam.Camera != null)
            NatCamU.Core.NatCam.Play();
        for (int i = 0; i < list_items.Count; ++i)
            DestroyImmediate(list_items[i], true);
        list_items.Clear();
        minimize_view.SetActive(true);
        expand_view.SetActive(false);
    }

    public void openPoi(int id)
    {
        if (id == 0)
            id = min_poi.id;
        
        for(int i = 0; i< pois.Count; ++i)
        {
            if(pois[i].id == id)
            {
                unhighlight();
                pois[i].billboard.GetComponent<Light>().enabled = true;
                hide();
                poi_handler_script.showBillboard(pois[i], true);
            }
        }
    }

    public void disableView()
    {
        polygon_id = 0;
        unhighlight();
        pois.Clear();
        minimize_view.SetActive(false);
    }

    public void enableView()
    {
        minimize_view.SetActive(true);
    }


}
