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

	private void Start()
	{
		throw new NotImplementedException();
	}

	public override void StateInitialization()
	{
		chaseState = new GridEnemyChase(this, null, pathFinding);
		glacialSpreadState = new BossUGGlacialSpreadState(this,null,glacialSpread);
		explosiveChargeState = new BossUGExplosiveChargeState(this,null,explosiveCharge, 10);
		canonBallState = new BossUGCanonBallState(this,null,canonBall,10);
		stateMachine.AddTransition(chaseState, canonBallState, new FuncPredicate(
			() => IsPlayerInAttackRange(4)//enter canon ball attack range
			)
		);
		stateMachine.AddTransition(canonBallState, chaseState, new FuncPredicate(
				() => canonBallState.isTurnComplete
			)
		);
	}
}