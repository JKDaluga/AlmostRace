using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * Eddie B
 * 
 * Based on the first minute and a half of https://www.youtube.com/watch?v=oBkfujKPZw8.
 * Currently only supports 1v1 mode though, as it doesn't account for multiple cameras.
 
     */

public class PlayerIndicator : MonoBehaviour
{
    public Image indicator;
    public Transform indicatedPlayer;
    public Camera cameraToView;

    // Update is called once per frame
    void Update()
    {
        indicator.transform.position = cameraToView.WorldToScreenPoint(indicatedPlayer.position);
        //would need a way to display it on multiple screens.
    }
}
