using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventpropagationHandler : MonoBehaviour {
    List<EventBehaviour> children;
	// Use this for initialization
	void Start () {
        if (children == null)
            children = new List<EventBehaviour>();
    }
	
	// Update is called once per frame
	void Update () {
		//if(Input.Key)
	}

    public void register(EventBehaviour child)
    {
        if(children == null)
            children = new List<EventBehaviour>();
        children.Add(child);
    }

    public void unregister(EventBehaviour child)
    {
        children.Remove(child);
    }

    private void OnGUI()
    {
        Event e  = Event.current;
        if (e.type == EventType.KeyUp)
        {
            for (int i = children.Count - 1; i >= 0; --i)
            {
               if (!children[i].OnEvent(e))
                   break;
            }
            e.Use();
        }
    }
}
