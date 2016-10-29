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
            TheLine.GetComponent<LineRenderer>().SetPosition(AnchorConnectionPoints.Count, AnchorConnectionPoints[AnchorConnectionPoints.Count - 1]);
            //Test.transform.position = AnchorConnectionPoints[AnchorConnectionPoints.Count - 1];
        }
    }

    void WallCheck(Vector3 Start, Vector3 End)
    {
        RaycastHit2D hit = Physics2D.Linecast(Start, End);

        Debug.DrawLine(Start, End);
        if (hit.transform.gameObject.name == "Platform")
        {
            //(hit.normal * 0.01f)
            //AnchorConnectionPoints.Add(hit.point);

            Vector3 closestVectice = Vector3.zero;
            float closestDistance = int.MaxValue;
            foreach(Vector3 point in hit.transform.GetComponent<MeshFilter>().mesh.vertices)
            {
                Vector3 rotatedPoint = hit.transform.TransformDirection(new Vector3(point.x * (hit.transform.localScale.x + 0.015f),
                                                                                    point.y * (hit.transform.localScale.y + 0.015f),
                                                                                    point.z * (hit.transform.localScale.z + 0.015f)));

                float distance = Vector3.Distance(hit.point, hit.transform.position + rotatedPoint);
                if(distance < closestDistance)
                {
                    closestDistance = distance;
                    closestVectice = rotatedPoint;
                }
            }

            //Debug.Log((hit.transform.position + closestVectice).x);
            Test.transform.position = hit.transform.position + closestVectice;
            AnchorConnectionPoints.Add(hit.transform.position + closestVectice);
        }
    }
}
