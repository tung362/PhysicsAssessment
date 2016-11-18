using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    //Smooth damp
    public float FollowSmoothness = 0.3f;
    private Vector3 Velo = Vector3.zero;

    //Start
    private Vector3 StartingPosition;

    private LevelTracker Tracker;

    void Start()
    {
        Tracker = FindObjectOfType<LevelTracker>();
        StartingPosition = transform.position;
    }

    void Update ()
    {
        //Camera Shake
        if (Tracker.StartCamShake == true)
        {
            transform.position = Vector3.SmoothDamp(transform.position, StartingPosition, ref Velo, FollowSmoothness);
            transform.position = new Vector3(transform.position.x + Random.Range(Tracker.CamShakePower.x, Tracker.CamShakePower.y), transform.position.y + Random.Range(Tracker.CamShakePower.x, Tracker.CamShakePower.y), transform.position.z + Random.Range(Tracker.CamShakePower.x, Tracker.CamShakePower.y));
        }
        else transform.position = Vector3.SmoothDamp(transform.position, StartingPosition, ref Velo, FollowSmoothness);
    }
}
