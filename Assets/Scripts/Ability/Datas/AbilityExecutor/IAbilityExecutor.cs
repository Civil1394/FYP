using UnityEngine;
using System.Collections;

public interface IAbilityExecutor 
{
	void Execute(CasterType casterType, HexDirection castDirection, HexCellComponent casterStandingCell, TimeType timeType);
}