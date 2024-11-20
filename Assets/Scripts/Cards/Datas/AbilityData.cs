using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Ability", menuName ="Ability/AbilityData")]
public class AbilityData : ScriptableObject
{
	public string id;

	[Header("Text")]
	public string title;
	public int initCost;
	[TextArea(5, 7)]
	public string desc;
	
	[Header("Trigger")]
	public AbilityTriggerTiming triggerTiming;             //WHEN does the ability trigger?

	[Header("Target")]
	public AbilityTarget target;               //WHO is targeted?

	[Header("Effect")]
	public EffectData[] effects;              //WHAT this does?
	public StatusData[] status;               //Status added by this ability (E.G CrowdControl //Duration passed to the effect (usually for status, 0=permanent)

	[Header("Chain/Choices")]
	public AbilityData[] chain_abilities;    //Abilities that will be triggered after this one

	[Header("FX")]//Future  Use
	public AudioClip cast_audio;
	public bool charge_target;


	public void TriggerAbility()
	{
		foreach (var effect in effects)
		{
			effect.ApplyEffect(BattleManager.Instance.GetPlayerCell().CellData.Coordinates);
			Debug.Log(BattleManager.Instance.GetPlayerCell().transform.position);
		}
	}
	

}

public enum AbilityTriggerTiming
{
	OnTurnStart,
	OnTurnEnd,
	OnCardPlayed,
}

public enum AbilityTarget
{
	Player,
	Enemy,
	Environment
}
