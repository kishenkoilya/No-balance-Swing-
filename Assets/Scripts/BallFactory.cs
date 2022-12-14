using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallFactory : MonoBehaviour
{
    [SerializeField] private Field field;
    [SerializeField] private Ball ballScript;
    [SerializeField] private List<Material> colors;
    public MovingObject[] specialBallPrefabs;
    private GameObject ballPrefab;
    private void Awake() {
        ballPrefab = ballScript.gameObject;
    }

    public int GetSpecialBallCount() {return specialBallPrefabs.Length;}

    public MovingObject SpawnBall(int colorsInUse, int maxWeight)
    {
        int colorIndex = Random.Range(0, colorsInUse);
        int weight = Random.Range(1, maxWeight + 1);
        GameObject go = GameObject.Instantiate(ballPrefab, transform.position, transform.rotation);
        Ball ball = go.GetComponent<Ball>();
        ball.Initialize(weight, colorIndex, field);
        ball.collumn = -1; //means that it is not on field yet
        ball.row = -1;
        go.GetComponent<Renderer>().material = colors[colorIndex];
        MovingObject mo = go.GetComponent<MovingObject>();
        return mo;
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

    public MovingObject SpawnSpecialBall(int prefabIndex)
    {
        GameObject go = GameObject.Instantiate(specialBallPrefabs[prefabIndex].gameObject, transform.position, transform.rotation);
        MovingObject mo = go.GetComponent<MovingObject>();
        mo.Initialize(field);
        return mo;
    }
}
