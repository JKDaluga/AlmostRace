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
    private float flashTime;

    public float incomingTime, dangerTime;

    public List<GameObject> attacksInRange;

    public float nearest;
    public bool fading;

    private void Start()
    {
        attacksInRange = new List<GameObject>();
        thisCar = transform.parent.gameObject;
        nearest = Mathf.Infinity;
        fading = false;
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
            if (Mathf.Abs(nearest) <= dangerDist)
            {
                if (warning[0].color.r != danger.r || warning[0].color.g != danger.g || warning[0].color.b != danger.b)
                {
                    warning[0].color = danger; ;
                    warning[1].color = danger;
                    flashTime = dangerTime;
                }
            }
            else 
            {
                if(warning[0].color.r != incoming.r || warning[0].color.g != incoming.g || warning[0].color.b != incoming.b)
                {
                    warning[0].color = incoming;
                    warning[1].color = incoming;
                    flashTime = incomingTime;
                }
            }


            if (!fading)
            {
                StartCoroutine(Fade());
            }

        }
        else
        {
            nearest = Mathf.Infinity;
            warning[0].color = safe;
            warning[1].color = safe;
            fading = false;
            StopAllCoroutines();
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Abilities"))
        {
            if (attacksInRange.Contains(other.gameObject))
            {
                attacksInRange.Remove(other.gameObject);
            }
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
            yield return null;

            warning[0].CrossFadeAlpha(0, flashTime, false);
            warning[1].CrossFadeAlpha(0, flashTime, false);

            yield return new WaitForSeconds(flashTime);

            warning[0].CrossFadeAlpha(1, flashTime, false);
            warning[1].CrossFadeAlpha(1, flashTime, false);

            yield return new WaitForSeconds(flashTime);
        }
    }


}
