using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScreen : ScreenScript
{
    [SerializeField] private Camera mainCamera;

    public void StartGame()
    {
        mainCamera.transform.position = new Vector3(13.75f,20.5f,-15.5f); //position where game field is seen
    }
}
