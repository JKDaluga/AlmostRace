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
    private HotSpotBotBehavior _hotSpotBotScript;
    private GameObject _playerObject;
    private GameObject _ballCollider;
    private GameObject _carMesh;
    private Transform _previousNode;
    private Transform _nextNode;
    private bool _movingCollider;
    private bool _movingCar;

    // Start is called before the first frame update
    void Start()
    {
        // If there is a hotspot in the map place the platform at the proper position and rotation
        if (HotSpotBotBehavior.instance != null)
        {
            _hotSpotBotScript = HotSpotBotBehavior.instance;
            _previousNode = _hotSpotBotScript.GetPreviousNode();
            _nextNode = _hotSpotBotScript.GetNextNode();

            transform.position = new Vector3(_previousNode.position.x,
                _previousNode.position.y + spawnHeight, _previousNode.position.z);

            transform.LookAt(new Vector3(_nextNode.position.x,
                transform.position.y, _nextNode.position.z));
        }
        // If there is no hotspot spawn the vehicle at its death location
        else
        {
            transform.position = new Vector3(_playerObject.transform.position.x,
                _playerObject.transform.position.y + spawnHeight, _playerObject.transform.position.z);
        }
        StartCoroutine(RespawnSequence());
    }

    // Update is called once per frame
    void Update()
    {
        // Move the vehicle collider and the model to the platform position
        if (_movingCollider)
        {
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
