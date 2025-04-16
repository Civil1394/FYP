using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraEffectManager : Singleton<CameraEffectManager>
{
	[SerializeField] VolumeProfile volumeProfile;
	private Vignette vignette;
	[SerializeField] private float hitVignetteIntensity = 0.5f; 
	[SerializeField] private Color hitVignetteColor = Color.red;
	[SerializeField] private float vignetteReturnSpeed = 2f;
	bool isPlayingHitReaction = false;
	private void Start()
	{
		if(!volumeProfile) throw new System.NullReferenceException(nameof(UnityEngine.Rendering.VolumeProfile));
		if(!volumeProfile.TryGet(out vignette)) throw new System.NullReferenceException(nameof(vignette));
	}
	public void PlayHitReaction()
	{
		if (vignette != null && !isPlayingHitReaction)
		{
				
			vignette.intensity.value = hitVignetteIntensity;
			vignette.color.value = hitVignetteColor;
        

			StartCoroutine(FadeVignetteToNormal());
		}
	}

	private IEnumerator FadeVignetteToNormal()
	{
		isPlayingHitReaction = true;
		float startIntensity = vignette.intensity.value;
		Color startColor = vignette.color.value;
		float startTime = Time.time;
		float duration = 1f; 
    
		while (Time.time < startTime + duration)
		{
			float t = (Time.time - startTime) / duration;
				
			vignette.intensity.value = Mathf.Lerp(startIntensity, 0.2f, t);
			vignette.color.value = Color.Lerp(startColor, Color.black, t);
        
			yield return null;
		}
			
		vignette.intensity.value = 0.2f;
		vignette.color.value = Color.black;
		isPlayingHitReaction = false;
	}
}