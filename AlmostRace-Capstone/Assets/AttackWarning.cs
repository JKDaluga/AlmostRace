using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackWarning : MonoBehaviour
{
    public GameObject thisCar;
    public List<Image> warning;

    public Color safe, incoming, danger;

    public float dangerDist = 15.0f;
    public float flashTime = .25f;

    public List<GameObject> attacksInRange;

    public float nearest;
    public bool fading;
    public float currAlpha;

    private void Start()
    {
        attacksInRange = new List<GameObject>();
        thisCar = transform.parent.gameObject;
        nearest = Mathf.Infinity;
        fading = false;
        currAlpha = 1;
    }

    private void Update()
    {
        if (attacksInRange.Count > 0)
        {
            for (int i = attacksInRange.Count - 1; i >= 0; i--)
            {
                if (attacksInRange[i] == null)
                {
                    attacksInRange.RemoveAt(i);
                }
                else if (!attacksInRange[i].activeSelf)
                {
                    attacksInRange.RemoveAt(i);
                }
                else
                {
                    float temp = Vector3.Distance(thisCar.transform.position, attacksInRange[i].transform.position);
                    if (temp < nearest)
                    {
                        nearest = temp;
                    }
                }
            }
            foreach (Image i in warning)
            {
                if (Mathf.Abs(nearest) <= dangerDist)
                {
                    if (i.color.r != danger.r || i.color.g != danger.g || i.color.b != danger.b)
                    {
                        i.color = danger;
                        flashTime = .1f;
                    }
                }
                else 
                {
                    if(i.color.r != incoming.r || i.color.g != incoming.g || i.color.b != incoming.b)
                    {
                        Color holder = new Color(incoming.r, incoming.g, incoming.b, currAlpha);
                        i.color = holder;
                        flashTime = .25f;
                    }
                }


                if (!fading)
                {
                    StartCoroutine(Fade());
                }

            }
        }
        else
        {
            nearest = Mathf.Infinity;
            foreach (Image i in warning)
            {
                i.color = safe;
            }
            currAlpha = 1;
            fading = false;
            StopAllCoroutines();
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Abilities"))
        {
            if (other.gameObject.transform.root.GetComponent<Projectile>() != null)
            {
                if (thisCar != other.gameObject.transform.root.GetComponent<Projectile>().getImmunePlayer())
                {
                    attacksInRange.Add(other.gameObject);
                }
            }
            else
            {
                attacksInRange.Add(other.gameObject);
            }
        }
    }


    IEnumerator Fade()
    {
        fading = true;
        while (true)
        {
            foreach (Image i in warning)
            {
                yield return null;

                i.CrossFadeAlpha(0, flashTime, false);

                yield return new WaitForSeconds(flashTime);

                i.CrossFadeAlpha(1, flashTime, false);

                yield return new WaitForSeconds(flashTime);
            }
        }
    }


}
