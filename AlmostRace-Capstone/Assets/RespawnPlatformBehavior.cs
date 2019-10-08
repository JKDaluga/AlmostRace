using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPlatformBehavior : MonoBehaviour
{
    public int respawnSeconds;
    private HotSpotBotBehavior _hotSpotBotScript;
    private GameObject _playerObject;
    private Transform _previousNode;
    private Transform _nextNode;
    private bool _moving = true;

    // Start is called before the first frame update
    void Start()
    {
        _hotSpotBotScript = GameObject.FindGameObjectWithTag("HotSpot").GetComponent<HotSpotBotBehavior>();
        transform.position = _hotSpotBotScript.GetPreviousNode().position;
        transform.position = new Vector3(_hotSpotBotScript.GetPreviousNode().position.x,
            _hotSpotBotScript.GetPreviousNode().position.y + 7, _hotSpotBotScript.GetPreviousNode().position.z);
        StartCoroutine(GuideVehicle());
    }

    // Update is called once per frame
    void Update()
    {
        if (_moving)
        {
            _playerObject.transform.position = transform.position;
        }
        Debug.Log(_moving);
    }

    private IEnumerator GuideVehicle()
    {
        yield return new WaitForSeconds(respawnSeconds);
        _playerObject.GetComponent<CarHeatManager>().Respawn();
        _moving = false;
    }

    public void SetPlayer(GameObject givenPlayer)
    {
        _playerObject = givenPlayer;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Vehicle"))
        {
            Destroy(gameObject, 3);
        }
    }
}
