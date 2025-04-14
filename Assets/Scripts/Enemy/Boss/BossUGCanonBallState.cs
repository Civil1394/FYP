using UnityEngine;
using System.Collections;

public class BossUGCanonBallState : EnemyBaseState 
{
	private AbilityData canonBallAbilityData = null;
	private BossUGController bossUGController;
	public bool isTurnComplete;
	private int progress = 0;
	private int attackCount; 
	public BossUGCanonBallState(AIBrain enemyBrain, Animator animator,
		AbilityData canonBallAD, int attackCount) : base(enemyBrain, animator)
	{
		canonBallAbilityData = canonBallAD;
		this.bossUGController = enemyBrain as BossUGController;
		this.attackCount = attackCount-1;
	}

	public override void OnEnter()
	{
		
		//sniperBulletAbilityData.TriggerAbility(CasterType.Enemy,);
		
	}
	public override void TurnAction()
	{
		if (progress >= attackCount)
		{
			isTurnComplete = true;
			return;
		}
		progress++;
		HexDirection dir =
			BattleManager.Instance.hexgrid.GetFuzzyHexDirectionBy2Cell(bossUGController.currentCell.ParentComponent,
				BattleManager.Instance.PlayerCell);

		canonBallAbilityData.TriggerAbility(CasterType.Enemy,
			bossUGController.currentCell.GetNeighbor(dir).ParentComponent, bossUGController.currentCell.ParentComponent,
			bossUGController.gameObject);
		if (Random.Range(-1, 1) > 0)
		{
			dir += Random.Range(1, 2);
		}
		else
		{
			dir -= Random.Range(1, 2);
		}
		bossUGController.Move(bossUGController.currentCell.GetNeighbor(dir));
	}
	public override void OnExit()
	{
		bossUGController.RandomizeAbilityIdx();
	}
}

