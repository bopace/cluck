using UnityEngine;
using System.Collections;

public class SoundController : MonoBehaviour {

	public AudioClip clipPlayerDeath;
	public AudioClip clipLevelComplete;
	public AudioClip clipPlayerFlap;
	public AudioClip[] clipWalkBoks;

	private AudioSource audioPlayerDeath;
	private AudioSource audioLevelComplete;
	private AudioSource audioPlayerFlap;
	private AudioSource[] audioWalkBoks;

	public AudioSource AddAudio(AudioClip clip, bool loop, bool playAwake, float vol) {
		AudioSource newAudio = gameObject.AddComponent<AudioSource>();
		newAudio.clip = clip;
		newAudio.loop = loop;
		newAudio.playOnAwake = playAwake;
		newAudio.volume = vol;
		return newAudio;
	}

	public void Awake() {
		audioPlayerDeath = AddAudio (clipPlayerDeath, false, false, 1.0f);
		audioLevelComplete = AddAudio (clipLevelComplete, false, false, 0.5f);
		audioPlayerFlap = AddAudio (clipPlayerFlap, false, false, 0.5f);

		audioWalkBoks = new AudioSource [clipWalkBoks.Length];
		for (int i = 0; i < clipWalkBoks.Length; i++) {
			audioWalkBoks [i] = AddAudio (clipWalkBoks [i], false, false, 1.0f);
		}
	}

	public void PlayDeathSound() {
		audioPlayerDeath.Play ();
	}

	public void PlayVictorySound() {
		audioLevelComplete.Play ();
	}

	public void PlayRandomBok() {
		audioWalkBoks [Random.Range (0, audioWalkBoks.Length)].Play ();
	}

	public void PlayFlapSound() {
		audioPlayerFlap.Play ();
	}
}
