using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingIconScript : MonoBehaviour, IPointerClickHandler
{
    public PoiNameBarScript poiNameBarScript;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (poiNameBarScript == null)
            poiNameBarScript = GameObject.Find("POINameBar").GetComponent<PoiNameBarScript>();
        poiNameBarScript.switchBuilding(transform.parent.GetComponent<InvisBuilding>());
        poiNameBarScript.expand();
    }
}
