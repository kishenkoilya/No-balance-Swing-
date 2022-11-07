using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreBonusText : MonoBehaviour
{
    [SerializeField] private TextMeshPro bonusText;
    [SerializeField] private float timeShown = 2;
    private float timerOfShowingBonus = 0;
    public bool occupied = false;
    private void Start() 
    {
        bonusText.SetText("");
    }

    private void Update() 
    {
        if (timerOfShowingBonus > 0)
        {
            if (timerOfShowingBonus > timeShown / 2)
            {
                bonusText.alpha += Time.deltaTime / (timeShown / 2);
            }
            else
            {
                bonusText.alpha -= Time.deltaTime / (timeShown / 2);
            }
            transform.position += Vector3.up * Time.deltaTime;
            timerOfShowingBonus -= Time.deltaTime;
            if (timerOfShowingBonus <= 0)
            {
                occupied = false;
                bonusText.alpha = 0;
            }
        }
    }

    public void ShowBonus(int bonusScore, Vector3 position)
    {
        if (bonusScore == 0)
            return;
        transform.position = position;
        timerOfShowingBonus = timeShown;
        bonusText.SetText("+" + bonusScore);
        occupied = true;
    }
}
