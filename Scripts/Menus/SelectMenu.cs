using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectMenu : MonoBehaviour {

	private GameObject selectedObj;
	public Button backButton;

	void Start() {
		selectedObj = EventSystem.current.currentSelectedGameObject;
		Cursor.visible = false;
	}

	void Update() {
		if (EventSystem.current.currentSelectedGameObject == null) {
			EventSystem.current.SetSelectedGameObject (selectedObj);
		}

		if (Input.GetButtonDown ("Cancel")) {
			backButton.onClick.Invoke ();
		}

		selectedObj = EventSystem.current.currentSelectedGameObject;
	}

	public void SceneSelection(int sceneNumber) {
		StartCoroutine (GameObject.Find ("GameManager").GetComponent<GameManager> ().NextLevel(sceneNumber));
	}

	public void GoBack() {
		if (SceneManager.GetActiveScene ().buildIndex == 6) {
			StartCoroutine (GameObject.Find ("GameManager").GetComponent<GameManager> ().NextLevel (0));
		} else {
			StartCoroutine (GameObject.Find ("GameManager").GetComponent<GameManager> ().NextLevel (6));
		}
	}
}
