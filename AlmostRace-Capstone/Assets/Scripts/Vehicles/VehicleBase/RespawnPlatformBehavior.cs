using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPlatformBehavior : MonoBehaviour
{
    public int respawnSeconds;
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
        if (GameObject.FindGameObjectWithTag("HotSpot"))
        {
            _hotSpotBotScript = GameObject.FindGameObjectWithTag("HotSpot").GetComponent<HotSpotBotBehavior>();
            transform.position = new Vector3(_hotSpotBotScript.GetPreviousNode().position.x,
                _hotSpotBotScript.GetPreviousNode().position.y + 10,
                _hotSpotBotScript.GetPreviousNode().position.z);
        }
        else
        {
            transform.position = new Vector3(_playerObject.transform.position.x,
                _playerObject.transform.position.y + 10, _playerObject.transform.position.z);
        }
        StartCoroutine(RespawnSequence());
    }

    // Update is called once per frame
    void Update()
    {
        if (_movingCollider)
        {
            _ballCollider.transform.position = new Vector3
                (transform.position.x, transform.position.y + 2, transform.position.z);
            _ballCollider.transform.rotation = transform.rotation;
            _carMesh.transform.rotation = transform.rotation;
        }
        if (_movingCar)
        {
            _playerObject.transform.position = new Vector3
                (transform.position.x, transform.position.y + 2, transform.position.z);
            _playerObject.transform.rotation = transform.rotation;
        }
    }

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

    public void SetPlayer(GameObject givenPlayer, GameObject givenColldier, GameObject givenModel)
    {
        _playerObject = givenPlayer;
        _ballCollider = givenColldier;
        _carMesh = givenModel;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Vehicle"))
        {
            Destroy(gameObject, 3);
        }
    }
}
