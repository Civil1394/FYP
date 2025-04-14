using UnityEngine;
using System.Collections;

public class BossUGCanonBallState : EnemyBaseState 
{
	private AbilityData canonBallAbilityData = null;
	private BossUGController bossUGController;
	public bool isTurnComplete;

	public BossUGCanonBallState(AIBrain enemyBrain, Animator animator,
		AbilityData canonBallAD) : base(enemyBrain, animator)
	{
		canonBallAbilityData = canonBallAD;
		this.bossUGController = enemyBrain as BossUGController;
	}

	public override void OnEnter()
	{
		
		//sniperBulletAbilityData.TriggerAbility(CasterType.Enemy,);
		
	}
	public override void TurnAction()
	{

	}

	private void GetTargetCell()
	{
		
	}
}

