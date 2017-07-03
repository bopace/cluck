using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingEnemyRange : MonoBehaviour {

	private JumpingEnemy parentEnemy;

	void Start() {
		parentEnemy = transform.parent.gameObject.GetComponent<JumpingEnemy> ();
	}

	// set active if player is in range
	void OnTriggerEnter2D(Collider2D col) {
		if (col.CompareTag ("Player")) {
			parentEnemy.SetActive (true);
		}
	}

	// set inactive if player is out of range
	void OnTriggerExit2D(Collider2D col) {
		if (col.CompareTag ("Player")) {
			parentEnemy.SetActive (false);
		}
	}
}
