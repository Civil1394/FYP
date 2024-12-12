using UnityEngine;
using System.Collections;

public class PendingActionVisualizer : MonoBehaviour 
{
	[SerializeField] private GameObject pendingActionPointerPrefab;
	[SerializeField] private Vector3 instantiatePosOffset;
	private GameObject activatedActionPointer;
	
	public void ShowPendingActionPointer(PlayerActionType action , HexCellComponent destination)
	{
		RemovePendingActionPointer();
		
		switch (action)
		{
			case PlayerActionType.Move:
				activatedActionPointer = 
					Instantiate(pendingActionPointerPrefab,
						destination.transform.position + instantiatePosOffset ,
									Quaternion.identity);
				break;
			case PlayerActionType.Cast:
				activatedActionPointer = 
					Instantiate(pendingActionPointerPrefab,
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