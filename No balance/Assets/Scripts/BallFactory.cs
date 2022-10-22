using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallFactory : MonoBehaviour
{
    [SerializeField] private Field field;
    [SerializeField] private Ball ballScript;
    [SerializeField] private List<Material> colors;
    private GameObject ballPrefab;
    private void Awake() {
        ballPrefab = ballScript.gameObject;
        if (field == null)
            field = FindObjectOfType<Field>();
    }

    public Ball SpawnBall(int colorsInUse, int maxWeight)
    {
        int colorIndex = Random.Range(0, colorsInUse);
        int weight = Random.Range(1, maxWeight);
        GameObject go = GameObject.Instantiate(ballPrefab, transform.position, transform.rotation);
        Ball ball = go.GetComponent<Ball>();
        ball.Initialize(weight, colorIndex, field);
        ball.collumn = -1; //means that it is not on field yet
        ball.row = -1;
        go.GetComponent<Renderer>().material = colors[colorIndex];
        return ball;
    }

    public Ball SpawnSpecificBall(int color, int weight)
    {
        GameObject go = GameObject.Instantiate(ballPrefab, transform.position, transform.rotation);
        Ball ball = go.GetComponent<Ball>();
        ball.Initialize(weight, color, field);
        ball.collumn = -1; //means that it is not on field yet
        ball.row = -1;
        go.GetComponent<Renderer>().material = colors[color];
        return ball;
    }
}
