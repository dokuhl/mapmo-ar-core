using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculatingRouteDialogueScript : MonoBehaviour {
    float opacity = 0;
    public GameObject routingHandler;
    // Use this for initialization
    void Start () {
		  
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void show()
    {
        opacity = 1;
    //    if (opacity == 0)
    //        StartCoroutine(showRoutine());
    }

    public void hide()
    {
        opacity = 0;
        gameObject.SetActive(false);
        //if (opacity == 1)
        //    StartCoroutine(hideRoutine());
    }

    public void abort()
    {
        routingHandler.GetComponent<RoutingHandler>().abort();
        hide();
    }

    IEnumerator showRoutine()
    {
        opacity = 0;
        while (opacity < 0.9f)
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

}
