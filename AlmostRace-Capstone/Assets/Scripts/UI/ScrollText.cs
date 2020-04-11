using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScrollText : MonoBehaviour
{
    public ScrollRect field;

    WinScreen wN;


    private void Start()
    {
        field = GetComponent<ScrollRect>();

        field.verticalNormalizedPosition = 1;
        wN = GetComponentInParent<WinScreen>();

        StartCoroutine(scrollIt());
    }


    IEnumerator scrollIt()
    {
        yield return new WaitForSeconds(wN.scrollPause);
        while (true)
        {
            field.verticalNormalizedPosition -= wN.scrollSpeed;
            yield return null;
            if (field.verticalNormalizedPosition < 0)
            {
                field.verticalNormalizedPosition = 1.5f;
            }
        }
    }
}
