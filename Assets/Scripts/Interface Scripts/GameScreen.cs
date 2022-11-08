using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameScreen : ScreenScript
{
    public struct GameData
    {
        public int Hours;
        public int Minutes;
        public int Seconds;
        public int Score;
        public int BallsDropped;
        public int Level;
    }
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
    private int level = 1;
    private void Awake() 
    {
        manipulator.ballThrown += IncreaseBallsDroppedCount;
    }

    private void Start()
    {
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
        (int Hours, int Minutes, int Seconds) t = SplitTimeElapsed(timeElapsed);
        string timeText = "";
        timeText += t.Hours > 0 ? (t.Hours + ":") : "";
        timeText += t.Minutes > 9 ? (t.Minutes + ":") : ("0" + t.Minutes + ":");
        timeText += t.Seconds > 9 ? ("" + t.Seconds) : ("0" + t.Seconds); 
        timerText.SetText(timeText);
    }

    private (int Hours, int Minutes, int Seconds) SplitTimeElapsed(float time)
    {
        (int Hours, int Minutes, int Seconds) TIME;
        TIME.Hours = (int)(time / 3600);
        time %= 3600;
        TIME.Minutes = (int)(time / 60);
        time %= 60;
        TIME.Seconds = (int)time;
        return TIME;    
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
        timeElapsed = 0;
        score = 0;
        ballsDropped = 0;
        level = 1;
        levelText.SetText("Level: " + level);
        TimerDisplay();
        ballsDroppedText.SetText("0");
        scoreText.SetText("0");
        ballDispenser.FillDispencer();
        manipulator.ActivateManipulator();
        gameInProgress = true;
    }

    public GameData GameLost()
    {
        gameInProgress = false;

        (int Hours, int Minutes, int Seconds) t = SplitTimeElapsed(timeElapsed);
        return new GameData{Hours = t.Hours, Minutes = t.Minutes, Seconds = t.Seconds, Score = score, BallsDropped = ballsDropped, Level = level};
    }
}
