using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBeep : MonoBehaviour
{

    public void Beep()
    {
        AudioManager.instance.PlayWithoutSpatial("Lux Shooting");
    }
}
