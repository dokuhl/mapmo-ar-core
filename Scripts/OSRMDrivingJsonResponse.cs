using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class OSRMDrivingJsonResponse
{
    public string code;
    public OSRMDrivingJsonResponse_route[] routes;
}

[Serializable]
public class OSRMDrivingJsonResponse_route
{
    public OSRMDrivingJsonResponse_leg[] legs;
}

[Serializable]
public class OSRMDrivingJsonResponse_leg
{
    public OSRMDrivingJsonResponse_step[] steps;
}

[Serializable]
public class OSRMDrivingJsonResponse_step {
    public string geometry;
    public OSRMDrivingJsonResponse_maneuver maneuver;
}

[Serializable]
public class OSRMDrivingJsonResponse_maneuver
{
    public float bearing_before;
    public float bearing_after;
    public string instruction;
    public string type;
    public float[] location;
    public string modifier;
}
