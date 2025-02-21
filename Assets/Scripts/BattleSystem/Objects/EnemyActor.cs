using System;
using UnityEngine;
using System.Collections;
using UnityEditor.UI;
using Random = UnityEngine.Random;

public class EnemyActor : TimedActor 
{
	[SerializeField] CapsuleCollider objectCollider;
	[SerializeField] private HourglassUIProduct hourglassUIProduct;
	private AIBrain aiBrain;


	public override void Init(Hourglass hourglass)
	{
		base.Init(hourglass);
		hourglassUIProduct.Init(hourglass);
	}

	protected override void Start()
	{
		aiBrain = gameObject.GetComponent<AIBrain>();
		OnTimerComplete += aiBrain.TurnAction;

		base.Start();
	}
	protected override void Update()
	{
		base.Update();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Projectile"))
		{
			Debug.Log("gethit");
			//TakeDamage(other.GetComponent<BulletActor>().Damage);
			var bullet = other.GetComponent<BulletActor>();
			TimeManipulate(bullet.TimeType,bullet.Speed);
			Destroy(other.gameObject);
		}
	}

	protected override void OverDrive()
	{
		base.OverDrive();
		aiBrain.currentCell.ClearCell();
		Destroy(this.gameObject);
	}
	
	protected override void Collapse()
	{
		base.Collapse();
		aiBrain.currentCell.ClearCell();
		Destroy(this.gameObject);
	}

}