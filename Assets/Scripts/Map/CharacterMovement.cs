using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterMovement : MonoBehaviour
{
    // Start is called before the first frame update
    public NavMeshAgent agent;
    void Start()
    {
        agent.destination = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Move(Vector3 dest)
    {
        agent.destination = dest;
    }
}
