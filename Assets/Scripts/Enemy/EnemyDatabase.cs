using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewEnemyDatabase", menuName = "Enemy/Enemy Database", order = 0)]
public class EnemyDatabase : ScriptableObject
{
	[System.Serializable]
	public class EnemyList
	{
		public string listName;
		public List<EnemyData> enemies;
	}

	public List<EnemyList> enemyLists = new List<EnemyList>();

	public EnemyData GetSpecEnemyFromList(string listName, string enemyId)
	{
		var list = enemyLists.Find(l => l.listName == listName);
		if (list != null)
		{
			return list.enemies.Find(e => e.id == enemyId);
		}
		return null;
	}

	public EnemyData GetRandomEnemyFromList(string listName)
	{
		var list = enemyLists.Find(l => l.listName == listName);
		if (list != null)
		{
			return list.enemies[Random.Range(0, list.enemies.Count)];
		}

		return null;
	}

	public List<EnemyData> GetEnemyList(string listName)
	{
		var list = enemyLists.Find(l => l.listName == listName);
		return list?.enemies;
	}
}