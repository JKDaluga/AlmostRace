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
    [Tooltip("Speed when the bot is to far from the players and needs to slow down")] public float farSpeed = 50;
    [Tooltip("Speed when the bot is just within reach")] public float reachSpeed = 10;
    public float farDistance = 50;
    public float closeDistance = 10;
    public float dropGracePeriod = 2;
    public MeshRenderer meshRenderer;
    public Collider botCollider;
    public GameObject hypeColliderObject;
    public static HotSpotBotBehavior instance;
    private List<Node> _branchNodes = new List<Node>();
    private List<Branch> _branchesAtStart = new List<Branch>();
    private SplinePlus _splinePlusScript;
    private HypeManager _hypeManagerScript;
    private HypeGateBehavior _currentArena;
    private Transform _currentArenaDesignation;
    private Transform _closestVehicle;
    private bool _beingHeld;
    private bool _inArena;
    private bool _allVehiclesIn;

    private void Awake()
    {
        instance = this;
        //AudioManager.instance.Play("Hotspot Engine");
    }

    // Start is called before the first frame update
    void Start()
    {
        _splinePlusScript = GameObject.FindGameObjectWithTag("HotSpotSpline").GetComponent<SplinePlus>();
        _splinePlusScript.SPData.Followers[0].Reverse = false;
        _hypeManagerScript = FindObjectOfType<HypeManager>();

        if (_hypeManagerScript == null)
        {
            Debug.LogError("Hype Manager not found!");
        }

        foreach (KeyValuePair<int, Branch> entry in _splinePlusScript.SPData.DictBranches)
        {
            _branchesAtStart.Add(entry.Value);
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
        if (!_inArena)
        {
            SetBotSpeed();

        }
    }

    public bool GetBeingHeld()
    {
        AudioManager.instance.Play("Hotspot Grabbed");
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

    public void SetBotSpeed()
    {
        for (int i = 0; i < _hypeManagerScript.vehicleList.Count; i++)
        {
            float distance = Vector3.Distance(transform.position, _hypeManagerScript.vehicleList[i].transform.position);
            if (_closestVehicle == null || distance <= Vector3.Distance(transform.position, _closestVehicle.transform.position))
            {
                _closestVehicle = _hypeManagerScript.vehicleList[i].transform;
            }
        }

        if (Vector3.Distance(transform.position, _closestVehicle.transform.position) > farDistance)
        {
            _splinePlusScript.SetSpeed(farSpeed);
        }
        else if (Vector3.Distance(transform.position, _closestVehicle.transform.position) < farDistance
        && Vector3.Distance(transform.position, _closestVehicle.transform.position) > closeDistance)
        {
            _splinePlusScript.SetSpeed(chaseSpeed);
        }
        else if (Vector3.Distance(transform.position, _closestVehicle.transform.position) < closeDistance)
        {
            _splinePlusScript.SetSpeed(reachSpeed);
        }
    }

    public IEnumerator SetPosition(Vector3 vehiclesPosition)
    {
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
                    positionToPlace = _branchNodes[i];
                }
            }
            Node pathPoint1 = SplinePlusAPI.CreateNode(_splinePlusScript.SPData, vehiclesPosition);
            int branchKey = SplinePlusAPI.ConnectTwoNodes(_splinePlusScript.SPData, pathPoint1, positionToPlace);
            _splinePlusScript.GoToNewBranch(branchKey);

            yield return new WaitForSeconds(dropGracePeriod);
            hypeColliderObject.SetActive(true);
            botCollider.enabled = true;
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
        int _hugeDistance = 9999;
        Vector3 closestWorldPoint = new Vector3(_hugeDistance, _hugeDistance, _hugeDistance);
        float lastDistance = _hugeDistance;
        int vectorsBackAdjustment;
        
        for (int i = 0; i < _branchesAtStart.Count; i++)
        {
            for (int j = 0; j < _branchesAtStart[i].Vertices.Count; j++)
            {
                float distance = Vector3.Distance(_branchesAtStart[i].Vertices[j], givenPosition);
                if (distance <= lastDistance)
                {
                    lastDistance = distance;
                    vectorsBackAdjustment = Mathf.Clamp(j + vectorsBack, 1, _branchesAtStart[i].Vertices.Count - 1);
                    closestWorldPoint = _branchesAtStart[i].Vertices[vectorsBackAdjustment];
                }
            }
        }
        return closestWorldPoint;
    }

}