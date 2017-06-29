using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Polyline : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public static List<float[]> decode(string encodedPoints)
    {
        List<float[]> coordinates = new List<float[]>();
        if (string.IsNullOrEmpty(encodedPoints))
            throw new ArgumentNullException("encodedPoints");

        char[] polylineChars = encodedPoints.ToCharArray();
        int index = 0;

        int currentLat = 0;
        int currentLng = 0;
        int next5bits;
        int sum;
        int shifter;

        while (index < polylineChars.Length)
        {
            // calculate next latitude
            sum = 0;
            shifter = 0;
            do
            {
                next5bits = (int)polylineChars[index++] - 63;
                sum |= (next5bits & 31) << shifter;
                shifter += 5;
            } while (next5bits >= 32 && index < polylineChars.Length);

            if (index >= polylineChars.Length)
                break;

            currentLat += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);

            //calculate next longitude
            sum = 0;
            shifter = 0;
            do
            {
                next5bits = (int)polylineChars[index++] - 63;
                sum |= (next5bits & 31) << shifter;
                shifter += 5;
            } while (next5bits >= 32 && index < polylineChars.Length);

            if (index >= polylineChars.Length && next5bits >= 32)
                break;

            currentLng += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);

            float[] c = new float[2];
            c[0] = (float)currentLat / 1E5f;
            c[1] = (float)currentLng / 1E5f;
            coordinates.Add(c);
        }
        return coordinates;
    }


}
