using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
public class ExplosiveActor : DamageActor
{
	public override event Action<GameObject> OnHitApplyStatusEffect;
	private SphereCollider sphereCollider;
	private float finalColliderRadius; // The radius you ultimately want
	private float expansionDuration = 0.5f; // How long the expansion should take (in seconds)
	private float expansionTimer = 0f;
	private bool isExpanding = true;

	private List<HexCellComponent> highlightedCells = new List<HexCellComponent>();

	private void Start()
	{
		sphereCollider = GetComponent<SphereCollider>();
		if (sphereCollider == null)
		{
			sphereCollider = gameObject.AddComponent<SphereCollider>();
			sphereCollider.isTrigger = true;
		}
		
		finalColliderRadius = sphereCollider.radius;
		sphereCollider.radius = 0.01f;
		
		StartCoroutine(ExpandCollider());
	}

	private IEnumerator ExpandCollider()
	{
		float startRadius = 0.01f;
		float elapsedTime = 0f;
    
		while (elapsedTime < expansionDuration)
		{
			float t = elapsedTime / expansionDuration;

			sphereCollider.radius = Mathf.Lerp(startRadius, finalColliderRadius, t);

			elapsedTime += Time.deltaTime;
			yield return null;
		}

		sphereCollider.radius = finalColliderRadius;
		isExpanding = false;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Cell"))
		{
			var currentCell = other.GetComponent<HexCellComponent>();
			print("hit cell");
			currentCell.HighLightCell(abilityData.ColorType);
			highlightedCells.Add(currentCell);
		}	
	}
	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Cell"))
		{
			other.GetComponent<HexCellComponent>().UnhighLightCell();
		}
	}
	private void OnDestroy()
	{
		foreach (var c in highlightedCells)
		{
			c.UnhighLightCell();
		}
	}
	public override void DoDamage(Action<float> damageAction,GameObject damagedObject,GameObject sourceVFX)
	{
		damageAction?.Invoke(_damage);
		OnHitApplyStatusEffect?.Invoke(damagedObject);
	}
	
}