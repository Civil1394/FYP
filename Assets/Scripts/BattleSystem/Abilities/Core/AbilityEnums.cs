using UnityEngine;
using System.Collections;

public enum AbilityType
{
	Projectile = 0,
	ProjectileVolley = 1,
	ParabolaProjectile = 2,
	ExplosiveCharge = 5,
	LocationalProjectile = 6,
	Blast = 10,
	Dash = 20
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

//Identify who cast the ability
public enum CasterType
{
	Player,
	Enemy,
	None
}
