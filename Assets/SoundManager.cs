using System;
using UnityEngine;
using System.Collections;

public class SoundManager : Singleton<SoundManager>
{
	public AudioClip NormalBGM;
	public AudioClip BossBGM;
	public AudioSource bgmSource;
	public AudioSource sfxSource;

	private void Start()
	{
		bgmSource.clip = NormalBGM;
		bgmSource.Play();
	}

	public void ChangeBossBGM()
	{
		bgmSource.clip = BossBGM;
		bgmSource.Play();
		
	}
	public void PlaySFX(AudioClip clip)
	{
		sfxSource.PlayOneShot(clip);
	}
	
}