#define UNITY_ANDROID

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DeadMosquito.AndroidGoodies;
using UnityEngine.UI;

public class ARCamera : MonoBehaviour
{
    public int polygon_id = 0;
    public GameObject skybox;
    public bool camera_fixed = false;
    public bool camera_was_fixed_before = false;
    public float[] o = new float[4];
    public float compassHeading = 0;
    public float heading_correction = 0f;
    private int deviationStep = 10;
    private float?[] heading_correction_table = new float?[36];
    // Use this for initializationy
    private float timeInterval = 0;
    private float oldAngleY = 0;
    public Canvas preFabPoiCanvas;
    public Text prefabText;
    public Vector3 angles = new Vector3();
    private ArrayList vectorList = new ArrayList();
    private ArrayList trueHeadings = new ArrayList();
    public TextMesh txt;
    private Vector3 lastVectorChanged;
    public PoiNameBarScript poi_name_bar_script;
    private Dictionary<int, Canvas> poiIds = new Dictionary<int, Canvas>();
    private DeviceOrientation currentOrientation;
    private new Transform transform;
    private DeviceBridge deviceBridge;
    public Skybox skyboxScript;
    
    void Start()
    {
        Application.targetFrameRate = 30;
        transform = GetComponent<Transform>();
        vectorList.Add(new Vector3(0, 0, 0));
        trueHeadings.Add(0f);
        Input.compass.enabled = true;
        Input.gyro.enabled = true;
        Input.compensateSensors = true;
        // Minimum time interval between location updates, in milliseconds.
        //const long minTimeInMillis = 200;
        // Minimum distance between location updates, in meters.
        //const float minDistanceInMetres = 1;
        //AGGPS.RequestLocationUpdates(minTimeInMillis, minDistanceInMetres, OnLocationChanged);
        currentOrientation = Input.deviceOrientation;
        for(int i = 0; i< heading_correction_table.Length; i++)
        {
            heading_correction_table[i] = null;
        }
        deviceBridge = GameObject.Find("DeviceBridge").GetComponent<DeviceBridge>();
        //skyboxScript = skybox.GetComponent<Skybox>();
        //Input.location.Start();
    }

    //private void OnLocationChanged(AGGPS.Location location)
    //{
    //    txt.text = location.Latitude.ToString() + " " + location.Longitude.ToString();
    //}
    private void FixedUpdate()
    {

    }

    void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out hit))
        {
            Transform objectHit = hit.transform;

            if (objectHit.name.Contains("fade_building"))
            {
                if (poi_name_bar_script.polygon_id != objectHit.GetComponent<InvisBuilding>().id)
                    poi_name_bar_script.switchBuilding(objectHit.GetComponent<InvisBuilding>());
            }
            else
                poi_name_bar_script.disableView();
            // Do something with the object that was hit by the raycast.
        }

        //if (Input.GetKeyUp(KeyCode.Escape))
        //{
        //    AndroidJavaClass jc = new AndroidJavaClass("de.mapmo.ARTest.UnityPlayerActivity");
        //    AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("UnityPlayerActivity");
        //    jo.Call("onBackPressed");
        //}
    }

    // Update is called once per frame (end of frame)
    private float additionalRotationX = 0;
    private float additionalRotationZ = 0;
    void LateUpdate()
    {
        //timeInterval += Time.deltaTime;
        ////float trueHeading = calcTrueHeading(Input.acceleration.x, Input.acceleration.z);

        //float angleX = -Input.gyro.rotationRateUnbiased.x;
        //float angleY = -Input.gyro.rotationRateUnbiased.y + oldAngleY;
        //float angleZ = Input.gyro.rotationRateUnbiased.z;

        //float x = Input.acceleration.x;
        //float y = Input.acceleration.y;
        //float z = Input.acceleration.z;
        //if (currentOrientation != Input.deviceOrientation)
        //{
        //    if (Input.deviceOrientation == DeviceOrientation.Portrait)
        //    {
        //        additionalRotationZ = 0;
        //    }
        //    else
        //        if (Input.deviceOrientation == DeviceOrientation.LandscapeLeft)
        //    {
        //        float newZ = transform.eulerAngles.z + 90;
        //        newZ %= 360;
        //        transform.Rotate(transform.eulerAngles.x, transform.eulerAngles.y, newZ);
        //        additionalRotationZ = 90;
        //    }
        //    else
        //        if (Input.deviceOrientation == DeviceOrientation.LandscapeRight)
        //    {
        //        float newZ = transform.eulerAngles.z - 90;
        //        if (newZ < 0) newZ += 360;
        //        transform.Rotate(transform.eulerAngles.x, transform.eulerAngles.y, newZ);
        //        additionalRotationZ = -90;
        //    }
        //    else
        //        if (Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown)
        //    {
        //        float newZ = transform.eulerAngles.z + 180;
        //        newZ %= 360;
        //        transform.Rotate(transform.eulerAngles.x, transform.eulerAngles.y, newZ);
        //        additionalRotationZ = 180;
        //    }
        //}
        //if (Input.deviceOrientation == DeviceOrientation.LandscapeLeft)
        //{
        //    x = -Input.acceleration.y;
        //    y = Input.acceleration.x;
        //}
        //else
        //if (Input.deviceOrientation == DeviceOrientation.LandscapeRight)
        //{
        //    x = Input.acceleration.y;
        //    y = -Input.acceleration.x;
        //}
        //else
        //if (Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown)
        //{
        //    x = -Input.acceleration.x;
        //    y = -Input.acceleration.y;
        //}
        //currentOrientation = Input.deviceOrientation;

        //float xrot = Mathf.Atan2(z, y);
        //float yzmag = Mathf.Sqrt(Mathf.Pow(y, 2) + Mathf.Pow(z, 2));
        //float zrot = Mathf.Atan2(x, yzmag);

        //float xangle = xrot * (180 / Mathf.PI) + 180;
        //if (xangle < 0) xangle += 360;
        //xangle %= 360;
        //float zangle = -zrot * (180 / Mathf.PI) + additionalRotationZ;
        //if (zangle < 0) zangle += 360;
        //zangle %= 360;


        //if (Mathf.Abs(Input.gyro.rotationRateUnbiased.y) < 0.1)
        //{
        //    float rotationY = transform.eulerAngles.y;
        //    float trueHeading = calcTrueHeading(x, z);
        //    compassHeading = trueHeading;
        //    trueHeading += heading_correction;
        //    float lastHeading = ((trueHeading - rotationY) < -180) ? rotationY - 360 : (((trueHeading - rotationY) > 180) ? rotationY + 360 : rotationY);
        //    float t = Time.deltaTime * Mathf.Abs(lastHeading - trueHeading) / 20;
        //    if(!camera_fixed)
        //        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(transform.eulerAngles.x + additionalRotationX, trueHeading, transform.eulerAngles.z), Time.deltaTime);
        //    oldAngleY = 0;
        //    timeInterval = 0;
        //    //txt.text = "1 x: " + transform.eulerAngles.x + "\n y: " + transform.eulerAngles.y + "\n z: " + transform.eulerAngles.z + "\n" + trueHeading + "\n" + Input.deviceOrientation;
        //}
        //else
        //{
        //    if (Mathf.Abs(angleY) > 0.5f)
        //    {
        //        transform.Rotate(0, angleY, 0);
        //        oldAngleY = 0;
        //    }
        //    else
        //    {
        //        oldAngleY = angleY;
        //    }
        //}

        //float rotationX = transform.eulerAngles.x;
        //float lastHeadingX = ((xangle - rotationX) < -180) ? rotationX - 360 : (((xangle - rotationX) > 180) ? rotationX + 360 : rotationX);
        //float timeX = Time.deltaTime * Mathf.Abs(lastHeadingX - xangle) / 2;
        //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(xangle, transform.eulerAngles.y, transform.eulerAngles.z), timeX);

        //float rotationZ = transform.eulerAngles.z;
        //float lastHeadingZ = ((zangle - rotationZ) < -180) ? rotationZ - 360 : (((zangle - rotationZ) > 180) ? rotationZ + 360 : rotationZ);
        //float timeZ = Time.deltaTime * Mathf.Abs(lastHeadingZ - zangle) / 4;
        //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, zangle), timeZ);
        if (!camera_fixed)
        {
            o = deviceBridge.getOrientation();
            Quaternion compass = new Quaternion(o[0], o[1], o[2], o[3]);
            float? current_heading_correction = null;
            int correctionIndex = Mathf.FloorToInt(compass.eulerAngles.y / deviationStep);
            // insert heading_correction in table if there was a correction drag
            if (camera_was_fixed_before)
            {
                camera_was_fixed_before = false;
                if (heading_correction < -180) heading_correction += 360;
                heading_correction_table[correctionIndex] = heading_correction;
                current_heading_correction = Mathf.Floor(heading_correction);
            }else
            {
                current_heading_correction = heading_correction_table[correctionIndex];
                //if there is no correction added in the table for the current angle calculate it from the nearest corrections
                if (current_heading_correction == null)
                {
                    float? valueLeft = null;
                    float? valueRight = null;
                    int indexDistanceLeft = 0;
                    int indexDistanceRight = 0;
                    int indexLeft = 0;
                    int indexRight = 0;
                    //search for the nearest corrections left and right of the current angle in the table
                    for (int i = 1; i < heading_correction_table.Length; i++)
                    {
                        if (valueLeft == null)
                        {
                            indexLeft = (heading_correction_table.Length + correctionIndex - i) % heading_correction_table.Length;
                            valueLeft = heading_correction_table[indexLeft];
                            if(valueLeft!=null)
                                indexDistanceLeft = i;
                        }
                        if (valueRight == null)
                        {
                            indexRight = (correctionIndex + i) % heading_correction_table.Length;
                            valueRight = heading_correction_table[indexRight];
                            if(valueRight!=null)
                                indexDistanceRight = i;
                        }
                        if (valueLeft != null && valueRight != null)
                        {
                            //calculate the correction in relation to the distance of the nearest corrections from the table
                            current_heading_correction = Mathf.Floor((float)(valueLeft * ((float)indexDistanceRight / (indexDistanceLeft + indexDistanceRight)) + valueRight * ((float)indexDistanceLeft / (indexDistanceLeft + indexDistanceRight))));
                            break;
                        }
                    }
                }
               
            }
            if (current_heading_correction == null) current_heading_correction = 0;
            float deltaTime = Time.deltaTime* Mathf.Abs((float)(transform.eulerAngles.y-compass.eulerAngles.y - current_heading_correction));
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(compass.eulerAngles.x, (float)(compass.eulerAngles.y + current_heading_correction), compass.eulerAngles.z), deltaTime);
            //transform.eulerAngles = new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + heading_correction, transform.rotation.eulerAngles.z);
        }else
        {
            if(!skyboxScript.isInside && !skyboxScript.isDistrict)
                camera_was_fixed_before = true;
        }

        //txt.text = "x: " + x + "\n y: " + y + "\n z: " + z;
        //txt.text += "\n true Y: " + Input.compass.trueHeading;
        //txt.text += "\n 0 smoothedHeading: " + rotationY;
        //txt.text = "";
    }

    public void rotateByDrag (Vector3 angles)
    {
        //transform.Rotate(angles);
        transform.rotation = Quaternion.Euler(angles.x+ transform.eulerAngles.x, angles.y + transform.eulerAngles.y, 0f);
    }

    private float getSmoothedTrueHeading(float newHeading)
    {
        float headingSum = 0f;
        float lastHeading = (float)trueHeadings[trueHeadings.Count - 1];

        for (int i = 0; i < trueHeadings.Count; i++)
        {
            float oldHeading = (float)trueHeadings[i];
            float delta = newHeading - oldHeading;
            headingSum += (delta < -180) ? oldHeading - 360 : ((delta > 180) ? oldHeading + 360 : oldHeading);
        }
        headingSum += newHeading;
        float calculatedHeading = headingSum / (trueHeadings.Count + 1);
        if (calculatedHeading < 0) calculatedHeading += 360;
        calculatedHeading %= 360;
        calculatedHeading = Mathf.Round(calculatedHeading);

        //float deltaHeading = calculatedHeading - lastHeading;
        //lastHeading = (deltaHeading < -180) ? lastHeading - 360 : ((deltaHeading > 180) ? lastHeading + 360 : lastHeading);
        trueHeadings.Add(calculatedHeading);
        if (trueHeadings.Count >= 2)
            trueHeadings.RemoveAt(0);
        //if (Mathf.Abs(lastHeading - calculatedHeading) > 20)
        //{
        return calculatedHeading;
        //}
        //return lastHeading;
    }
    private float calcTrueHeading(float x, float z)
    {
        float trueHeading = Input.compass.trueHeading;

        if (Input.deviceOrientation == DeviceOrientation.Portrait || Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown)
        {
            float yangle = 0;
            float yrot = 0;
            yrot = Mathf.Atan2(z, x);
            if (Input.deviceOrientation == DeviceOrientation.Portrait)
                yangle = yrot * (180 / Mathf.PI) + 90;
            else
                yangle = yrot * (180 / Mathf.PI) - 90;
            trueHeading -= yangle;
        }
        else if (Input.deviceOrientation == DeviceOrientation.LandscapeLeft)
        {
            trueHeading += 90;
        }
        else if (Input.deviceOrientation == DeviceOrientation.LandscapeRight)
        {
            trueHeading -= 90;
        }

        if (trueHeading < 0)
            trueHeading += 360;
        trueHeading %= 360;
        //return trueHeading;
        return getSmoothedTrueHeading(trueHeading);
    }
}