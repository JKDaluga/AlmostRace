using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinishedPanels : MonoBehaviour
{

    public int logoVersion;

    HypeManager hm;

    public Sprite[] logos;

    public Image bg, logo;

    public RaycastCar car;

    private void OnEnable()
    {
        switch(logoVersion)
        {
            case 1:
                switch (car.playerID)
                {
                    case 1:
                        bg.color = new Color32(193, 62, 62, 255);
                        break;
                    case 2:
                        bg.color = new Color32(43, 142, 43, 255);
                        break;
                    case 3:
                        bg.color = new Color32(119, 68, 178, 255);
                        break;
                    case 4:
                        bg.color = new Color32(209, 154, 62, 255);
                        break;
                }
                break;
            case 3:
                switch (car.playerID)
                {
                    case 1:
                        logo.sprite = logos[0];
                        break;
                    case 2:
                        logo.sprite = logos[1];
                        break;
                    case 3:
                        logo.sprite = logos[2];
                        break;
                    case 4:
                        logo.sprite = logos[3];
                        break;
                }
                break;
            case 4:
                switch (car.playerID)
                {
                    case 1:
                        bg.color = new Color32(193, 62, 62, 255);
                        logo.sprite = logos[0];
                        break;
                    case 2:
                        bg.color = new Color32(43, 142, 43, 255);
                        logo.sprite = logos[1];
                        break;
                    case 3:
                        bg.color = new Color32(119, 68, 178, 255);
                        logo.sprite = logos[2];
                        break;
                    case 4:
                        bg.color = new Color32(209, 154, 62, 255);
                        logo.sprite = logos[3];
                        break;
                }
                break;
        }   
    }

}
