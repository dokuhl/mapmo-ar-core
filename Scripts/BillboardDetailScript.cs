using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;
using UnityEngine.UI;

public class BillboardDetailScript : EventBehaviour
{

    public Poi poi;
    public PoiNameBarScript poi_name_bar;
    public PoiHandler poi_handler_script;
    public GameObject caption;
    public GameObject content;
    public GameObject bg_img;
    public GameObject mainview;
    public string website;
    public MainViewScript main_view_script;
    public GameObject[] itemviews;
    public bool showing = false;
    public float padding_right = 150f;
    public float width = 980f;
    public int max_description = 25;
    public Color billboardColor;
    public bool called_from_poi_name_bar = false;


    // Use this for initialization
    void Start()
    {
        width = Screen.width;
    }
    
    
    // Update is called once per frame
    void Update()
    {

    }

    public void hide()
    {
        showing = false;
        caption.SetActive(false);
        content.SetActive(false);
        if (called_from_poi_name_bar)
        {
            poi_name_bar.show();
        }
        called_from_poi_name_bar = false;
    }

    public void show()
    {
        showing = true;
        caption.SetActive(true);
        content.SetActive(true);
    }

    public void show(Poi poi, bool called_from_poi_name_bar)
    {
        this.called_from_poi_name_bar = called_from_poi_name_bar;
        show(poi);
    }

    public void show(Poi poi)
    {

        if (this.poi!=null && this.poi.basic_poi != null && this.poi.basic_poi.id == poi.basic_poi.id)
        {
            show();
        }
        else
        {
            clearBillboard();
            this.poi = poi;
            show();
            width = content.GetComponent<RectTransform>().rect.width;
            caption.transform.Find("img_bg/txt_title").GetComponent<Text>().text = poi.basic_poi.original_name;
            Texture2D texture = (!poi.basic_poi.icon.Equals("") && !poi.basic_poi.icon.Equals("null")) ? Resources.Load("icons/" + poi.basic_poi.icon.Substring(0, poi.basic_poi.icon.Length - 4), typeof(Texture2D)) as Texture2D : GameObject.Find("Main Camera/POINameBar").GetComponent<PoiNameBarScript>().default_tex;
            caption.transform.Find("img_bg/img_pictogram").GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
            StartCoroutine(loadDetail());
        }

    }

    private void clearBillboard()
    {
        DestroyImmediate(mainview, true);
        mainview = null;
        for (int i = 0; i < itemviews.Length; ++i)
            DestroyImmediate(itemviews[i], true);
        itemviews = new GameObject[0];
    }


    private void initDetail(PoiJsonResponse_PoiDetail p)
    {
        poi.detail_poi = p;
        int viewcount = 0;

        
        GameObject _container = transform.Find("scrollview/viewport/content").gameObject;
        _container.GetComponent<RectTransform>().sizeDelta = new Vector2(((p.items.Length > 0) ? (1 + p.items.Length) * (width + padding_right) : (1) * (width + padding_right)), _container.GetComponent<RectTransform>().sizeDelta.y);


        mainview = Instantiate(Resources.Load("mainview"), new Vector3(0, -15, 0), Quaternion.identity) as GameObject;
        main_view_script = mainview.GetComponent<MainViewScript>();
        RectTransform rt = mainview.GetComponent<RectTransform>();
        mainview.GetComponent<RectTransform>().sizeDelta = new Vector2(GameObject.Find("Main Camera/BillboardDetail").GetComponent<RectTransform>().sizeDelta.x, mainview.GetComponent<RectTransform>().sizeDelta.y); 

        main_view_script.txt_title.text = poi.basic_poi.original_name;

        rt.rect.Set(rt.rect.x, rt.rect.y, width, rt.rect.height);
        mainview.transform.SetParent(_container.transform);
        mainview.transform.localScale = new Vector3(1, 1, 1);
        mainview.transform.localEulerAngles = new Vector3(0, 0, 0);
        mainview.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0.05f);

        if (p.description.Split(' ').Length > max_description)
        {
            string txt_str = "";
            string[] txt = p.description.Split(' ');
            for (int i = 0; i < max_description - 2; ++i)
                txt_str += txt[i] + " ";
            txt_str += txt[txt.Length - 1] + "...";
            main_view_script.txt_description.text = txt_str;
        }
        else
            main_view_script.txt_description.text = p.description;

        //mainview.transform.Find("listview").GetComponent<ScrollSnapRect>().width = width;
        //mainview.transform.Find("listview").GetComponent<ScrollSnapRect>().padding_right = padding_right;
        //mainview.transform.Find("listview").GetComponent<ScrollSnapRect>()._container = _container.GetComponent<RectTransform>();
        //mainview.transform.Find("listview").GetComponent<ScrollSnapRect>().index = viewcount;

        if (poi.basic_poi.fan_count > 0)
            mainview.transform.Find("listview/viewport/content/social_buttons/like/txt").GetComponent<UnityEngine.UI.Text>().text = poi.basic_poi.fan_count + " Likes";

        if(poi.basic_poi.contacts.Length == 0)
        {
            mainview.transform.Find("listview/viewport/content/txt_contacts").gameObject.SetActive(false);
            main_view_script.txt_email.gameObject.SetActive(false);
            main_view_script.txt_phone.gameObject.SetActive(false);
            main_view_script.txt_mobile.gameObject.SetActive(false);
            main_view_script.website_button.gameObject.SetActive(false);
        }

        for (int i = 0; i < poi.basic_poi.contacts.Length; ++i) {
            if(!main_view_script.txt_email.gameObject.activeSelf && poi.basic_poi.contacts[i].email != null)
            {
                main_view_script.txt_email.gameObject.SetActive(true);
                main_view_script.txt_email.text = poi.basic_poi.contacts[i].email;
            }

            if(!main_view_script.txt_phone.gameObject.activeSelf && poi.basic_poi.contacts[i].phone != null)
            {
                main_view_script.txt_phone.gameObject.SetActive(true);
                main_view_script.txt_phone.text = poi.basic_poi.contacts[i].phone;
            }

            if(!main_view_script.txt_mobile.gameObject.activeSelf && poi.basic_poi.contacts[i].mobile != null)
            {
                main_view_script.txt_mobile.gameObject.SetActive(true);
                main_view_script.txt_mobile.text = poi.basic_poi.contacts[i].mobile;
            }
            

            if (!main_view_script.website_button.activeSelf && poi.basic_poi.contacts[i].website != null)
            {
                main_view_script.website_button.SetActive(true);
                this.website = poi.basic_poi.contacts[i].website;
                main_view_script.website_button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate {
                    Application.OpenURL(website);
                });
            }

        }


        //if (p.items.Length > 0)
        //{

        //    mainview.transform.Find("listview/viewport/content/txt_toc").GetComponent<UnityEngine.UI.Text>().text = "Offers";
        //    //viewcount++;
        //    itemviews = new GameObject[p.items.Length];


        //    for (int i = 0; i < p.items.Length; ++i)
        //    {
        //        GameObject btn = Instantiate(Resources.Load("tocbutton"), new Vector3(0, -15, 0), Quaternion.identity) as GameObject;
        //        btn.GetComponent<TOCButton>().init(viewcount, _container.GetComponent<RectTransform>(), width, padding_right);
        //        btn.transform.Find("txt_btn").GetComponent<Text>().text = p.items[i].name;
        //        btn.GetComponent<Image>().color = billboardColor;
        //        btn.transform.SetParent(mainview.transform.Find("listview/viewport/content"), false);

        //        itemviews[i] = Instantiate(Resources.Load("itemview"), new Vector3(0, -15, 0), Quaternion.identity) as GameObject;
        //        itemviews[i].transform.SetParent(_container.transform);
        //        itemviews[i].transform.localScale = new Vector3(1, 1, 1);
        //        itemviews[i].transform.localEulerAngles = new Vector3(0, 0, 0);
        //        itemviews[i].GetComponent<RectTransform>().localPosition = new Vector3(viewcount * (width + padding_right), 0, 0.05f);
        //        itemviews[i].transform.Find("txt_category").GetComponent<UnityEngine.UI.Text>().text = p.items[i].name;
        //        //itemviews[i].transform.Find("listview").GetComponent<ScrollSnapRect>().width = width;
        //        //itemviews[i].transform.Find("listview").GetComponent<ScrollSnapRect>()._container = _container.GetComponent<RectTransform>();
        //        //itemviews[i].transform.Find("listview").GetComponent<ScrollSnapRect>().padding_right = padding_right;
        //        //itemviews[i].transform.Find("listview").GetComponent<ScrollSnapRect>().index = viewcount;


        //        for (int j = 0; j < p.items[i].values.Length; ++j)
        //        {
        //            if (p.items[i].values[j].is_special)
        //            {
        //                GameObject prod = null;
        //                prod = Instantiate(Resources.Load("product"), new Vector3(0, -15, 0), Quaternion.identity) as GameObject;

        //                prod.transform.Find("hlayout/txt_title").gameObject.GetComponent<Text>().text = p.items[i].values[j].name;
        //                if (p.items[i].values[j].price != 0)
        //                    prod.transform.Find("hlayout/txt_price").gameObject.GetComponent<Text>().text = p.items[i].values[j].price + " €";
        //                if (p.items[i].values[j].date_from != null && !p.items[i].values[j].date_from.Equals(""))
        //                {
        //                    prod.transform.Find("date_layout").gameObject.SetActive(true);
        //                    System.DateTime date_from = new System.DateTime(0), date_to = new System.DateTime(0);

        //                    date_from = System.DateTime.Parse(p.items[i].values[j].date_from);
        //                    if (p.items[i].values[j].date_to != null && !p.items[i].values[j].date_to.Equals(""))
        //                        date_to = System.DateTime.Parse(p.items[i].values[j].date_to);
        //                    prod.transform.Find("date_layout/txt_time").gameObject.GetComponent<Text>().text = date_from.ToShortDateString() + " " + date_from.ToShortTimeString();
        //                    if (date_to.Subtract(new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc)).TotalMilliseconds != 0 && !date_from.Equals(date_to) && date_to.Subtract(date_from).Milliseconds > 60000)
        //                    {
        //                        if (date_from.DayOfYear.Equals(date_to.DayOfYear))
        //                            prod.transform.Find("date_layout/txt_time").gameObject.GetComponent<Text>().text += " - " + date_to.ToShortTimeString();
        //                        else
        //                            prod.transform.Find("date_layout/txt_time").gameObject.GetComponent<Text>().text += " - " + date_to.ToShortDateString() + " " + date_to.ToShortTimeString();
        //                    }
        //                }
        //                prod.transform.SetParent(mainview.transform.Find("listview/viewport/content"), false);
        //            }


        //            GameObject prod2 = null;
        //            prod2 = Instantiate(Resources.Load("product"), new Vector3(0, -15, 0), Quaternion.identity) as GameObject;
        //            if (p.items[i].values[j].product_cover != null && !p.items[i].values[j].product_cover.Equals("") && !p.items[i].values[j].product_cover.Equals("NULL"))
        //            {
        //                prod2.transform.Find("cover_layout").gameObject.SetActive(true);
        //                prod2.transform.Find("cover_layout/text/caption/txt_title").gameObject.GetComponent<Text>().text = p.items[i].values[j].name;
        //                if (p.items[i].values[j].price != 0)
        //                    prod2.transform.Find("cover_layout/text/caption/txt_price").gameObject.GetComponent<Text>().text = p.items[i].values[j].price + " €";
        //                prod2.transform.Find("cover_layout/text/txt_description").gameObject.GetComponent<Text>().text = p.items[i].values[j].description;
        //                StartCoroutine(loadProductImage(p.items[i].values[j].product_cover, prod2.transform.Find("cover_layout/img_cover").gameObject.GetComponent<Image>()));
        //            }
        //            else
        //            {
        //                prod2.transform.Find("hlayout").gameObject.SetActive(true);
        //                prod2.transform.Find("hlayout/txt_title").gameObject.GetComponent<Text>().text = p.items[i].values[j].name;
        //                if (p.items[i].values[j].price != 0)
        //                    prod2.transform.Find("hlayout/txt_price").gameObject.GetComponent<Text>().text = p.items[i].values[j].price + " €";
        //                if (p.items[i].values[j].date_from != null && !p.items[i].values[j].date_from.Equals(""))
        //                {
        //                    prod2.transform.Find("date_layout").gameObject.SetActive(true);
        //                    System.DateTime date_from = new System.DateTime(0), date_to = new System.DateTime(0);

        //                    date_from = System.DateTime.Parse(p.items[i].values[j].date_from);
        //                    if (p.items[i].values[j].date_to != null && !p.items[i].values[j].date_to.Equals(""))
        //                        date_to = System.DateTime.Parse(p.items[i].values[j].date_to);
        //                    prod2.transform.Find("date_layout/txt_time").gameObject.GetComponent<Text>().text = date_from.ToShortDateString() + " " + date_from.ToShortTimeString();
        //                    if (date_to.Subtract(new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc)).TotalMilliseconds != 0 && !date_from.Equals(date_to) && date_to.Subtract(date_from).Milliseconds > 60000)
        //                    {
        //                        if (date_from.DayOfYear.Equals(date_to.DayOfYear))
        //                            prod2.transform.Find("date_layout/txt_time").gameObject.GetComponent<Text>().text += " - " + date_to.ToShortTimeString();
        //                        else
        //                            prod2.transform.Find("date_layout/txt_time").gameObject.GetComponent<Text>().text += " - " + date_to.ToShortDateString() + " " + date_to.ToShortTimeString();
        //                    }
        //                }
        //            }
        //            prod2.transform.SetParent(itemviews[i].transform.Find("listview/viewport/content"), false);
        //        }

        //        viewcount++;

        //    }
        //}
    }

    IEnumerator loadDetail()
    {
        WWW www = new WWW(Poi.poi_detail_base_url + "?id=" + poi.basic_poi.id);
        yield return www;
        initDetail(JsonUtility.FromJson<PoiJsonResponse_PoiDetail>(www.text));
    }

    IEnumerator loadProductImage(string url, Image img)
    {
        WWW www = new WWW(url);
        yield return www;
        img.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));
    }

    public override bool OnEvent(Event e)
    {

        if (e.type == EventType.KeyUp && e.keyCode == KeyCode.Escape && showing)
        {
            e.Use();
            hide();
            return false;
        }
        return true;
    }
}