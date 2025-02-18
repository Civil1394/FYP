using System.Collections;


public abstract class SingletonBase<T> where T : class, new()
{
	private static readonly object _lock = new object();
	private static T _instance;

	public static T Instance
	{
		get
		{
			lock (_lock)
			{
				if (_instance == null)
				{
					_instance = new T();
				}
				return _instance;
			}
		}
	}

	protected SingletonBase()
	{
		if (_instance != null)
		{
			throw new System.Exception($"Cannot create multiple instances of singleton {typeof(T)}");
		}
		Init();
	}

	protected virtual void Init() { }
}