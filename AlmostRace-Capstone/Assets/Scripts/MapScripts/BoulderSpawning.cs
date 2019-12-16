using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 Author: Mike Romeo
 Purpose: Respawns Boulders
*/

public class BoulderSpawning : MonoBehaviour
{
    private Vector3 _randOffset;

    public Transform bouldersSpawnLocation;

    public float offset = 160;

    [Tooltip("Put Boulder here.")]
    public GameObject boulder;

    private void Start()
    {
        InvokeRepeating("SpawnRock", 0, 4);
    }

    private void SpawnRock()
    {
        _randOffset = new Vector3(Random.Range(-offset, offset), 0, Random.Range(-offset, offset));
        GameObject spawnedBoulder= Instantiate(boulder, bouldersSpawnLocation.position+_randOffset, bouldersSpawnLocation.rotation);
    }
}
