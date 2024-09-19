using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField]
    private List<Transform> waypoints;
    private State currentState;
    
    // Start is called before the first frame update
    void Start()
    {
        currentState = this.GetComponent<EnemyState>().state;
    }

    // Update is called once per frame
    void Update()
    {
        switch(currentState) {

        }
    }
}
