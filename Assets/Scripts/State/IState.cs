using UnityEngine;

public interface IState 
{
    void OnEnter();
    void OnExit();
    void FixedUpdate();
    void Update();
    void TurnAction();
}