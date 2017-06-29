using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueHandler : MonoBehaviour {

    private new Transform transform;

    private void Start()
    {
        transform = GetComponent<Transform>();
    }

    public void showLoadingScreen(string text)
    {
        transform.Find("LoadingScreen").gameObject.SetActive(true);
        transform.Find("LoadingScreen/img_dialogue_bg/txt_dialogue").gameObject.GetComponent<UnityEngine.UI.Text>().text = text;
    }

    public void showLoadingScreen(string text, float seconds)
    {
        showLoadingScreen(text);
        StartCoroutine(waitHide(seconds));
    }

    public void hideLoadingScreen()
    {
        transform.Find("LoadingScreen").gameObject.SetActive(false);
    }

    private IEnumerator waitHide(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        hideLoadingScreen();
    }
}
