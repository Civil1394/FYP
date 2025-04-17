using System;
using UnityEngine;
using System.Collections;
using DG.Tweening;
public class ExplosiveActor : DamageActor
{
	public override event Action<GameObject> OnHitApplyStatusEffect;
	private SphereCollider sphereCollider;
	private float finalColliderRadius; // The radius you ultimately want
	private float expansionDuration = 0.5f; // How long the expansion should take (in seconds)
	private float expansionTimer = 0f;
	private bool isExpanding = true;

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
	
	
	public override void DoDamage(Action<float> damageAction,GameObject damagedObject,GameObject sourceVFX)
	{
		damageAction?.Invoke(_damage);
		OnHitApplyStatusEffect?.Invoke(damagedObject);
	}
	
}