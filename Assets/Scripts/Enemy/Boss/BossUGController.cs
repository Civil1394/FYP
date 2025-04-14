using System;
using UnityEngine;
using System.Collections;

public class BossUGController : AIBrain
{
	private BossUGExplosiveShootState explosiveShootState;
	private BossUGGlacialSpreadState glacialSpreadState;
	private BossUGCanonBallState canonBallState;
	private GridEnemyChase chaseState;
	
	public AbilityData explosiveShoot;
	public AbilityData glacialSpread;
	public AbilityData canonBall;

	public int abilityIdx = 2;

	public override void StateInitialization()
	{
		chaseState = new GridEnemyChase(this, null, pathFinding);
		glacialSpreadState = new BossUGGlacialSpreadState(this,null,glacialSpread);
		explosiveShootState = new BossUGExplosiveShootState(this,null,explosiveShoot, 10);
		canonBallState = new BossUGCanonBallState(this,null,canonBall,3);
		stateMachine.AddTransition(chaseState, canonBallState, new FuncPredicate(
			() => IsPlayerInAttackRange(4) &&//enter canon ball attack range
			abilityIdx == 0
			)
		);
		stateMachine.AddTransition(canonBallState, chaseState, new FuncPredicate(
				() => canonBallState.isTurnComplete
			)
		);
		
		stateMachine.AddTransition(chaseState, glacialSpreadState, new FuncPredicate(
				() => IsPlayerInAttackRange(4) &&//enter canon ball attack range
				      abilityIdx == 1
			)
		);
		stateMachine.AddTransition(glacialSpreadState, chaseState, new FuncPredicate(
				() => glacialSpreadState.isTurnComplete
			)
		);
		
		stateMachine.AddTransition(chaseState, explosiveShootState, new FuncPredicate(
				() => IsPlayerInAttackRange(4) &&//enter canon ball attack range
				      abilityIdx == 2
			)
		);
		stateMachine.AddTransition(explosiveShootState, chaseState, new FuncPredicate(
				() => explosiveShootState.isTurnComplete
			)
		);
		stateMachine.SetState(chaseState);
	}

	public void RandomizeAbilityIdx()
	{
		abilityIdx = Mathf.FloorToInt(UnityEngine.Random.Range(0, 3));
	}
}