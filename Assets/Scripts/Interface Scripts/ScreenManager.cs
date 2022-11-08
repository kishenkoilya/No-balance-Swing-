using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    [SerializeField] private Field field;
    [SerializeField] private BallDispenser ballDispenser;
    [SerializeField] private MainScreen mainScreen;
    [SerializeField] private GameScreen gameScreen;
    [SerializeField] private LoseScreen loseScreen;
    private ScreenScript[] screens;

    private void Awake() 
    {
        screens = new ScreenScript[3];
        screens[0] = mainScreen;
        screens[1] = gameScreen;
        screens[2] = loseScreen;   
    }

    private void Start() 
    {
        ActivateScreen(mainScreen);
        field.gameLostEvent += GameLost;
    }

    public void StartGame()
    {
        field.ClearField();
        mainScreen.StartGame();
        ActivateScreen(gameScreen);
        gameScreen.StartGame();
    }

    private void ActivateScreen(ScreenScript screen)
    {
        for (int i = 0; i < screens.Length; i++)
            screens[i].DeactivateScreen();
        screen.ActivateScreen();
    }

    private void GameLost(object sender, EventArgs args)
    {
        GameScreen.GameData gameData = gameScreen.GameLost();
        loseScreen.SetGameData(gameData);
        ActivateScreen(loseScreen);
    }
}
