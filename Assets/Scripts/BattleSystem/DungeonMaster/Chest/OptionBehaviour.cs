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
		try
		{
			clickAction?.Invoke();
			clickAction = null;
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}
	}
	public void Set(Sprite iconSprite, String titleText, String descriptionText, Action clickAction)
	{
		this.icon.sprite = iconSprite;
		this.titleText.text = titleText;
		this.descriptionText.text = descriptionText;
		this.clickAction = clickAction;
	}
}