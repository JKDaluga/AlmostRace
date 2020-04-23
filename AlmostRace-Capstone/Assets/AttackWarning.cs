using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackWarning : MonoBehaviour
{
    public GameObject thisCar;
    public Image warning;

    public Color safe, incoming, danger;

    public float dangerDist = 15.0f;
    public float flashTime = .25f;

    public List<GameObject> attacksInRange;

    private float nearest;
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
                    if(temp < nearest)
                    {
                        nearest = temp;
                    }
                }
            }
            if(warning.color == safe)
            {
                Color holder = new Color(incoming.r, incoming.g, incoming.b, currAlpha);
                warning.color = holder;
                flashTime = .25f;
            }

            if(nearest <= dangerDist)
            {
                if(warning.color == safe || warning.color == incoming)
                {
                    warning.color = danger;
                    flashTime = .1f;
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
            warning.color = safe;
            currAlpha = 1;
            fading = false;
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

            warning.CrossFadeAlpha(0, flashTime, false);

            yield return new WaitForSeconds(flashTime);

            warning.CrossFadeAlpha(1, flashTime, false);

            yield return new WaitForSeconds(flashTime);
        }
    }


}
