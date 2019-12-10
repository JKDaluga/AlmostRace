using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HypePopup : MonoBehaviour
{

    private Transform _startPos;
    private Transform _endPos;
    private float _duration;

    public void DestroyText()
    {
        Destroy(gameObject);
    }

    public void GiveInfo(Transform startPos, Transform endPos, float duration)
    {
        _startPos = startPos;
        _endPos = endPos;
        _duration = duration;
        StartCoroutine("StartLerp");
    }

    IEnumerator StartLerp()
    {
        float timePassed = 0;
        float timeRatio = 0;
        while(timePassed <= _duration)
        {
            timePassed += Time.deltaTime;
            timeRatio = timePassed / _duration;
            if (DataManager.instance.getNumActivePlayers() == 2)
            {
                this.gameObject.transform.localScale = (-2.22f * ((timeRatio - .25f) * (timeRatio - .25f)) + 1.25f) * (new Vector3(1, 2, 1));
            }
            else
            {
                this.gameObject.transform.localScale = (-2.22f * ((timeRatio - .25f) * (timeRatio - .25f)) + 1.25f) * (new Vector3(1, 1, 1));

            }
            gameObject.transform.position = Vector3.Lerp(_startPos.position, _endPos.position, timeRatio);
            
            yield return null;

        }
        Destroy(gameObject);
    }

}
