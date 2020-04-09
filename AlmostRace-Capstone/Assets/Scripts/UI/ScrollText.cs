using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScrollText : MonoBehaviour
{
    public ScrollRect field;

    public float scrollSpeed = .005f;


    private void Start()
    {
        field = GetComponent<ScrollRect>();

        field.verticalNormalizedPosition = 1;

        StartCoroutine(scrollIt());
    }


    IEnumerator scrollIt()
    {
        yield return new WaitForSeconds(1);
        while (true)
        {
            field.verticalNormalizedPosition -= scrollSpeed;
            yield return null;
            if (field.verticalNormalizedPosition < 0)
            {
                field.verticalNormalizedPosition = 1.5f;
            }
        }
    }
}
