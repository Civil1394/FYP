using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Ability", menuName ="Ability/AbilityData")]
public class AbilityData : ScriptableObject
{
	public string id;

	[Header("Text")]
	public string Title;
	public int InitCost;
	[TextArea(5, 7)]
	public string Desc;
	
	[Header("Attributes value")]
	public float Cooldown;
	//damage, healing point...
	[Header("Cast")]
	public int castRange;
	public AbilityCastTiming castTiming;             //WHEN does the ability trigger?
	public AbilityCastType CastType;             //How the ability is cast?
	[Header("Target")]
	public AbilityTarget target;               //WHO is targeted?

	[Header("Effect")]
	public EffectData[] Effects;              //WHAT this does?
	public StatusData[] status;               //Status added by this ability (E.G CrowdControl //Duration passed to the effect (usually for status, 0=permanent)

	[Header("Chain/Choices")]
	public AbilityData[] chain_abilities;    //Abilities that will be triggered after this one

	[Header("VFX")]//Future  Use
	public AudioClip cast_audio;
	public bool charge_target;


	public void TriggerAbility()
	{
	}
	

}

public enum AbilityCastTiming
{
	OnTurnStart,
	OnTurnEnd,
	OnCardPlayed,
}

public enum AbilityCastType
{
	Auto_targeted,
	Direction_targeted,
	Location_targeted,
	Unit_targeted,
	Self_cast,
}
public enum AbilityTarget
{
	None,
	Player,
	Enemy,
	Environment
}

public enum AbilityCasterType
{
	None,
	Player,
	Enemy
}
