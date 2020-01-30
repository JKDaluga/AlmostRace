using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    /*
    Author: Jake Velicer
    Purpose: Contains the behavior for the hot spot bot. This includes its behavior for setting
    its position back on the spline after it has been dropped, if it is being held, a function
    for finding the nearest point on the hot spot bot spline, and the hot spot bot behavior in arenas.
    */

public class HotSpotBotBehavior : MonoBehaviour
{
    [Tooltip("Speed when the bot is within chase range")] public float chaseSpeed = 100;
    [Tooltip("Speed when the bot is to far ahead of the players and needs to slow down")] public float farSpeed = 50;
    public bool reverseDirection;
    public float dropGracePeriod = 2;
    public int branchesToPlaceAhead = 1;
    public float setPositionSpeedMultiplier = 2;
    public MeshRenderer meshRenderer;
    public Collider botCollider;
    public GameObject hypeColliderObject;
    public static HotSpotBotBehavior instance;
    private List<Node> _branchNodes = new List<Node>();
    private Dictionary<int, Branch> _branchesAtStart = new Dictionary<int, Branch>();
    private float[] _branchOfVehicle;
    private float _currentBranchOfBot;
    private SplinePlus _splinePlusScript;
    private HypeManager _hypeManagerScript;
    private HypeGateBehavior _currentArena;
    private Transform _currentArenaDesignation;
    private bool _beingHeld;
    private bool _inArena;
    private bool _allVehiclesIn;
    private bool _vehiclesReceived;
    private bool _currentlySettingPosition;
    private bool _canGoForward;
    private readonly int _hugeDistance = 9999;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        _splinePlusScript = GameObject.FindGameObjectWithTag("HotSpotSpline").GetComponent<SplinePlus>();
        _splinePlusScript.SPData.Followers[0].Reverse = reverseDirection;
        _hypeManagerScript = FindObjectOfType<HypeManager>();
        if (_hypeManagerScript == null)
        {
            Debug.LogError("Hype Manager not found!");
        }
        StartCoroutine(WaitForVehicleCount());

        _branchesAtStart = new Dictionary<int, Branch>(_splinePlusScript.SPData.DictBranches);
        foreach (KeyValuePair<int, Branch> entry in _branchesAtStart)
        {
            for(int i = 0; i < entry.Value.Nodes.Count; i++)
            {
                if(!_branchNodes.Contains(entry.Value.Nodes[i]))
                {
                    _branchNodes.Add(entry.Value.Nodes[i]);
                }
            }
        }
        SetBeingHeld(false);
    }

    void Update()
    {
        if (!_inArena && _vehiclesReceived)
        {
            DistanceChecker();
            SetBotSpeed();
        }
    }

    private IEnumerator WaitForVehicleCount()
    {
        while (_hypeManagerScript.vehicleList.Count == 0)
        {
            yield return null;
        }
        _branchOfVehicle = new float[_hypeManagerScript.vehicleList.Count];
        _vehiclesReceived = true;
    }

    public bool GetBeingHeld()
    {
        return _beingHeld;
    }

    public bool GetInArena()
    {
        return _inArena;
    }

    public void SetBeingHeld(bool isBeingHeld)
    {
        _beingHeld = isBeingHeld;
    }

    public void SetVehiclesIn(bool allVehiclesIn)
    {
        _allVehiclesIn = allVehiclesIn;
    }

    public void SetCanGoForward(bool canGoForward)
    {
        _canGoForward = canGoForward;
    }

    public void SetBotSpeed()
    {
        //Debug.Log("Bot's Key: " + _currentBranchOfBot);
        //Debug.Log("Bot Speed: " + _splinePlusScript.SPData.Followers[0].Speed);
        if (_canGoForward)
        {
            if (_currentlySettingPosition)
            {
                _splinePlusScript.SetSpeed(chaseSpeed * setPositionSpeedMultiplier);
            }
            else
            {
                for (int i = 0; i < _branchOfVehicle.Length; i++)
                {
                    //Debug.Log("Vehicle: " + _hypeManagerScript.vehicleList[i] + " Key: " + _branchOfVehicle[i]);
                    if (_branchOfVehicle[i] > _currentBranchOfBot)
                    {
                        if (!_currentlySettingPosition)
                        {
                            StartCoroutine(SetPosition(_hypeManagerScript.vehicleList[i].transform.position));
                        }
                    }

                    if (_branchOfVehicle[i] < _currentBranchOfBot)
                    {
                        _splinePlusScript.SetSpeed(farSpeed);
                    }
                    else if (_branchOfVehicle[i] == _currentBranchOfBot)
                    {
                        _splinePlusScript.SetSpeed(chaseSpeed);
                    }
                }
            }
        }
        else
        {
            _splinePlusScript.SetSpeed(0);
        }
    }

    public void DistanceChecker()
    {
        float lastVehicleDistance = _hugeDistance;
        float lastDistance = _hugeDistance;

        foreach (KeyValuePair<int, Branch> entry in _branchesAtStart)
        {
            for (int j = 0; j < entry.Value.Vertices.Count; j++)
            {
                float hotSpotDistance = Vector3.Distance(entry.Value.Vertices[j], transform.position);
                if (hotSpotDistance <= lastDistance)
                {
                    lastDistance = hotSpotDistance;
                    _currentBranchOfBot = entry.Key;
                }
                for (int k = 0; k < _branchOfVehicle.Length; k++)
                {
                    float vehicleDistance = Vector3.Distance(entry.Value.Vertices[j], _hypeManagerScript.vehicleList[k].transform.position);
                    if (vehicleDistance <= lastVehicleDistance)
                    {
                        lastVehicleDistance = vehicleDistance;
                        _branchOfVehicle[k] = entry.Key;
                    }
                }
            }
        }
    }

    public IEnumerator SetPosition(Vector3 vehiclesPosition)
    {
        _currentlySettingPosition = true;
        Node positionToPlace = _branchNodes[0];
        float distance = Vector3.Distance(_branchNodes[0].Point.position, vehiclesPosition);
        
        if(_inArena)
        {
            StartCoroutine(LerpToArenaLocation());
        }
        else
        {
            hypeColliderObject.SetActive(false);
            botCollider.enabled = false;

            for (int i = 0; i < _branchNodes.Count; i++)
            {
                if(distance > Vector3.Distance(_branchNodes[i].Point.position, vehiclesPosition))
                {
                    distance = Vector3.Distance(_branchNodes[i].Point.position, vehiclesPosition);
                    if ((i + branchesToPlaceAhead) < _branchNodes.Count)
                    {
                        positionToPlace = _branchNodes[i + branchesToPlaceAhead];
                    }
                    else
                    {

                        positionToPlace = _branchNodes[i];
                    }
                }
            }
            Node pathPoint1 = SplinePlusAPI.CreateNode(_splinePlusScript.SPData, vehiclesPosition);
            int branchKey = SplinePlusAPI.ConnectTwoNodes(_splinePlusScript.SPData, pathPoint1, positionToPlace);
            _splinePlusScript.GoToNewBranch(branchKey);

            yield return new WaitForSeconds(dropGracePeriod);
            hypeColliderObject.SetActive(true);
            botCollider.enabled = true;
            _currentlySettingPosition = false;
        }
    }

    public HypeGateBehavior GetCurrentArena()
    {
        return _currentArena;
    }

    public Transform GetCurrentArenaDesignation()
    {
        return _currentArenaDesignation;
    }

    public void DetachFromSpline(HypeGateBehavior currentArenaIn)
    {
        _inArena = true;
        _currentArena = currentArenaIn;
        _currentArenaDesignation = _currentArena.hotSpotLocation;
        _splinePlusScript.SPData.Followers[0].FollowerGO = null;
        if(_beingHeld)
        {
            for(int i = 0; i < _hypeManagerScript.vehicleList.Count; i++)
            {
                if (_hypeManagerScript.vehicleList[i].GetComponent<HotSpotVehicleAdministration>().holdingTheBot)
                {
                    _hypeManagerScript.vehicleList[i].GetComponent<HotSpotVehicleAdministration>().DropTheBot();
                }
            }
        }
        else
        {
            StartCoroutine(LerpToArenaLocation());
        }
    }

    private IEnumerator LerpToArenaLocation()
    {
        float speed = 3.0F;
        float dist = 9999;
        hypeColliderObject.SetActive(false);
        botCollider.enabled = false;
        while(dist >= 0.1f)
        {
            dist = Vector3.Distance(_currentArenaDesignation.position, transform.position);
            transform.position = Vector3.Lerp(transform.position, _currentArenaDesignation.position, Time.deltaTime * speed);
            yield return null;
        }
        if (!_allVehiclesIn)
        {
            meshRenderer.enabled = false;
            while(!_allVehiclesIn)
            {
                yield return null;
            }
            meshRenderer.enabled = true;
        }
        hypeColliderObject.SetActive(true);
        botCollider.enabled = true;
    }

    public void ReAttachToSpline()
    {
        _inArena = false;
        _currentArena = null;
        _splinePlusScript.SPData.Followers[0].FollowerGO = gameObject;
        if(_beingHeld)
        {
            for(int i = 0; i < _hypeManagerScript.vehicleList.Count; i++)
            {
                if (_hypeManagerScript.vehicleList[i].GetComponent<HotSpotVehicleAdministration>().holdingTheBot)
                {
                    _hypeManagerScript.vehicleList[i].GetComponent<HotSpotVehicleAdministration>().DropTheBot();
                }
            }
        }
        else
        {
            StartCoroutine(SetPosition(_currentArenaDesignation.position));
        }
    }

    public Vector3 GetNearestPointOnSpline(Vector3 givenPosition, int vectorsBack)
    {
        Vector3 closestWorldPoint = new Vector3(_hugeDistance, _hugeDistance, _hugeDistance);
        float lastDistance = _hugeDistance;
        int vectorsBackAdjustment;
        
        foreach (KeyValuePair<int, Branch> entry in _branchesAtStart)
        {
            for (int j = 0; j < entry.Value.Vertices.Count; j++)
            {
                float distance = Vector3.Distance(entry.Value.Vertices[j], givenPosition);
                if (distance <= lastDistance)
                {
                    lastDistance = distance;
                    vectorsBackAdjustment = Mathf.Clamp(j + vectorsBack, 1, entry.Value.Vertices.Count - 1);
                    closestWorldPoint = entry.Value.Vertices[vectorsBackAdjustment];
                }
            }
        }
        return closestWorldPoint;
    }

}