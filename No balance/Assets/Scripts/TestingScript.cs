using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingScript : MonoBehaviour
{

    // [SerializeField] private Ball ball;
    // [SerializeField] private Transform destination;
    [SerializeField] BallFactory factory;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 50; i++)
        {
            GameObject go = factory.SpawnBall(25, 10);
            go.transform.position = new Vector3(i / 5 * 2, i % 5 * 2, -1);
        }
            
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
