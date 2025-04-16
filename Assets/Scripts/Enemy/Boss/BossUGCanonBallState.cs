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
		this.attackCount = attackCount;
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
		HexDirection dir =
			BattleManager.Instance.hexgrid.GetFuzzyHexDirectionBy2Cell(bossUGController.currentCell.ParentComponent,
				BattleManager.Instance.PlayerCell);

		canonBallAbilityData.TriggerAbility(CasterType.Enemy,
			bossUGController.currentCell.GetNeighbor(dir).ParentComponent, bossUGController.currentCell.ParentComponent,
			bossUGController.gameObject);
		if (Random.Range(-1, 1) > 0)
		{
			dir = (HexDirection)(((int)dir + 2) % 6);
		}
		else
		{
			dir = (HexDirection)(((int)dir - 2) % 6);
		}

		while((int)dir<0)
		{
			dir += 6;
		}
		Debug.Log(dir);
		bossUGController.Move(bossUGController.currentCell.GetNeighbor(dir));
		progress++;
	}
	public override void OnExit()
	{
		bossUGController.RandomizeAbilityIdx();
	}
}

