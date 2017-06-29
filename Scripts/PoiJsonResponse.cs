using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PoiJsonResponse{
    public PoiJsonResponse_Poi[] response;
}

[Serializable]
public class PoiJsonResponse_Poi
{
    public int id;
    public int parent_polygon;
    public float bearing;
    public float pos_lat;
    public float pos_lon;
    public float elevation;
    public int fan_count;
    public int checkins;
    public int foreign_id;
    public Contacts[] contacts;
    //public float width;
    public string original_name;
    public int[] super_categories;
    public string bv_image;
    public string icon;
}

[Serializable]
public class Contacts
{
    public string website;
    public string email;
    public string phone;
    public string mobile;
}

[Serializable]
public class PoiJsonResponse_PoiDetail
{
    public int id;
    public string description;
    public string video;
    public PoiJsonResponse_Item[] items;
}

[Serializable]
public class PoiJsonResponse_Item
{
    public string name;
    //public string description;
    public PoiJsonResponse_Product[] values;
}

[Serializable]
public class PoiJsonResponse_Product
{
    public int id;
    public string name;
    public string description;
    public string date_from;
    public string date_to;
    public string booking_link;
    public bool is_special;
    public string product_cover;
    public float price;
}