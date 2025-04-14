using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BossUGExplosiveChargeState : EnemyBaseState 
{
	private AbilityData explosiveChargeAbilityData = null;
	private BossUGController bossUGController;
	public bool isTurnComplete;
	private int progress = 0;
	private int attackCount;
	public BossUGExplosiveChargeState(AIBrain enemyBrain, Animator animator,
		AbilityData sniperBulletAD, int attackCount) : base(enemyBrain, animator)
	{
		explosiveChargeAbilityData = sniperBulletAD;
		this.bossUGController = enemyBrain as BossUGController;
		this.attackCount = attackCount-1;
	}

	public override void OnEnter()
	{
		
	}
	public override void TurnAction()
	{
		if (progress >= attackCount)
		{
			isTurnComplete = true;
			return;
		}
		explosiveChargeAbilityData.TriggerAbility(CasterType.Enemy, GetTargetCell(),
			enemyBrain.currentCell.ParentComponent, bossUGController.gameObject);
	}

	private HexCellComponent GetTargetCell()
	{
		List<HexCellComponent> cellList = BattleManager.Instance.hexgrid
			.GetCellsInRange(BattleManager.Instance.PlayerCell, 4).ToList();
		int randIdx = Random.Range(0, cellList.Count);
		return cellList[randIdx];
	}
}