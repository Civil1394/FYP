
public abstract class EffectParameter 
{
}
[System.Serializable]
public class DashParameter : EffectParameter
{
	
}

[System.Serializable]
public class ExplosiveParameter : EffectParameter
{
	public float radius;
	public float damage;
	public float force;
}

[System.Serializable]
public class ProjectileParameter : EffectParameter
{
	public int Damage;
	public float FlowSpeed;
	public float LifeTime;
}