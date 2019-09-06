using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitiesBehavior : MonoBehaviour
{
    public enum BasicAbility {GravSlam, VoidStep};
    public enum SignatureAbility {GravBurst, VoidSting};
    public BasicAbility basicAbility;
    public SignatureAbility signatureAbility;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void BasicAbilityAction()
    {
        switch(basicAbility)
        {
            case BasicAbility.GravSlam:
            
                break;

            case BasicAbility.VoidStep:
            
                break;
        }
    }

    void SignatureAbilityAction()
    {
        switch(signatureAbility)
        {
            case SignatureAbility.GravBurst:
            
                break;

            case SignatureAbility.VoidSting:
            
                break;
        }
    }

}
