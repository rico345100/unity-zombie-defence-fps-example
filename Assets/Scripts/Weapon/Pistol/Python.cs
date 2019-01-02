using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Python : MonoBehaviour {
	[Header("Sounds")]
	public SoundManager soundManager;
	public AudioClip fireSound;
	public AudioClip magOutSound;
	public AudioClip magInSound;
	public AudioClip boltSound;

	void OnFire() {
		soundManager.Play(fireSound);
	}

	void OnMagOut() {
		soundManager.Play(magOutSound);
	}

	void OnMagIn() {
		soundManager.Play(magInSound);
	}

	void OnBoltPulled() {
		soundManager.Play(boltSound);
	}
}
