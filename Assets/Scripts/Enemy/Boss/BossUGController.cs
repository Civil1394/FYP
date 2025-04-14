using System;
using UnityEngine;
using System.Collections;

public class BossUGController : AIBrain
{
	private GridEnemyAttack sniperBulletState;
	private GridEnemyAttack glacialSpreadState;
	private GridEnemyAttack canonBallState;
	private GridEnemyChase chaseState;
	
	public AbilityData sniperBullet;
	public AbilityData glacialSpread;
	public AbilityData canonBall;

	private void Start()
	{
		throw new NotImplementedException();
	}

	public override void StateInitialization()
	{
		
	}
}