using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TEMP_Debug_Manager : MonoBehaviour
{
    public TextMeshProUGUI numberText;
    private int numberTextInt = 0;
    public TextMeshProUGUI output;
    public TextMeshProUGUI stack;
    // Start is called before the first frame update
    void Start()
    {
        output.text = "";
        stack.text = "";
        Application.logMessageReceived += HandleLog;

        StartCoroutine(UpdateNumber());
        StartCoroutine(UpdateLog());

    }

    public IEnumerator UpdateNumber()
    {
        while (true)
        {
            Debug.Log("this is a test message!");
            if (numberTextInt == 0)
            {
                numberTextInt = 1;
                numberText.text = numberTextInt + "";
            }
            else if (numberTextInt == 1)
            {
                numberTextInt = 0;
                numberText.text = numberTextInt + "";
            }
            yield return new WaitForSeconds(1);
        }
    }

    public IEnumerator UpdateLog()
    {

        while (true)
        {
            Application.logMessageReceived += HandleLog;

            yield return new WaitForSeconds(1);
        }

    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        output.text = logString;
        stack.text = stackTrace;
    }
}
