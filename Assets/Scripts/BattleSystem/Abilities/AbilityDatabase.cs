using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewAbilityDatabase", menuName = "Ability/Ability Database")]
public class AbilityDatabase : ScriptableObject
{
	[System.Serializable]
	public class AbilityList
	{
		public string listName;
		public List<AbilityData> abilities;
	}

	public List<AbilityList> abilityLists = new List<AbilityList>();

	public AbilityData GetSpecAbilityFromList(string listName, string abilityId)
	{
		var list = abilityLists.Find(l => l.listName == listName);
		if (list != null)
		{
			return list.abilities.Find(a => a.id == abilityId);
		}
		return null;
	}

	public AbilityData GetRandomAbilityFromList(string listName)
	{
		var list = abilityLists.Find(l => l.listName == listName);
		if (list != null)
		{
			return list.abilities[Random.Range(0, list.abilities.Count)];
		}

		return null;
	}
	public List<AbilityData> GetAbilityList(string listName)
	{
		var list = abilityLists.Find(l => l.listName == listName);
		return list?.abilities;
	}
	
}

