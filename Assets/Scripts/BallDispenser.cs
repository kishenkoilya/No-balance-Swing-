using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallDispenser : MonoBehaviour
{
    [SerializeField] private BallFactory factory;
    private MovingObject[][] balls;
    private Vector3[][] ballsCoordinates;
    private bool coordinatesSet = false;
    [SerializeField] private Vector3 firstBallCoordinates;
    [SerializeField] private float specialBallSpawnChance = 0.1f;
    private int collumnsNumber = 8;
    private int rowsNumber = 2;
    private int currentLevel = 4;
    void Start()
    {
        ballsCoordinates = new Vector3[collumnsNumber][];
        collumnsNumber = transform.parent.GetComponent<Field>().collumnsNumber;
        balls = new MovingObject[collumnsNumber][];
        for (int i = 0; i < collumnsNumber; i++)
        {
            balls[i] = new MovingObject[rowsNumber];
        }
    }

    public void FillDispencer()
    {
        ClearDispencer();
        if (!coordinatesSet)
            SetBallsCoordinates();
        for (int i = 0; i < collumnsNumber; i++)
        {
            for (int j = 0; j < rowsNumber; j++)
            {
                int decision = DecideWhichBallToSpawn();        
                if (decision == -1)
                    balls[i][j] = factory.SpawnBall(currentLevel, currentLevel + 1);
                else
                    balls[i][j] = factory.SpawnSpecialBall(decision);
                balls[i][j].transform.position = ballsCoordinates[i][j];
                balls[i][j].transform.parent = transform;
            }
        }
    }

    private void ClearDispencer()
    {
        for (int i = 0; i < collumnsNumber; i++)
        {
            for (int j = 0; j < rowsNumber; j++)
            {
                if (balls[i][j])
                {
                    GameObject.Destroy(balls[i][j].gameObject);
                    balls[i][j] = null;
                }
            }
        }    
    }

    private void SetBallsCoordinates()
    {
        float collumnsDistance = transform.parent.GetComponent<Field>().collumnsDistance;
        float rowsDistance = transform.parent.GetComponent<Field>().rowsDistance;
        for (int i = 0; i < collumnsNumber; i++)
        {
            ballsCoordinates[i] = new Vector3[rowsNumber];
            for (int j = 0; j < rowsNumber; j++)
            {
                ballsCoordinates[i][j] = new Vector3(firstBallCoordinates.x + collumnsDistance * i,
                                                    firstBallCoordinates.y + rowsDistance * j,
                                                    firstBallCoordinates.z);
            }
        }
        coordinatesSet = true;
    }

    public MovingObject DispenceBall(int collumnIndex)
    {
        MovingObject dispencedBall = balls[collumnIndex][0];
        balls[collumnIndex][0] = balls[collumnIndex][1];
        balls[collumnIndex][0].transform.position = ballsCoordinates[collumnIndex][0];

        int decision = DecideWhichBallToSpawn();        
        if (decision == -1)
            balls[collumnIndex][1] = factory.SpawnBall(currentLevel, currentLevel + 1);
        else
            balls[collumnIndex][1] = factory.SpawnSpecialBall(decision);
        
        balls[collumnIndex][1].transform.position = ballsCoordinates[collumnIndex][1];
        balls[collumnIndex][1].transform.parent = transform;
        return dispencedBall;
    }

    private int DecideWhichBallToSpawn()
    {
        int index = -1;
        for (int i = factory.GetSpecialBallCount() - 1; i >= 0; i--)
        {
            if (Random.Range(0f, 1f) < specialBallSpawnChance)
                index = i;
        }
        return index;
    }
}
