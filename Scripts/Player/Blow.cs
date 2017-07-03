using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blow : MonoBehaviour {

	Vector2 blowOrigin;

	float windSpeed = 1500;

	Player player;

	void Start() {
		blowOrigin = transform.GetChild (0).transform.position;
		player = transform.parent.parent.GetComponent<Player> ();
	}

	// suck/blow enemies and items
	void OnTriggerEnter2D(Collider2D col) {
		blowOrigin = transform.GetChild (0).transform.position;
		if (col.attachedRigidbody != null && col.gameObject.CompareTag ("Enemy")) {
			Vector2 blowDirection = (blowOrigin - (Vector2)col.attachedRigidbody.transform.position);
			// negative direction to get things to blow away. positive to suck things in
			if (player.sucking) {
				col.attachedRigidbody.AddForce (blowDirection * windSpeed * Time.deltaTime);
			} else {
				col.attachedRigidbody.AddForce (-blowDirection * windSpeed * Time.deltaTime);
			}
		} else if (col.attachedRigidbody != null && col.gameObject.CompareTag ("Item")) {
			Vector2 blowDirection = (blowOrigin - (Vector2)col.attachedRigidbody.transform.position);
			// negative direction to get things to blow away. positive to suck things in
			if (player.sucking) {
				col.attachedRigidbody.AddForce (2 * blowDirection * windSpeed * Time.deltaTime);
			} else {
				col.attachedRigidbody.AddForce (-blowDirection * windSpeed * Time.deltaTime);
			}
		}
	}
}
