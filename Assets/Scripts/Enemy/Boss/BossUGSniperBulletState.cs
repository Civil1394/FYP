using UnityEngine;
using System.Collections;

public class BossUGSniperBulletState : EnemyBaseState 
{
	private AbilityData sniperBulletAbilityData = null;
	private BossUGController bossUGController;
	public bool isTurnComplete;
	public BossUGSniperBulletState(AIBrain enemyBrain, Animator animator, BossUGController bossUgController,
		AbilityData sniperBulletAD) : base(enemyBrain, animator)
	{
		sniperBulletAbilityData = sniperBulletAD;
		this.bossUGController = bossUgController;
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