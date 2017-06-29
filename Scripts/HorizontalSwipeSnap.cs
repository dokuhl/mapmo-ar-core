using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalSwipeSnap : MonoBehaviour
{
    public bool _lerp = false;
    public int currentIndex = 0;
    public Vector3 _lerp_target;
    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
        if (_lerp)
        {
            GetComponent<RectTransform>().localPosition = Vector3.Lerp(GetComponent<RectTransform>().localPosition, _lerp_target, 7.5f * Time.deltaTime);
            if (Vector3.Distance(GetComponent<RectTransform>().localPosition, _lerp_target) < 15f)
            {
                GetComponent<RectTransform>().localPosition = _lerp_target;
                _lerp = false;
            }
        }
    }
}
