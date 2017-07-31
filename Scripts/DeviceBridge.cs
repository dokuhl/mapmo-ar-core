using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeviceBridge : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
    public void AndroidCallNonStatic()
    {
        //AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        //unityPlayer.Call("test");
        //AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        //AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");
        //activity.CallStatic("test", context);

        
    }
    public void testStartSensor()
    {
        GameObject.Find("sstxt").GetComponent<UnityEngine.UI.Text>().text = "Start Sensor: " + startSensor().ToString();
    }

    public void testStopSensor()
    {
        GameObject.Find("ststxt").GetComponent<UnityEngine.UI.Text>().text = "Stop Sensor: " + startSensor().ToString();
    }

    public void testStartCalibration()
    {
        startCalibration();
        StartCoroutine(testProgress());
    }

    IEnumerator testProgress()
    {
        float p = 0; 
        while (p < 1)
        {
            p = getCalibrationProgress();
            GameObject.Find("sctxt").GetComponent<UnityEngine.UI.Text>().text = "Calibration: " + p;
            yield return new WaitForSeconds(0.05f);
        }
    }
    public int startSensor() {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("de.mapmo.ARTest.UnityPlayerActivity");
        return unityPlayer.CallStatic<int>("onSensorStart");
    }
    public int stopSensor() {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("de.mapmo.ARTest.UnityPlayerActivity");
        return unityPlayer.CallStatic<int>("onSensorStop");
    }
    public void startCalibration() {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("de.mapmo.ARTest.UnityPlayerActivity");
        unityPlayer.CallStatic("startCalibration");
    }
    public float getCalibrationProgress() {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("de.mapmo.ARTest.UnityPlayerActivity");
        return unityPlayer.CallStatic<float>("getCalibrationProgress");
    }
    public float[] getOrientation() {
        //return new float[4]  {1,2,3,4};
        if(Application.platform == RuntimePlatform.Android) {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("de.mapmo.ARTest.UnityPlayerActivity");
            return unityPlayer.CallStatic<float[]>("getOrientation");
        } else
        {
            return new float[4] { 0, 0, 0, 0 };
        }
        
    }


    public void AndroidCallStatic()
    {
        string txt = "ss:";
        txt += startSensor();
        txt += "sts:"+stopSensor();
        startCalibration();
        txt += "cp:" + getCalibrationProgress();
        float[] f1 = getOrientation();
        txt += "x:" + f1[0] + "y:" + f1[1] + "z:" + f1[2] + "w:" + f1[3];
        GameObject.Find("btn14txt").GetComponent<UnityEngine.UI.Text>().text = txt;
        //AndroidJavaClass unityPlayer = new AndroidJavaClass("de.mapmo.ARTest.UnityPlayerActivity");
        //string txt = unityPlayer.CallStatic<string>("testStatic");

    }
    //public void shareScreenshot(string na)
    //{
    //    shareScreenshot();
    //}
    //public void shareScreenshot()
    //{
    //    DeadMosquito.AndroidGoodies.AGShare.ShareScreenshot(true);
    //}

    public void HelloFromAndroid(string dataReceived)
    {
        Debug.Log("Received data from Android plugin: " + dataReceived);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
