using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventOnStart : MonoBehaviour
{
    public UnityEvent events;

    // Start is called before the first frame update
    void Start()
    {
        events.Invoke();
    }
}
