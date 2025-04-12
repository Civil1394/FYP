using System;
using UnityEngine;
using System.Collections;

public class BossUGController : MonoBehaviour
{
	private GridEnemyAttack sniperBulletState;
	private GridEnemyAttack glacialSpreadState;
	private GridEnemyAttack canonBallState;
	private GridEnemyChase chaseState;
	
	public AbilityData sniperBullet;
	public AbilityData glacialSpread;
	public AbilityData canonBall;

	public HexCellComponent standingCell;
	private void Start()
	{
		throw new NotImplementedException();
	}

	void StateInitialization()
	{
		
	}
}