using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Ability", menuName ="Ability/AbilityData")]
public class AbilityData : ScriptableObject
{
	public string id;

	[Header("Text")]
	public string title;
	[TextArea(5, 7)]
	public string desc;
	
	[Header("Trigger")]
	public AbilityTriggerTiming trigger;             //WHEN does the ability trigger?

	[Header("Target")]
	public AbilityTarget target;               //WHO is targeted?

	[Header("Effect")]
	public EffectData[] effects;              //WHAT this does?
	public StatusData[] status;               //Status added by this ability (E.G CrowdControl 
	public int value;                         //Value passed to the effect (deal X damage)
	public int duration;                      //Duration passed to the effect (usually for status, 0=permanent)

	[Header("Chain/Choices")]
	public AbilityData[] chain_abilities;    //Abilities that will be triggered after this one

	[Header("Activated Ability")]
	public int mana_cost;                   //Mana cost for  activated abilities
	public bool exhaust;                    //Action cost for activated abilities

	[Header("FX")]//Future  Use
	public GameObject board_fx;
	public GameObject caster_fx;
	public GameObject target_fx;
	public AudioClip cast_audio;
	public AudioClip target_audio;
	public bool charge_target;


	public void TriggerAbility()
	{
		foreach (var effect in effects)
		{
			effect.TriggerEffect();
		}
	}
	

}

public enum AbilityTriggerTiming
{
	
}

public enum AbilityTarget
{
	
}
