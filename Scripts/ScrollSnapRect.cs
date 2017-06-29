using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class ScrollSnapRect : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler {

 
    public RectTransform _container;
    private Vector2 Δp_total = new Vector2();
    public float threshold = 50f;
    public float width;
    public float padding_right = 50f;
    public int index = 0;
   
    //------------------------------------------------------------------------
    void Start() {

    }

    //------------------------------------------------------------------------
    void Update() {
       
    }


    //------------------------------------------------------------------------
    private void LerpToPage(int aPageIndex) {

    }

    //------------------------------------------------------------------------
    private void NextScreen() {
        LerpToPage(1);
    }

    //------------------------------------------------------------------------
    private void PreviousScreen() {
        LerpToPage(1);
    }

    //------------------------------------------------------------------------
    private int GetNearestPage() {
        return 0;
    }

    //------------------------------------------------------------------------
    public void OnBeginDrag(PointerEventData aEventData) {
        Δp_total = new Vector2();
        
    }

    //------------------------------------------------------------------------
    public void OnEndDrag(PointerEventData aEventData)
    {
        if (Mathf.Abs(Δp_total.x) > 0.33f * width)
        {
            if (Δp_total.x > 0)
            {
                if (index > 0)
                {
                    Debug.Log("swooop");
                    _container.GetComponent<HorizontalSwipeSnap>()._lerp_target = new Vector3(-(index - 1) * (width + padding_right), _container.transform.localPosition.y, _container.transform.localPosition.z);
                    _container.GetComponent<HorizontalSwipeSnap>().currentIndex = index - 1;
                }
                //else
                //{
                //    Debug.Log("boing");
                //    _container.GetComponent<HorizontalSwipeSnap>()._lerp_target = new Vector3(-index * (width + padding_right), _container.transform.localPosition.y, _container.transform.localPosition.z);
                //}
            }
            else
            {
                
                if (index < (transform.parent.parent.childCount-1))
                {
                    Debug.Log("swishh");
                    _container.GetComponent<HorizontalSwipeSnap>()._lerp_target = new Vector3(-(index + 1) * (width + padding_right), _container.transform.localPosition.y, _container.transform.localPosition.z);
                    _container.GetComponent<HorizontalSwipeSnap>().currentIndex = index +1;
                }
                //else
                //{
                //    Debug.Log("boing");
                //    _container.GetComponent<HorizontalSwipeSnap>()._lerp_target = new Vector3(-index * (width + padding_right), _container.transform.localPosition.y, _container.transform.localPosition.z);
                //}

            }
        }
        else
        {
            Debug.Log("boing");
            _container.GetComponent<HorizontalSwipeSnap>()._lerp_target = new Vector3( -index * (width + padding_right), _container.transform.localPosition.y, _container.transform.localPosition.z);
        }
        _container.GetComponent<HorizontalSwipeSnap>()._lerp = true;

    }


    public void OnMouseUp()
    {
        
    }

    //------------------------------------------------------------------------
    public void OnDrag(PointerEventData aEventData) {

        Vector2 Δp = aEventData.delta;
        Δp_total += Δp;
        //Debug.Log(_container.GetComponent<RectTransform>().transform.localPosition);

        //Debug.Log(Δp_total.x + " " + Δp_total.y);

        if (Mathf.Abs(Δp_total.x) > Mathf.Abs(Δp_total.y) && Δp_total.magnitude > threshold)
        {
            _container.transform.localPosition = new Vector3(-index * (width + padding_right) + Δp_total.x, _container.transform.localPosition.y, _container.transform.localPosition.z);
        } 


    }
}
