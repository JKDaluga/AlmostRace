using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SetUniversalUIControls : MonoBehaviour
{
    private StandaloneInputModule standaloneInputModule;

    private void Awake()
    {
        standaloneInputModule = gameObject.GetComponent<StandaloneInputModule>();
        if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
        {
            standaloneInputModule.horizontalAxis = "HorizontalUIMac";
        }
        else
        {
            standaloneInputModule.horizontalAxis = "HorizontalUI";
        }        
    }
}
