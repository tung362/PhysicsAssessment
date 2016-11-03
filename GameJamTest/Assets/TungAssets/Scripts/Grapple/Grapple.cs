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
    private Vector2 lastPos;

    void Start()
    {
        TheDistanceJoint = GetComponent<DistanceJoint2D>();
        //StartCoroutine(WallCheckCo());
    }

    void FixedUpdate()
    {
        //AnchorConnectionPoints.Clear();
        //WallCheck(TheDistanceJoint.connectedAnchor, transform.position);
        //AnchorConnectionPoints.Add(Vector2.zero);
        //AnchorConnectionPoints[AnchorConnectionPoints.Count - 1


        if (AnchorConnectionPoints.Count == 0)
        {
            WallCheck(TheDistanceJoint.connectedAnchor, transform.position); //0
            //StartCoroutine(WallCheckCo(TheDistanceJoint.connectedAnchor, transform.position));
        }
        else
        {
            WallCheck(AnchorConnectionPoints[AnchorConnectionPoints.Count - 1], transform.position); //1
            //StartCoroutine(WallCheckCo(AnchorConnectionPoints[AnchorConnectionPoints.Count - 1], transform.position));
            TheLine.GetComponent<LineRenderer>().SetVertexCount(AnchorConnectionPoints.Count + 1); //|2
            TheLine.GetComponent<LineRenderer>().SetPositions(AnchorConnectionPoints.ToArray());

            TheLine.GetComponent<LineRenderer>().SetPosition(0, TheDistanceJoint.connectedAnchor);
            TheLine.GetComponent<LineRenderer>().SetPosition(AnchorConnectionPoints.Count, AnchorConnectionPoints[AnchorConnectionPoints.Count - 1]);
            //Test.transform.position = AnchorConnectionPoints[AnchorConnectionPoints.Count - 1];
        }
    }

    //Only supports box and circle colliders >:V
    void WallCheck(Vector2 Start, Vector2 End)
    {
        RaycastHit2D hit = Physics2D.Linecast(Start, End);        

        Debug.DrawLine(Start, End);

        if (hit.transform.gameObject.name == "Platform")
        {
            if (hit.transform.GetComponent<CircleCollider2D>() != null)
            {
                Vector2 ObjectCenter = hit.transform.position;
                Vector2 hitDirection = (ObjectCenter - hit.point).normalized;
                AnchorConnectionPoints.Add(ObjectCenter - ((new Vector2(hitDirection.x * hit.transform.localScale.x, hitDirection.y * hit.transform.localScale.y)) * hit.transform.GetComponent<CircleCollider2D>().radius * 1.05f)); //Offset
            }
            else if (hit.transform.GetComponent<BoxCollider2D>() != null)
            {
                //Define edge points of the box collider
                BoxCollider2D theCollider = hit.transform.GetComponent<BoxCollider2D>();
                Vector2 size = theCollider.size;
                Vector2 worldPosition = hit.transform.TransformPoint(theCollider.offset);

                float up = (size.y / 2);
                float down = -(size.y / 2);
                float right = (size.x / 2);
                float left = -(size.x / 2);

                List<Vector2> points = new List<Vector2>();
                points.Add(new Vector2(up, left));
                points.Add(new Vector2(down, left));
                points.Add(new Vector2(up, right));
                points.Add(new Vector2(down, right));


                //Find closest edge point
                Vector2 closestVectice = Vector3.zero;
                float closestDistance = int.MaxValue;
                foreach (Vector2 point in points)
                {
                    //Getting local space position
                    Vector2 rotatedPoint = hit.transform.TransformDirection(point.x * (hit.transform.localScale.x + 0.055f),
                                                                                        point.y * (hit.transform.localScale.y + 0.055f),
                                                                                        transform.position.z);

                    //Getting world position
                    Vector2 pointWorldPosition = worldPosition + rotatedPoint;

                    //Closest point calculation
                    float distance = Vector2.Distance(hit.point, pointWorldPosition);
                    if (distance < closestDistance && pointWorldPosition != Start)
                    {
                        closestDistance = distance;
                        closestVectice = pointWorldPosition;
                    }


                    //Second Closest Point/////////////////////////////////////
                    Vector2 SecondclosestVectice = Vector3.zero;
                    float SecondclosestDistance = int.MaxValue;
                    foreach (Vector2 point2 in points)
                    {
                        //Getting local space position
                        Vector2 rotatedPoint2 = hit.transform.TransformDirection(point2.x * (hit.transform.localScale.x + 0.055f),
                                                                                            point.y * (hit.transform.localScale.y + 0.055f),
                                                                                            transform.position.z);
                        //Getting world position
                        Vector2 pointWorldPosition2 = worldPosition + rotatedPoint2;

                        //Second closest point calculation
                        float secondDistance = Vector2.Distance(hit.point, pointWorldPosition2);
                        if (secondDistance < SecondclosestDistance && pointWorldPosition2 != closestVectice)
                        {
                            SecondclosestDistance = secondDistance;
                            SecondclosestVectice = pointWorldPosition2;
                        }
                    }
                    ////////////////////////////////////////////////////////////

                    Vector2 lastDir = (lastPos - Start).normalized;

                    Vector2 currDir = (End - Start).normalized;
                    Vector2 midDir = (currDir - lastPos) * 0.5f;

                    Vector2 a = (point - Start).normalized;
                    Vector2 b = (SecondclosestVectice - Start).normalized;

                    float disA = Vector2.Distance(midDir, a);
                    float disB = Vector2.Distance(midDir, b);

                    if (disA > disB) closestVectice = SecondclosestVectice;
                }

                AnchorConnectionPoints.Add(closestVectice);
            }
            //To do: support poly Collider
            else if (hit.transform.GetComponent<PolygonCollider2D>() != null)
            {

            }

            //WallCheck(AnchorConnectionPoints[AnchorConnectionPoints.Count - 1], transform.position);
            lastPos = transform.position;
        }
        //if (hit.transform.gameObject.name == "Platform")
        //{
        //    //(hit.normal * 0.01f)
        //    //AnchorConnectionPoints.Add(hit.point);

        //    Vector3 closestVectice = Vector3.zero;
        //    float closestDistance = int.MaxValue;
        //    foreach(Vector3 point in hit.transform.GetComponent<MeshFilter>().mesh.vertices)
        //    {
        //        Vector3 rotatedPoint = hit.transform.TransformDirection(new Vector3(point.x * (hit.transform.localScale.x + 0.055f),
        //                                                                            point.y * (hit.transform.localScale.y + 0.055f),
        //                                                                            point.z * (hit.transform.localScale.z + 0.055f)));

        //        float distance = Vector2.Distance(hit.point, hit.transform.position + rotatedPoint);
        //        Vector2 flatRotatedPoint = hit.transform.position + rotatedPoint;

        //        if (distance < closestDistance && flatRotatedPoint != new Vector2(Start.x, Start.y))
        //        {
        //            closestDistance = distance;
        //            closestVectice = rotatedPoint;
        //        }
        //    }

        //    //Debug.Log((hit.transform.position + closestVectice).x);
        //    Test.transform.position = hit.transform.position + closestVectice;
        //    AnchorConnectionPoints.Add(hit.transform.position + closestVectice);
        //}
    }

    IEnumerator WallCheckCo(Vector2 Start, Vector2 End)
    {
        RaycastHit2D hit = Physics2D.Linecast(Start, End);

        Debug.DrawLine(Start, End);

        if (hit.transform.gameObject.name == "Platform")
        {
            if (hit.transform.GetComponent<CircleCollider2D>() != null)
            {
                Vector2 ObjectCenter = hit.transform.position;
                Vector2 hitDirection = (ObjectCenter - hit.point).normalized;
                AnchorConnectionPoints.Add(ObjectCenter - ((new Vector2(hitDirection.x * hit.transform.localScale.x, hitDirection.y * hit.transform.localScale.y)) * hit.transform.GetComponent<CircleCollider2D>().radius * 1.05f)); //Offset
            }
            else if (hit.transform.GetComponent<BoxCollider2D>() != null)
            {
                //Define edge points of the box collider
                BoxCollider2D theCollider = hit.transform.GetComponent<BoxCollider2D>();
                Vector2 size = theCollider.size;
                Vector2 worldPosition = hit.transform.TransformPoint(theCollider.offset);

                float up = (size.y / 2);
                float down = -(size.y / 2);
                float right = (size.x / 2);
                float left = -(size.x / 2);

                List<Vector2> points = new List<Vector2>();
                points.Add(new Vector2(up, left));
                points.Add(new Vector2(down, left));
                points.Add(new Vector2(up, right));
                points.Add(new Vector2(down, right));


                //Find closest edge point
                Vector3 closestVectice = Vector3.zero;
                float closestDistance = int.MaxValue;
                foreach (Vector2 point in points)
                {
                    Vector2 rotatedPoint = hit.transform.TransformDirection(point.x * (hit.transform.localScale.x + 0.055f),
                                                                                        point.y * (hit.transform.localScale.y + 0.055f),
                                                                                        transform.position.z);

                    Vector2 pointWorldPosition = worldPosition + rotatedPoint;
                    float distance = Vector2.Distance(hit.point, pointWorldPosition);

                    if (distance < closestDistance && pointWorldPosition != Start)
                    {
                        closestDistance = distance;
                        closestVectice = pointWorldPosition;
                    }
                }

                AnchorConnectionPoints.Add(closestVectice);
            }
            //To do: support poly Collider
            else if (hit.transform.GetComponent<PolygonCollider2D>() != null)
            {

            }

            //WallCheck(AnchorConnectionPoints[AnchorConnectionPoints.Count - 1], transform.position);
            yield return null;
        }
    }

        void Unwrap()
    {

    }
}
