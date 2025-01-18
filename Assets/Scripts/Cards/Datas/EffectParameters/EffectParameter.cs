
public enum TimeManipulationType
{
	Boost,
	Slow,
	None
}
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
	public TimeManipulationType timeManipulationType;
}

[System.Serializable]
public class ProjectileParameter : EffectParameter
{
	public float Damage;
	public float FlowSpeed;
	public float LifeTime;
	public TimeManipulationType TimeManipulationType;
}