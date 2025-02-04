using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Ability", menuName ="Ability/AbilityData")]
public class AbilityData : ScriptableObject
{
	public string id;

	[Header("Text")]
	public string Title;

	[TextArea(5, 7)]
	public string Desc;
	
	[Header("Cast")]
	public AbilityCastType CastType; //How the ability is cast?
	
	[Header("Target")]
	public AbilityTarget target;               //WHO is targeted?

	[Header("Effect")]
	public EffectData[] Effects;              //WHAT this does?
	
	[Header("VFX")]//Future  Use
	public AudioClip cast_audio;
	public bool charge_target;
	
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


