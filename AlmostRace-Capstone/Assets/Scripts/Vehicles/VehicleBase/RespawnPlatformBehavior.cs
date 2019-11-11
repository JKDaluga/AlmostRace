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
    private bool _movingCollider;
    private bool _movingCar;
    private bool _spawnOnEnemy;

    // Start is called before the first frame update
    void Start()
    {
        if(HypeManager.HM != null)
        {
            _hypeManagerScript = HypeManager.HM;
        }
        
        if (HotSpotBotBehavior.instance != null)
        {
            _hotSpotBotScript = HotSpotBotBehavior.instance;
            if (_hotSpotBotScript.GetBeingHeld())
            {
                if (_playerObject.GetComponent<HotSpotVehicleAdministration>().holdingTheBot)
                {
                    Debug.Log("Bot dropped, spawn behind it");
                    _playerObject.GetComponent<HotSpotVehicleAdministration>().DropTheBot();
                    SpawnBehindBot();
                }
                else
                {
                    Debug.Log("Spawn behind enemy with bot");
                    SpawnBehindEnemyWithBot();
                }
            }
            else if (!_hotSpotBotScript.GetBeingHeld())
            {
                Debug.Log("No one has the bot");
                SpawnBehindBot();
            }
        }
        else
        {
            Debug.Log("There is no bot in the scene");
            SpawnOnSelf();
        }
        StartCoroutine(RespawnSequence());
    }

    private void SpawnBehindBot()
    {
        Transform bot = GameObject.Find("HotSpotBot").transform;
        Vector3 nearestPointOnSpline = _hotSpotBotScript.GetNearestPointOnSpline(bot.position, distanceBehind);

        transform.position = new Vector3(nearestPointOnSpline.x,
            nearestPointOnSpline.y + spawnHeight, nearestPointOnSpline.z);
        transform.LookAt(new Vector3(bot.position.x, transform.position.y, bot.position.z));
    }

    // Place the platform at the proper position and rotation above the enemy with the hotspot
    private void SpawnBehindEnemyWithBot()
    {
        for(int i = 0; i < _hypeManagerScript._vehicleList.Count; i++)
        {
            if (_hypeManagerScript._vehicleList[i].GetComponent<HotSpotVehicleAdministration>().holdingTheBot)
            {
                _otherVehicle = _hypeManagerScript._vehicleList[i];
            }
        }
        Vector3 nearestPointOnSpline = _hotSpotBotScript.GetNearestPointOnSpline(_otherVehicle.transform.position, distanceBehind);

        transform.position = new Vector3(nearestPointOnSpline.x,
            nearestPointOnSpline.y + spawnHeight, nearestPointOnSpline.z);

        _spawnOnEnemy = true;
        transform.LookAt(new Vector3(_otherVehicle.transform.position.x,
            transform.position.y, _otherVehicle.transform.position.z));
    }

    // If there is no hotspot spawn the vehicle at its death location
    private void SpawnOnSelf()
    {
        transform.position = new Vector3(_playerObject.transform.position.x,
            _playerObject.transform.position.y + spawnHeight, _playerObject.transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        // Move the vehicle collider and the model to the platform position
        if (_movingCollider)
        {
            if (_spawnOnEnemy)
            {
                transform.LookAt(new Vector3(_otherVehicle.transform.position.x,
                    transform.position.y, _otherVehicle.transform.position.z));
            }
            _ballCollider.transform.position = new Vector3
                (transform.position.x, transform.position.y + 2, transform.position.z);
            _ballCollider.transform.rotation = transform.rotation;
            _carMesh.transform.rotation = transform.rotation;
        }
        // Move the vehicle logic object to the platform position which includes the camera
        if (_movingCar)
        {
            _playerObject.transform.position = new Vector3
                (transform.position.x, transform.position.y + 2, transform.position.z);
            _playerObject.transform.rotation = transform.rotation;
        }
    }

    // Sets the variables from the ones given by the vehicle that spawned the platform
    public void SetPlayer(GameObject givenPlayer, GameObject givenColldier, GameObject givenModel)
    {
        _playerObject = givenPlayer;
        _ballCollider = givenColldier;
        _carMesh = givenModel;
    }

    // The time sequence for setting when to move the vehicle and when the vehicle runs its respawn function
    private IEnumerator RespawnSequence()
    {
        _movingCollider = true;
        yield return new WaitForSeconds(respawnSeconds / 2f);
        _movingCar = true;
        yield return new WaitForSeconds(respawnSeconds / 5f);
        _playerObject.GetComponent<CarHeatManager>().Respawn();
        _movingCar = false;
        _movingCollider = false;
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
