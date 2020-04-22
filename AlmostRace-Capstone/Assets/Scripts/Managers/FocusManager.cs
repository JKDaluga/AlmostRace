using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FocusManager : MonoBehaviour
{
    EventSystem eventSystem;
    GameObject selectedGameObject;

    // Start is called before the first frame update
    void Start()
    {
        eventSystem = GetComponent<EventSystem>();
    }
    void OnApplicationFocus(bool hasFocus)
    {
        eventSystem = GetComponent<EventSystem>();
        if(eventSystem != null)
        {
            selectedGameObject = eventSystem.currentSelectedGameObject;
        }
    }

    void OnApplicationPause(bool pauseStatus)
    {
        eventSystem = GetComponent<EventSystem>();
        if (eventSystem != null && selectedGameObject != null)
        {
            eventSystem.SetSelectedGameObject(selectedGameObject);
        }
    }
}
