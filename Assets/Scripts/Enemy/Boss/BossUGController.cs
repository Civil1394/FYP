using System;
using UnityEngine;
using System.Collections;

public class BossUGController : AIBrain
{
	private BossUGExplosiveChargeState explosiveChargeState;
	private BossUGGlacialSpreadState glacialSpreadState;
	private BossUGCanonBallState canonBallState;
	private GridEnemyChase chaseState;
	
	public AbilityData explosiveCharge;
	public AbilityData glacialSpread;
	public AbilityData canonBall;

	public int abilityIdx = 1;

	public override void StateInitialization()
	{
		chaseState = new GridEnemyChase(this, null, pathFinding);
		glacialSpreadState = new BossUGGlacialSpreadState(this,null,glacialSpread);
		explosiveChargeState = new BossUGExplosiveChargeState(this,null,explosiveCharge, 3);
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
		
		stateMachine.AddTransition(chaseState, explosiveChargeState, new FuncPredicate(
				() => IsPlayerInAttackRange(4) &&//enter canon ball attack range
				      abilityIdx == 2
			)
		);
		stateMachine.AddTransition(explosiveChargeState, chaseState, new FuncPredicate(
				() => explosiveChargeState.isTurnComplete
			)
		);
		stateMachine.SetState(chaseState);
	}

	public void RandomizeAbilityIdx()
	{
		abilityIdx = Mathf.FloorToInt(UnityEngine.Random.Range(0, 3));
	}
}