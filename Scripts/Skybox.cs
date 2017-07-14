using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

public class Skybox : EventBehaviour {

    public float scale = 45000f;
    public float upper_scale = 45000f;
    public float low_scale = 18000f;
    public float opacity_incr = 500f;
    public bool isPlaying = false;
    public bool isDistrict = false;
    public bool isInside = false;
    public string district_image = "";
    Texture2D cached_skybox;
    private Transform skyboxTransform;
    private Texture2D default_skybox;
	void Start ()
    {
        skyboxTransform = transform.Find("skybox");
        //GameObject.Find("event").GetComponent<EventpropagationHandler>().register(this);
	}
	
	void Update () {

	}

    public void fadeIn()
    {
        skyboxTransform.GetComponent<Renderer>().enabled = true;
        StartCoroutine(fadeInRoutine());
    }

    public void fadeOut()
    {
        skyboxTransform.GetComponent<Renderer>().enabled = false;
        StartCoroutine(fadeOutRoutine());
    }
    private IEnumerator fadeInRoutine()
    {

        while (scale > low_scale)
        {
            yield return new WaitForSeconds(0.03f);
            scale -= opacity_incr;
            skyboxTransform.localScale = new Vector3(scale, scale, scale);
        }

        scale = low_scale;
        skyboxTransform.localScale = new Vector3(scale, scale, scale);
        GameObject.Find("Main Camera").GetComponent<DialogueHandler>().hideLoadingScreen();
    }

    private IEnumerator fadeOutRoutine()
    {
        
        while (scale < upper_scale)
        {
            yield return new WaitForSeconds(0.03f);
            scale += opacity_incr;
            skyboxTransform.localScale = new Vector3(scale, scale, scale);
        }

        scale = upper_scale;
        skyboxTransform.localScale = new Vector3(scale, scale, scale);
        GameObject.Find("Main Camera").GetComponent<DialogueHandler>().hideLoadingScreen();
    }

    public void stepInsideFoto(string url)
    {

    }

    public void onPlayOrPause()
    {
        if (isInside)
        {
            if (isPlaying)
            {
                //skyboxTransform.GetComponent<MediaPlayerCtrl>().Pause();
                isPlaying = false;
            }
            else
            {
                //skyboxTransform.GetComponent<MediaPlayerCtrl>().Play();
                isPlaying = true;
            }
        }
    }

    public void stepInsideVideo(string url)
    {
        fadeIn();
        disablePois();
        disableBuildings();
        isPlaying = true;
        isInside = true;
        //transform.Find("skybox").GetComponent<YoutubeEasyMovieTexture>().youtubeVideoIdOrUrl = url;
        //transform.Find("skybox").GetComponent<YoutubeEasyMovieTexture>().LoadYoutubeInTexture();
        //transform.Find("skybox").GetComponent<MediaPlayerCtrl>().Play();
        //transform.Find("skybox").GetComponent<Renderer>().material.mainTextureScale = new Vector2(-1, 1);
        //transform.Find("skybox").GetComponent<Renderer>().material.mainTextureOffset = new Vector2(1, 0);
    }



    public void districtFoto(string url)
    {

    }

    public void districtVideo(string url)
    {

    }


    public void playVideo(string url)
    {
        //skyboxTransform.GetComponent<MediaPlayerCtrl>().m_strFileName = url;
        //skyboxTransform.GetComponent<MediaPlayerCtrl>().Play();
    }

    public void pauseVideo()
    {
        //skyboxTransform.GetComponent<MediaPlayerCtrl>().Pause();
    }

    public void disablePois()
    {
        foreach (Poi p in GameObject.Find("PoiHandler").GetComponent<PoiHandler>().pois)
        {
            p.billboard.SetActive(false);
        }
    } 

    public void enablePois()
    {
        foreach (Poi p in GameObject.Find("PoiHandler").GetComponent<PoiHandler>().pois)
        {
            p.billboard.SetActive(true);
        }
    }

    public void disableBuildings()
    {
        foreach (InvisBuilding ib in GameObject.Find("InvisBuildingHandler").GetComponent<InvisBuildingHandler>().all_buildings)
            ib.gameObject.SetActive(false);
    } 

    public void enableBuildings()
    {
        foreach (InvisBuilding ib in GameObject.Find("InvisBuildingHandler").GetComponent<InvisBuildingHandler>().all_buildings)
            ib.gameObject.SetActive(true);
    }

    public void showDistrict(string image, float offset)
    {
        isDistrict = true;
        if(skyboxTransform == null)
            skyboxTransform = transform.Find("skybox");
        //skyboxTransform.GetComponent<Renderer>().material.mainTextureScale = new Vector2(-1,-1);
        //skyboxTransform.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(1, 1);
        //GameObject.Find("Main Camera").GetComponent<DialogueHandler>().showLoadingScreen("going to district");
        skyboxTransform.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(offset, 0);
        if (image.Equals("") || image == null)
        {
            Texture t = skyboxTransform.GetComponent<Renderer>().material.mainTexture;
            if(t!=null && t.name != "360_standard")
            {
                Destroy(t);
            }
            //standard image
            if (default_skybox == null)
                default_skybox = Resources.Load("Textures/360_standard", typeof(Texture2D)) as Texture2D;
            skyboxTransform.GetComponent<Renderer>().material.mainTexture = default_skybox;
            fadeIn();
        }
        else
        {
            //load 360 image
            StartCoroutine(loadImg(image));
        }
            
    }

    private IEnumerator loadImg(string image)
    {
        this.district_image = image;
        WWW www = new WWW(image);
        yield return www;
        GameObject.Find("Main Camera").GetComponent<ARCamera>().camera_fixed = false;
        Texture t = skyboxTransform.GetComponent<Renderer>().material.mainTexture;
        if (t != null && t.name != "360_standard")
        {
            Destroy(t);
        }
        skyboxTransform.GetComponent<Renderer>().material.mainTexture = www.texture;
        Destroy(cached_skybox);
        cached_skybox = www.texture;
        fadeIn();
    }

    public override bool OnEvent(Event e)
    {
        
        if(e.type == EventType.KeyUp && e.keyCode == KeyCode.Escape && (isDistrict || isInside))
        {
            e.Use();
            Debug.Log("ESCAPE");
            if (isDistrict && isInside)
            {
                //skyboxTransform.GetComponent<MediaPlayerCtrl>().m_strFileName = "";
                loadImg(district_image);
                isInside = false;
                GameObject.Find("Main Camera").GetComponent<DialogueHandler>().showLoadingScreen("returning to district");
                GameObject.Find("Main Camera").GetComponent<ARCamera>().camera_fixed = false;
                enableBuildings();
                enablePois();
                Texture t = skyboxTransform.GetComponent<Renderer>().material.mainTexture;
                if (t != null && t.name != "360_standard")
                {
                    Destroy(t);
                }
                skyboxTransform.GetComponent<Renderer>().material.mainTexture = cached_skybox;
                return false;
            }
            else if(isDistrict && !isInside)
            {
                isDistrict = false;
                Debug.Log("returning");
                GameObject.Find("Main Camera/CameraFeed").GetComponent<ar_back_plane>().WebCamPlay();
                GameObject.Find("Main Camera").GetComponent<DialogueHandler>().showLoadingScreen("returning to position");
                GameObject.Find("Main Camera").GetComponent<ARCamera>().camera_fixed = false;
                Map map = GameObject.Find("Map").GetComponent<Map>();
                map.lat = map.orig_lat;
                map.lon = map.orig_lon;
                map.orig_lat = 0f;
                map.orig_lon = 0f;
                map.updateCenter();
                map.gps_fixed = false;
                fadeOut();
                return false;
            } else if(!isDistrict && isInside)
            {
                //skyboxTransform.GetComponent<MediaPlayerCtrl>().m_strFileName = "";
                isInside = false;
                enableBuildings();
                enablePois();
                GameObject.Find("Main Camera/CameraFeed").GetComponent<ar_back_plane>().WebCamPlay();
                fadeOut();
                
                GameObject.Find("Main Camera").GetComponent<DialogueHandler>().showLoadingScreen("returning to position");
                GameObject.Find("Main Camera").GetComponent<ARCamera>().camera_fixed = false;
                
                return false;
            }

            
        }
        return true;
    }
}
