using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class AIBehaviour : MonoBehaviour
{

    public bool reverseDirection;
    public float inputForward, inputTurn, inputBrake;

    public bool _inArena;
    public bool canDrive = false;
    public float offsetAngle = 10f;
    public int nodesToLookAhead = 5;

    public RaycastCar thisCar;
    

    //Copied from HotSpotBehaviour.cs script. It creates a spline that is capable of following the hotspot bot spline
    private SplinePlus _aiSplineScript;
    private List<Node> _branchNodes = new List<Node>();

    //From hotspotbehaviour script, I believe this is a dictionary of all branches on the map
    private Dictionary<int, Branch> _branchesAtStart = new Dictionary<int, Branch>();

    private float _currentBranchOfBot;
    private readonly int _hugeDistance = 9999;
    private readonly int _hugeTurn = 9999;
    private Vector3 closestVertex = Vector3.zero;
    [HideInInspector] public int closestIndex = 0;
    private Vector3 vertexAim = Vector3.zero;

    private AIObstacleAvoidance avo;

    private GameObject[] _aiSplines;
    public GameObject[] orderedSplines;
    private int splineIndex;
    

    // Start is called before the first frame update
    void Start()
    {
        _aiSplines = GameObject.FindGameObjectsWithTag("AISpline");
        /*Dictionary<float, GameObject> near = new Dictionary<float, GameObject>();

        foreach(GameObject i in _aiSplines)
        {
            near.Add(Vector3.Distance(i.GetComponent<SplinePlus>().SPData.DictBranches[0].Vertices[0], transform.position), i);
        }
        int num = near.Count;
        for(int i = 0; i < num; i++)
        {
            orderedSplines.Add(near[near.Keys.Min()]);
            near.Remove(near.Keys.Min());
        }*/
        RaceManager rc = FindObjectOfType<RaceManager>();
        orderedSplines = rc.orderedSplines;

        splineIndex = 0;
        avo = GetComponentInChildren<AIObstacleAvoidance>();
       // print(orderedSplines[splineIndex].name);
        //Sets ai spline to find/follow hotspotspline
        _aiSplineScript = orderedSplines[splineIndex].GetComponent<SplinePlus>();
        //_aiSplineScript.SPData.Followers[0].Reverse = reverseDirection;

        _branchesAtStart = new Dictionary<int, Branch>(_aiSplineScript.SPData.DictBranches);

        thisCar = GetComponent<RaycastCar>();

        //thisCar.drift = true;

        foreach (KeyValuePair<int, Branch> entry in _branchesAtStart)
        {
            for (int i = 0; i < entry.Value.Nodes.Count; i++)
            {
                if (!_branchNodes.Contains(entry.Value.Nodes[i]))
                {
                    _branchNodes.Add(entry.Value.Nodes[i]);
                }
            }
        }
    }

    public void SwapSpline()
    {
        splineIndex++;
        print("SPLINE SWAPPED : " + splineIndex);
        _aiSplineScript = orderedSplines[splineIndex].GetComponent<SplinePlus>();

        //print(orderedSplines[splineIndex].name);
        _branchesAtStart = new Dictionary<int, Branch>(_aiSplineScript.SPData.DictBranches);

        foreach (KeyValuePair<int, Branch> entry in _branchesAtStart)
        {
            for (int i = 0; i < entry.Value.Nodes.Count; i++)
            {
                if (!_branchNodes.Contains(entry.Value.Nodes[i]))
                {
                    _branchNodes.Add(entry.Value.Nodes[i]);
                }
            }
        }

        _inArena = !_inArena;
    }

    // Update is called once per frame
    void Update()
    {

        if (canDrive)
        {
            //A.I on single direction vehicle track
            SetAiSpeed();
        }

    }

    public void SetAiSpeed()
    {
        float lastDistance = _hugeDistance;
        float lastTurn = _hugeTurn;

        float currentPosition, angleBetween;
        int placeHolder = 0;

        foreach (KeyValuePair<int, Branch> entry in _branchesAtStart)
        {
            for (int j = 0; j < entry.Value.Vertices.Count; j++)
            {
                currentPosition = Vector3.Distance(entry.Value.Vertices[j], transform.position);

                if (closestVertex == Vector3.zero) {

                    closestVertex = entry.Value.Vertices[j];
                    lastDistance = currentPosition;
                    closestIndex = j;
                    }
                else if (currentPosition <= lastDistance)
                {
                    thisCar.throttle = inputForward;
                    thisCar.horizontal = inputTurn;

                    placeHolder = j;
                    placeHolder = Mathf.Clamp(placeHolder+nodesToLookAhead, 0, entry.Value.Vertices.Count-1);

                    vertexAim = entry.Value.Vertices[placeHolder];

                    lastDistance = currentPosition;
                    closestVertex = vertexAim;
                    closestIndex = placeHolder;
                }

            }
        }
        currentPosition = Vector3.Distance(closestVertex, transform.position);
        angleBetween = Mathf.Acos(Vector3.Dot((closestVertex-transform.position).normalized, thisCar.transform.right)) * 180 / Mathf.PI;


        //print("Current Turn Angle = " + angleBetween);
        //print("Current Turn number = " + inputTurn);

        if (!avo.turnL && !avo.turnR)
        {
            if (angleBetween < 90 - offsetAngle)
            {
                inputTurn = Mathf.Pow((-(angleBetween - 90) / 90), (1 / 2));
            }
            else if (angleBetween > 90 + offsetAngle)
            {
                inputTurn = -Mathf.Pow(((angleBetween - 90) / 90), (1 / 2));
            }
            else
            {
                inputTurn = 0;
            }
        }
        


        inputForward = Mathf.Clamp((1.2f) - Mathf.Abs(inputTurn), 0, 1);

    }
}
