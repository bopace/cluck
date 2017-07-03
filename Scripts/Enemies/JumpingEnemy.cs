using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingEnemy : MonoBehaviour {

	public float walkSpeed = 5f;

	public Transform groundedCheck;
	public CircleCollider2D playerCheck;
	public GameObject player;

	public LayerMask groundLayerMask;

	float blowerTimer;
	float jumpTimer;
	float jumpPower = 750f;
	float jumpDirection;

	bool active;
	bool grounded;
	bool inBlower;

	private Rigidbody2D enemy_rb;

	void Start() {
		enemy_rb = gameObject.GetComponent<Rigidbody2D> ();
		float blowerTimer = 0;
		active = false;
	}

	void Update () {

		// check if enemy is grounded
		grounded = Physics2D.OverlapCircle(groundedCheck.position, 0.1f, groundLayerMask);

		// blower logic
		if (blowerTimer > 0) {
			blowerTimer -= Time.deltaTime;
		}

		if (blowerTimer < 0) {
			blowerTimer = 0;
		}

		if (blowerTimer == 0) {
			inBlower = false;
		}

		// jump timer logic
		if (jumpTimer > 0 && grounded && !inBlower) {
			jumpTimer -= Time.deltaTime;
		}

		if (jumpTimer < 0) {
			jumpTimer = 0;
		}

		if (jumpTimer == 0) {
			inBlower = false;
		}

		// if active, check if player is to the left or right of enemy
		if (active) {
			var relativePoint = transform.InverseTransformPoint (player.transform.position);
			if (relativePoint.x < 0.0) {
				jumpDirection = -1;
			} else {
				jumpDirection = 1;
			}
		}

	}

	void FixedUpdate() {
		// jump
		if (!inBlower && grounded && active && jumpTimer == 0) {
			enemy_rb.AddForce (new Vector2(jumpDirection * jumpPower/2, jumpPower));
			jumpTimer = 1f;
		}
	}

	void Die() {
		Destroy (gameObject);
	}

	public void SetActive(bool val) {
		active = val;
	}

	void OnTriggerEnter2D(Collider2D col) {
		if (col.CompareTag ("Blow")) {
			if (blowerTimer == 0) {
				enemy_rb.velocity = new Vector2 (enemy_rb.velocity.x, 0);
			}
			inBlower = true;
			blowerTimer = 0.5f;
		}

		if (col.CompareTag ("Death")) {
			Die ();
		}
	}

	void OnTriggerStay2D(Collider2D col) {
		if (col.CompareTag ("Blow")) {
			inBlower = true;
			blowerTimer = 0.5f;
		}
	}
}
