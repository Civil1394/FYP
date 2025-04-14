using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BossUGExplosiveChargeState : EnemyBaseState 
{
	private AbilityData explosiveChargeAbilityData = null;
	private BossUGController bossUGController;
	public bool isTurnComplete;
	public BossUGExplosiveChargeState(AIBrain enemyBrain, Animator animator,
		AbilityData sniperBulletAD) : base(enemyBrain, animator)
	{
		explosiveChargeAbilityData = sniperBulletAD;
		this.bossUGController = enemyBrain as BossUGController;
	}

	public override void OnEnter()
	{
		
		//sniperBulletAbilityData.TriggerAbility(CasterType.Enemy,);
		
	}
	public override void TurnAction()
	{
		explosiveChargeAbilityData.TriggerAbility(CasterType.Enemy, GetTargetCell(),
			enemyBrain.currentCell.ParentComponent, enemyBrain.gameObject);
	}

	private HexCellComponent GetTargetCell()
	{
		List<HexCellComponent> cellList = BattleManager.Instance.hexgrid
			.GetCellsInRange(BattleManager.Instance.PlayerCell, 4).ToList();
		int randIdx = Random.Range(0, cellList.Count);
		return cellList[randIdx];
	}
}