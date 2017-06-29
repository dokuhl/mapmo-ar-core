using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DistrictArrowJsonResponse
{
    public DistrictArrowJsonResponse_DistrictArrow[] response;
}

[Serializable]
public class DistrictArrowJsonResponse_DistrictArrow : IComparable<DistrictArrowJsonResponse_DistrictArrow>
{
    private float dist_to_center;
    private float bearing;
    public string original_name;
    public string skybox_image;
    public float lat;
    public float lon;



    public int CompareTo(DistrictArrowJsonResponse_DistrictArrow other)
    {
        return other.dist_to_center.CompareTo(this.dist_to_center);
    }

    public float getDistToCenter()
    {
        return dist_to_center;
    }

    public void setDistToCenter( float dist_to_center)
    {
        this.dist_to_center = dist_to_center;
    }

    public float getBearing()
    {
        return bearing;
    }

    public void  setBearing(float bearing)
    {
        this.bearing = bearing;
    }
}