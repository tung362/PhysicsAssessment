using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
    public void AppearOnClick(int ID)
    {
        AppearOnCall[] AppearOnCalls = FindObjectsOfType(typeof(AppearOnCall)) as AppearOnCall[];
        foreach(AppearOnCall objectsToAppear in AppearOnCalls)
        {
            if (objectsToAppear.ID == ID) objectsToAppear.Called = true;
        }
    }

    public void LoadLevel(string NextLevel)
    {
        SceneManager.LoadScene(NextLevel);
    }
    public void Exit()
    {
        Application.Quit();
    }
}
