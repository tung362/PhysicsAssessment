using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UICamShake : MonoBehaviour
{
    [Header("Default")]
    public bool IsTriggered = false;
    public Vector3 targetPosition;
    public float TravelSpeed = 8000;
    [Header("Screen Shake")]
    public bool CanShakeScreen = false;
    public float ShakeDuration = 0.3f;
    public Vector2 ShakePower = new Vector2(-2, 2);
    [Header("Chain Reaction")]
    //If you want another UI move to trigger
    public UICamShake[] TriggeredObjects;
    public float TriggerAtDistance = 0;

    private Vector3 PreviousPosition;

    private LevelTracker Tracker;
    private RectTransform TheRectTransform;

    void Start ()
    {
        Tracker = FindObjectOfType<LevelTracker>();
        TheRectTransform = GetComponent<RectTransform>();

        PreviousPosition = TheRectTransform.anchoredPosition3D;
    }
	
	void Update ()
    {
        if (IsTriggered)
        {
            TheRectTransform.anchoredPosition3D = Vector3.MoveTowards(TheRectTransform.anchoredPosition3D, targetPosition, TravelSpeed * Time.deltaTime);

            float distance = Vector3.Distance(TheRectTransform.anchoredPosition3D, targetPosition);
            if (distance <= TriggerAtDistance)
            {
                foreach (UICamShake ui in TriggeredObjects) ui.IsTriggered = true;
                if(CanShakeScreen)
                {
                    Tracker.CamShakeDuration = ShakeDuration;
                    Tracker.CamShakePower = ShakePower;
                    Tracker.StartCamShake = true;
                }
            }
        }
	}

    public void Reset()
    {
        IsTriggered = false;
        TheRectTransform.anchoredPosition3D = PreviousPosition;
    }

    //For Buttons to have access
    public void Triggered(bool isTriggered)
    {
        IsTriggered = isTriggered;
    }
}
