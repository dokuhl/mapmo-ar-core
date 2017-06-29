using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ar_back_plane: MonoBehaviour
{
    public GameObject pln;
    private Quaternion baseRotation;
    public WebCamTexture wct;
    public Camera cam;
    public TextMesh txt;
    public float nearer = 30000f;
    private float pos;
    private float cam_aspect_ratio;
    private new Transform transform;
    private Transform cam_transform;
    // Use this for initialization
    void Start()
    {
        transform = GetComponent<Transform>();
        cam_transform = cam.transform;
        WebCamDevice[] wcd = WebCamTexture.devices;
        wct = new WebCamTexture();
        if (wcd.Length > 0)
        {
            wct.requestedHeight = Camera.main.pixelHeight;
            wct.requestedWidth = Camera.main.pixelWidth;
            wct.requestedFPS = 30;
            wct.deviceName = wcd[0].name;
            wct.Play();
        }
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.mainTexture = wct;
        baseRotation = transform.rotation;
        pos = cam.farClipPlane - nearer;
        wct.filterMode = FilterMode.Trilinear;
        
        
    }

    private void Awake()
    {
        Application.targetFrameRate = 30;
        if (wct != null)
            wct.Play();
    }

    private void OnApplicationPause(bool pause)
    {
        if (wct != null)
        {
            if (pause)
            {
                wct.Pause();
            }
            else
            {
                wct.Play();
            }
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        if (wct != null)
        {
            if (focus)
            {
                wct.Play();
            }
            else
            {
                wct.Pause();
            }
        }
    }


    public void WebCamPause()
    {
        if (wct != null)
            wct.Pause();
    }

    public void WebCamPlay()
    {
        if (wct != null)
            wct.Play();
    }

    float old_width = 0;
    float old_height = 0;
    float old_rotationAngle = 0;
    // Update is called once per frame
    void Update()
    {
        if (wct.width > 100 && (wct.width!=old_width || wct.height!=old_height || wct.videoRotationAngle!=old_rotationAngle))
        {
            old_width = wct.width;
            old_height = wct.height;
            old_rotationAngle = wct.videoRotationAngle;
            pos = cam.farClipPlane - nearer;
            Vector3 pln_scale = new Vector3();
            //txt.text = wct.videoRotationAngle.ToString() + " " + wct.videoVerticallyMirrored.ToString();
            
            
            //position the plane near the end of the cameraview
            transform.position = cam_transform.position + cam_transform.forward * pos;

            //get webcam rotation angle + mirrored
            float rot = wct.videoRotationAngle + ((wct.videoVerticallyMirrored) ? 180f : 0f);

            // rotate plane by rotation angle + mirrored
            transform.localRotation = baseRotation * Quaternion.AngleAxis(rot, Vector3.down);

            // height of plane in gu 
            float h = Mathf.Tan(cam.fieldOfView * Mathf.Deg2Rad * 0.5f) * pos * 2f;


            // webcamt aspect ratio as vector
            cam_aspect_ratio = (float)wct.width / (float)wct.height;

           
            // project on x and z TODO
            // dirty fix for now:
            pln_scale = new Vector3(h / 10, 1f, h / (10 * cam_aspect_ratio) );
            transform.localScale = pln_scale;
            //GameObject.Find("gyro_debug").GetComponent<UnityEngine.UI.Text>().text = wct.width + " " + wct.height + " " + cam_aspect_ratio;
        }
        
    }
}