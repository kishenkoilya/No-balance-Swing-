using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour
{
    private GameObject[][] field;
    private Vector3[][] fieldCoordinates;
    [SerializeField] private Vector3 firstFieldSlot;
    [SerializeField] private float collumnsDistance;
    [SerializeField] private float rowsDistance;
    private int collumnsNumber = 8;
    private int rowsNumber = 10;

    private void Awake() 
    {
        fieldCoordinates = new Vector3[collumnsNumber][];
        field = new GameObject[collumnsNumber][];
        for (int i = 0; i < collumnsNumber; i++)
        {
            field[i] = new GameObject[rowsNumber];
            fieldCoordinates[i] = new Vector3[rowsNumber];
            for (int j = 0; j < rowsNumber; j++)
            {
                fieldCoordinates[i][j] = new Vector3(firstFieldSlot.x + collumnsDistance * i,
                                                    firstFieldSlot.y + rowsDistance * j,
                                                    firstFieldSlot.z);
            }
        }
    }

    public Vector3 GetFieldPositionCoordinates(int collumn, int row)
    {
        return fieldCoordinates[collumn][row];
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
