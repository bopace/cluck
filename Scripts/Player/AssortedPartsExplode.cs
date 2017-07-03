using UnityEngine;
using System.Collections;

public class AssortedPartsExplode : MonoBehaviour {

	public GameObject[] parts;

	// when the object is instantiated, send the gibs flying
	void Start() {
		foreach (GameObject part in parts) {
			MakeExplode (part);
		}
	}

	void MakeExplode(GameObject part) {
		Vector2 randomVector = new Vector2 (Random.Range(-800.0f, 800.0f), Random.Range(-800.0f, 800.0f));
		part.GetComponent<Rigidbody2D> ().AddForce (randomVector);
	}
}
