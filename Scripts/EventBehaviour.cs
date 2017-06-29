using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EventBehaviour : MonoBehaviour {
    public abstract bool OnEvent(Event e);
}
