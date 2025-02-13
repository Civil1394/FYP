
public abstract class AbilityParameter 
{
}
[System.Serializable]
public class DashParameter : AbilityParameter
{
	
}

[System.Serializable]
public class ExplosiveParameter : AbilityParameter
{
	public float radius;
	public float damage;
	public float force;
}

[System.Serializable]
public class ProjectileParameter : AbilityParameter
{
	public float Damage;
	public float FlowSpeed;
	public float LifeTime;
}