using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour {

	private static MusicManager instance = null;

	public AudioSource audioBGMusic;

	// music manager is singleton
	public static MusicManager Instance {
		get { return instance; }
	}

	// retain music across scene loading
	void Awake() {
		if (instance != null && instance != this) {
			Destroy (this.gameObject);
			return;
		} else {
			instance = this;
		}
		DontDestroyOnLoad (this.gameObject);

		audioBGMusic.Play ();
	}
}
