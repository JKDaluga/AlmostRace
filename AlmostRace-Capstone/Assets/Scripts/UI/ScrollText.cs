using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScrollText : MonoBehaviour
{
    public TextMeshProUGUI text;
    public float speed = 2;

    public float height;
    Vector3 startPos;
    public float scrollPos;

    private void Start()
    {
        height = text.preferredHeight;
        startPos = text.rectTransform.localPosition;

        scrollPos = height * 2 / 3;

        StartCoroutine(scrollIt());
    }

    private void Update()
    {
        Vector2 textSize = text.GetPreferredValues(text.text);
        // Adjust the button size / scale.
        text.rectTransform.localScale = textSize;
    }
    IEnumerator scrollIt()
    {
        yield return new WaitForSeconds(1);

        while (true)
        {
            text.rectTransform.localPosition = new Vector3(startPos.x, scrollPos % height, startPos.z);

            scrollPos += speed * 20 * Time.deltaTime;
            yield return null;
        }
    }
}
