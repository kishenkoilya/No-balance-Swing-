using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncomingBalls : MonoBehaviour
{
    [SerializeField] private BallFactory factory;
    private Ball[][] balls;
    private Vector3[][] ballsCoordinates;
    [SerializeField] private Vector3 firstBallCoordinates;
    [SerializeField] private float collumnsDistance;
    [SerializeField] private float rowsDistance;
    private int collumnsNumber = 8;
    private int rowsNumber = 2;
    private int startingLevel = 4;
    private void Awake() {

    }
    void Start()
    {
        balls = new Ball[collumnsNumber][];
        ballsCoordinates = new Vector3[collumnsNumber][];
        for (int i = 0; i < collumnsNumber; i++)
        {
            balls[i] = new Ball[rowsNumber];
            ballsCoordinates[i] = new Vector3[rowsNumber];
            for (int j = 0; j < rowsNumber; j++)
            {
                ballsCoordinates[i][j] = new Vector3(firstBallCoordinates.x + collumnsDistance * i,
                                                    firstBallCoordinates.y + rowsDistance * j,
                                                    firstBallCoordinates.z);
                balls[i][j] = factory.SpawnBall(startingLevel, startingLevel + 1);
                balls[i][j].transform.position = ballsCoordinates[i][j];
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
