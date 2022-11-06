using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScreen : ScreenScript
{
    [SerializeField] private BallDispenser ballDispenser;
    [SerializeField] private Manipulator manipulator;

    private void Awake() 
    {
        if (ballDispenser == null)
            ballDispenser = GameObject.FindObjectOfType<BallDispenser>();
        if (manipulator == null)
            manipulator = GameObject.FindObjectOfType<Manipulator>();
    }

    public void StartGame()
    {
        ballDispenser.FillDispencer();
        manipulator.ActivateManipulator();
    }
}
