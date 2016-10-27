using UnityEngine;
using System.Collections;

public class GrappleShoot : MonoBehaviour
{
    public GameObject test;

	void Start ()
    {
	
	}
	
	void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit mouseHit;

            if (Physics.Raycast(mouseRay, out mouseHit))
            {
                test.transform.position = new Vector3(mouseHit.point.x, mouseHit.point.y, 0);
                //mouseHit.collider.GetComponent<Renderer>().material.color = Color.red;
                //Debug.Log(mouseHit.transform.gameObject.name);
            }
        }
    }
}
