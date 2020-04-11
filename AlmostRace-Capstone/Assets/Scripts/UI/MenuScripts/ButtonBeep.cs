using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBeep : MonoBehaviour
{
    public string button = "Menu Selection";

    public void Beep()
    {
        AudioManager.instance.PlayWithoutSpatial(button);
    }
}
