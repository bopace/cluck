using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {

	public float walkSpeed = 10f;
	public float throwPower = 750f;
	public float jumpPower = 1000f;
	public float jumpTimer = 0f;
	public float jumpPushOff = 300f;
	public float pushOffTimer = 0f;
	public float flapPower = 50f;
	public float bokTimer = 0f;
	public float maxFallSpeed = 30f;
	public float blowPowerAir = 50f;
	public float blowPowerGround = 20f;

	public bool moving;
	public bool grounded;
	public bool jumping;
	public bool sloped;
	public bool wallSliding;
	public bool pushingOff = false;
	public bool pushOffRight;
	public bool facingRight;
	public bool frozen = false;
	public bool dead = false;
	public bool levelComplete = false;
	public bool sucking;
	public bool blowing;
	public bool boosting;
	bool holdingItem;

	private Rigidbody2D player_rb;
	private Animator anim;
	private Animator blowerDirectionAnim;
	public GameObject soundController;
	public GameObject assortedParts;
	public GameObject blowerDirection;
	public GameObject blowSide;
	public GameObject blowDown;
	public GameObject blowCurrent;
	public GameObject heldItem;
	public Transform groundedCheckPoint;
	public Transform slidingCheckPoint;
	public Transform slopeCheckPoint;
	public Transform slopeCheckPoint2;
	public Transform itemCheckPoint;
	public LayerMask groundLayerMask;
	public LayerMask slopeLayerMask;
	public LayerMask itemLayerMask;
	public AudioSource audioSource;

	void Start() {
		player_rb = gameObject.GetComponent<Rigidbody2D> ();
		anim = gameObject.GetComponent<Animator> ();
		blowerDirectionAnim = blowerDirection.GetComponent<Animator> ();
		soundController = GameObject.Find ("SoundController");
		grounded = true;
		blowCurrent = blowSide;
		facingRight = true;
	}

	void Update() {
		if (!dead && !levelComplete) {
			// player movement state
			if (Input.GetAxis ("Horizontal") < -0.1f && player_rb.velocity.x < -0.1f) {
				transform.localScale = new Vector3 (-1, 1, 1);
				facingRight = false;
			}

			if (Input.GetAxis ("Horizontal") > 0.1f && player_rb.velocity.x > 0.1f) {
				transform.localScale = new Vector3 (1, 1, 1);
				facingRight = true;
			}

			if (Input.GetKeyDown ("right")) {
				moving = true;
			}

			if (Input.GetKeyDown ("left")) {
				moving = true;
			}

			if ((Input.GetKeyUp ("right") || Input.GetKeyUp ("left")) &&
			   (!Input.GetKey ("right") && !Input.GetKey ("left"))) {
				moving = false;
			}

			if (Input.GetButton ("Boost")) {
				boosting = true;
				blowCurrent = blowDown;
				sucking = false;
				blowing = true;
			} else {
				boosting = false;
				sucking = false;
				blowing = false;
			}

			if (!boosting) {
				blowCurrent.SetActive (false);
				blowCurrent = blowSide;
			}

			blowerDirectionAnim.SetBool ("FacingDown", boosting);



			// blowing mechanics
			if (Input.GetButton ("Blow")) {
				blowCurrent.SetActive (true);
				sucking = false;
				blowing = true;
			} else {
				blowing = false;
			}

			if (Input.GetButton ("Suck")) {
				blowCurrent.SetActive (true);
				sucking = true;
			} else {
				sucking = false;
			}

			if (!blowing && !sucking) {
				blowCurrent.SetActive (false);
			}

			if (boosting) {
				blowCurrent.SetActive (true);
			}

			// jumping mechanics
			if (Input.GetButtonDown ("Jump") && !wallSliding) {
				if (grounded || sloped) {
					// jump
					player_rb.AddForce (Vector2.up * jumpPower / 2);
					jumping = true;
					jumpTimer = 0.5f;
					soundController.GetComponent<SoundController> ().PlayFlapSound ();
				} else {
					// flap
					player_rb.velocity = new Vector2 (player_rb.velocity.x, 0);
					player_rb.AddForce (Vector2.up * flapPower);
					anim.Play ("Player_Flap");
					soundController.GetComponent<SoundController> ().PlayFlapSound ();
				}
			}

			if (Input.GetButtonUp ("Jump") && jumping) {
				jumping = false;
			}

			if (grounded) {
				if (pushingOff) {
					//player_rb.velocity = new Vector2 (0, player_rb.velocity.y);
					pushingOff = false;
				}
			}
		}

		if (jumpTimer > 0) {
			jumpTimer -= Time.deltaTime;
		}

		// check if player is grounded
		grounded = Physics2D.OverlapCircle(groundedCheckPoint.position, 0.1f, groundLayerMask);

		// if level is complete, pretend chicken is in the air
		if (levelComplete) {
			grounded = false;
		}

		// check if player is wall sliding
		wallSliding = !grounded && Physics2D.OverlapCircle (slidingCheckPoint.position, 0.1f, groundLayerMask);

		if (wallSliding) {
			pushingOff = false;
			HandleWallSliding();
		}

		// check if standing on slope
		sloped = Physics2D.OverlapCircle (slopeCheckPoint.position, 0.1f, slopeLayerMask) || Physics2D.OverlapCircle (slopeCheckPoint2.position, 0.1f, slopeLayerMask);

		anim.SetBool ("IsWalking", moving && grounded);
		anim.SetBool ("Grounded", grounded || sloped);

		if (moving && grounded && bokTimer <= 0) {
			bokTimer = 0.5f;
			soundController.GetComponent<SoundController> ().PlayRandomBok ();
		} else if (bokTimer > 0) {
			bokTimer -= Time.deltaTime;
		}

		// handle sucking in object
		if (Physics2D.OverlapCircle (itemCheckPoint.position, 0.5f, itemLayerMask)) {
			GameObject suckingItem = Physics2D.OverlapCircle (itemCheckPoint.position, 0.5f, itemLayerMask).gameObject;
			if (suckingItem.GetComponent<Rock> ().beingSucked) {
				heldItem = suckingItem;
				heldItem.GetComponent<Rigidbody2D> ().constraints = RigidbodyConstraints2D.FreezeAll;
				holdingItem = true;
			} else if (!holdingItem) {
				heldItem = null;
			}
		}

		if (heldItem != null) {
			heldItem.transform.position = gameObject.transform.position;
		}

		// launch item
		if (Input.GetButtonDown ("Blow") && heldItem != null) {
			GameObject launchItem = heldItem;
			heldItem = null;
			launchItem.GetComponent<Rigidbody2D> ().constraints = RigidbodyConstraints2D.None;
			if (boosting) {
				launchItem.GetComponent<Rigidbody2D> ().AddForce (new Vector2 (0, -500));
			} else if (facingRight) {
				launchItem.GetComponent<Rigidbody2D> ().AddForce (new Vector2 (700, 350));
			} else {
				launchItem.GetComponent<Rigidbody2D> ().AddForce (new Vector2 (-700, 350));
			}
		}
	}

	// wall sliding and jumping
	void HandleWallSliding() {
		if (player_rb.velocity.y <= 0) {
			player_rb.velocity = new Vector2 (player_rb.velocity.x, -2.5f);
		}

		if (Input.GetButtonDown ("Jump")) {
			player_rb.velocity = new Vector2 (0, 0);
			if (facingRight) {
				player_rb.AddForce (new Vector2(-jumpPushOff, jumpPower / 2));
				pushingOff = true;
				pushOffTimer = 0.5f;
				jumping = true;
				jumpTimer = 0.5f;
				transform.localScale = new Vector3 (-1, 1, 1);
				facingRight = false;
				pushOffRight = false;
				soundController.GetComponent<SoundController> ().PlayFlapSound ();
			} else {
				player_rb.AddForce (new Vector2(jumpPushOff, jumpPower / 2));
				pushingOff = true;
				pushOffTimer = 0.5f;
				jumping = true;
				jumpTimer = 0.5f;
				transform.localScale = new Vector3 (1, 1, 1);
				facingRight = true;
				pushOffRight = true;
				soundController.GetComponent<SoundController> ().PlayFlapSound ();
			}
		}
	}

	void SetPushingOffFalse() {
		pushingOff = false;
	}

	void FixedUpdate() {
		// actual player movement mechanics happen here
		float moveHorizontal = Input.GetAxis ("Horizontal");

		float xMove = moveHorizontal;

		if (moveHorizontal < 1 && moveHorizontal >= 0 && grounded && !moving) {
			xMove = 0;
		} else if (moveHorizontal > -1 && moveHorizontal <= 0 && grounded && !moving) {
			xMove = 0;
		}

		if (grounded) {
			player_rb.AddForce (new Vector2 (xMove * walkSpeed, player_rb.velocity.y));
		}

		if (pushingOff && xMove != 0) {
			player_rb.AddForce (new Vector2 (xMove * walkSpeed * 2, player_rb.velocity.y));
		}


		// for responsive starting/stopping
		if (player_rb.velocity.x > xMove * walkSpeed && grounded) {
			player_rb.velocity = new Vector2(xMove * walkSpeed, player_rb.velocity.y);
		} else if (player_rb.velocity.x < xMove * walkSpeed  && grounded) {
			player_rb.velocity = new Vector2(xMove * walkSpeed, player_rb.velocity.y);
		}

		// for air control
		if (Mathf.Abs(xMove) < 0.1f && !grounded && !pushingOff) {
			player_rb.velocity = new Vector2(player_rb.velocity.x * 0.9f, player_rb.velocity.y);
		} else if (Mathf.Abs(xMove) > 0.1f  && !grounded && !pushingOff) {
			player_rb.velocity = new Vector2 (xMove * walkSpeed, player_rb.velocity.y);
		}

		// prevent slope sliding
		if ((sloped && xMove == 0 && !jumping) || frozen) {
			player_rb.constraints = RigidbodyConstraints2D.FreezeAll;
		} else {
			player_rb.constraints = RigidbodyConstraints2D.FreezeRotation;
		}

		// holding jump button
		if (Input.GetButton ("Jump") && jumping && jumpTimer > 0f && !pushingOff) {
			player_rb.AddForce (Vector2.up * jumpPower / 25);
		}

		if (Input.GetButton ("Jump") && jumping && jumpTimer > 0f && pushingOff) {
			if (pushOffRight) {
				player_rb.AddForce (new Vector2 (0, jumpPower / 25));
			} else {
				player_rb.AddForce (new Vector2 (0, jumpPower / 25));
			}
		}

		// holding blow button
		float blowPowerCurr = blowPowerAir;
		if (Input.GetButton ("Blow") && !pushingOff && !boosting) {
			if (grounded) {
				blowPowerCurr = blowPowerGround;
			}

			if (facingRight) {
				player_rb.AddForce (Vector2.left * blowPowerCurr);
			} else {
				player_rb.AddForce (Vector2.right * blowPowerCurr);
			}
			// holding suck button
		} else if (Input.GetButton ("Suck") && !pushingOff && !boosting) {
			if (grounded) {
				blowPowerCurr = blowPowerGround;
			}

			if (facingRight) {
				player_rb.AddForce (Vector2.right * blowPowerCurr);
			} else {
				player_rb.AddForce (Vector2.left * blowPowerCurr);
			}
		} else if (boosting) {
			player_rb.AddForce (Vector2.up * blowPowerCurr / 4);
		}

		// capping falling speed
		if (player_rb.velocity.y < -maxFallSpeed) {
			player_rb.velocity = new Vector2 (player_rb.velocity.x, -maxFallSpeed);
		}

		// push off timer logic
		if (pushOffTimer > 0) {
			pushOffTimer -= Time.deltaTime;
		}

		if (pushOffTimer <= 0f) {
			pushingOff = false;
		}

	}

	IEnumerator Die() {
		dead = true;
		gameObject.GetComponentInChildren<BoxCollider2D> ().enabled = false;
		Instantiate (assortedParts, player_rb.transform.position, Quaternion.identity);
		gameObject.GetComponent<SpriteRenderer> ().enabled = false;
		StartCoroutine (PauseMovement ());
		soundController.GetComponent<SoundController> ().PlayDeathSound();
		yield return new WaitForSeconds (1.0f);
		SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
	}

	IEnumerator EndLevel() {
		levelComplete = true;
		StartCoroutine (PauseMovement ());
		soundController.GetComponent<SoundController> ().PlayVictorySound ();
		yield return new WaitForSeconds (1.5f);
		StartCoroutine (NextLevel ());
	}

	IEnumerator NextLevel() {
		float fadeTime = GameObject.Find ("GameManager").GetComponent<Fading> ().BeginFade (1);
		yield return new WaitForSeconds (fadeTime);
		if (SceneManager.GetActiveScene ().buildIndex == 13) {
			SceneManager.LoadScene (1);
		} else {
			SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex + 1);
		}
	}

	void OnTriggerEnter2D(Collider2D col) {
		if (col.CompareTag ("Death") || col.CompareTag("Boundary")) {
			StartCoroutine(Die ());
		}

		if (col.CompareTag ("Eggsit")) {
			StartCoroutine (EndLevel ());
		}
	}

	void OnCollisionEnter2D(Collision2D col) {
		if (col.gameObject.CompareTag ("Enemy")) {
			StartCoroutine(Die ());
		}
	}

	IEnumerator PauseMovement() {
		frozen = true;
		yield return new WaitForSeconds (2.5f);
		frozen = false;
	}
}
