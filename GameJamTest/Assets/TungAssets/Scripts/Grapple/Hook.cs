using UnityEngine;
using System.Collections;

public class Hook : MonoBehaviour
{
    public bool HasHit = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter2D(Collision2D other)
    {
        //Stick to platforms
        if (other.transform.tag == "Platform")
        {
            transform.parent = other.transform;
            Destroy(transform.GetComponent<Rigidbody2D>());
            Destroy(transform.GetComponent<Collider2D>());
            HasHit = true;
        }
    }
}
