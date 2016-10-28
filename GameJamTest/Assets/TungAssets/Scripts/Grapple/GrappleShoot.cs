using UnityEngine;
using System.Collections;

public class GrappleShoot : MonoBehaviour
{
    //Temp
    public GameObject test;
    private DistanceJoint2D TheDistanceJoint;

	void Start ()
    {
        TheDistanceJoint = GetComponent<DistanceJoint2D>();
	}
	
	void FixedUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit mouseHit;

            if (Physics.Raycast(mouseRay, out mouseHit)) TheDistanceJoint.connectedAnchor = new Vector3(mouseHit.point.x, mouseHit.point.y, 0);
        }
    }
}
