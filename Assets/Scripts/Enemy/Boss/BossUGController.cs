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
		explosiveChargeState = new BossUGExplosiveChargeState(this,null,explosiveCharge);
		canonBallState = new BossUGCanonBallState(this,null,canonBall);

	}
}