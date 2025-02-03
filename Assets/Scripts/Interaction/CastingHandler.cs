using System;
using UnityEngine;

public class CastingHandler : MonoBehaviour
{
	public event Action<HexCellComponent> OnLocationCast;
	public event Action<AbilityData> OnDirectionalCast;
	public event Action<AbilityData> OnUnitCast;
	//private AbilityData currentCastingAbility; //storing the pending abilitydata to be cast

	private void Start()
	{
		//BattleManager.Instance.InputHandler.OnCastClick.AddListener<HexCellComponent>(HandleDirectionalCast);
	}
	
	//Check the casting cell is in range or other condition is fulfilled
	public bool CastIsLegit(AbilityData ability, HexCellComponent clickedCell)
	{
		switch (ability.CastType)
		{
			case AbilityCastType.Direction_targeted:
				return BattleManager.Instance.hexgrid.CheckCellInRange(clickedCell,
					BattleManager.Instance.PlayerCell, 1) && clickedCell.CellData.CellType == CellType.Empty ;
				break;
		}

		return false;
	}
	
	public void ExecuteAbility(AbilityData ability, HexCellComponent targetCell = null)
	{
		switch (ability.CastType)
		{
			case AbilityCastType.Direction_targeted:
				HandleDirectionalCast(ability, targetCell);
				break;
		}
		

		BattleManager.Instance.TurnManager.ExecuteAction(TurnActionType.PlayCard, $"castedability:{ability.name}");
	}
	private void HandleDirectionalCast(AbilityData ability,HexCellComponent directionCell)
	{
		HexCellComponent playerStandingCell = BattleManager.Instance.PlayerCell;
		HexDirection castDirection =
			BattleManager.Instance.hexgrid.CheckNeigborCellDirection(playerStandingCell, directionCell);
		
		foreach (var effect in ability.Effects)
		{
			effect.ApplyEffectDirection(BattleManager.Instance.PlayerActorInstance.transform, castDirection, playerStandingCell);
		}
		
		ResetCasting();
	}
	
	public void CancelCasting()
	{
		ResetCasting();
	}
	
	private void ResetCasting()
	{
		//currentCastingAbility = null;
		BattleManager.Instance.InputHandler.SetInputState(InputState.Move);
	}
}