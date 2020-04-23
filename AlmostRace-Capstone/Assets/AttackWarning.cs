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
    public float flashTime = 1.0f;

    private List<GameObject> attacksInRange;

    private float nearest;
    private bool flashing;

    private void Start()
    {
        attacksInRange = new List<GameObject>();
        thisCar = transform.parent.gameObject;
        nearest = Mathf.Infinity;
        flashing = false;
    }

    private void FixedUpdate()
    {
        if (attacksInRange.Count > 0)
        {
            for (int i = attacksInRange.Count - 1; i >= 0; i--)
            {
                if (attacksInRange[i] == null)
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

            if(nearest < dangerDist)
            {
                warning.color = danger;
                flashTime = .5f;
            }
            else
            {
                warning.color = incoming;
                flashTime = 1;
            }

            if (!flashing)
            {
                StartCoroutine(Flash());
            }

        }
        else
        {
            nearest = Mathf.Infinity;
            warning.color = safe;
            flashing = false;
            StopAllCoroutines();
        }
    }

    private IEnumerator Flash()
    {
        flashing = true;
        bool fadeOut = true;
        Color currColor = warning.color;
        float targetAlpha = 0;
        while (true)
        {
            yield return null;
            if (fadeOut)
            {
                currColor.a = Mathf.Lerp(currColor.a, targetAlpha, flashTime * Time.deltaTime);
                warning.color = currColor;

                if(warning.color.a == 0)
                {
                    fadeOut = false;
                    targetAlpha = 1;
                }
            }
            else
            {
                currColor.a = Mathf.Lerp(currColor.a, targetAlpha, flashTime * Time.deltaTime);
                warning.color = currColor;

                if (warning.color.a == 0)
                {
                    fadeOut = true;
                    targetAlpha = 0;
                }
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

}
