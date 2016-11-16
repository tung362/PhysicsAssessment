using UnityEngine;
using System.Collections;

public class GrappleShoot : MonoBehaviour
{
    //Temp
    public GameObject Hook;
    public float FireSpeed = 10;
    private DistanceJoint2D TheDistanceJoint;
    private GameObject trackedHook;

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

            if (Physics.Raycast(mouseRay, out mouseHit))
            {
                //If theres an existing hook destroy it and reset the grapple
                if (trackedHook != null)
                {
                    Destroy(trackedHook);
                    GetComponent<Grapple>().Reset();
                }
                //Creates the hook
                trackedHook = (GameObject)Instantiate(Hook, transform.position, Hook.transform.rotation);
                //Edit the hook
                Vector2 normalizedDirection = (mouseHit.point - transform.position).normalized;
                trackedHook.GetComponent<Rigidbody2D>().velocity = normalizedDirection * FireSpeed * Time.deltaTime;
                GetComponent<Grapple>().Hook = trackedHook;
            }
        }
    }
}
