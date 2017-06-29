using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
namespace Assets.Scripts
{
    public class Marker : MonoBehaviour, IPointerClickHandler
    {
        public float m_x, m_y, z, t_s_x, t_s_y;
        public double lat, lon;
        public Map map;
        public Poi poi;
        public PoiHandler poi_handler_script;
        private new Transform transform;
        private Transform renderer_transform;
        private Transform mainCameraTransform;
        // Use this for initialization
        void Start()
        {
            transform = GetComponent<Transform>();
            mainCameraTransform = Camera.main.transform;
            renderer_transform = GetComponent<Renderer>().transform;
        }

        public void init(double lat, double lon)
        {
            transform = GetComponent<Transform>();
            mainCameraTransform = Camera.main.transform;
            renderer_transform = GetComponent<Renderer>().transform;
            this.lat = lat;
            this.lon = lon;
            if (map == null)
                map = GameObject.Find("Map").GetComponent<Map>();
            if (poi_handler_script == null)
                poi_handler_script = GameObject.Find("PoiHandler").GetComponent<PoiHandler>();
            float[] xy = map.tilepos(lon, lat, map.zoom);
            this.init(map.zoom, xy[0], xy[1], 10, 10);
        }

        public void init(Poi poi, double lat, double lon)
        {
            transform = GetComponent<Transform>();
            mainCameraTransform = Camera.main.transform;
            renderer_transform = GetComponent<Renderer>().transform;
            this.poi = poi;
            this.gameObject.name = "marker_" + poi.basic_poi.original_name;
            this.lat = lat;
            this.lon = lon;
            if (map == null)
                map = GameObject.Find("Map").GetComponent<Map>();
            if (poi_handler_script == null)
                poi_handler_script = GameObject.Find("PoiHandler").GetComponent<PoiHandler>();
            float[] xy = map.tilepos(lon, lat, map.zoom);
            if (poi.basic_poi.icon.Length > 4) {
                Texture2D texture = Resources.Load("icons/" + poi.basic_poi.icon.Substring(0, poi.basic_poi.icon.Length - 4), typeof(Texture2D)) as Texture2D;
                if (texture != null)
                    transform.Find("img_marker").GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
            }
            this.init(map.zoom, xy[0], xy[1], 10, 10);
        }

        public void init(float z, float m_x, float m_y, float t_s_x, float t_s_y)
        {
            if (map == null)
                map = GameObject.Find("Map").GetComponent<Map>();
            // set tile pos
            this.m_x = m_x;
            this.m_y = m_y;
            this.t_s_x = t_s_x;
            this.t_s_y = t_s_y;
            this.z = z;
            transform.localPosition = new Vector3(0.5f, 0, 0);
            moveMarker();
        }

        public void moveMarker()
        {
            TileHandler th = GameObject.Find("MapBackground/TileHandler").GetComponent<TileHandler>();
            float c_x = th.c_x;
            float c_y = th.c_y;
            float p_x = -1 * (c_x % 1) * t_s_x + t_s_x * (m_x - Mathf.Floor(c_x));
            float p_y = (c_y % 1) * t_s_y + t_s_y * (Mathf.Floor(c_y) - m_y);
            renderer_transform.position = new Vector3(p_x, -14.75f, p_y);
        }
        int updateCount = 0;
        void LateUpdate()
        {
            if (updateCount < 2) updateCount++;
            else
            {
                transform.rotation = Quaternion.Euler(90, mainCameraTransform.rotation.eulerAngles.y, 0);
                updateCount = 0;
            }     
        }
        

        public void OnPointerClick(PointerEventData eventData)
        {
            poi_handler_script.showBillboard(poi);
        }
    }
}