using System.Collections.Generic;
using UnityEngine;
//ez as fk
public class MultipleCharacterControlSystem : MonoBehaviour 
{
	public List<PlayerActor> charactersActorList = new List<PlayerActor>();//could make it sorted base on time left to achieve auto switch
	private InputHandler inputHandler;

	private void Start()
	{
		inputHandler = BattleManager.Instance.InputHandler;
	}
	
	public void SwitchCharacter(int characterIndex)
	{
		if (characterIndex < 0 || characterIndex >= charactersActorList.Count)
		{
			Debug.LogWarning("selected character index is out of range");
			return;
		}
		//if character deceased return;
		inputHandler.OnMoveClick.RemoveAllListener();
		inputHandler.OnCastClick.RemoveAllListener();
		
		inputHandler.OnMoveClick.AddListener<HexCellComponent>(charactersActorList[characterIndex].QueueMoveAction);
		inputHandler.OnCastClick.AddListener<HexCellComponent>(charactersActorList[characterIndex].QueueCastAction);
	}
}