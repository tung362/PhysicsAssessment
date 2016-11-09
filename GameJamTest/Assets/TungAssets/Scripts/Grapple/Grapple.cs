using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Just stores the data used by the rope, wanted to avoid using multiple lists
//If you don't like it, i can always switch it back to seperate lists
[System.Serializable]
public struct RopeData
{
    public Vector3 AnchorConnectionPoint;
    public Vector2 Normal;
    public RopeData(Vector3 aAnchorConnectionPoints)
    {
        AnchorConnectionPoint = aAnchorConnectionPoints;
        Normal = Vector2.zero;
    }
    public RopeData(Vector3 aAnchorConnectionPoints, Vector2 aNormal)
    {
        AnchorConnectionPoint = aAnchorConnectionPoints;
        Normal = aNormal;
    }
}

//In Physics2DSettings make sure to uncheck "Queries Start In Collider"
public class Grapple : MonoBehaviour
{
    public List<RopeData> RopeDatas; //First index reserved for hook, all the data of current and previous connected ropes

    //Tracked Hook
    public GameObject Hook; //The projectile that is the hook
    private Vector2 HookEnd;
    private Vector2 PreviousHookEnd;

    //Tracked Grapple
    private Vector2 GrappleEnd;
    private Vector2 PreviousGrappleEnd;

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
        RopeDatas.Add(new RopeData(TheDistanceJoint.connectedAnchor)); //Start
        GrappleEnd = TheDistanceJoint.connectedAnchor;
        PreviousGrappleEnd = GrappleEnd;

        //Applying list to line renderer
        TheLineRenderer.SetVertexCount(RopeDatas.Count + 1);
        TheLineRenderer.SetPosition(0, TheDistanceJoint.connectedAnchor);
        TheLineRenderer.SetPosition(1, transform.position);
        /////////////
    }

    void Update ()
    {
        UpdateGrapple(); //Takes care of wrapping and unwrapping when the player moves
        UpdateHook(); //Takes care of wrapping and unwrapping when the hook moves (The projectile that was shot out)
    }

    //The Core
    void UpdateGrapple()
    {
        UpdateLineRenderer();
        RaycastHit2D hit = Physics2D.Linecast(transform.position, GrappleEnd); //Linecast from the player to the last valid hit point

        Debug.DrawLine(transform.position, GrappleEnd, Color.red);
        Debug.DrawLine(transform.position, PreviousGrappleEnd, Color.blue);

        //If the grappling hook collides with any platforms create a new point (Wrapping)
        if (hit.collider != null)
        {
            if (hit.transform.gameObject.name == "Platform")
            {
                RopeDatas.Add(new RopeData(OffsetedHitPoint(hit), hit.normal));
                UpdateLineRenderer();
                PreviousGrappleEnd = GrappleEnd;
                GrappleEnd = OffsetedHitPoint(hit);
            }
        }
        UnwrapCheck();
    }

    void UpdateHook()
    {
    }

    //Unwraps the rope
    void UnwrapCheck()
    {
        //Don't ask me how it works, this gave me nightmares >:V
        if (RopeDatas.Count > 1)
        {
            Vector2 hookDirection = (PreviousGrappleEnd - GrappleEnd).normalized;
            Vector2 playerDirection = (new Vector2(transform.position.x, transform.position.y) - GrappleEnd).normalized;
            Vector2 normalDirection = ((GrappleEnd - RopeDatas[RopeDatas.Count - 1].Normal) - GrappleEnd).normalized;

            float dot = Vector2.Dot(playerDirection, new Vector2(-hookDirection.y, hookDirection.x));
            float dotNormal = Vector2.Dot(normalDirection, new Vector2(-hookDirection.y, hookDirection.x));

            if ((dotNormal > 0 && dot < 0) || (dotNormal < 0 && dot > 0)) GoBackAStep();
        }
    }

    //Reverting back a point (Unwrap)
    void GoBackAStep()
    {
        GrappleEnd = PreviousGrappleEnd;
        if (RopeDatas.Count > 2) PreviousGrappleEnd = RopeDatas[RopeDatas.Count - 3].AnchorConnectionPoint;
        else PreviousGrappleEnd = GrappleEnd;

        RopeDatas.RemoveAt(RopeDatas.Count - 1);
        UpdateLineRenderer();
    }

    //Offsets the hit point away from the original
    Vector2 OffsetedHitPoint(RaycastHit2D hit)
    {
        Vector2 direction = (hit.point - new Vector2(hit.transform.position.x, hit.transform.position.y)).normalized * 0.01f;
        return hit.point + direction;
    }

    //Applys AnchorConnectionPoints to line renderer
    void UpdateLineRenderer()
    {
        TheLineRenderer.SetVertexCount(RopeDatas.Count + 1); //+1 because there needs to be extra room for the end point
        TheLineRenderer.SetPosition(0, TheDistanceJoint.connectedAnchor); //Start
        for (int i = 1; i < RopeDatas.Count; ++i) TheLineRenderer.SetPosition(i, RopeDatas[i].AnchorConnectionPoint); //Applying all the AnchorConnectionPoints to the line renderer (should have one spot left)
        TheLineRenderer.SetPosition(RopeDatas.Count, transform.position); //End
    }
}
