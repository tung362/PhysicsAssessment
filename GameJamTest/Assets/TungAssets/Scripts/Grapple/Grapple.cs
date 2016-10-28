using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grapple : MonoBehaviour
{
    public GameObject TheLine;
    public GameObject Test;

    //Last connection point = the real connection point, the rest are fallbacks
    public List<Vector3> AnchorConnectionPoints;

    private DistanceJoint2D TheDistanceJoint;


    void Start ()
    {
        TheDistanceJoint = GetComponent<DistanceJoint2D>();
    }
	
	void FixedUpdate()
    {
        //AnchorConnectionPoints.Clear();
        //WallCheck(TheDistanceJoint.connectedAnchor, transform.position);
        //AnchorConnectionPoints.Add(Vector2.zero);
        //AnchorConnectionPoints[AnchorConnectionPoints.Count - 1

        if (AnchorConnectionPoints.Count == 0) WallCheck(TheDistanceJoint.connectedAnchor, transform.position);
        else
        {
            WallCheck(AnchorConnectionPoints[AnchorConnectionPoints.Count - 1], transform.position);
            TheLine.GetComponent<LineRenderer>().SetVertexCount(AnchorConnectionPoints.Count + 1);
            TheLine.GetComponent<LineRenderer>().SetPositions(AnchorConnectionPoints.ToArray());
            TheLine.GetComponent<LineRenderer>().SetPosition(0, TheDistanceJoint.connectedAnchor);
            TheLine.GetComponent<LineRenderer>().SetPosition(AnchorConnectionPoints.Count - 1, AnchorConnectionPoints[AnchorConnectionPoints.Count - 1]);
            Test.transform.position = AnchorConnectionPoints[AnchorConnectionPoints.Count - 1];
        }
    }

    void WallCheck(Vector3 Start, Vector3 End)
    {
        RaycastHit2D hit = Physics2D.Linecast(Start, End);
        if (hit.transform.gameObject.name == "Platform")
        {
            //Vector2 hitPoint = Vector2.zero;

            //if(TheDistanceJoint.connectedAnchor.x <= transform.position.x) hitPoint.x = hit.collider.bounds.max.x;
            //else hitPoint.x = hit.collider.bounds.min.x;

            //if (TheDistanceJoint.connectedAnchor.y <= transform.position.y) hitPoint.y = hit.collider.bounds.min.y;
            //else hitPoint.y = hit.collider.bounds.max.y;

            //AnchorConnectionPoints.Add(hitPoint);

            AnchorConnectionPoints.Add(hit.point);
        }
    }
}
