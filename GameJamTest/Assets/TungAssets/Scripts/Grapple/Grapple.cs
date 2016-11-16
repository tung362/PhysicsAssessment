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
    //Used for unwrapping the first point
    private Vector2 HookPreviousNormal;

    //Tracked Grapple
    private Vector2 GrappleEnd;
    private Vector2 PreviousGrappleEnd;
    //Used for unwrapping the first point
    private Vector2 GrapplePreviousNormal;

    //Components
    private LineRenderer TheLineRenderer;
    private DistanceJoint2D TheDistanceJoint;

    void Start ()
    {
        //Assigning components
        TheLineRenderer = GetComponent<LineRenderer>();
        TheDistanceJoint = GetComponent<DistanceJoint2D>();

        //Temporary//
        RopeDatas.Add(new RopeData(Vector3.zero, Vector2.zero, false)); //Start
    }

    void Update ()
    {
        //If there is an actual hook
        if(Hook != null)
        {
            //Enable DistanceJoint2D
            if (TheDistanceJoint.enabled == false) TheDistanceJoint.enabled = true;

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
        else
        {
            //Disable DistanceJoint2D when not being used
            if (TheDistanceJoint.enabled == true) TheDistanceJoint.enabled = false;
        }
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
            if (hit.transform.tag == "Platform")
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
            if (hit.transform.tag == "Platform")
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
        //Compares the angle between the grapple direction with the player direction and the normal determines which side of the 180 to unwrap
        if (RopeDatas.Count > 1)
        {
            //Grapple
            Vector2 grappleDirection = (PreviousGrappleEnd - GrappleEnd).normalized;
            Vector2 grapplePlayerDirection = (new Vector2(transform.position.x, transform.position.y) - GrappleEnd).normalized;
            Vector2 grappleNormalDirection = ((GrappleEnd - RopeDatas[RopeDatas.Count - 1].Normal) - GrappleEnd).normalized;

            float grappleDot = Vector2.Dot(grapplePlayerDirection, new Vector2(-grappleDirection.y, grappleDirection.x));
            float grappleDotNormal = Vector2.Dot(grappleNormalDirection, new Vector2(-grappleDirection.y, grappleDirection.x));
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
            //If the grapple is unwrapping on it's side instead of the hook's side
            if (RopeDatas[RopeDatas.Count - 1].HookSide == false)
            {
                //Regular
                if ((grappleDotNormal > 0 && grappleDot < 0) || (grappleDotNormal < 0 && grappleDot > 0)) GrappleGoBackAStep();
            }
            else
            {
                //If its the start of the wrap, handled differently because the grapple creates and point on both the grapple and hook side
                if(RopeDatas.Count == 2)
                {
                    //Updates the old data
                    grappleDirection = (ToVec2(Hook.transform.position) - HookEnd).normalized;
                    grapplePlayerDirection = (ToVec2(transform.position) - HookEnd).normalized;
                    grappleNormalDirection = ((HookEnd - GrapplePreviousNormal) - HookEnd).normalized;

                    grappleDot = Vector2.Dot(grapplePlayerDirection, new Vector2(-grappleDirection.y, grappleDirection.x));
                    grappleDotNormal = Vector2.Dot(grappleNormalDirection, new Vector2(-grappleDirection.y, grappleDirection.x));

                    if ((grappleDotNormal > 0 && grappleDot < 0) || (grappleDotNormal < 0 && grappleDot > 0)) GrappleGoBackAStep();
                }
                //After the first wrap
                else
                {
                    //Updates the old data
                    grappleDirection = (ToVec2(RopeDatas[RopeDatas.Count - 2].AnchorConnectionPoint) - GrappleEnd).normalized;

                    if ((grappleDotNormal > 0 && grappleDot < 0) || (grappleDotNormal < 0 && grappleDot > 0)) GrappleGoBackAStep();
                }
            }

            //Double checks to make sure theres still more than 1 rope count because the values might of changed after the first unwrap
            if(RopeDatas.Count > 1)
            {
                //If the hook is unwrapping on it's side instead of the grapple's side
                if (RopeDatas[1].HookSide == true)
                {
                    //Regular
                    if ((hookDotNormal > 0 && hookDot < 0) || (hookDotNormal < 0 && hookDot > 0)) HookGoBackAStep();
                }
                else
                {
                    //If its the start of the wrap, handled differently because the grapple creates and point on both the grapple and hook side
                    if (RopeDatas.Count == 2)
                    {
                        //Updates the old data
                        hookDirection = (ToVec2(transform.position) - GrappleEnd).normalized;
                        hookPlayerDirection = (ToVec2(Hook.transform.position) - GrappleEnd).normalized;
                        hookNormalDirection = ((GrappleEnd - HookPreviousNormal) - GrappleEnd).normalized;

                        hookDot = Vector2.Dot(hookPlayerDirection, new Vector2(-hookDirection.y, hookDirection.x));
                        hookDotNormal = Vector2.Dot(hookNormalDirection, new Vector2(-hookDirection.y, hookDirection.x));

                        if ((hookDotNormal > 0 && hookDot < 0) || (hookDotNormal < 0 && hookDot > 0)) HookGoBackAStep();
                    }
                    //After the first wrap
                    else
                    {
                        //Updates the old data
                        hookDirection = (ToVec2(RopeDatas[2].AnchorConnectionPoint) - HookEnd).normalized;

                        if ((hookDotNormal > 0 && hookDot < 0) || (hookDotNormal < 0 && hookDot > 0)) HookGoBackAStep();
                    }
                }
            }
        }
    }

    //Reverting back a point from the player position (Unwrap)
    void GrappleGoBackAStep()
    {
        GrapplePreviousNormal = RopeDatas[RopeDatas.Count - 1].Normal;

        GrappleEnd = PreviousGrappleEnd;
        if (RopeDatas.Count > 2) PreviousGrappleEnd = RopeDatas[RopeDatas.Count - 3].AnchorConnectionPoint;
        else PreviousGrappleEnd = GrappleEnd;

        RopeDatas.RemoveAt(RopeDatas.Count - 1);
        UpdateLineRenderer();
    }

    //Reverting back a point from the hook position (Unwrap)
    void HookGoBackAStep()
    {
        HookPreviousNormal = RopeDatas[1].Normal;

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

        //Anchor point always at last point
        TheDistanceJoint.connectedAnchor = RopeDatas[RopeDatas.Count - 1].AnchorConnectionPoint;
    }

    //Call on this function when creating a new grapple hook, resets everything
    public void Reset()
    {
        Hook = null;
        RopeDatas.Clear();
        RopeDatas.Add(new RopeData(Vector3.zero, Vector2.zero, false)); //Start
    }

    Vector2 ToVec2(Vector3 vec2)
    {
        return new Vector2(vec2.x, vec2.y);
    }
}
