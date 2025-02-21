using System;
using UnityEngine;

public class ActionLogicHandler : MonoBehaviour
{
	public event Action<HexCellComponent> OnLocationCast;
	public event Action<AbilityData> OnDirectionalCast;
	public event Action<AbilityData> OnUnitCast;
	private PlayerActor playerActor;
	private HexGrid hexGrid;
	//private AbilityData currentCastingAbility; //storing the pending abilitydata to be cast

	private void Start()
	{
		//BattleManager.Instance.InputHandler.OnCastClick.AddListener<HexCellComponent>(HandleDirectionalCast);
		playerActor = GetComponent<PlayerActor>();
		hexGrid = BattleManager.Instance.hexgrid;
	}

	public HexCellComponent FacingIsLegit(HexDirection direction)
	{
		var c = BattleManager.Instance.PlayerCell.CellData.GetNeighbor(direction);
		if (c == null)
		{
			return null;
		}

		return c.ParentComponent;
	}
	public static bool MoveIsLegit()
	{
		return false;
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
				HandleDirectionalCast(ability, targetCell,HourglassInventory.Instance.hourglassesList[0].TimeType);
				break;
		}
		

		BattleManager.Instance.TurnManager.ExecuteAction(TurnActionType.PlayCard, $"castedability:{ability.name}");
	}
	private void HandleDirectionalCast(AbilityData abilityData,HexCellComponent directionCell,TimeType timeType)
	{
		HexCellComponent playerCell = BattleManager.Instance.PlayerCell;
		HexDirection castDirection =
			BattleManager.Instance.hexgrid.CheckNeigborCellDirection(playerCell, directionCell);
		
		abilityData.TriggerAbility(playerActor.transform, castDirection, playerCell,timeType);
		
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