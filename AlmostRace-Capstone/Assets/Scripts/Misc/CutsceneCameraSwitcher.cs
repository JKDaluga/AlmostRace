using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneCameraSwitcher : MonoBehaviour
{
    public Camera[] cutsceneCameras;
    private int currentCamera;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentCamera >= cutsceneCameras.Length - 1)
            {
                currentCamera = 0;
            }
            else
            {
                currentCamera++;
            }
            SwitchCamera();
        }
        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentCamera <= 0)
            {
                currentCamera = cutsceneCameras.Length - 1;
            }
            else
            {
                currentCamera--;
            }
            SwitchCamera();
        }
    }

    private void SwitchCamera()
    {
        for(int i = 0; i < cutsceneCameras.Length; i++)
        {
            if (i == currentCamera)
            {
                cutsceneCameras[i].enabled = true;
            }
            if (cutsceneCameras[i].enabled == true && i != currentCamera)
            {
                cutsceneCameras[i].enabled = false;
            }
        }
    }
}
