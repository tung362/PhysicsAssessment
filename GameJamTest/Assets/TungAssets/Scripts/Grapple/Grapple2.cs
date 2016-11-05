using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//In Physics2DSettings make sure to uncheck "Queries Start In Collider"
public class Grapple2 : MonoBehaviour
{
    //Tracked
    public List<Vector3> AnchorConnectionPoints; //First index reserved for hook
    private Vector2 Hook;
    private Vector2 PreviousHook;
    private bool PreviousIsClockwise = true;
    private bool CurrentIsClockwise = true;

    //Components
    private LineRenderer TheLineRenderer;
    private DistanceJoint2D TheDistanceJoint;

    void Start ()
    {
        //Assigning components
        TheLineRenderer = GetComponent<LineRenderer>();
        TheDistanceJoint = GetComponent<DistanceJoint2D>();

        //Temporary//
        //Setting the defaults
        AnchorConnectionPoints.Add(TheDistanceJoint.connectedAnchor); //Start
        Hook = TheDistanceJoint.connectedAnchor;
        PreviousHook = Hook;

        //Applying list to line renderer
        TheLineRenderer.SetVertexCount(AnchorConnectionPoints.Count + 1);
        TheLineRenderer.SetPosition(0, TheDistanceJoint.connectedAnchor);
        TheLineRenderer.SetPosition(1, transform.position);
        /////////////
    }

    void Update ()
    {
        UpdateGrapple();
    }

    //The Core
    void UpdateGrapple()
    {
        UpdateLineRenderer();
        RaycastHit2D hit = Physics2D.Linecast(transform.position, Hook);
        //RaycastHit2D hitPrevious = Physics2D.Linecast(transform.position, PreviousHook);

        Debug.DrawLine(transform.position, Hook, Color.red);
        Debug.DrawLine(transform.position, PreviousHook, Color.blue);

        //If the grappling hook collides with any platforms create a new point
        if (hit.collider != null)
        {
            if (hit.transform.gameObject.name == "Platform")
            {
                AnchorConnectionPoints.Add(OffsetedHitPoint(hit));
                UpdateLineRenderer();
                PreviousHook = Hook;
                Hook = OffsetedHitPoint(hit);
            }
        }

        UnwrapCheck();
    }

    //Applys AnchorConnectionPoints to line renderer
    void UpdateLineRenderer()
    {
        TheLineRenderer.SetVertexCount(AnchorConnectionPoints.Count + 1); //+1 because there needs to be extra room for the end point
        TheLineRenderer.SetPosition(0, TheDistanceJoint.connectedAnchor); //Start
        for (int i = 1; i < AnchorConnectionPoints.Count; ++i) TheLineRenderer.SetPosition(i, AnchorConnectionPoints[i]); //Applying all the AnchorConnectionPoints to the line renderer (should have one spot left)
        TheLineRenderer.SetPosition(AnchorConnectionPoints.Count, transform.position); //End
    }

    //Reverting back a point (Unwrap)
    void GoBackAStep()
    {
        if(AnchorConnectionPoints.Count > 1)
        {
            AnchorConnectionPoints.RemoveAt(AnchorConnectionPoints.Count - 1);
            UpdateLineRenderer();
            Hook = PreviousHook;
        }

        if (AnchorConnectionPoints.Count > 1) PreviousHook = AnchorConnectionPoints[AnchorConnectionPoints.Count - 2];
        else PreviousHook = Hook;
    }

    //Offsets the hit point away from the original
    Vector2 OffsetedHitPoint(RaycastHit2D hit)
    {
        Vector2 direction = (hit.point - new Vector2(hit.transform.position.x, hit.transform.position.y)).normalized * 0.01f;
        return hit.point + direction;
    }

    void UnwrapCheck()
    {

        Vector2 hookDirection = (PreviousHook - Hook).normalized;
        Vector2 playerDirection = (new Vector2(transform.position.x, transform.position.y) - Hook).normalized;

        //float ang = Vector2.Angle(hookDirection, playerDirection);
        //Vector3 testAgainst = Vector3.Cross(hookDirection, playerDirection);
        //if (testAgainst.z > 0) ang = 360 - ang;

        float augoo = Vector2.Dot(hookDirection, playerDirection);


        //Debug.Log(testAgainst.z);

        Debug.Log(augoo);

        if (AnchorConnectionPoints.Count > 1)
        {
            //if (ang >= 180 && testAgainst.z > 0) GoBackAStep();
            //if (ang <= 180 && testAgainst.z < 0) GoBackAStep();
        }
    }
}
