using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Billboard : MonoBehaviour {
    // the map
    Map map;
    public Poi poi;
    public GameObject basic_panel;
    public GameObject detail_panel;
    public GameObject icon_panel;
    public GameObject mainview;
    public GameObject tocview;
    public GameObject[] itemviews;
    public float padding_right = 150f;
    public float width = 980f;
    public float basic_opacity = 0.4f;
    public float fullscreen_opacity = 0.95f;
    public Color billboardColor;
    public bool detail_poi_is_loading = false;
    
    private new Transform transform;
    private Transform mainCameraTransform;
    //lat, lon, bearing
    double lat, lon, bearing;

    // initial scale (to make it look better)
    float initialScale = 0.004f;

    //
    float angle;

    public bool is_active = false;

    // how much the billboard is transformed from world pos (0) to cam fixed pos (1) 
    public float to_cam_ratio = 0f;
    // how much the billboard is transformed to its final world pos (1)
    public float switch_ratio = 0f;

    // animation duration in seconds
    public float anim_dur_to_cam = 0.5f;
    public float anim_dur_switch = 0.5f;

    // animation speed in seconds/frame
    public float anim_speed_to_cam = 0.02f;
    public float anim_speed_switch = 0.02f;

    // dist to cam fullscreen
    public float to_cam_final_z = 0.5f;


    //
    float d = 0f, b = 0f, h = 0f, h_signboard = 0f;

    // Use this for initialization
    void Start () {
        transform = GetComponent<Transform>();
        mainCameraTransform = Camera.main.transform;
	}

    public void navigate()
    {
        //GameObject.Find("RoutingHandler").GetComponent<RoutingHandler>().clearRoute();
        //GameObject.Find("RoutingHandler").GetComponent<RoutingHandler>().buildRoute((float)map.lat, (float)map.lon, (float)poi.lat, (float)poi.lon);
        //switchAnimWorldCam();
    }

    //private void OnGUI()
    //{
    //    Event e = Event.current;
    //    if (e.type == EventType.KeyUp && e.keyCode == KeyCode.Escape && detail_panel.activeSelf )
    //    {
    //        GameObject _container = transform.Find("detail_panel/scrollview/viewport/content").gameObject;
    //        if (_container.GetComponent<HorizontalSwipeSnap>().currentIndex > 0)
    //        {
    //            _container.GetComponent<HorizontalSwipeSnap>()._lerp_target = new Vector3(0 * (width + padding_right), _container.transform.localPosition.y, _container.transform.localPosition.z);
    //            _container.GetComponent<HorizontalSwipeSnap>()._lerp = true;
    //            _container.GetComponent<HorizontalSwipeSnap>().currentIndex = 0;
    //            e.Use();
    //        }
    //        else
    //        {
    //            switchAnimWorldCam();
    //            e.Use();
    //        }
    //    }
    //}

    //public void initDetail(PoiJsonResponse_PoiDetail p)
    //{
    //    poi.detail_poi = p;
    //    //transform.Find("detail_panel/view/viewport/content/txt_description").gameObject.GetComponent<Text>().text = poi.detail_poi.description;
    //    int viewcount = 0;

    //    GameObject _container = transform.Find("detail_panel/scrollview/viewport/content").gameObject;
    //    _container.GetComponent<RectTransform>().sizeDelta= new Vector2(((p.items.Length>0)?(1+p.items.Length)*(width+padding_right): (1) * (width + padding_right)), _container.GetComponent<RectTransform>().sizeDelta.y);

    //    mainview = Instantiate(Resources.Load("mainview"), new Vector3(0, -15, 0), Quaternion.identity) as GameObject;
    //    mainview.transform.SetParent(_container.transform);
    //    mainview.transform.localScale = new Vector3(1, 1, 1);
    //    mainview.transform.localEulerAngles = new Vector3(0, 0, 0);
    //    mainview.GetComponent<RectTransform>().localPosition = new Vector3(0,0,0.05f);
    //    mainview.transform.Find("txt_category").GetComponent<UnityEngine.UI.Text>().text = "";
    //    if(p.description.Split(' ').Length > 15)
    //    {
    //        string txt_str = "";
    //        string[] txt = p.description.Split(' ');
    //        for (int i = 0; i < 15 - 2; ++i)
    //            txt_str += txt[i] + " ";
    //        txt_str += txt[txt.Length - 1] + "...";
    //        mainview.transform.Find("listview/viewport/content/txt_description").GetComponent<UnityEngine.UI.Text>().text = txt_str;
    //    } else
    //        mainview.transform.Find("listview/viewport/content/txt_description").GetComponent<UnityEngine.UI.Text>().text = p.description;
    //    mainview.transform.Find("listview").GetComponent<ScrollSnapRect>().width = width;
    //    mainview.transform.Find("listview").GetComponent<ScrollSnapRect>().padding_right = padding_right;
    //    mainview.transform.Find("listview").GetComponent<ScrollSnapRect>()._container = _container.GetComponent<RectTransform>();
    //    mainview.transform.Find("listview").GetComponent<ScrollSnapRect>().index = viewcount;
    //    if (p.video != null && !p.video.Equals(""))
    //        mainview.transform.Find("txt_category/StepInside").gameObject.SetActive(true);
    //    else
    //        mainview.transform.Find("txt_category/StepInside").gameObject.SetActive(false);
    //    viewcount++;

    //    if (p.items.Length > 0)
    //    {


    //        //tocview = Instantiate(Resources.Load("tocview"), new Vector3(0, -15, 0), Quaternion.identity) as GameObject;
    //        //tocview.transform.SetParent(_container.transform);
    //        //tocview.transform.localScale = new Vector3(1, 1, 1);
    //        //tocview.transform.localEulerAngles = new Vector3(0, 0, 0);
    //        //tocview.GetComponent<RectTransform>().localPosition = new Vector3(viewcount * (width + padding_right), 0, 1);
    //        mainview.transform.Find("listview/viewport/content/txt_toc").GetComponent<UnityEngine.UI.Text>().text = "Offers";
    //        //mainview.transform.Find("toc").GetComponent<ScrollSnapRect>().width = width;
    //        //mainview.transform.Find("toc").GetComponent<ScrollSnapRect>().padding_right = padding_right;
    //        //mainview.transform.Find("toc").GetComponent<ScrollSnapRect>()._container = _container.GetComponent<RectTransform>();
    //        //mainview.transform.Find("toc").GetComponent<ScrollSnapRect>().index = viewcount;
    //        //viewcount++;
    //        itemviews = new GameObject[p.items.Length];


    //        for (int i = 0; i < p.items.Length; ++i)
    //        {
    //            GameObject btn = Instantiate(Resources.Load("tocbutton"), new Vector3(0, -15, 0), Quaternion.identity) as GameObject;
    //            btn.GetComponent<TOCButton>().init(viewcount, _container.GetComponent<RectTransform>(), width, padding_right);
    //            btn.transform.Find("txt_btn").GetComponent<Text>().text = p.items[i].name;
    //            btn.GetComponent<Image>().color = billboardColor;
    //            btn.transform.SetParent(mainview.transform.Find("listview/viewport/content"), false);

    //            itemviews[i] = Instantiate(Resources.Load("itemview"), new Vector3(0, -15, 0), Quaternion.identity) as GameObject;
    //            itemviews[i].transform.SetParent(_container.transform);
    //            itemviews[i].transform.localScale = new Vector3(1, 1, 1);
    //            itemviews[i].transform.localEulerAngles = new Vector3(0, 0, 0);
    //            itemviews[i].GetComponent<RectTransform>().localPosition = new Vector3(viewcount * (width + padding_right), 0, 0.05f);
    //            itemviews[i].transform.Find("txt_category").GetComponent<UnityEngine.UI.Text>().text = p.items[i].name;
    //            itemviews[i].transform.Find("listview").GetComponent<ScrollSnapRect>().width = width;
    //            itemviews[i].transform.Find("listview").GetComponent<ScrollSnapRect>()._container = _container.GetComponent<RectTransform>();
    //            itemviews[i].transform.Find("listview").GetComponent<ScrollSnapRect>().padding_right = padding_right;
    //            itemviews[i].transform.Find("listview").GetComponent<ScrollSnapRect>().index = viewcount;


    //            for (int j = 0; j < p.items[i].values.Length; ++j)
    //            {
    //                if (p.items[i].values[j].is_special)
    //                {
    //                    GameObject prod = null;
    //                    prod = Instantiate(Resources.Load("product"), new Vector3(0, -15, 0), Quaternion.identity) as GameObject;

    //                    prod.transform.Find("hlayout/txt_title").gameObject.GetComponent<Text>().text = p.items[i].values[j].name;
    //                    if (p.items[i].values[j].price != 0)
    //                        prod.transform.Find("hlayout/txt_price").gameObject.GetComponent<Text>().text = p.items[i].values[j].price + " €";
    //                    if (p.items[i].values[j].date_from != null && !p.items[i].values[j].date_from.Equals(""))
    //                    {
    //                        prod.transform.Find("date_layout").gameObject.SetActive(true);
    //                        System.DateTime date_from = new System.DateTime(0), date_to = new System.DateTime(0);

    //                        date_from = System.DateTime.Parse(p.items[i].values[j].date_from);
    //                        if (p.items[i].values[j].date_to != null && !p.items[i].values[j].date_to.Equals(""))
    //                            date_to = System.DateTime.Parse(p.items[i].values[j].date_to);
    //                        prod.transform.Find("date_layout/txt_time").gameObject.GetComponent<Text>().text = date_from.ToShortDateString() + " " + date_from.ToShortTimeString();
    //                        if (date_to.Subtract(new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc)).TotalMilliseconds != 0 && !date_from.Equals(date_to) && date_to.Subtract(date_from).Milliseconds > 60000)
    //                        {
    //                            if (date_from.DayOfYear.Equals(date_to.DayOfYear))
    //                                prod.transform.Find("date_layout/txt_time").gameObject.GetComponent<Text>().text += " - " + date_to.ToShortTimeString();
    //                            else
    //                                prod.transform.Find("date_layout/txt_time").gameObject.GetComponent<Text>().text += " - " + date_to.ToShortDateString() + " " + date_to.ToShortTimeString();
    //                        }
    //                    }
    //                    prod.transform.SetParent(mainview.transform.Find("listview/viewport/content"), false);
    //                }


    //                GameObject prod2 = null;
    //                prod2 = Instantiate(Resources.Load("product"), new Vector3(0, -15, 0), Quaternion.identity) as GameObject;
    //                if (p.items[i].values[j].product_cover != null && !p.items[i].values[j].product_cover.Equals("") && !p.items[i].values[j].product_cover.Equals("NULL"))
    //                {
    //                    prod2.transform.Find("cover_layout").gameObject.SetActive(true);
    //                    prod2.transform.Find("cover_layout/text/caption/txt_title").gameObject.GetComponent<Text>().text = p.items[i].values[j].name;
    //                    if (p.items[i].values[j].price != 0)
    //                        prod2.transform.Find("cover_layout/text/caption/txt_price").gameObject.GetComponent<Text>().text = p.items[i].values[j].price + " €";
    //                    prod2.transform.Find("cover_layout/text/txt_description").gameObject.GetComponent<Text>().text = p.items[i].values[j].description;
    //                    StartCoroutine(loadProductImage(p.items[i].values[j].product_cover, prod2.transform.Find("cover_layout/img_cover").gameObject.GetComponent<Image>()));
    //                }
    //                else
    //                {
    //                    prod2.transform.Find("hlayout").gameObject.SetActive(true);
    //                    prod2.transform.Find("hlayout/txt_title").gameObject.GetComponent<Text>().text = p.items[i].values[j].name;
    //                    if (p.items[i].values[j].price != 0)
    //                        prod2.transform.Find("hlayout/txt_price").gameObject.GetComponent<Text>().text = p.items[i].values[j].price + " €";
    //                    if (p.items[i].values[j].date_from != null && !p.items[i].values[j].date_from.Equals(""))
    //                    {
    //                        prod2.transform.Find("date_layout").gameObject.SetActive(true);
    //                        System.DateTime date_from = new System.DateTime(0), date_to = new System.DateTime(0);

    //                        date_from = System.DateTime.Parse(p.items[i].values[j].date_from);
    //                        if(p.items[i].values[j].date_to != null && !p.items[i].values[j].date_to.Equals(""))
    //                            date_to = System.DateTime.Parse(p.items[i].values[j].date_to);
    //                        prod2.transform.Find("date_layout/txt_time").gameObject.GetComponent<Text>().text = date_from.ToShortDateString() + " " + date_from.ToShortTimeString();
    //                        if (date_to.Subtract(new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc)).TotalMilliseconds != 0 && !date_from.Equals(date_to) && date_to.Subtract(date_from).Milliseconds  > 60000)
    //                        {
    //                            if (date_from.DayOfYear.Equals(date_to.DayOfYear))
    //                                prod2.transform.Find("date_layout/txt_time").gameObject.GetComponent<Text>().text += " - " + date_to.ToShortTimeString();
    //                            else
    //                                prod2.transform.Find("date_layout/txt_time").gameObject.GetComponent<Text>().text += " - " + date_to.ToShortDateString() + " " + date_to.ToShortTimeString();
    //                        }
    //                    }
    //                }
    //                prod2.transform.SetParent(itemviews[i].transform.Find("listview/viewport/content"), false);
    //            }

    //            viewcount++;
    //            //GameObject prod = Instantiate(Resources.Load("product"), new Vector3(0, -15, 0), Quaternion.identity) as GameObject;

    //            //prod.transform.SetParent(billboard.transform.Find("detail_panel/view/viewport/content"), false);

    //        }
    //    }
    //}

    //IEnumerator loadProductImage(string url, Image img)
    //{
    //    WWW www = new WWW(url);
    //    yield return www;
    //    img.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));

    //}

    //IEnumerator loadDetail()
    //{
    //    WWW www = new WWW(Poi.poi_detail_base_url + "?id="+ poi.basic_poi.id);
    //    yield return www;
    //    initDetail(JsonUtility.FromJson<PoiJsonResponse_PoiDetail>(www.text));
    //    detail_poi_is_loading = false;
    //}

    public void init(double[] bb_coords, double bb_bearing)
    {
        //init(bb_coords[0], bb_coords[1], bb_bearing);
    }

    public void init(double bb_lat, double bb_lon, double bb_bearing, Poi poi)
    {
        transform = GetComponent<Transform>();
        mainCameraTransform = Camera.main.transform;
        transform.name = poi.basic_poi.original_name;
        
        this.poi = poi;
        //this.h_signboard = poi.signboard.GetComponent<Signboard>().h;
        lat = bb_lat;
        lon = bb_lon;
        bearing = bb_bearing;
        if (map == null)
            map = GameObject.Find("Map").GetComponent<Map>();



        //initialize view & content
        if (!gameObject.activeSelf)
            return;
        loadImage(poi.basic_poi.bv_image);
        billboardColor = map.poi_handler.GetComponent<PoiHandler>().getColorForCategory(poi.basic_poi.super_categories);
        GetComponent<Image>().color = billboardColor;
        GetComponent<Light>().color = billboardColor;
        GetComponent<Light>().enabled = false;

        ColorBlock cbBtn = transform.Find("detail_panel/btn_expand").gameObject.GetComponent<Button>().colors;
        cbBtn.normalColor = billboardColor;
        transform.Find("detail_panel/btn_expand").gameObject.GetComponent<Button>().colors = cbBtn;
        transform.Find("basic_panel/btn_expand").gameObject.GetComponent<Button>().colors = cbBtn;

        transform.Find("detail_panel/btn_expand/title_bar/txt_title").gameObject.GetComponent<Text>().text = poi.basic_poi.original_name;
        transform.Find("basic_panel/btn_expand/txt_title").gameObject.GetComponent<Text>().text = poi.basic_poi.original_name;

        if (poi.basic_poi.icon != null && !poi.basic_poi.icon.Equals(""))
        {
            Texture2D texture = Resources.Load("icons/" + poi.basic_poi.icon.Substring(0, poi.basic_poi.icon.Length - 4), typeof(Texture2D)) as Texture2D;
            if (texture != null)
            {
                transform.Find("basic_panel/btn_expand/img_pictogram").gameObject.GetComponent<UnityEngine.UI.Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
                transform.Find("icon_panel/btn_expand/img_pictogram").gameObject.GetComponent<UnityEngine.UI.Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
                transform.Find("detail_panel/btn_expand/title_bar/img_pictogram").gameObject.GetComponent<UnityEngine.UI.Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
            }
        }


        //rotate billboard

        angle = 180 - map.horizontalFov / 2 - 90;
        transform.localScale = new Vector3(initialScale, initialScale, initialScale);
        transform.Rotate(Vector3.up, (float)bb_bearing - 270f);
        
        updateBillboard();
    }
    
    public void switchPanel() {
        if(basic_panel.activeSelf || icon_panel.activeSelf)
        {
            // detail_panel is set active when billboard-switch is finished
            //basic_panel.SetActive(false);
            //detail_panel.SetActive(true);
        }
        else
        {
            if (d > 40)
                icon_panel.SetActive(true);
            else
                basic_panel.SetActive(true);
            detail_panel.SetActive(false);
        }
    }

    public void updateBillboard()
    {
        if(poi.camera_lock) return;
        d = 1000 * (float)map.dist(lat, lon);
        b = (float)map.bearing(lat, lon);
        h = 0;
        RectTransform t = transform as RectTransform;
        float hypotenuse = d / Mathf.Sin(angle * Mathf.Deg2Rad);
        float fovWidth = 2 * (Mathf.Sqrt(Mathf.Pow(hypotenuse, 2) - Mathf.Pow(d, 2)));
        float nearViewScale = fovWidth / t.sizeDelta.x;
        //if (to_cam_ratio == 0) {
        //    if (basic_panel.activeSelf && d > 60)
        //    {
        //        basic_panel.SetActive(false);
        //        icon_panel.SetActive(true);
        //    } else if (icon_panel.activeSelf && d < 40)
        //    {
        //        basic_panel.SetActive(true);
        //        icon_panel.SetActive(false);
        //    }
        //}
        if (nearViewScale<initialScale)
        {
            transform.localScale = new Vector3(nearViewScale, nearViewScale, initialScale);
        }else
        {
            transform.localScale = new Vector3(initialScale, initialScale, initialScale);
        }
        transform.position = new Vector3(d * Mathf.Sin(Mathf.Deg2Rad * b), h + transform.localScale.y*t.sizeDelta.y / 2, d * Mathf.Cos(Mathf.Deg2Rad * b));
        //transform.Rotate(Vector3.up, (float)bearing - 270f);
        //transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);

        //DeadMosquito.AndroidGoodies.AGUIMisc.ShowToast(Camera.main.transform.eulerAngles.y + " " + transform.rotation.eulerAngles.y + " " + bearing + " " + lookRotation);
    }
    //public void scaleOut()
    //{
    //    StartCoroutine(scaleOutRoutine());
    //}

    //IEnumerator scaleOutRoutine()
    //{
    //    while (switch_ratio > 0)
    //    {
    //        switch_ratio -= anim_speed_switch / anim_dur_switch;
    //        yield return new WaitForSeconds(anim_speed_switch);
    //    }
    //    //while (transform.localScale.x > 0)
    //    //{
    //    //    transform.localScale -= new Vector3(0.001f, 0.001f, 0.001f);
    //    //    yield return new WaitForSeconds(0.02f);
    //    //}
    //    transform.gameObject.SetActive(false);
    //    //StopCoroutine(this.scaleOutRoutine());
    //}
    //public void scaleIn()
    //{
    //    StartCoroutine(scaleInRoutine());
    //}

    //IEnumerator scaleInRoutine()
    //{
    //    while (switch_ratio < 1)
    //    {
    //        switch_ratio += anim_speed_switch/ anim_dur_switch;
    //        yield return new WaitForSeconds(anim_speed_switch);
    //    }
        
    //    //while (transform.localScale.x < initialScale)
    //    //{
    //    //    transform.localScale += new Vector3(0.001f, 0.001f, 0.001f);
    //    //    yield return new WaitForSeconds(0.02f);
    //    //}
    //    //StopCoroutine(this.scaleInRoutine());
    //}

    //public void animateToCamera()
    //{
    //    StartCoroutine(animateToCameraRoutine());
    //}

    //IEnumerator animateToCameraRoutine()
    //{
    //    if(poi.detail_poi == null && !detail_poi_is_loading)
    //    {
    //        detail_poi_is_loading = true;
    //        StartCoroutine(loadDetail());
    //    }

    //    while(to_cam_ratio < 1) {
    //        to_cam_ratio += anim_speed_to_cam / anim_dur_to_cam;
    //        yield return new WaitForSeconds(anim_speed_to_cam);
    //    }
    //    basic_panel.SetActive(false);
    //    icon_panel.SetActive(false);
    //    detail_panel.SetActive(true);
        
    //    to_cam_ratio = 1;
    //}


    //public void animateToWorldPos()
    //{
    //    StartCoroutine(animateToWorldPosRoutine());
    //}


    //IEnumerator animateToWorldPosRoutine()
    //{
    //    while (to_cam_ratio > 0)
    //    {
    //        to_cam_ratio -= anim_speed_to_cam / anim_dur_to_cam;
    //        yield return new WaitForSeconds(anim_speed_to_cam);
    //    }
    //    to_cam_ratio = 0;
    //    poi.camera_lock = false;
    //    transform.rotation = Quaternion.Euler(new Vector3(0f, (float)bearing - 270f + (poi.flipBillboard*180f), 0f));

    //    //
    //    //if (!poi.showCase.Equals("billboard"))
    //    //{
    //    //    poi.signboard.SetActive(true);
    //    //    transform.gameObject.SetActive(false);
    //    //}
    //}

    public void loadImage(string url)
    {
        StartCoroutine(loadImg(url));
    }

    IEnumerator loadImg(string url)
    {
        if (!url.Equals(""))
        {
            WWW www = new WWW(url);
            yield return www;
            transform.Find("basic_panel/btn_expand/img_background").gameObject.GetComponent<UnityEngine.UI.Image>().sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));

        }
    }
    //public void switchAnimWorldCam()
    //{
    //    switchPanel();
    //    poi.animateSwitch();
    //}
    
    // Update is called once per frame
    void LateUpdate () {
        if (to_cam_ratio == 0 && switch_ratio == 1)
            return;
        RectTransform t = transform as RectTransform;
        if (to_cam_ratio != 0)
        {
            
            //position
            transform.position = new Vector3(
                (1 - to_cam_ratio) * (d * Mathf.Sin(Mathf.Deg2Rad * b)) + to_cam_ratio * (mainCameraTransform.position.x + mainCameraTransform.forward.x * to_cam_final_z),
                (1 - to_cam_ratio) * (h + transform.localScale.y * t.sizeDelta.y / 2) + to_cam_ratio * (mainCameraTransform.position.y + mainCameraTransform.forward.y * to_cam_final_z),
                (1 - to_cam_ratio) * (d * Mathf.Cos(Mathf.Deg2Rad * b)) + to_cam_ratio * (mainCameraTransform.position.z + mainCameraTransform.forward.z * to_cam_final_z)
                );

            //scale
            float hypotenuse = to_cam_final_z / Mathf.Sin(angle * Mathf.Deg2Rad);
            float fovWidth = 2 * (Mathf.Sqrt(Mathf.Pow(hypotenuse, 2) - Mathf.Pow(to_cam_final_z, 2)));
            float nearViewScale = fovWidth / t.sizeDelta.x;
            transform.localScale = new Vector3(
                (1 - to_cam_ratio) * initialScale + to_cam_ratio * nearViewScale, 
                (1 - to_cam_ratio) * initialScale + to_cam_ratio * nearViewScale, 
                1);
            
            t.sizeDelta = new Vector2(t.sizeDelta.x, ((float)Camera.main.pixelHeight / (float)Camera.main.pixelWidth * t.sizeDelta.x - t.sizeDelta.x) * to_cam_ratio + t.sizeDelta.x);
            //rotation
            transform.rotation = Quaternion.LookRotation(transform.position - mainCameraTransform.position);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, mainCameraTransform.eulerAngles.z);
        }
        //else if (switch_ratio != 1){
        //    //position
        //    transform.position = new Vector3(
        //        (d * Mathf.Sin(Mathf.Deg2Rad * b)),
        //        switch_ratio * (h + transform.localScale.y * t.sizeDelta.y / 2) + (1 - switch_ratio) * h_signboard,
        //        (d * Mathf.Cos(Mathf.Deg2Rad * b))
        //        );

        //    //scale
        //    float hypotenuse = to_cam_final_z / Mathf.Sin(angle * Mathf.Deg2Rad);
        //    float fovWidth = 2 * (Mathf.Sqrt(Mathf.Pow(hypotenuse, 2) - Mathf.Pow(to_cam_final_z, 2)));
        //    float nearViewScale = fovWidth / t.sizeDelta.x;
        //    transform.localScale = new Vector3(
        //        (switch_ratio) * initialScale,
        //        (switch_ratio) * initialScale,
        //        1);

        //    //rotation
        //    //transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);


        //}
        else{
            float hypotenuse = d / Mathf.Sin(angle * Mathf.Deg2Rad);
            float fovWidth = 2 * (Mathf.Sqrt(Mathf.Pow(hypotenuse, 2) - Mathf.Pow(d, 2)));
            float nearViewScale = fovWidth / t.sizeDelta.x;

            if (nearViewScale < initialScale)
            {
                transform.localScale = new Vector3(nearViewScale, nearViewScale, initialScale);
            }
            else
            {
                transform.localScale = new Vector3(initialScale, initialScale, initialScale);
            }
        }
    }
}
