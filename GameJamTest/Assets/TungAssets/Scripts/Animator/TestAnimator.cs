using UnityEngine;
using System.Collections;

public class TestAnimator : MonoBehaviour
{
    private Animator TheAnimator;
	void Start ()
    {
        TheAnimator = GetComponent<Animator>();
    }

	void Update ()
    {
        if (Input.GetKeyDown("p"))
        {
            TheAnimator.SetBool("IsBarking", true);
            TheAnimator.SetBool("BarkingToggle", true);
        }

    }
}
