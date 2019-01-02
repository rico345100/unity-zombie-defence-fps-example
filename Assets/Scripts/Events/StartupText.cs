using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class StartupText : MonoBehaviour {

	public AudioMixerGroup output; 

	Text text;
	GlobalSoundManager globalSoundManager;
	int remainSeconds;
	
	public AudioClip prepare;
	public AudioClip gameBegins;
	public AudioClip beep;

	void Start() {
		globalSoundManager = GlobalSoundManager.Get();
		
		globalSoundManager.Play(prepare);

		text = GetComponent<Text>();
		remainSeconds = 10;

		StartCoroutine(StartAnimation());
	}

	IEnumerator StartAnimation() {
		for(int i = remainSeconds; i > 0; i--) {
			if(i <= 5) {
				globalSoundManager.Play(beep);
			}

			UpdateText(i);
			yield return new WaitForSeconds(1f);
		}

		text.text = "FIGHT!";
		globalSoundManager.Play(gameBegins);

		yield return new WaitForSeconds(3f);
		Destroy(gameObject);
	}

	void UpdateText(int sec) {
		text.text = "Prepare to fight...\nBegins at " + sec + " seconds.";
	}
}
