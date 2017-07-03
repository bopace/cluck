using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour {

	private GameObject selectedObj;
	public GameObject pauseUI;
	public GameObject resume;
	public GameObject restart;
	public GameObject backToMenu;
	public GameObject soundController;

	private bool paused = false;

	void Start() {
		selectedObj = EventSystem.current.currentSelectedGameObject;
		if (selectedObj == null) {
			selectedObj = resume;
		}
		Cursor.visible = false;
		pauseUI.SetActive (false);
	}

	void Update() {
		// set menu item to be selected if none is
		if (EventSystem.current.currentSelectedGameObject == null) {
			EventSystem.current.SetSelectedGameObject (selectedObj);
		}

		selectedObj = EventSystem.current.currentSelectedGameObject;

		if (Input.GetButtonDown ("Cancel")) {
			paused = !paused;
			if (!paused) {
				soundController.GetComponent<SoundController> ().PlayDeathSound ();
			}
		}



		if (paused) {
			// display pause menu
			pauseUI.SetActive (true);
			Time.timeScale = 0;

			if (Input.GetButtonDown ("Vertical")) {
				soundController.GetComponent<SoundController> ().PlayRandomBok ();
			}
			if (Input.GetButtonDown ("Submit")) {
				soundController.GetComponent<SoundController> ().PlayDeathSound ();
			}

		} else {
			// unpause
			selectedObj = resume;
			EventSystem.current.SetSelectedGameObject (null);
			resume.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
			restart.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
			backToMenu.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
			pauseUI.SetActive (false);
			Time.timeScale = 1;
		}
	}

	public void Resume() {
		paused = false;
	}

	public void Restart() {
		SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
	}

	public void ExitToMenu() {
		paused = false;
		StartCoroutine (gameObject.GetComponent<GameManager>().NextLevel(1));
	}
}
