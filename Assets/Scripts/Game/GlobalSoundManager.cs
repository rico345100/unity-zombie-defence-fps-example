using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSoundManager : MonoBehaviour {
	public void Play(AudioClip clip) {
		AudioSource audioSource = gameObject.AddComponent<AudioSource>();

		audioSource.clip = clip;
		audioSource.Play();

		StartCoroutine(RemoveSoundComponent(audioSource));
	}

	IEnumerator RemoveSoundComponent(AudioSource audioSource) {
		yield return new WaitForSeconds(audioSource.clip.length);
		Destroy(audioSource);
	}

	// For support multiplayer, it needs to distinguish between players
	public static GlobalSoundManager Get() {
		return GameObject.Find("GlobalSoundManager").GetComponent<GlobalSoundManager>();
	}
}
