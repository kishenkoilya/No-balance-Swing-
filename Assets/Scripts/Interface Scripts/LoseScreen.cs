using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoseScreen : ScreenScript
{
    [SerializeField] private TextMeshProUGUI levelNumber;
    [SerializeField] private TextMeshProUGUI timeNumber;
    [SerializeField] private TextMeshProUGUI scoreNumber;
    [SerializeField] private TextMeshProUGUI ballsDroppedNumber;
    public void SetGameData(GameScreen.GameData gameData)
    {
        string timeText = "";
        timeText += gameData.Hours > 0 ? (gameData.Hours + ":") : "";
        timeText += gameData.Minutes > 9 ? (gameData.Minutes + ":") : ("0" + gameData.Minutes + ":");
        timeText += gameData.Seconds > 9 ? ("" + gameData.Seconds) : ("0" + gameData.Seconds); 
        levelNumber.SetText("" + gameData.Level);
        timeNumber.SetText("" + timeText);
        scoreNumber.SetText("" + gameData.Score);
        ballsDroppedNumber.SetText("" + gameData.BallsDropped);
    }
}
