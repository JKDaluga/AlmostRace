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
    [Tooltip("Amount of seconds it takes for the respawn cycle of the vehicle")] public int respawnSeconds;
    [Tooltip("Amount of height off the ground the platform spawns at")] public float spawnHeight;
    [Tooltip("Amount of distance to spawn behind the hot spot")] public int distanceBehind;
    private HotSpotBotBehavior _hotSpotBotScript;
    private HypeManager _hypeManagerScript;
    private GameObject _playerObject;
    private GameObject _ballCollider;
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
        if (HotSpotBotBehavior.instance != null)
        {
            _hotSpotBotScript = HotSpotBotBehavior.instance;
        }

        if (GameObject.FindGameObjectWithTag("HotSpotSpline").GetComponent<SplinePlus>() != null)
        {
            if (GameObject.FindGameObjectWithTag("HotSpotSpline").GetComponent<SplinePlus>().SPData.Followers[0].FollowerGO != null)
            {
                if (_hotSpotBotScript.GetBeingHeld())
                {
                    if (_playerObject.GetComponent<HotSpotVehicleAdministration>().holdingTheBot)
                    {
                        SpawnBehindBotAfterDropping();
                    }
                    else
                    {
                        SpawnBehindEnemyWithBot();
                    }
                }
                else if (!_hotSpotBotScript.GetBeingHeld())
                {
                    SpawnBehindBot();
                }
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
    public void SetPlayer(GameObject givenPlayer, GameObject givenColldier, GameObject givenModel)
    {
        _playerObject = givenPlayer;
        _ballCollider = givenColldier;
        _carMesh = givenModel;
        
    }

    // Vehicle respawning had the HotsSpotBot, spawn it behind the dropped bot
    private void SpawnBehindBotAfterDropping()
    {
        _playerObject.GetComponent<HotSpotVehicleAdministration>().DropTheBot();
        Vector3 nearestPointOnSpline = _hotSpotBotScript.GetNearestPointOnSpline(_playerObject.transform.position, distanceBehind);
        Vector3 pointOnSplineForward = _hotSpotBotScript.GetNearestPointOnSpline(_playerObject.transform.position, -3);

        transform.position = new Vector3(nearestPointOnSpline.x,
            nearestPointOnSpline.y + spawnHeight, nearestPointOnSpline.z);
        transform.LookAt(new Vector3(pointOnSplineForward.x, transform.position.y, pointOnSplineForward.z));
    }

    // No one had the HotsSpotBot, spawn the vehicle behind it
    private void SpawnBehindBot()
    {
        Transform bot = GameObject.Find("HotSpotBot").transform;
        Vector3 nearestPointOnSpline = _hotSpotBotScript.GetNearestPointOnSpline(bot.position, distanceBehind);

        transform.position = new Vector3(nearestPointOnSpline.x,
            nearestPointOnSpline.y + spawnHeight, nearestPointOnSpline.z);
        transform.LookAt(new Vector3(bot.position.x, transform.position.y, bot.position.z));
    }

    // Another vehicle has the HotSpotBot, spawn the vehicle respawning behind the vehicle with the HotSpotBot
    private void SpawnBehindEnemyWithBot()
    {
        for(int i = 0; i < _hypeManagerScript.vehicleList.Count; i++)
        {
            if (_hypeManagerScript.vehicleList[i].GetComponent<HotSpotVehicleAdministration>().holdingTheBot)
            {
                _otherVehicle = _hypeManagerScript.vehicleList[i];
            }
        }
        Vector3 nearestPointOnSpline = _hotSpotBotScript.GetNearestPointOnSpline(_otherVehicle.transform.position, distanceBehind);

        transform.position = new Vector3(nearestPointOnSpline.x,
            nearestPointOnSpline.y + spawnHeight, nearestPointOnSpline.z);

        _spawnOnEnemy = true;
        transform.LookAt(new Vector3(_otherVehicle.transform.position.x,
            transform.position.y, _otherVehicle.transform.position.z));
    }

    // If there is no HotSpotBot, spawn the vehicle at the nearest point on the spline from its death location
    private void SpawnOnNearestSplinePoint()
    {
        Vector3 nearestPointOnSpline = _hotSpotBotScript.GetNearestPointOnSpline(_playerObject.transform.position, distanceBehind);
        Vector3 pointOnSplineForward = _hotSpotBotScript.GetNearestPointOnSpline(_playerObject.transform.position, -3);

        transform.position = new Vector3(nearestPointOnSpline.x,
            nearestPointOnSpline.y + spawnHeight, nearestPointOnSpline.z);
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
        _playerObject.GetComponent<CarHeatManager>().Respawn();
        _movingCar = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Move the vehicle logic object to the platform position which includes the camera
        if (_movingCar)
        {
            _ballCollider.transform.position = new Vector3
                (transform.position.x, transform.position.y + 2, transform.position.z);
            _ballCollider.transform.rotation = transform.rotation;
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
