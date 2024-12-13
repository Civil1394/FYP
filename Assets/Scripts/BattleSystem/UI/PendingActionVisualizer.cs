using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;

public class PendingActionVisualizer : MonoBehaviour 
{
	[FormerlySerializedAs("pendingActionPointerPrefab")] [SerializeField] private GameObject pendingMovePointerPrefab;
	[SerializeField] private GameObject pendingAttackPointerPrefab;
	[SerializeField] private Vector3 instantiatePosOffset;
	private GameObject activatedActionPointer;
	
	public void ShowPendingActionPointer(PlayerActionType action , HexCellComponent destination)
	{
		RemovePendingActionPointer();
		
		switch (action)
		{
			case PlayerActionType.Move:
				activatedActionPointer = 
					Instantiate(pendingMovePointerPrefab,
						destination.transform.position + instantiatePosOffset ,
									Quaternion.identity);
				break;
			case PlayerActionType.Cast:
				activatedActionPointer = 
					Instantiate(pendingAttackPointerPrefab,
						destination.transform.position + instantiatePosOffset ,
									Quaternion.identity);
				break;
		}
	}

	public void RemovePendingActionPointer()
	{
		if(activatedActionPointer != null)
			Destroy(activatedActionPointer);
	}
}