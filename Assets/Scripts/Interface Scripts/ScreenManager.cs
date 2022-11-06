using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    [SerializeField] private MainScreen mainScreen;
    [SerializeField] private GameScreen gameScreen;
    [SerializeField] private LoseScreen loseScreen;
    private ScreenScript[] screens;

    private void Awake() 
    {
        if (mainScreen == null)
            mainScreen = GameObject.FindObjectOfType<MainScreen>();    
        if (gameScreen == null)
            gameScreen = GameObject.FindObjectOfType<GameScreen>();    
        if (loseScreen == null)
            loseScreen = GameObject.FindObjectOfType<LoseScreen>(); 
        screens = new ScreenScript[3];
        screens[0] = mainScreen;
        screens[1] = gameScreen;
        screens[2] = loseScreen;   
    }

    private void Start() 
    {
        ActivateScreen(mainScreen);
    }

    public void StartGame()
    {
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
}
