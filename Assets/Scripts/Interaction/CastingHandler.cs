using System;
using UnityEngine;
using System.Collections;

public class CastingHandler : MonoBehaviour
{
	public event Action<HexCellComponent> OnLocationCast;
	public event Action<AbilityData> OnDirectionalCast;
	public event Action<AbilityData> OnUnitCast;
	private AbilityData currentCastingAbility;

	private void Start()
	{
		BattleManager.Instance.InputHandler.OnCastClick += HandleDirectionalCast;
	}
	
	public void HandleCast(AbilityData ability)
	{
		currentCastingAbility = ability;
		BattleManager.Instance.InputHandler.SetInputState(InputState.CastingAbility);
		
		OnDirectionalCast?.Invoke(ability);
	}
	
	private void HandleDirectionalCast(HexCellComponent ClickedCell)
	{
		if (currentCastingAbility == null) return;
		
		ExecuteAbility(currentCastingAbility, ClickedCell);
		
		ResetCasting();
	}
	
	private void ExecuteAbility(AbilityData ability, HexCellComponent directionCell)
	{
		foreach (var effect in ability.effects)
		{
			effect.ApplyEffect(directionCell);
		}

		BattleManager.Instance.TurnManager.ExecuteAction(TurnActionType.PlayCard, $"castedability:{ability.name}");
	}
	
	public void CancelCasting()
	{
		ResetCasting();
	}
	
	private void ResetCasting()
	{
		currentCastingAbility = null;
		BattleManager.Instance.InputHandler.SetInputState(InputState.Move);
	}
}