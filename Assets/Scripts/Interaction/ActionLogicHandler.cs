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

	private void Awake()
	{
		playerActor = GetComponent<PlayerActor>();
	}

	private void Start()
	{
		hexGrid = BattleManager.Instance.hexgrid;
	}

	public HexCellComponent FacingIsLegit(HexDirection direction)
	{
		var c = playerActor.standingCell.CellData.GetNeighbor(direction);
		if (c == null || c.CellType == CellType.Invalid)
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
	
	public void ExecuteAbility(AbilityData ability, HexDirection direction)
	{
		switch (ability.CastType)
		{
			case AbilityCastType.Direction_targeted:
				var c = playerActor.standingCell.CellData.GetNeighbor(direction);
				if (c == null || c.CellType == CellType.Invalid) return;
				HandleDirectionalCast(ability, c.ParentComponent ,HourglassInventory.Instance.hourglassesList[0].TimeType);
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