using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingScript : MonoBehaviour
{

    [SerializeField] private Ball ball;
    [SerializeField] private Transform destination;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        ball.SetDestination(destination.position);
    }
}
