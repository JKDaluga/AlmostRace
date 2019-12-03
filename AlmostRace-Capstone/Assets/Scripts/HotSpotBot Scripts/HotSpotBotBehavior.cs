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
    private List<Node> branchNodes = new List<Node>();
    private SplinePlus _splinePlusScript;
    private HypeManager _hypeManagerScript;
    private Transform currentArenaDesignation;
    private Transform _closestVehicle;
    private bool _beingHeld;
    private bool _inArena;
    private bool _allVehiclesIn;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        _splinePlusScript = GameObject.FindGameObjectWithTag("HotSpotSpline").GetComponent<SplinePlus>();
        _splinePlusScript.SPData.Followers[0].Reverse = true;
        _hypeManagerScript = FindObjectOfType<HypeManager>();

        if (_hypeManagerScript == null)
        {
            Debug.LogError("Hype Manager not found!");
        }
         _closestVehicle = _hypeManagerScript.vehicleList[1].transform;

        foreach (KeyValuePair<int, Branch> entry in _splinePlusScript.SPData.DictBranches)
        {
            for(int i = 0; i < entry.Value.Nodes.Count; i++)
            {
                if(!branchNodes.Contains(entry.Value.Nodes[i]))
                {
                    branchNodes.Add(entry.Value.Nodes[i]);
                }
            }
        }
        SetBeingHeld(false);
    }

    void Update()
    {
        SetBotSpeed();
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

    public void SetBotSpeed()
    {
        for (int i = 0; i < _hypeManagerScript.vehicleList.Count; i++)
        {
            float distance = Vector3.Distance(transform.position, _hypeManagerScript.vehicleList[i].transform.position);
            if (distance <= Vector3.Distance(transform.position, _closestVehicle.transform.position))
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
        Node positionToPlace = branchNodes[0];
        float distance = Vector3.Distance(branchNodes[0].Point.position, vehiclesPosition);
        
        if(_inArena)
        {
            StartCoroutine(LerpToArenaLocation());
        }
        else
        {
            hypeColliderObject.SetActive(false);
            botCollider.enabled = false;

            for (int i = 0; i < branchNodes.Count; i++)
            {
                if(distance > Vector3.Distance(branchNodes[i].Point.position, vehiclesPosition))
                {
                    distance = Vector3.Distance(branchNodes[i].Point.position, vehiclesPosition);
                    positionToPlace = branchNodes[i];
                }
            }
            Node pathPoint1 = SplinePlusAPI.CreateNode(_splinePlusScript.SPData, vehiclesPosition);
            int branchKey = SplinePlusAPI.ConnectTwoNodes(_splinePlusScript.SPData, pathPoint1, positionToPlace);
            _splinePlusScript.GoToNewBranch(branchKey);
            _splinePlusScript.SPData.Followers[0].DistanceData.Index = branchKey;

            yield return new WaitForSeconds(dropGracePeriod);
            hypeColliderObject.SetActive(true);
            botCollider.enabled = true;
        }
    }

    public void DetachFromSpline(Transform givenLocationDesignation)
    {
        _inArena = true;
        currentArenaDesignation = givenLocationDesignation;
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
            dist = Vector3.Distance(currentArenaDesignation.position, transform.position);
            transform.position = Vector3.Lerp(transform.position, currentArenaDesignation.position, Time.deltaTime * speed);
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
            StartCoroutine(SetPosition(currentArenaDesignation.position));
        }
    }

    public Vector3 GetNearestPointOnSpline(Vector3 givenPosition, int vectorsBack)
    {
        int _hugeDistance = 9999;
        Vector3 closestWorldPoint = new Vector3(_hugeDistance, _hugeDistance, _hugeDistance);
        float lastDistance = _hugeDistance;
        int vectorsBackAdjustment;

        List<Branch> currentBranches = new List<Branch>();
        foreach (KeyValuePair<int, Branch> entry in _splinePlusScript.SPData.DictBranches)
        {
            currentBranches.Add(entry.Value);
        }
        
        for (int i = 0; i < currentBranches.Count; i++)
        {
            for (int j = 0; j < currentBranches[i].Vertices.Count; j++)
            {
                float distance = Vector3.Distance(currentBranches[i].Vertices[j], givenPosition);
                if (distance <= lastDistance)
                {
                    lastDistance = distance;
                    vectorsBackAdjustment = Mathf.Clamp(j + vectorsBack, 0, currentBranches[i].Vertices.Count - 1);
                    closestWorldPoint = currentBranches[i].Vertices[vectorsBackAdjustment];
                }
            }
        }
        return closestWorldPoint;
    }

}