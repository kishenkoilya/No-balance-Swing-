using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallDispencer : MonoBehaviour
{
    [SerializeField] private BallFactory factory;
    private Ball[][] balls;
    private Vector3[][] ballsCoordinates;
    [SerializeField] private Vector3 firstBallCoordinates;
    private int collumnsNumber = 8;
    private int rowsNumber = 2;
    private int currentLevel = 4;
    void Start()
    {
        balls = new Ball[collumnsNumber][];
        ballsCoordinates = new Vector3[collumnsNumber][];
        float collumnsDistance = transform.parent.GetComponent<Field>().GetCollumnsDistance();
        float rowsDistance = transform.parent.GetComponent<Field>().GetRowsDistance();
        collumnsNumber = transform.parent.GetComponent<Field>().GetCollumnsNumber();
        for (int i = 0; i < collumnsNumber; i++)
        {
            balls[i] = new Ball[rowsNumber];
            ballsCoordinates[i] = new Vector3[rowsNumber];
            for (int j = 0; j < rowsNumber; j++)
            {
                ballsCoordinates[i][j] = new Vector3(firstBallCoordinates.x + collumnsDistance * i,
                                                    firstBallCoordinates.y + rowsDistance * j,
                                                    firstBallCoordinates.z);
                balls[i][j] = factory.SpawnBall(currentLevel, currentLevel + 1);
                balls[i][j].transform.position = ballsCoordinates[i][j];
            }
        }
    }

    public Ball DispenceBall(int collumnIndex)
    {
        Ball dispencedBall = balls[collumnIndex][0];
        balls[collumnIndex][0] = balls[collumnIndex][1];
        balls[collumnIndex][0].transform.position = ballsCoordinates[collumnIndex][0];
        balls[collumnIndex][1] = factory.SpawnBall(currentLevel, currentLevel + 1);
        balls[collumnIndex][1].transform.position = ballsCoordinates[collumnIndex][1];
        return dispencedBall;
    }
}
