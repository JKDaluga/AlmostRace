using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FocusManager : MonoBehaviour
{
    GameObject selectedGameObject;

    private void Start()
    {
        selectedGameObject = EventSystem.current.firstSelectedGameObject;
    }

    private void Update()
    {
        if (EventSystem.current != null)
        {
            if(selectedGameObject != null && EventSystem.current.currentSelectedGameObject == null)
            {
                EventSystem.current.SetSelectedGameObject(selectedGameObject);
            }
            else
            {
                selectedGameObject = EventSystem.current.currentSelectedGameObject;
            }
        }
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if(EventSystem.current != null && EventSystem.current.currentSelectedGameObject != null)
        {
            selectedGameObject = EventSystem.current.currentSelectedGameObject;
        }
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (EventSystem.current != null && selectedGameObject != null)
        {
            EventSystem.current.SetSelectedGameObject(selectedGameObject);
        }
    }
}
