using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationDialogueScript : MonoBehaviour {
    public float lat, lon;
    public GameObject routingHandler;
    public GameObject tileHandler;
    public GameObject map;
    public GameObject calculatingRouteDialogue;
    float opacity = 0;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void show()
    {
        if (opacity == 0)
            StartCoroutine(showRoutine());
    }

    public void hide()
    {
        if(opacity == 1)
            StartCoroutine(hideRoutine());
        tileHandler.GetComponent<TileHandler>().goalMarker.gameObject.SetActive(false);
    }

    IEnumerator showRoutine()
    {
        opacity = 0;
        while(opacity < 0.9f)
        {
            yield return new WaitForSeconds(0.033f);
            opacity += 0.1f;
            gameObject.GetComponent<CanvasGroup>().alpha = opacity;
        }
        opacity = 1;
        gameObject.GetComponent<CanvasGroup>().alpha = opacity;
    }

    IEnumerator hideRoutine()
    {
        opacity = 1;
        while (opacity > 0.1f)
        {
            yield return new WaitForSeconds(0.033f);
            opacity -= 0.1f;
            gameObject.GetComponent<CanvasGroup>().alpha = opacity;
        }
        opacity = 0;
        gameObject.GetComponent<CanvasGroup>().alpha = opacity;
        gameObject.SetActive(false);
    }
    public void Cancel()
    {
        hide();
    }

    public void StartNavigation()
    {
        routingHandler.GetComponent<RoutingHandler>().clearRoute();
        routingHandler.GetComponent<RoutingHandler>().buildRoute((float)map.GetComponent<Map>().lat, (float)map.GetComponent<Map>().lon, lat, lon);
        hide();
        calculatingRouteDialogue.SetActive(true);
        calculatingRouteDialogue.GetComponent<CalculatingRouteDialogueScript>().show();
    }
}
