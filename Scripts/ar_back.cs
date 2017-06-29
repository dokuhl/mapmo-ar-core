using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ar_back : MonoBehaviour
{
    public UnityEngine.UI.RawImage ri;
    public Quaternion baseRotation;
    public WebCamTexture wct;
    public Canvas canvas;
    public TextMesh txt;

    // Use this for initialization
    void Start()
    {
        WebCamDevice[] wcd = WebCamTexture.devices;
        wct = new WebCamTexture();
        if (wcd.Length > 0)
        {

            wct.deviceName = wcd[0].name;
            wct.Play();
        }

        wct.filterMode = FilterMode.Trilinear;
        ri.material.mainTexture = wct;
        ri.texture = wct;
        baseRotation = transform.rotation;
        Screen.orientation = ScreenOrientation.Portrait;
        
    }

    // Update is called once per frame
    void Update()
    {
            //txt.text = wct.width.ToString() +" " + wct.height.ToString() + " " + ((float)wct.width / (float)wct.height).ToString();


            //if (wct.width < 100)
            //    return;

            int cwNeeded = wct.videoRotationAngle;
            int ccwNeeded = -cwNeeded;
            if (wct.videoVerticallyMirrored) ccwNeeded += 180;

            // you'll be using a UI RawImage, so simply spin the RectTransform

            ri.rectTransform.localEulerAngles = new Vector3(0f, 0f, ccwNeeded);

            float videoRatio = (float)wct.width / (float)wct.height;
            //ri.rectTransform.localScale = new Vector3(ri.rectTransform.localScale.y * videoRatio, ri.rectTransform.localScale.y, 1);

            ri.GetComponent<RectTransform>().sizeDelta = new Vector2(ri.GetComponent<RectTransform>().sizeDelta.y * videoRatio, ri.GetComponent<RectTransform>().sizeDelta.y);
            // you'll be using an AspectRatioFitter on the Image, so simply set it

            //ri.GetComponent<UnityEngine.UI.AspectRatioFitter>().aspectRatio = videoRatio;

            // alert, the ONLY way to mirror a RAW image, is, the uvRect.
            // changing the scale is completely broken.
            //if (wct.videoVerticallyMirrored)
            //    ri.uvRect = new Rect(1, 0, -1, 1);  // means flip on vertical axis
            //else
            //    ri.uvRect = new Rect(0, 0, 1, 1);  // means no flip

            // devText.text =
            //  videoRotationAngle+"/"+ratio+"/"+wct.videoVerticallyMirrored;
    }
}