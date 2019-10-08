using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoulderSpawning : MonoBehaviour
{
    private Vector3 _randOffset;

    public Transform bouldersSpawnLocation;

    public float xOffset = 5;
    public float zOffset = 5;

    [Tooltip("Put Boulder here.")]
    public GameObject boulder;

    private void Start()
    {
        InvokeRepeating("SpawnRock", 0, 1);
    }

    private void SpawnRock()
    {
        //_randOffset = new Vector3(Random.Range(-xOffset, xOffset), 0, Random.Range(-xOffset, xOffset));
        _randOffset = new Vector3(-110, 200, -25);

        GameObject spawnedBoulder= Instantiate(boulder, _randOffset, Quaternion.identity);

        //Instantiate(lightningMissile, missileSpawnLocation.transform.position + _randOffset, missileSpawnLocation.transform.rotation);
    }
}
