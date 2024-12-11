using UnityEngine;
using System.Collections;

public class PlayerAction
{
	public PlayerActionType Type { get; private set; }
	public HexCellComponent TargetCell { get; private set; }
	public Card CardToCast { get; private set; }

	public PlayerAction(PlayerActionType type, HexCellComponent targetCell = null, Card card = null)
	{
		Type = type;
		TargetCell = targetCell;
		CardToCast = card;
	}
}

public enum PlayerActionType
{
	Move,
	Cast,
	None
}