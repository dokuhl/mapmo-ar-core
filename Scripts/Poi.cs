using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{

    public class Poi : UnityEngine.Object
    {
        public static string poi_detail_base_url = "http://api.mapmo.de/mapmo/poi_details";
        public Marker marker;
        Map map;
        public double bearing, lat, lon, elevation;
        public string original_name;
        public string description;
        public int[] super_categories;
        public int id;
        public PoiJsonResponse_Poi basic_poi;
        public PoiJsonResponse_PoiDetail detail_poi;
        

        public GameObject billboard;
        //public GameObject signboard;
        //public String showCase = "signboard";
        public bool camera_lock = false;
        bool initRotationDone = false;
        public int flipBillboard =0;

        public override bool Equals(System.Object obj)
        {
            if (obj == null)
            {
                return false;
            }
            Poi p = obj as Poi;
            return Equals(this.id, p.id);
        }

        public void kill()
        {
            DestroyImmediate(billboard,true);
            //DestroyImmediate(signboard, true);
            DestroyImmediate(marker.gameObject, true);
            DestroyImmediate(this, true);
        }

        

        public Poi(PoiJsonResponse_Poi p)
        {
            if (map == null)
                map = GameObject.Find("Map").GetComponent<Map>();
            this.lat = p.pos_lat;
            this.lon = p.pos_lon;
            this.id = p.id;
            this.bearing = p.bearing;
            this.elevation = p.elevation;
            this.original_name = p.original_name;
            this.super_categories = p.super_categories;
            this.basic_poi = p;
            billboard = Instantiate(Resources.Load("billboard"), new Vector3(0, -15, 0), Quaternion.identity) as GameObject;
            billboard.transform.SetParent(GameObject.Find("PoiContainer").transform);
            //  signboard = Instantiate(Resources.Load("signboard"), new Vector3(0, -15, 0), Quaternion.identity) as GameObject;
            billboard.GetComponent<Billboard>().init(this.lat, this.lon, this.bearing, this);
            //signboard.GetComponent<Signboard>().init(this.lat, this.lon, this.bearing, this);

            marker = (Instantiate(Resources.Load("Marker"), new Vector3(0, -14f, 0), Quaternion.identity) as GameObject).GetComponent<Marker>();
            marker.init(this, this.lat, this.lon);
            updatePoi(0);
        }

        public Poi(double lat, double lon, double bearing)
        {
            this.lat = lat;
            this.lon = lon;
            this.bearing = bearing;

            if (map == null)
                map = GameObject.Find("Map").GetComponent<Map>();

            billboard = Instantiate(Resources.Load("billboard"), new Vector3(0, -15, 0), Quaternion.identity) as GameObject;
            //signboard = Instantiate(Resources.Load("signboard"), new Vector3(0, -15, 0), Quaternion.identity) as GameObject;

            billboard.GetComponent<Billboard>().init(this.lat, this.lon, this.bearing, this);
            //signboard.GetComponent<Signboard>().init(this.lat, this.lon, this.bearing, this);
            updatePoi(0);
        }

        public void updatePoi(int index)
        {
            // TODO: Performancekiller killen
            //if (signboard == null || billboard == null) return;
            float cameraAngle = 0;
            float lookRotation = 0;

            //switch (showCase)
            //{
            //    case "signboard":
            //        cameraAngle = Quaternion.LookRotation(signboard.transform.position - Camera.main.transform.position).eulerAngles.y;                  
            //        //Debug.Log("signboard: " + lookRotation + " " + Quaternion.LookRotation(signboard.transform.position - Camera.main.transform.position).eulerAngles.y);
            //        break;
            //    case "billboard":
            //        cameraAngle = Quaternion.LookRotation(billboard.transform.position - Camera.main.transform.position).eulerAngles.y;
            //        //Debug.Log("lookRotation: " + lookRotation + " " + bearing + " " + Quaternion.LookRotation(billboard.transform.position - Camera.main.transform.position).eulerAngles.y);
            //        break;
            //}
            if (!initRotationDone)
            {
                cameraAngle = Quaternion.LookRotation(billboard.transform.position - Camera.main.transform.position).eulerAngles.y;
                float rotation = Mathf.Abs(cameraAngle + 90 - (float)bearing);
                rotation %= 360;
                lookRotation = Mathf.Abs(rotation - 180);
                if (lookRotation > 90)
                {                
                    billboard.transform.Rotate(new Vector3(0, 180, 0));
                    flipBillboard = 1;
                }
                //lookRotation = 180 - lookRotation;
            }
            initRotationDone = true;
            //DeadMosquito.AndroidGoodies.AGUIMisc.ShowToast(" " + lookRotation);
            float d = 1000 * (float)map.dist(lat, lon);
            //String newShowCase = "billboard";
            //if (d < 100)
            //{
            //    if ((showCase.Equals("billboard") && lookRotation > 65.0f) || (!showCase.Equals("billboard") && lookRotation > 45.0f))
            //    {
            //        newShowCase = "signboard";
            //    }
            //}
            //else
            //{
            //    newShowCase = "signboard";
            //}
            //if (!newShowCase.Equals(showCase) && !camera_lock)
            //{
            //    switch (newShowCase)
            //    {
            //        case "signboard":
            //            billboard.GetComponent<Billboard>().scaleOut();
            //            signboard.SetActive(true);
            //            signboard.GetComponent<Signboard>().scaleIn();
            //            break;
            //        case "billboard":
            //            signboard.GetComponent<Signboard>().scaleOut();
            //            billboard.SetActive(true);
            //            billboard.GetComponent<Billboard>().scaleIn();
            //            break;
            //    }
            //    showCase = newShowCase;
            //}
            //switch (showCase)
            //{
            //    case "signboard":
            //        signboard.GetComponent<Signboard>().updateSignboard();
            //        break;
            //    case "billboard":
            //        billboard.GetComponent<Billboard>().updateBillboard();
            //        break;
            //}
            billboard.GetComponent<Billboard>().updateBillboard();
            
        }


        //public void animateSwitch()
        //{
        //    if(camera_lock && billboard.GetComponent<Billboard>().to_cam_ratio==1)
        //    {
        //        animateBillboardToWorldPos();
        //    } else
        //    {
        //        animateBillboardToCamera();
        //    }
        //}

        //public void animateBillboardToCamera()
        //{
        //    camera_lock = true;
        //    if (!billboard.activeSelf)
        //    {
        //        billboard.SetActive(true);
        //    }
        //    if (!ReferenceEquals(null,map.selectedPoi) && !Equals(map.selectedPoi))
        //    {
        //        map.selectedPoi.billboard.GetComponent<Billboard>().switchPanel();
        //        map.selectedPoi.animateBillboardToWorldPos();
        //    }          
           
        //    map.selectedPoi = this;
        //    billboard.GetComponent<Billboard>().animateToCamera();
        //}

        //public void animateBillboardToWorldPos()
        //{
        //    if (Equals(map.selectedPoi))
        //        map.selectedPoi = null;
        //    billboard.GetComponent<Billboard>().animateToWorldPos();
        //}
    }
}
