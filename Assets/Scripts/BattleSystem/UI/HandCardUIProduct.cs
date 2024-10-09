using UnityEngine;
using System.Collections;
using TMPro;

public class HandCardUIProduct : MonoBehaviour
{
	[SerializeField] private TMP_Text cost;
	[SerializeField] private TMP_Text title;
	[SerializeField] private TMP_Text desc;
	public void Init(AbilityData abilityData)
	{
		cost.text = abilityData.initCost.ToString();
		title.text = abilityData.title;
		desc.text = abilityData.desc;
	}
}