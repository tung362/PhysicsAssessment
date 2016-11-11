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
    public Vector2 HitPoint;
    public bool HookSide;
    public RopeData(Vector3 aAnchorConnectionPoints, Vector2 ahitPoint, bool isHookSide)
    {
        AnchorConnectionPoint = aAnchorConnectionPoints;
        Normal = Vector2.zero;
        HitPoint = ahitPoint;
        HookSide = isHookSide;
    }
    public RopeData(Vector3 aAnchorConnectionPoints, Vector2 aNormal, Vector2 ahitPoint, bool isHookSide)
    {
        AnchorConnectionPoint = aAnchorConnectionPoints;
        Normal = aNormal;
        HitPoint = ahitPoint;
        HookSide = isHookSide;
    }
}

//In Physics2DSettings make sure to uncheck "Queries Start In Collider"
public class Grapple : MonoBehaviour
{
    //First index reserved for hook, all the data of current and previous connected ropes
    public List<RopeData> RopeDatas;

    //Tracked Hook
    //The projectile that is the hook
    public GameObject Hook;
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
        RopeDatas.Add(new RopeData(Hook.transform.position, Vector2.zero, false)); //Start
    }

    void Update ()
    {
        //Default
        if (RopeDatas.Count == 1)
        {
            //Grapple
            RopeDatas[0] = new RopeData(Hook.transform.position, Vector2.zero, false);
            GrappleEnd = ToVec2(RopeDatas[RopeDatas.Count - 1].AnchorConnectionPoint);
            PreviousGrappleEnd = GrappleEnd;
            //Hook
            HookEnd = transform.position;
            PreviousHookEnd = HookEnd;
        }

        //Takes care of wrapping and unwrapping when the hook moves (The projectile that was shot out)
        UpdateHook();
        //Takes care of wrapping and unwrapping when the player moves
        UpdateGrapple();
        UnwrapCheck();
    }

    //Hook to player
    void UpdateHook()
    {
        UpdateLineRenderer();
        //Linecast from the Hook to the last valid hit point
        RaycastHit2D hit = Physics2D.Linecast(Hook.transform.position, HookEnd);

        Debug.DrawLine(Hook.transform.position, HookEnd, Color.red);
        Debug.DrawLine(Hook.transform.position, PreviousHookEnd, Color.blue);

        //If the grappling hook collides with any platforms create a new point (Wrapping)
        if (hit.collider != null)
        {
            if (hit.transform.gameObject.name == "Platform")
            {
                RopeDatas.Insert(1, new RopeData(OffsetedHitPoint(hit), hit.normal, hit.point, true));
                UpdateLineRenderer();
                PreviousHookEnd = HookEnd;
                HookEnd = OffsetedHitPoint(hit);
            }
        }
    }

    //Player to hook
    void UpdateGrapple()
    {
        UpdateLineRenderer();
        //Linecast from the player to the last valid hit point
        RaycastHit2D hit = Physics2D.Linecast(transform.position, GrappleEnd);

        Debug.DrawLine(transform.position, GrappleEnd, Color.red);
        Debug.DrawLine(transform.position, PreviousGrappleEnd, Color.blue);

        //If the grappling hook collides with any platforms create a new point (Wrapping)
        if (hit.collider != null)
        {
            if (hit.transform.gameObject.name == "Platform")
            {
                RopeDatas.Add(new RopeData(OffsetedHitPoint(hit), hit.normal, hit.point, false));
                UpdateLineRenderer();
                PreviousGrappleEnd = GrappleEnd;
                GrappleEnd = OffsetedHitPoint(hit);
            }
        }
    }

    //Unwraps the rope
    void UnwrapCheck()
    {
        //Don't ask me how it works, this gave me nightmares >:V
        if (RopeDatas.Count > 1)
        {
            //Grapple
            Vector2 grappleDirection = (PreviousGrappleEnd - GrappleEnd).normalized;
            Vector2 grappleplayerDirection = (new Vector2(transform.position.x, transform.position.y) - GrappleEnd).normalized;
            Vector2 grapplenormalDirection = ((GrappleEnd - RopeDatas[RopeDatas.Count - 1].Normal) - GrappleEnd).normalized;

            float grappleDot = Vector2.Dot(grappleplayerDirection, new Vector2(-grappleDirection.y, grappleDirection.x));
            float grappleDotNormal = Vector2.Dot(grapplenormalDirection, new Vector2(-grappleDirection.y, grappleDirection.x));

            //Hook
            Vector2 hookDirection = (PreviousHookEnd - HookEnd).normalized;
            Vector2 hookPlayerDirection = (new Vector2(Hook.transform.position.x, Hook.transform.position.y) - HookEnd).normalized;
            Vector2 hookNormalDirection = ((HookEnd - RopeDatas[1].Normal) - HookEnd).normalized;

            float hookDot = Vector2.Dot(hookPlayerDirection, new Vector2(-hookDirection.y, hookDirection.x));
            float hookDotNormal = Vector2.Dot(hookNormalDirection, new Vector2(-hookDirection.y, hookDirection.x));

            //Double Check
            RaycastHit2D hit = Physics2D.Linecast(transform.position, RopeDatas[RopeDatas.Count - 1].HitPoint);
            Debug.DrawLine(transform.position, RopeDatas[RopeDatas.Count - 1].AnchorConnectionPoint, Color.yellow);

            //Unwrap
            if (RopeDatas[RopeDatas.Count - 1].HookSide == false)
            {
                if ((grappleDotNormal > 0 && grappleDot < 0) || (grappleDotNormal < 0 && grappleDot > 0)) GrappleGoBackAStep();
            }
            else
            {
                //Reversed
                //if ((grappleDotNormal > 0 && grappleDot < 0) || (grappleDotNormal < 0 && grappleDot > 0))
                //{
                //    Debug.Log("WTF");
                //    GrappleGoBackAStep();
                //    //if(RopeDatas.Count == 2) HookGoBackAStep();
                //}
                Debug.Log("OHSHIT");
            }

            //Unwrap
            //if ((hookDotNormal > 0 && hookDot < 0) || (hookDotNormal < 0 && hookDot > 0))
            //{
            //    HookGoBackAStep();
            //    //if (RopeDatas.Count == 2) GrappleGoBackAStep();
            //}
            //if (RopeDatas.Count == 2 && hit.collider == null) HookGoBackAStep();
        }
    }

    //Reverting back a point from the player position (Unwrap)
    void GrappleGoBackAStep()
    {
        GrappleEnd = PreviousGrappleEnd;
        if (RopeDatas.Count > 2) PreviousGrappleEnd = RopeDatas[RopeDatas.Count - 3].AnchorConnectionPoint;
        else PreviousGrappleEnd = GrappleEnd;

        RopeDatas.RemoveAt(RopeDatas.Count - 1);
        UpdateLineRenderer();
    }

    //Reverting back a point from the hook position (Unwrap)
    void HookGoBackAStep()
    {
        HookEnd = PreviousHookEnd;
        if (RopeDatas.Count > 3) PreviousHookEnd = RopeDatas[3].AnchorConnectionPoint;
        else PreviousHookEnd = HookEnd;

        if (RopeDatas.Count > 1) RopeDatas.RemoveAt(1);
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
        TheLineRenderer.SetPosition(0, Hook.transform.position); //End
        for (int i = 1; i < RopeDatas.Count; ++i) TheLineRenderer.SetPosition(i, RopeDatas[i].AnchorConnectionPoint); //Applying all the AnchorConnectionPoints to the line renderer (should have one spot left)
        TheLineRenderer.SetPosition(RopeDatas.Count, transform.position); //End
    }

    Vector2 ToVec2(Vector3 vec2)
    {
        return new Vector2(vec2.x, vec2.y);
    }
}
