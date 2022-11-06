using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenScript : MonoBehaviour
{
    [SerializeField] private GameObject[] screenObjects;

    public void ActivateScreen() 
    {
        SetObjectsStatus(true);
    }
    public void DeactivateScreen() 
    {
        SetObjectsStatus(false);
    }

    private void SetObjectsStatus(bool status) 
    {
        foreach (GameObject obj in screenObjects) 
        {
            obj.SetActive(status);
        }
    }

    public virtual void ExitGame()
    {
        if (UnityEditor.EditorApplication.isPlaying)
            UnityEditor.EditorApplication.isPlaying = false;
        else
            Application.Quit();
    }
}
