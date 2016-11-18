using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UICamShake : MonoBehaviour
{
    public Vector3 targetPosition;
    public float TravelSpeed = 0.3f;
    private LevelTracker Tracker;
    private RectTransform TheRectTransform;


    void Start ()
    {
        Tracker = FindObjectOfType<LevelTracker>();
        TheRectTransform = GetComponent<RectTransform>();
    }
	
	void Update ()
    {
        TheRectTransform.anchoredPosition3D = Vector3.Lerp(TheRectTransform.anchoredPosition3D, targetPosition, TravelSpeed * Time.deltaTime);
        Debug.Log(Vector3.Distance(targetPosition, TheRectTransform.anchoredPosition3D));
	}
}
