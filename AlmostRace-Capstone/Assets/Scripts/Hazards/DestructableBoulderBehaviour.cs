using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Mike Romeo 06/11/2019
 * Functionality for destuctable boulder that the player can shoot at
 * or collide with. 
 */

public class DestructableBoulderBehaviour : MonoBehaviour
{

    [Tooltip("Set Destuctable Boulder total amount of health")]
    public int rockHealth = 100;

    [Tooltip("Amount of damage the boulder can take")]
    public int damage = 20;

    [Tooltip("Amount of damage it does to the vehicle")]
    public int carDamage = 20;

    [Tooltip("Attach boulder particle effect")]
    public GameObject boulderParticles;

    private Renderer rend;
    private Collider coll;

    [Tooltip("A reference to destruction sound")]
    public AudioClip deathSound;

    // Start is called before the first frame update
    void Start()
    {
        rend = this.GetComponent<Renderer>();
        coll = this.GetComponent<Collider>();
        rend.enabled = true;
        coll.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        // Disable every thing after health hits 0 and play particle effect.
        if(rockHealth <= 0)
        {
            boulderParticles.SetActive(true);
            rend.enabled = false;
            coll.enabled = false;

            AudioSource.PlayClipAtPoint(deathSound, gameObject.transform.position);

            Invoke("KillBoulder", 3);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Untagged"))
        {

            if (collision.gameObject.CompareTag("Vehicle"))
            {
                if (collision.gameObject.GetComponent<CarHeatManager>() != null)
                {
                    Debug.Log(collision.gameObject);
                    collision.gameObject.GetComponent<CarHeatManager>().AddHeat(carDamage);
                }
            }

            rockHealth -= damage;
        }
       
    }

    private void KillBoulder()
    {
        // kill the container of this parent
        Destroy(transform.parent.gameObject);
    }

}
