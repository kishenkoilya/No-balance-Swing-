using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCounter : MonoBehaviour
{
    [SerializeField] private int additionToWeight;
    [SerializeField] private float scoreMultiplier = 1;
    [SerializeField] private GameObject scoreBonusPrefab;
    [SerializeField] private int scoreBonusesCount = 5;
    private List<ScoreBonusText> scoreBonuses = new List<ScoreBonusText>();
    public delegate void ScoreAdded(int score);
    public ScoreAdded scoreAdder;

    private void Start() 
    {
        for (int i = 0; i < scoreBonusesCount; i++)
        {
            GameObject go = GameObject.Instantiate(scoreBonusPrefab, Vector3.zero, transform.rotation, transform);
            scoreBonuses.Add(go.GetComponent<ScoreBonusText>());
        }
    }
    public void CountScoreBonus(List<MovingObject> Objects)
    {
        bool theyBurn = true;
        foreach (MovingObject mo in Objects)
        {
            if (!mo.isBurning)
            {
                theyBurn = false;
                return;
            }
        }

        int score = 0;
        int weightSum = 0;
        if (theyBurn)
        {
            foreach (MovingObject mo in Objects)
            {
                weightSum += additionToWeight + mo.GetWeight();
            }
            score = (int)(weightSum * Objects.Count * scoreMultiplier);
        }

        bool bonusWasDisplayed = false;
        foreach (ScoreBonusText bonusText in scoreBonuses)
        {
            if (bonusText.occupied)
                continue;
            bonusText.ShowBonus(score, Objects[0].transform.position + Vector3.up * 1 + Vector3.back * 2);
            bonusWasDisplayed = true;
            break;
        }
        if (!bonusWasDisplayed)
            Debug.Log("Need more score bonuses!!");

        scoreAdder?.Invoke(score);
    }
}
