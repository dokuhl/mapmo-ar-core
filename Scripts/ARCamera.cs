#define UNITY_ANDROID

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        //initialize heading_correction_table with all null
        for(int i = 0; i< heading_correction_table.Length; i++)
        {
            heading_correction_table[i] = null;
        }
        deviceBridge = GameObject.Find("DeviceBridge").GetComponent<DeviceBridge>();
        if (poi_name_bar_script == null)
            poi_name_bar_script = GameObject.Find("POINameBar").GetComponent<PoiNameBarScript>();
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
            else if (objectHit.name.Contains("building_icon"))
            {
                if (poi_name_bar_script.polygon_id != objectHit.parent.GetComponent<InvisBuilding>().id)
                    poi_name_bar_script.switchBuilding(objectHit.parent.GetComponent<InvisBuilding>());
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
            float deltaDegrees = Mathf.Abs((float)(transform.eulerAngles.y - compass.eulerAngles.y - current_heading_correction));
            float newHeading = (float)(compass.eulerAngles.y + current_heading_correction);
            if (deltaDegrees > 180)
            {
                newHeading += (newHeading > 180) ? -360 : 360;
                deltaDegrees = Mathf.Abs(transform.eulerAngles.y - newHeading);
            }
            float deltaTime = Time.deltaTime* deltaDegrees;
            
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(compass.eulerAngles.x, newHeading, compass.eulerAngles.z), deltaTime);
            //transform.eulerAngles = new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + heading_correction, transform.rotation.eulerAngles.z);
        }else
        {
            if(!skyboxScript.isInside && !skyboxScript.isDistrict)
                camera_was_fixed_before = true;
        }
    }

    public void rotateByDrag (Vector3 angles)
    {
        transform.rotation = Quaternion.Euler(angles.x+ transform.eulerAngles.x, angles.y + transform.eulerAngles.y, 0f);
    }
}