using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TOCButton : MonoBehaviour {
    public int index = 0;
    public RectTransform _container;
    public float view_width;
    public float view_padding_right = 50f;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void init(int index, RectTransform container, float view_width, float view_padding_right)
    {
        this.index = index;
        this._container = container;
        this.view_width = view_width;
        this.view_padding_right = view_padding_right;
    }

    public void swipeTo()
    {
        _container.GetComponent<HorizontalSwipeSnap>()._lerp_target = new Vector3(-index * (view_width + view_padding_right), _container.transform.localPosition.y, _container.transform.localPosition.z);
        _container.GetComponent<HorizontalSwipeSnap>()._lerp = true;
        _container.GetComponent<HorizontalSwipeSnap>().currentIndex = index;
    }
}
