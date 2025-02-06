using System;
using UnityEngine;
using System.Collections;
using UnityEditor.UI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(EnemyHourGlassControllerBaseImpl))]
public class EnemyActor : TimedActor 
{
	[SerializeField] CapsuleCollider objectCollider;
	[SerializeField] private GlobalCanvasHourGlass GlobalCanvasHourGlass;
	private AIBrain aiBrain;
	
	
	protected override void Start()
	{
		aiBrain = gameObject.GetComponent<AIBrain>();
		if (GlobalCanvasHourGlass != null)
		{
			OnTimerStart += GlobalCanvasHourGlass.CountTime;
			OnTimerComplete += aiBrain.TurnAction;
		}
		ActionCooldown = (int)Random.Range(1f, 5f);
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
			TimeManipulate(bullet.timeManipulationType,bullet.Speed);
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