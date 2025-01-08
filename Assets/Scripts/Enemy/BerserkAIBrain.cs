using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class BerserkAIBrain : AIBrain 
{
	private void Start()
	{
		mColor = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

		playerDetector = GetComponentInChildren<PlayerDetector>();
		pathFinding = new PathFinding();
		stateMachine = new StateMachine();
        
		var wanderState = new GridEnemyWander(this, null, 10, pathFinding);
		var chaseState = new GridEnemyChase(this, null, pathFinding);
		var attackState = new GridEnemyAttack(this, null);
		#region Set up state transition
		stateMachine.AddTransition(
			chaseState, wanderState, new FuncPredicate(
				() => !playerDetector.CanDetectPlayer(out playerGrid) && chaseState.HasReachedDestination()
			)
		);
		stateMachine.AddTransition(
			wanderState, chaseState, new FuncPredicate(
				() => playerDetector.CanDetectPlayer(out playerGrid)
			)
		);

		stateMachine.AddTransition(
			chaseState, attackState, new FuncPredicate(
				() =>
					playerDetector.CanDetectPlayer(out playerGrid) &&
					IsPlayerInAttackRange(enemyConfig.AttackRangeInCell) &&
					attackDur <= 0
			)
		);

		stateMachine.AddTransition(
			attackState, chaseState, new FuncPredicate(
				() => !isAttacking
			)
		);
		#endregion
		stateMachine.SetState(wanderState);

		InitializeAttackStrategy();
	}

	void Update()
	{
		stateMachine.Update();
	}
}