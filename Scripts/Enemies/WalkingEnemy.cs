using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingEnemy : MonoBehaviour {

	public float walkSpeed = 5f;

	public Transform wallCheck;
	public Transform groundCheck;
	public Transform groundedCheck;

	public LayerMask groundLayerMask;

	float currDirection;
	float blowerTimer;
	bool facingRight;
	bool grounded;
	bool inBlower;

	private Rigidbody2D enemy_rb;

	void Start() {
		enemy_rb = gameObject.GetComponent<Rigidbody2D> ();
		currDirection = -1;
		float blowerTimer = 0;
	}

	void Update () {

		// check if walking towards a wall or a gap
		if (grounded) {
			if (Physics2D.OverlapCircle (wallCheck.position, 0.1f, groundLayerMask) ||
			   !Physics2D.OverlapCircle (groundCheck.position, 0.1f, groundLayerMask)) {
				// turn around if needed
				if (facingRight) {
					transform.localScale = new Vector3 (1, 1, 1);
					currDirection = -1;
					facingRight = false;
				} else {
					transform.localScale = new Vector3 (-1, 1, 1);
					currDirection = 1;
					facingRight = true;
				}
			}
		}

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

	}

	void FixedUpdate() {
		// if enemy is being pushed by the blower or is stunned, don't move. otherwise, walk as normal
		if (!inBlower && grounded) {
			enemy_rb.velocity = new Vector2 (currDirection * walkSpeed, enemy_rb.velocity.y);
		}
	}

	void Die() {
		Destroy (gameObject);
	}

	void OnTriggerEnter2D(Collider2D col) {
		if (col.CompareTag ("Blow")) {
			inBlower = true;
			blowerTimer = 0.5f;
		}

		if (col.CompareTag ("Death")) {
			Die ();
		}
	}
}
