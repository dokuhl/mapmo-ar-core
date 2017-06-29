using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainViewScript : MonoBehaviour {

    private Billboard billboard= null;
    public UnityEngine.UI.Text txt_title;
    public UnityEngine.UI.Text txt_description;
    public UnityEngine.UI.Text txt_email;
    public UnityEngine.UI.Text txt_phone;
    public UnityEngine.UI.Text txt_mobile;
    public GameObject website_button;

    public void navigate()
    {
        Billboard b = getBillboard();
        if(b!=null)b.navigate();

    }
    Billboard getBillboard()
    {
        if (billboard == null)
        {
            bool found = false;
            Transform p = transform.parent;
            while (!found)
            {
                if (p.GetComponent<Billboard>() != null)
                    found = true;
                else if (p.root.Equals(p))
                    return null;
                else
                    p = p.parent;
            }
            billboard = p.GetComponent<Billboard>();
        }
            return billboard;
    }

    public void stepInside()
    {

        Billboard b = getBillboard();
        b.switchPanel();
        b.to_cam_ratio = 0.0001f;
        new WaitForSeconds(5.5f);
        GameObject.Find("Main Camera").GetComponent<DialogueHandler>().showLoadingScreen("Inside of: " + b.poi.basic_poi.original_name);
        GameObject.Find("skybox_container").GetComponent<Skybox>().stepInsideVideo(b.poi.detail_poi.video);
        //GameObject.Find("Main Camera").GetComponent<LoadingHandler>().showLoadingScreen("Inside of: " + b.poi.name, 2);


    }

    IEnumerator stepInside(Billboard b)
    {
        yield return new WaitForSeconds(1f);
  
        
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
