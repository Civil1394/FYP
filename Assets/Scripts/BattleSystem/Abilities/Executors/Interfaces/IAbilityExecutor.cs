using UnityEngine;
using System.Collections;

public interface IAbilityExecutor 
{
	void Execute(CasterType casterType, HexCellComponent castCell, HexCellComponent casterStandingCell,GameObject casterObject, TimeType timeType);
}