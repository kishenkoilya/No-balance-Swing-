using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameScreen : ScreenScript
{
    [SerializeField] private BallDispenser ballDispenser;
    [SerializeField] private Manipulator manipulator;
    [SerializeField] private ScoreCounter scoreCounter;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI ballsDroppedText;
    [SerializeField] private TextMeshProUGUI scoreText;
    private bool gameInProgress = false;
    private float timeElapsed = 0;
    private int ballsDropped = 0;
    private int score = 0;
    private void Awake() 
    {
        if (ballDispenser == null)
            ballDispenser = GameObject.FindObjectOfType<BallDispenser>();
        if (manipulator == null)
            manipulator = GameObject.FindObjectOfType<Manipulator>();
        manipulator.ballThrown += IncreaseBallsDroppedCount;
    }

    private void Start()
    {
        levelText.SetText("Level: " + 1);
        timerText.SetText("0:00");
        ballsDroppedText.SetText("0");
        scoreText.SetText("0");
        scoreCounter.scoreAdder += AddScore;
    }

    private void Update() 
    {
        if (gameInProgress)
        {
            timeElapsed += Time.deltaTime;
            TimerDisplay();
        }
    }

    private void TimerDisplay()
    {
        int hours, minutes, seconds;
        float time = timeElapsed;
        hours = (int)(time / 3600);
        time %= 3600;
        minutes = (int)(time / 60);
        time %= 60;
        seconds = (int)time;
        string timeText = "";
        timeText += hours > 0 ? (hours + ":") : "";
        timeText += minutes > 9 ? (minutes + ":") : ("0" + minutes + ":");
        timeText += seconds > 9 ? ("" + seconds) : ("0" + seconds); 
        timerText.SetText(timeText);
    }

    private void IncreaseBallsDroppedCount(object sender, EventArgs args)
    {
        ballsDropped++;
        ballsDroppedText.SetText("" + ballsDropped);
    }

    private void AddScore(int S)
    {
        score += S;
        scoreText.SetText("" + score);
    }

    public void StartGame()
    {
        ballDispenser.FillDispencer();
        manipulator.ActivateManipulator();
        gameInProgress = true;
    }
}
