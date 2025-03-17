using UnityEngine;
using System.Collections;

public class PlayerAction
{
	public PlayerActionType Type { get; private set; }
	public HexCellComponent TargetCell { get; private set; }
	public PlayerAction(PlayerActionType type, HexCellComponent targetCell = null)
	{
		Type = type;
		TargetCell = targetCell;
	}
}

public enum PlayerActionType
{
	Move,
	Cast,
	None
}