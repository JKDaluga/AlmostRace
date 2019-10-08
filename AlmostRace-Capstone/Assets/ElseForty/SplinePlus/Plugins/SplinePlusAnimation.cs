using UnityEngine;
using System.Reflection;

public class SplinePlusAnimation : MonoBehaviour
{

    Animator Animator;
    SplinePlus SplinePlus;

    void Start()
    {
        Animator = GetComponent<Animator>();
        SplinePlus = GetComponent<SplinePlus>();
    }

     void Update()
    {
        if (Animator != null && SplinePlus != null)
        {
            if (Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !Animator.IsInTransition(0))
            {
                SplinePlus.SplineCreationClass.UpdateAllBranches(SplinePlus.SPData);
            }
        }
    }
}
