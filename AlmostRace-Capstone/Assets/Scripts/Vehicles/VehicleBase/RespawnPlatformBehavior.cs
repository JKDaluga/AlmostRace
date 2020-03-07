using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    /*
    Author: Jake Velicer
    Purpose: Holds the logic for the platform that the vehicle respawns on.
    The platform teleports to a position farther back on the path of the hotspot
    and faces the proper direction. The dead car is then teleported to this
    platform instance and respawned (turned back on).
    */

public class RespawnPlatformBehavior : MonoBehaviour
{
    [Tooltip("Amount of seconds it takes for the respawn cycle of the vehicle")]
    public int respawnSeconds;
    [Tooltip("Amount of height off the ground the platform spawns at")]
    public float spawnHeight;
    [Tooltip("Amount of distance to spawn behind the hot spot")]
    public int distanceBehind;
    [Tooltip("Amount of distance forward to make the platform look at, used when spawning after holding the bot or if there is no bot in the scene")]
    public int lookDistanceForward;
    private HypeGateTimeBehavior arena;
    private HypeManager _hypeManagerScript;
    private GameObject _playerObject;
   // private GameObject _ballCollider;
    private GameObject _carMesh;
    private GameObject _otherVehicle;
    private Transform _previousNode;
    private Transform _nextNode;
    private bool _movingCar;
    private bool _spawnOnEnemy;

    // Start is called before the first frame update
    void Start()
    {
        _hypeManagerScript = FindObjectOfType<HypeManager>();
        if (_hypeManagerScript == null)
        {
            Debug.LogError("Hype Manager not found!");
        }
        if (arena != null)
        {
            arena = FindObjectOfType<HypeGateTimeBehavior>();
        }

        if (GameObject.FindGameObjectWithTag("AISpline") != null)
        {
            if(arena != null && arena.isActivated)
            {
                SpawnOnArenaSpawnPoint();
            }
            else
            {
            SpawnOnNearestSplinePoint();
            }
        }
        else
        {
            Debug.Log("There is no hot spot in the scene");
            SpawnOnSelf();
        }
        StartCoroutine(RespawnSequence());
        Destroy(gameObject, 5);
    }

    // Sets the variables from the ones given by the vehicle that spawned the platform
    public void SetPlayer(GameObject givenPlayer, GameObject givenModel)
    {
        _playerObject = givenPlayer;
       // _ballCollider = givenColldier;
        _carMesh = givenModel;
        
    }

    // If we are in an arena, vehicle drops the bot if they have it, the vehicle spawns at a random spawn point in the arena
    private void SpawnOnArenaSpawnPoint()
    {
        HypeGateTimeBehavior arena = FindObjectOfType<HypeGateTimeBehavior>();
        Transform randomSpawnPoint = arena.spawnPoints[Random.Range(0, arena.spawnPoints.Length)];

        transform.position = new Vector3(randomSpawnPoint.position.x, randomSpawnPoint.position.y + spawnHeight, randomSpawnPoint.position.z);
        transform.rotation = randomSpawnPoint.rotation;
    }

    // If there is no HotSpotBot, spawn the vehicle at the nearest point on the spline from its death location
    private void SpawnOnNearestSplinePoint()
    {
        RaycastCar car = _playerObject.GetComponent<RaycastCar>();
        Vector3 nearestPointOnSpline = car.GetNearestPointOnSpline(0);
        transform.position = new Vector3(nearestPointOnSpline.x, nearestPointOnSpline.y + spawnHeight, nearestPointOnSpline.z);
        
        Vector3 pointOnSplineForward = car.GetNearestPointOnSpline(lookDistanceForward);
        transform.LookAt(new Vector3(pointOnSplineForward.x, transform.position.y, pointOnSplineForward.z));
    }

    // If there is no HotSpotBot, and there is not HotSpotBotSpline, spawn the vehicle at its death location
    private void SpawnOnSelf()
    {
        transform.position = new Vector3(_playerObject.transform.position.x,
            _playerObject.transform.position.y + spawnHeight, _playerObject.transform.position.z);
    }

    // The time sequence for setting when to move the vehicle and when the vehicle runs its respawn function
    private IEnumerator RespawnSequence()
    {
        yield return new WaitForSeconds(0.1f);
        _carMesh.SetActive(false);
        yield return new WaitForSeconds(respawnSeconds / 2f);
        _movingCar = true;
        _carMesh.SetActive(true);
        yield return new WaitForSeconds(respawnSeconds / 5f);
        _playerObject.GetComponent<CarHealthBehavior>().Respawn();
        _movingCar = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Move the vehicle logic object to the platform position which includes the camera
        if (_movingCar)
        {
          //  _ballCollider.transform.position = new Vector3
                //(transform.position.x, transform.position.y + 2, transform.position.z);
        //    _ballCollider.transform.rotation = transform.rotation;
            _carMesh.transform.rotation = transform.rotation;
            _playerObject.transform.position = new Vector3
                (transform.position.x, transform.position.y + 2, transform.position.z);
            _playerObject.transform.rotation = transform.rotation;
        }
    }

    // The platform destroys itself after the vehicle leaves it
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Vehicle"))
        {
            Destroy(gameObject, 3);
        }
    }
}
