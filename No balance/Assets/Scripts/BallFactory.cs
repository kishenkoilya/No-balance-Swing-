using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallFactory : MonoBehaviour
{
    [SerializeField] private Ball ballScript;
    [SerializeField] private List<Material> colors;
    private GameObject ballPrefab;
    private void Awake() {
        ballPrefab = ballScript.gameObject;
    }

    public Ball SpawnBall(int colorsInUse, int maxWeight)
    {
        int colorIndex = Random.Range(0, colorsInUse);
        int weight = Random.Range(1, maxWeight);
        GameObject go = GameObject.Instantiate(ballPrefab, transform.position, transform.rotation);
        Ball ball = go.GetComponent<Ball>();
        ball.SetWeightAndColor(weight, colorIndex);
        go.GetComponent<Renderer>().material = colors[colorIndex];
        return ball;
    }
}
