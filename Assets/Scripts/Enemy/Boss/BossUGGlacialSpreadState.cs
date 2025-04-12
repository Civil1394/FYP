using UnityEngine;
using System.Collections;

public class BossUGGlacialSpreadState : EnemyBaseState 
{
	private AbilityData glacialSpreadAbilityData = null;
	private BossUGController bossUGController;
	public bool isTurnComplete;

	public BossUGGlacialSpreadState(AIBrain enemyBrain, Animator animator, BossUGController bossUgController,
		AbilityData glacialSpreadAD) : base(enemyBrain, animator)
	{
		glacialSpreadAbilityData = glacialSpreadAD;
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