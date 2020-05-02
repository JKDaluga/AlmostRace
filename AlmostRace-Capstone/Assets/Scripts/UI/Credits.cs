using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Credits : MonoBehaviour
{
    public GameObject bars, creds, menu;
    public float duration, pause;

    public Vector3 barEnd, credEnd;

    public EventSystem main;

    public GameObject selected;

    Vector3 credStart = new Vector3(0, -850, 0);

    public AudioClip menuSong, creditsSong;

    private void OnEnable()
    {
        bars.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        creds.GetComponent<RectTransform>().anchoredPosition = credStart;

        

        StartCoroutine(scroll());
    }

    private void Start()
    {
        OnEnable();
    }

    private void Update()
    {
        if (Input.GetButtonDown("ExitUI"))
        {
            Close();
        }
    }

    public void Open()
    {
        main.SetSelectedGameObject(this.gameObject);
        menu.SetActive(false);

        AudioManager.instance.ChangeBGM(creditsSong);
        
        this.gameObject.SetActive(true);
    }

    public void Close()
    {
        menu.SetActive(true);

        this.gameObject.SetActive(false);

        AudioManager.instance.ChangeBGM(menuSong);

        main.SetSelectedGameObject(selected);
    }

    IEnumerator scroll()
    {

        yield return new WaitForSeconds(pause);
        float timer = 0;

        while (timer < duration)
        {
            yield return null;
            timer += Time.deltaTime;

            float pos = timer / duration;

            bars.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(Vector3.zero, barEnd, pos);
            creds.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(credStart, credEnd, pos);
        }
        yield return new WaitForSeconds(.2f);

        Close();

    }

}
