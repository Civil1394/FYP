using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionBehaviour : MonoBehaviour 
{
	public Image icon;
	public TMP_Text titleText;
	public TMP_Text descriptionText;
	public Button btn;
	public Action clickAction;
	private void Start()
	{
		btn.onClick.AddListener(Select);
	}

	public void Select()
	{
		if (clickAction != null)
		{
			clickAction();
			clickAction = null;
		}
		else
		{
			print("the click actin is lost");
		}
	}
	public void Set(Image icon, TMP_Text titleText, TMP_Text descriptionText, Action clickAction)
	{
		this.icon = icon;
		this.titleText = titleText;
		this.descriptionText = descriptionText;
		this.clickAction = clickAction;
	}
}