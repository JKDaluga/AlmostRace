using UnityEngine;

public class ProjectionClass {

    public void Raycasts( SPData SPData, Branch branch) 
    {
        int layerMask = ~LayerMask.GetMask("Ray");
        for (int i = 0; i < branch.Nodes.Count; i++)
        {
            RaycastHit Hit, Hit1;
 
             var origin = branch.Nodes[i].Point.position + Vector3.up * SPData.RaycastLength;
            if (Physics.Raycast(origin, -Vector3.up, out Hit, SPData.RaycastLength * 2, layerMask) )
            {
                branch.Nodes[i].Point.position = Hit.point + Vector3.up * SPData.Offset;
                if (SPData.MeshOrientation) branch.Nodes[i].Normal = Hit.normal; 
            }
            if (SPData.HandlesProjection)
            {
                origin = branch.Nodes[i].Point1.position + Vector3.up * SPData.RaycastLength;
                if (Physics.Raycast(origin, -Vector3.up, out Hit1, SPData.RaycastLength * 2, layerMask))
                {
                   branch.Nodes[i].Point1.position = Hit1.point + Vector3.up * SPData.Offset;
                   branch.Nodes[i].Point2.localPosition = -branch.Nodes[i].Point1.localPosition;
                }
            }
        }
    }
}
