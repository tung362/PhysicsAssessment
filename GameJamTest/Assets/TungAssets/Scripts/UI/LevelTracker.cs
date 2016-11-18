using UnityEngine;
using System.Collections;

public class LevelTracker : MonoBehaviour
{
    //Cam Shake
    public bool StartCamShake = false;
    [HideInInspector]
    public float CamShakeTimer = 0;
    public float CamShakeDuration = 4;
    public Vector2 CamShakePower = new Vector2(-2, 2);
	
	void Update ()
    {
        //Cam Shake
        if (StartCamShake == true)
        {
            CamShakeTimer += Time.deltaTime;
            if (CamShakeTimer >= CamShakeDuration)
            {
                StartCamShake = false;
                CamShakeTimer = 0;
            }
        }
    }
}
