using System;using UnityEngine;
using System.Collections;
[CreateAssetMenu(fileName = "EffectData", menuName ="Effect/EffectData")]
public class EffectData : ScriptableObject
{
	[SerializeField] private EffectType effectType;
	[TextArea(5, 7)]
	public string desc;
	public EffectData(EffectType _effectType)
	{
		effectType = _effectType;
	}

	public void TriggerEffect()
	{
		if (effectType == 0)
		{
			GameObject bullet =Instantiate(new GameObject("Bullet"));
			bullet.AddComponent<Bullet>();
			Debug.Log(desc);
		}
	}
}

public enum EffectType
{
	Projectile = 0
}
