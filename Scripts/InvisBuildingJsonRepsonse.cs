using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InvisBuildingJsonRepsonse {
    public InvisBuildingJsonRepsonse_Building[] response;
}

[Serializable]
public class InvisBuildingJsonRepsonse_Building
{
    public int id;
    public float[] startPoint;
    public float[] building;
}
