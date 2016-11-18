using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

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

    public void DisableAppearOnClick(int ID)
    {
        AppearOnCall[] AppearOnCalls = FindObjectsOfType(typeof(AppearOnCall)) as AppearOnCall[];
        foreach (AppearOnCall objectsToAppear in AppearOnCalls)
        {
            if (objectsToAppear.ID == ID) objectsToAppear.Called = false;
        }
    }

    public void DisableAllAppearOnClick()
    {
        AppearOnCall[] AppearOnCalls = FindObjectsOfType(typeof(AppearOnCall)) as AppearOnCall[];
        foreach (AppearOnCall objectsToAppear in AppearOnCalls) objectsToAppear.Called = false;
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
