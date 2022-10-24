using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldSlotsSimulator : MonoBehaviour
{
    public Field f;
    public BallFactory factory;
    public MovingObject[][] field;
    public GameObject cube;
    public int colCount;
    public int rowCount;
    public float colDist;
    public float rowDist;

    private void Start() {
        field = f.field;
    }
    private void Update() {
            field = f.field;
            RespawnField();
    }

    private void RespawnField()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            GameObject.Destroy(transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < colCount; i++)
        {
            for (int j = 0; j < rowCount; j++)
            {
                Vector3 spawnPosition = transform.position + new Vector3 (i * colDist, j * rowDist, 0);
                if (field[i][j] == null)
                    continue;
                if (field[i][j].GetComponent<ScalesCup>())
                {
                    GameObject.Instantiate(cube, spawnPosition, transform.rotation, transform);
                }
                Ball b = field[i][j].GetComponent<Ball>();
                if (b)
                {
                    Ball nb = factory.SpawnSpecificBall(b.colorIndex, b.GetWeight());
                    nb.transform.position = spawnPosition;
                    nb.transform.parent = transform;
                }
            }
        }
    }
}
