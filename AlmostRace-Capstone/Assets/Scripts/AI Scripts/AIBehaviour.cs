using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AIBehaviour : MonoBehaviour
{

    public bool reverseDirection;
    public float inputForward, inputTurn, inputBrake;

    private bool _inArena;
    public bool canDrive = false;

    RaycastCar thisCar;
    

    //Copied from HotSpotBehaviour.cs script. It creates a spline that is capable of following the hotspot bot spline
    private SplinePlus _aiSplineScript;
    private List<Node> _branchNodes = new List<Node>();

    //From hotspotbehaviour script, I believe this is a dictionary of all branches on the map
    private Dictionary<int, Branch> _branchesAtStart = new Dictionary<int, Branch>();

    private float _currentBranchOfBot;
    private readonly int _hugeDistance = 9999;
    private readonly int _hugeTurn = 9999;
    private Vector3 closestVertex = Vector3.zero;
    private Vector3 vertexAim = Vector3.zero;
    

    // Start is called before the first frame update
    void Start()
    {
        //Sets ai spline to find/follow hotspotspline
        _aiSplineScript = GameObject.FindGameObjectWithTag("AISpline").GetComponent<SplinePlus>();
        //_aiSplineScript.SPData.Followers[0].Reverse = reverseDirection;

        _branchesAtStart = new Dictionary<int, Branch>(_aiSplineScript.SPData.DictBranches);

        thisCar = GetComponent<RaycastCar>();

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

    // Update is called once per frame
    void Update()
    {

        if (canDrive)
        {
            if (!_inArena)
            {
                //A.I on single direction vehicle track
                SetAiSpeed();
            }
            else
            {
                //Run A.I in arena script
            }

            
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
                    }
                else if (currentPosition <= lastDistance)
                {
                    thisCar.throttle = inputForward;
                    thisCar.horizontal = inputTurn;
                    thisCar.drift = true;

                    placeHolder = j;
                    placeHolder = Mathf.Clamp(placeHolder+10, 0, entry.Value.Vertices.Count-1);

                    vertexAim = entry.Value.Vertices[placeHolder];

                    lastDistance = currentPosition;
                    closestVertex = vertexAim;
                }

            }
        }
        currentPosition = Vector3.Distance(closestVertex, transform.position);
        angleBetween = Mathf.Acos(Vector3.Dot((closestVertex-transform.position).normalized, thisCar.transform.right)) * 180 / Mathf.PI;


        print("Current Turn Angle = " + angleBetween);
        print("Current Turn number = " + inputTurn);

        if (angleBetween < 87.5)
        {
            inputTurn =  Mathf.Pow((-(angleBetween - 90) / 90), (1/2));
        }
        else if (angleBetween > 97.5)
        {
            inputTurn =  -Mathf.Pow(((angleBetween - 90) / 90), (1/2));
        }
        else
        {
            inputTurn = 0;
        }

        


        inputForward = Mathf.Clamp((1.2f) - Mathf.Abs(inputTurn), 0, 1);

    }

    private void OnDrawGizmos()
    {
        float angleBetween = Mathf.Acos(Vector3.Dot((closestVertex - transform.position).normalized, thisCar.transform.right)) * 180 / Mathf.PI;
        Gizmos.color = new Color(0, 0, 1, 1);
        Vector3 direction = (-Mathf.Pow(((angleBetween - 90) / 90), 3)*transform.right)*4;
        Gizmos.DrawRay(new Vector3(transform.position.x, transform.position.y, transform.position.z), direction);
        Gizmos.DrawCube(vertexAim, new Vector3(4, 4, 4));
    }
}
