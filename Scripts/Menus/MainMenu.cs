using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour {

	private GameObject selectedObj;
	public GameObject soundController;

	void Start() {
		selectedObj = EventSystem.current.currentSelectedGameObject;
		soundController = GameObject.Find ("SoundController");
		Cursor.visible = false;
	}

	void Update() {
		if (EventSystem.current.currentSelectedGameObject == null) {
			EventSystem.current.SetSelectedGameObject (selectedObj);
		}

		selectedObj = EventSystem.current.currentSelectedGameObject;

		if (Input.GetButtonDown ("Vertical")) {
			soundController.GetComponent<SoundController> ().PlayRandomBok ();
		}

		if (Input.GetButtonDown ("Horizontal")) {
			soundController.GetComponent<SoundController> ().PlayRandomBok ();
		}

		if (Input.GetButtonDown ("Submit")) {
			soundController.GetComponent<SoundController> ().PlayDeathSound ();
		}
	}

	public void StartGame() {
		StartCoroutine (GameObject.Find ("GameManager").GetComponent<GameManager> ().NextLevel(6));
	}

	public void ExitGame() {
		Application.Quit ();
	}
}
