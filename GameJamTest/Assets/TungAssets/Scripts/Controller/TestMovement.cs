using UnityEngine;
using System.Collections;

public class TestMovement : MonoBehaviour
{
    public float Speed = 10;
    public float GrappleSpeed = 0.5f;

    private Rigidbody2D TheRigidbody;
    private DistanceJoint2D TheDistanceJoint;

    void Start ()
    {
        TheRigidbody = GetComponent<Rigidbody2D>();
        TheDistanceJoint = GetComponent<DistanceJoint2D>();
    }
	
	void FixedUpdate()
    {
        //Apply Movement
        Vector3 movement = new Vector2(0, 0);
        if (Input.GetKey(KeyCode.A)) movement.x = -1;
        if (Input.GetKey(KeyCode.D)) movement.x = 1;
        TheRigidbody.velocity = new Vector2(TheRigidbody.velocity.x + (movement.x * Speed * Time.deltaTime), TheRigidbody.velocity.y + (movement.y * Speed * Time.fixedDeltaTime));

        //Apply Grapple
        if (Input.GetKey(KeyCode.Space)) TheDistanceJoint.distance += GrappleSpeed * Time.fixedDeltaTime;
        if (Input.GetKey(KeyCode.LeftShift)) TheDistanceJoint.distance -= GrappleSpeed * Time.fixedDeltaTime;
    }
}
