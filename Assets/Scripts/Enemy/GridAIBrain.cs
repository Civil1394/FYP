using UnityEngine;
using System.Collections;

public class GridAIBrain : MonoBehaviour 
{
    StateMachine stateMachine;
    private void Start()
    {
        stateMachine = new StateMachine();
    }
    private void Update()
    {
        stateMachine.Update();
    }
    private void OnTurnStart()
    {
        stateMachine.OnTurnStart();
    }
}