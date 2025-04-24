using System;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class AbilityPreviewController : MonoBehaviour 
{
	[SerializeField] GameObject prefab;
	[SerializeField] float duration = 0.3f;
	[SerializeField] Transform container;
	
	readonly Queue<GameObject> pool = new();
	readonly List<GameObject> active = new();
	public PlayerActor playerActor;

	public void Show(List<HexCell> path)
	{
		while (pool.Count < path.Count)
			pool.Enqueue(Instantiate(prefab, container));

		foreach (var cell in path)
		{
			var obj = pool.Dequeue();
			obj.SetActive(true);
			obj.transform.position = playerActor.standingCell.transform.position;
			obj.transform.DOMove(cell.ParentComponent.transform.position, duration).SetEase(Ease.OutQuad);
			active.Add(obj);
		}
	}

	public void Unshow()
	{
		foreach (var obj in active)
		{
			transform.DOComplete();
			obj.transform.DOMove(playerActor.standingCell.transform.position - Vector3.down, duration)
				.SetEase(Ease.OutQuad).OnComplete(() =>
				{
					obj.transform.DOKill();
					obj.gameObject.SetActive(false);
					pool.Enqueue(obj);
				});
		}
		active.Clear();
	}
}