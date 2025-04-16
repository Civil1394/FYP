using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BossUGExplosiveShootState : EnemyBaseState 
{
	private AbilityData explosiveShootAbilityData = null;
	private BossUGController bossUGController;
	public bool isTurnComplete;
	private int progress = 0;
	private int attackCount;
	public BossUGExplosiveShootState(AIBrain enemyBrain, Animator animator,
		AbilityData explosiveShootAD, int attackCount) : base(enemyBrain, animator)
	{
		explosiveShootAbilityData = explosiveShootAD;
		this.bossUGController = enemyBrain as BossUGController;
		this.attackCount = attackCount-1;
	}

	public override void OnEnter()
	{
		isTurnComplete = false;
		progress = 0;
	}
	public override void TurnAction()
	{
		if (progress >= attackCount)
		{
			isTurnComplete = true;
			return;
		}
		explosiveShootAbilityData.TriggerAbility(CasterType.Enemy, GetTargetCell(),
			enemyBrain.currentCell.ParentComponent, bossUGController.gameObject);
		progress++;

	}

	public override void OnExit()
	{
		bossUGController.RandomizeAbilityIdx();
	}

	private HexCellComponent GetTargetCell()
	{
		List<HexCellComponent> cellList = BattleManager.Instance.hexgrid
			.GetCellsInRange(BattleManager.Instance.PlayerCell, 2).ToList();
		int randIdx = Random.Range(0, cellList.Count);
		return cellList[randIdx];
	}
}