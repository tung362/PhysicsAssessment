using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
    public string NextLevel;
    public GameObject TextInputObject1;
    public GameObject TextInputObject2;
    public GameObject TextInputObject3;
    public GameObject TextInputObject4;
    public GameObject TextInputObject5;

    public void Join()
    {
        TextInputObject1.GetComponent<AppearOnCall>().Called = true;
        TextInputObject2.GetComponent<AppearOnCall>().Called = true;
        TextInputObject3.GetComponent<AppearOnCall>().Called = true;
        TextInputObject4.GetComponent<AppearOnCall>().Called = true;
        TextInputObject5.GetComponent<AppearOnCall>().Called = true;
    }
    public void Host()
    {
        SceneManager.LoadScene(NextLevel);
    }
    public void Exit()
    {
        Application.Quit();
    }
}
