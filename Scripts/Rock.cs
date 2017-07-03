using UnityEngine;
using System.Collections;

public class Rock : MonoBehaviour {

	Rigidbody2D rock_rb;
	public GameObject playerObject;
	Player player;

	float beingSuckedTimer;

	public bool beingSucked;
	public bool beingFlung;

	public LayerMask enemyLayerMask;

	void Start() {
		rock_rb = gameObject.GetComponent<Rigidbody2D> ();
		player = playerObject.GetComponent<Player> ();
		beingSucked = false;
		beingSuckedTimer = 0;
	}

	void Update() {
		if (beingSuckedTimer > 0) {
			beingSuckedTimer -= Time.deltaTime;
		}

		if (beingSuckedTimer <= 0) {
			beingSucked = false;
		}

		if (Mathf.Abs(rock_rb.velocity.x) > 5 || Mathf.Abs(rock_rb.velocity.y) > 5) {
			beingFlung = true;
		}

		if (Mathf.Abs(rock_rb.velocity.x) < 5 && Mathf.Abs(rock_rb.velocity.y) < 5) {
			beingFlung = false;
		}

		if (beingFlung && Physics2D.OverlapCircle (transform.position, 0.5f, enemyLayerMask)) {
			Physics2D.OverlapCircle (transform.position, 0.5f, enemyLayerMask).gameObject.SendMessageUpwards("Die");
		}
	}

	void OnTriggerEnter2D(Collider2D col) {
		if (col.CompareTag ("Blow") && player.sucking) {
			beingSucked = true;
			beingSuckedTimer = 0.1f;
		}
	}

	void OnCollisionEnter2D(Collision2D col) {
		// player collects rock if it's being sucked
		if (col.gameObject.CompareTag ("Player") && beingSucked) {
			beingSucked = false;
		}
	}
}
