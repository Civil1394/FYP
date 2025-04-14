using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BossUGGlacialSpreadState : EnemyBaseState 
{
	private AbilityData glacialSpreadAbilityData = null;
	private BossUGController bossUGController;
	public bool isTurnComplete = false;
	private List<HexCellComponent> cells;
	private int progress = 0;
	public BossUGGlacialSpreadState(AIBrain enemyBrain, Animator animator,
		AbilityData glacialSpreadAD) : base(enemyBrain, animator)
	{
		glacialSpreadAbilityData = glacialSpreadAD;
		this.bossUGController = enemyBrain as BossUGController;
	}

	public override void OnEnter()
	{
		cells = GetTargetCell();
		//sniperBulletAbilityData.TriggerAbility(CasterType.Enemy,);

	}
	public override void TurnAction()
	{
		if (progress >= cells.Count-1)
		{
			isTurnComplete = true;
			return;
		}

		glacialSpreadAbilityData.TriggerAbility(CasterType.Enemy, cells[progress++],
			bossUGController.currentCell.ParentComponent, bossUGController.gameObject);
	}

	private List<HexCellComponent> GetTargetCell()
	{
		List<HexCellComponent> cellsList = BattleManager.Instance.hexgrid
			.GetCellsInRange(bossUGController.currentCell.ParentComponent,1).ToList();

		return cellsList;
	}
}