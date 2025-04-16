using UnityEngine;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour 
{
	[SerializeField] private Slider slider;
	[SerializeField] private Slider ShadeHPBar;
	[SerializeField] protected TMP_Text HPText;
	protected float maxHP;
	protected float HP;

	private object HPBarTweeningID;
	private object ShadeHPBarTweeningID;

	private void Awake()
	{
		//slider = GetComponent<Slider>();
	}
	protected void Start()
	{
		HPBarTweeningID = null;
	}

	public virtual void SetMaxHP(float _maxHP)
	{
		maxHP = _maxHP;
		slider.maxValue = _maxHP;
		HPText.text = HP + " / " + _maxHP;
		if (ShadeHPBar) ShadeHPBar.maxValue = maxHP;
	}

	public virtual void SetHP(float _HP)
	{
		HP = _HP;
		DOTween.Kill(gameObject);

		HPText.text = HP + " / " + maxHP;

		if (slider) HPBarTweeningID = slider.DOValue(HP, 0.4f).id;
		if (ShadeHPBar) ShadeHPBarTweeningID = ShadeHPBar.DOValue(HP, 0.8f).id;
	}
}