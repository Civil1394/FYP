using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class ScrollingStripe : MonoBehaviour
{
	public float scrollSpeed = 0.5f;
	private RawImage rawImage;
	private float textureUnitSizeX;

	[SerializeField] private RectTransform screenRef;

	void Start()
	{
		rawImage = GetComponent<RawImage>();
		textureUnitSizeX = rawImage.uvRect.width;
		if (screenRef != null)
			GetComponent<RectTransform>().sizeDelta =
				new Vector2(screenRef.rect.width, GetComponent<RectTransform>().sizeDelta.y);

	}

	void Update()
	{
		float moveAmount = scrollSpeed * Time.deltaTime;

		rawImage.uvRect = new Rect(
			rawImage.uvRect.x + moveAmount,
			rawImage.uvRect.y,
			rawImage.uvRect.width,
			rawImage.uvRect.height
		);

		if (rawImage.uvRect.x >= textureUnitSizeX)
		{
			rawImage.uvRect = new Rect(
				rawImage.uvRect.x - textureUnitSizeX,
				rawImage.uvRect.y,
				rawImage.uvRect.width,
				rawImage.uvRect.height
			);
		}
	}
}