using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileBehavior : MonoBehaviour
{
    private Rigidbody rigidBody;
    public float missileSpeed = 400f;
    public float missileDamage = 20f;
    public GameObject explosionPrefab;
    public float fuseTime = 5f;
    private GameObject immunePlayer;
    public float missileLifeTime = 3f;
    // Start is called before the first frame update

    private void Start()
    {
        rigidBody = gameObject.GetComponent<Rigidbody>();
        rigidBody.velocity = transform.TransformDirection(Vector3.up * missileSpeed);
        Invoke("ExplodeMissile", missileLifeTime);
    }

    public void ExplodeMissile()
    {
      GameObject spawnedExplosion = Instantiate(explosionPrefab, transform.position, transform.rotation);
        spawnedExplosion.GetComponent<MissileExplosionBehavior>().SetImmunePlayer(immunePlayer);
        spawnedExplosion.GetComponent<MissileExplosionBehavior>().SetExplosionDamage(missileDamage);

        Destroy(gameObject);
    }

    public void SetImmunePlayer(GameObject newPlayer)
    {
        immunePlayer = newPlayer;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject != immunePlayer)
        {
            ExplodeMissile();
            
        }
    }



}
